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
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.ServiceRegister;

public interface IRegistrableContainer : IContainer {
    public Channel<(IServiceData, ISubjectData, IRegisterData)> MainChannel { get; }
}

public class ServiceRegisterGroup {
    public delegate void RegisterInitConfirmDelegate((Type attributeType, Type serviceType) initServiceType);

    public delegate void RegisterUpdateConfirmDelegate((Type attributeType, Type serviceType) receiveServiceType,
        (Type attributeType, Type serviceType) updateServiceType);

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

    public ServiceRegisterGroup() {
        _originalRegister = new Register();
        _service2Unregister = [];
        _service2Register = [];
        _service2Init = [];
        _registerLock = new object();
    }

    private static bool CheckReceiveLegal((Type attributeType, Type serviceType) serviceType) {
        return Util.Is(serviceType.attributeType, typeof(IReceiveTrue));
    }

    private static void CheckAndSend((Type attributeType, Type serviceType) serviceType,
        Channel<(IServiceData, ISubjectData, IRegisterData)> channel,
        (IServiceData, ISubjectData, IRegisterData) msg) {
        if (!CheckReceiveLegal(serviceType)) {
            return;
        }

        channel.Writer.TryWrite(msg);
    }

    private void InitConfirm((Type attributeType, Type serviceType) initServiceType) {
        if (!_service2Init.TryGetValue(initServiceType, out var taskSource)) {
            throw new Exception("Service is not init-ing");
        }

        _service2Init.Remove(initServiceType, out _);
        taskSource.SetResult();
    }

    public void AddConfirm((Type attributeType, Type serviceType) receiverServiceType,
        (Type attributeType, Type serviceType) addServiceType) {
        if (!_service2Register.TryGetValue(addServiceType, out var value)) {
            throw new Exception("Service is not registering");
        }

        value.otherServiceList.Remove(receiverServiceType, out _);

        if (value.otherServiceList.Count == 0) {
            _service2Register.Remove(addServiceType, out _);
            value.completionSource.SetResult();
        }
    }

    private void UnregisterConfirm((Type attributeType, Type serviceType) receiverServiceType,
        (Type attributeType, Type serviceType) removeServiceType) {
        if (!_service2Unregister.TryGetValue(removeServiceType, out var value)) {
            throw new Exception("Service is not unregistering");
        }

        value.otherServiceList.Remove(receiverServiceType, out _);

        if (value.otherServiceList.Count == 0) {
            _service2Unregister.Remove(removeServiceType, out _);
            value.completionSource.SetResult();
        }
    }


    private void WaitInitComplete((Type attributeType, Type serviceType) serviceType,
        Channel<(IServiceData, ISubjectData, IRegisterData)> channel, Register register) {
        if (_service2Init.ContainsKey(serviceType)) {
            throw new Exception("Service is already init-ing");
        }

        if (!CheckReceiveLegal(serviceType)) {
            return;
        }

        var taskSource = new TaskCompletionSource();
        var task = taskSource.Task;
        _service2Init.TryAdd(serviceType, taskSource);
        CheckAndSend(serviceType, channel,
            (new NullServiceData(), new NullSubjectData(), new InitRegisterData(register, InitConfirm)));

        task.Wait();
    }


    public async Task RegisterAndBroadcast(IRegistrableContainer container) {
        Task task;

        lock (_registerLock) {
            var registerChannel = container.MainChannel;
            var registerType = container.ServiceAttributeType;

            if (_originalRegister.ServiceTypes.ContainsKey(registerType)) {
                throw new Exception("Service already registered");
            }

            _originalRegister.ServiceTypes.TryAdd(registerType, registerChannel);
            WaitInitComplete(registerType, registerChannel, _originalRegister);
            if (_originalRegister.ServiceTypes.Count == 1) {
                return;
            }


            var dic = new ConcurrentDictionary<(Type attributeType, Type serviceType), object?>();
            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                if (serviceType == registerType) {
                    continue;
                }

                if (CheckReceiveLegal(serviceType)) {
                    dic.TryAdd(serviceType, null);
                }
            }

            _service2Register.TryAdd(registerType, (dic, new TaskCompletionSource()));
            task = _service2Register[registerType].completionSource.Task;

            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                if (serviceType == registerType) {
                    continue;
                }

                CheckAndSend(serviceType, dataChannel, (
                    new NullServiceData(),
                    new NullSubjectData(),
                    new AddRegisterData(registerType, registerChannel, AddConfirm)
                ));
            }
        }

        await task;
    }


    public async Task UnregisterAndBroadcast(IRegistrableContainer container) {
        Task task;
        var unRegisterType = container.ServiceAttributeType;
        var registerChannel = container.MainChannel;
        lock (_registerLock) {
            if (!_originalRegister.ServiceTypes.ContainsKey(unRegisterType)) {
                throw new Exception("Service not registered");
            }

            _originalRegister.ServiceTypes.TryRemove(unRegisterType, out _);
            if (_originalRegister.ServiceTypes.Count == 0) {
                return;
            }

            var dic = new ConcurrentDictionary<(Type attributeType, Type serviceType), object?>();
            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                if (CheckReceiveLegal(serviceType)) {
                    dic.TryAdd(serviceType, null);
                }
            }

            _service2Unregister.TryAdd(unRegisterType, (dic, new TaskCompletionSource()));
            task = _service2Unregister[unRegisterType].completionSource.Task;

            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                CheckAndSend(serviceType, dataChannel, (
                    new NullServiceData(),
                    new NullSubjectData(),
                    new RemoveRegisterData(unRegisterType, UnregisterConfirm)
                ));
            }
        }

        await task;
        WaitInitComplete(unRegisterType, registerChannel, new Register());
        foreach (var (registerType, (list, completionSource)) in _service2Register) {
            if (list.Keys.Contains(unRegisterType)) {
                AddConfirm(unRegisterType, registerType);
            }
        }

        foreach (var (registerType, (list, completionSource)) in _service2Unregister) {
            if (list.Keys.Contains(unRegisterType)) {
                UnregisterConfirm(unRegisterType, registerType);
            }
        }
    }
}