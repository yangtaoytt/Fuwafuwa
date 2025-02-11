using System.Collections.Concurrent;
using System.Threading.Channels;
using Fuwafuwa.Core.Attributes.ReceiveRegisterBool.Implements;
using Fuwafuwa.Core.Container.Level0;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.ServiceRegister;

public interface IRegistrableContainer : IContainer {
    public Channel<(IServiceData, ISubjectData, IRegisterData)> MainChannel { get; }
}

public class ServiceRegisterGroup {
    public delegate void RegisterInitConfirmDelegate((Type attributeType, Type serviceType) initServiceType);

    public delegate void RegisterUpdateConfirmDelegate((Type attributeType, Type serviceType) receiveServiceType,
        (Type attributeType, Type serviceType) updateServiceType);

    private readonly Logger2Event? _logger;

    private readonly Register _originalRegister;

    private readonly object _registerLock;
    private readonly ConcurrentDictionary<(Type attributeType, Type serviceType), TaskCompletionSource> _service2Init;

    private readonly
        ConcurrentDictionary<(Type attributeType, Type serviceType), (
            ConcurrentDictionary<(Type attributeType, Type serviceType), object?> otherServiceList, TaskCompletionSource
            completionSource)> _service2Register;

    private readonly
        ConcurrentDictionary<(Type attributeType, Type serviceType), (
            ConcurrentDictionary<(Type attributeType, Type serviceType), object?> otherServiceList, TaskCompletionSource
            completionSource)> _service2Unregister;

    public ServiceRegisterGroup(Logger2Event? logger) {
        _logger = logger;
        _originalRegister = new Register(this);
        _service2Unregister = [];
        _service2Register = [];
        _service2Init = [];
        _registerLock = new object();
    }

    private static bool CheckReceiveLegal((Type attributeType, Type serviceType) serviceType) {
        return Util.Is(serviceType.attributeType, typeof(IReceiveTrue));
    }

    private void InitConfirm((Type attributeType, Type serviceType) initServiceType) {
        _logger?.Debug(this, $"InitConfirm {initServiceType.serviceType.Name}");

        if (!_service2Init.TryGetValue(initServiceType, out var taskSource)) {
            throw new Exception("Service is not init-ing");
        }

        try {
            _service2Init.Remove(initServiceType, out _);
            taskSource.SetResult();
        } catch (Exception e) {
            _logger?.Error(this, $"Error[{e.Message}] in InitConfirm");
            throw;
        }
    }

    public void AddConfirm((Type attributeType, Type serviceType) receiverServiceType,
        (Type attributeType, Type serviceType) addServiceType) {
        _logger?.Debug(this, $"AddConfirm {addServiceType.serviceType.Name}");

        lock (_registerLock) {
            if (!_service2Register.TryGetValue(addServiceType, out var value)) {
                throw new Exception("Service is not registering");
            }

            try {
                value.otherServiceList.Remove(receiverServiceType, out _);

                if (value.otherServiceList.Count == 0) {
                    _service2Register.Remove(addServiceType, out _);
                    value.completionSource.SetResult();
                }
            } catch (Exception e) {
                _logger?.Error(this, $"Error[{e.Message}] in AddConfirm");
                throw;
            }
        }
    }

    private void UnregisterConfirm((Type attributeType, Type serviceType) receiverServiceType,
        (Type attributeType, Type serviceType) removeServiceType) {
        _logger?.Debug(this, $"UnregisterConfirm {removeServiceType.serviceType.Name}");
        lock (_registerLock) {
            if (!_service2Unregister.TryGetValue(removeServiceType, out var value)) {
                throw new Exception("Service is not unregistering");
            }

            try {
                value.otherServiceList.Remove(receiverServiceType, out _);

                if (value.otherServiceList.Count == 0) {
                    _service2Unregister.Remove(removeServiceType, out _);
                    value.completionSource.SetResult();
                }
            } catch (Exception e) {
                _logger?.Error(this, $"Error[{e.Message}] in UnregisterConfirm");
                throw;
            }
        }
    }


    private void WaitInitComplete((Type attributeType, Type serviceType) serviceType,
        Channel<(IServiceData, ISubjectData, IRegisterData)> channel, Register register) {
        _logger?.Debug(this, $"WaitInitComplete {serviceType.serviceType.Name}");

        if (_service2Init.ContainsKey(serviceType)) {
            throw new Exception("Service is already init-ing");
        }

        if (!CheckReceiveLegal(serviceType)) {
            return;
        }

        var taskSource = new TaskCompletionSource();
        var task = taskSource.Task;
        _service2Init.TryAdd(serviceType, taskSource);
        channel.Writer.TryWrite((new NullServiceData(), new NullSubjectData(),
            new InitRegisterData(register, InitConfirm)));
        task.Wait();
    }


    public async Task RegisterAndBroadcast(IRegistrableContainer container) {
        _logger?.Info(this, $"RegisterAndBroadcast {container.ServiceAttributeType.serviceType.Name}");
        Task task;
        var registerChannel = container.MainChannel;
        var registerType = container.ServiceAttributeType;
        lock (_registerLock) {
            if (_originalRegister.ServiceTypes.ContainsKey(registerType)) {
                throw new Exception("Service already registered");
            }

            _originalRegister.ServiceTypes.TryAdd(registerType, registerChannel);
            WaitInitComplete(registerType, registerChannel, _originalRegister);
            if (_originalRegister.ServiceTypes.Count == 1) {
                _logger?.Debug(this, $"{registerType.serviceType.Name} is the first service");
                return;
            }

            var dic = new ConcurrentDictionary<(Type attributeType, Type serviceType), object?>();
            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                if (serviceType == registerType) {
                    continue;
                }

                if (CheckReceiveLegal(serviceType)) {
                    _logger?.Debug(this, $"Add {serviceType.serviceType.Name} to add confirm service list");
                    dic.TryAdd(serviceType, null);
                }
            }

            _service2Register.TryAdd(registerType, (dic, new TaskCompletionSource()));
            task = _service2Register[registerType].completionSource.Task;

            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                if (serviceType == registerType) {
                    continue;
                }

                if (CheckReceiveLegal(serviceType)) {
                    _logger?.Debug(this, $"Send add msg to {serviceType.serviceType.Name}");
                    dataChannel.Writer.TryWrite((
                        new NullServiceData(),
                        new NullSubjectData(),
                        new AddRegisterData(registerType, registerChannel, AddConfirm)
                    ));
                }
            }
        }

        _logger?.Debug(this, $"Before wait for {registerType.serviceType.Name} complete register");
        await task;
        _logger?.Debug(this, $"After wait for {registerType.serviceType.Name} complete register");
    }


    public async Task UnregisterAndBroadcast(IRegistrableContainer container) {
        _logger?.Info(this, $"UnregisterAndBroadcast {container.ServiceAttributeType.serviceType.Name}");
        Task task;
        var unRegisterType = container.ServiceAttributeType;
        var registerChannel = container.MainChannel;
        lock (_registerLock) {
            if (!_originalRegister.ServiceTypes.ContainsKey(unRegisterType)) {
                throw new Exception("Service not registered");
            }

            _originalRegister.ServiceTypes.TryRemove(unRegisterType, out _);
            if (_originalRegister.ServiceTypes.Count == 0) {
                _logger?.Debug(this, "No service left");
                return;
            }

            var dic = new ConcurrentDictionary<(Type attributeType, Type serviceType), object?>();
            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                if (CheckReceiveLegal(serviceType)) {
                    _logger?.Debug(this, $"Add {serviceType.serviceType.Name} to remove confirm service list");
                    dic.TryAdd(serviceType, null);
                }
            }

            _service2Unregister.TryAdd(unRegisterType, (dic, new TaskCompletionSource()));
            task = _service2Unregister[unRegisterType].completionSource.Task;

            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                if (CheckReceiveLegal(serviceType)) {
                    _logger?.Debug(this, $"Send remove msg to {serviceType.serviceType.Name}");
                    dataChannel.Writer.TryWrite((
                        new NullServiceData(),
                        new NullSubjectData(),
                        new RemoveRegisterData(unRegisterType, UnregisterConfirm)
                    ));
                }
            }
        }

        _logger?.Debug(this, $"Before wait for {unRegisterType.serviceType.Name}");
        await task;
        _logger?.Debug(this, $"After wait for {unRegisterType.serviceType.Name}");
        WaitInitComplete(unRegisterType, registerChannel, new Register(this));
        foreach (var (registerType, (list, completionSource)) in _service2Register) {
            if (list.Keys.Contains(unRegisterType)) {
                _logger?.Debug(this, $"Manually send add msg to {registerType.serviceType.Name}");
                AddConfirm(unRegisterType, registerType);
            }
        }

        foreach (var (registerType, (list, completionSource)) in _service2Unregister) {
            if (list.Keys.Contains(unRegisterType)) {
                _logger?.Debug(this, $"Manually send remove msg to {registerType.serviceType.Name}");
                UnregisterConfirm(unRegisterType, registerType);
            }
        }
    }

    public List<Channel<(IServiceData, ISubjectData, IRegisterData)>> GetTypeChannel(Type type) {
        lock (_registerLock) {
            return _originalRegister.GetTypeChannel(type);
        }
    }
}