using Fuwafuwa.Core.Attributes.Bool.Implements;
using Fuwafuwa.Core.Attributes.Group.Abstract;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.DataChannel;
using Fuwafuwa.Core.Utils;

namespace Fuwafuwa.Core.ServiceRegister;

public interface IRegistrableContainer {
    public DataChannel<IData, IPrimaryInfo> MainChannel { get; }
    public Type GetServiceType();
}

public interface IServiceRegisterGroup {
    public delegate void UnregisterConfirmDelegate(Type unregisterType, Type serviceType);
}

public class ServiceRegisterGroup<TServiceGroup, TServiceGroupReceive> : IServiceRegisterGroup
    where TServiceGroup : IServiceGroup
    where TServiceGroupReceive : IServiceGroup<ITrue> {
    private readonly object _lock;
    private readonly Register _originalRegister;
    private readonly Dictionary<Type, List<Type>> _serviceTypesToUnregister;
    private readonly Dictionary<Type, TaskCompletionSource> _serviceTypesToUnregisterTasks;


    public ServiceRegisterGroup() {
        _originalRegister = new Register();
        _serviceTypesToUnregister = [];
        _serviceTypesToUnregisterTasks = [];
        _lock = new object();
    }

    private bool CheckInGroupIllegal(Type inputType) {
        return !Util.Is(inputType, typeof(TServiceGroup));
    }

    private bool CheckInGroupReceiveLegal(Type inputType) {
        return Util.Is(inputType, typeof(TServiceGroupReceive));
    }

    private void CheckAndSend(Type receiver, DataChannel<IData, IPrimaryInfo> channel,
        DataObject<IData, IPrimaryInfo> msg) {
        if (CheckInGroupReceiveLegal(receiver)) {
            channel.Writer.TryWrite(msg);
        }
    }


    public void RegisterAndBroadcast(IRegistrableContainer container) {
        var registerChannel = container.MainChannel;
        var registerType = container.GetServiceType();
        if (CheckInGroupIllegal(registerType)) {
            throw new Exception(
                $"The Service[{registerType}] is not of the category of this register Group[{typeof(TServiceGroup)}]");
        }

        lock (_lock) {
            if (!_originalRegister.ServiceTypes.TryAdd(registerType, registerChannel)) {
                throw new Exception("Service already registered");
            }

            CheckAndSend(registerType, registerChannel, new DataObject<IData, IPrimaryInfo>(
                new EmptyData(), new InitServiceInfo(new Register(_originalRegister))));


            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                if (serviceType == registerType) {
                    continue;
                }

                CheckAndSend(serviceType, dataChannel, new DataObject<IData, IPrimaryInfo>(new EmptyData(),
                    new AddServiceInfo(
                        registerType, registerChannel)));
            }
        }
    }


    // cause of lock, this method can't be async
    public Task UnregisterAndBroadcast(IRegistrableContainer container) {
        var registerType = container.GetServiceType();
        if (CheckInGroupIllegal(registerType)) {
            throw new Exception("The Service is not of the category of this register Group");
        }

        lock (_lock) {
            if (!_originalRegister.ServiceTypes.ContainsKey(registerType)) {
                throw new Exception("Service not registered");
            }

            CheckAndSend(registerType, _originalRegister.ServiceTypes[registerType],
                new DataObject<IData, IPrimaryInfo>(
                    new EmptyData(), new InitServiceInfo(new Register())));


            _originalRegister.ServiceTypes.TryRemove(registerType, out _);

            if (!_serviceTypesToUnregister.TryAdd(registerType, [
                    .._originalRegister.ServiceTypes.Keys.Where(
                        CheckInGroupReceiveLegal)
                ])) {
                throw new Exception("Service already unregistered");
            }

            var taskSource = new TaskCompletionSource();
            if (!_serviceTypesToUnregisterTasks.TryAdd(registerType, taskSource)) {
                throw new Exception("Service already unregistered");
            }

            foreach (var (serviceType, dataChannel) in _originalRegister.ServiceTypes) {
                CheckAndSend(serviceType, dataChannel, new DataObject<IData, IPrimaryInfo>(
                    new EmptyData(), new RemoveServiceInfo(registerType, UnregisterConfirm)));
            }

            return taskSource.Task;
        }
    }

    public void UnregisterConfirm(Type unregisterType, Type serviceType) {
        if (CheckInGroupIllegal(unregisterType) || CheckInGroupIllegal(serviceType)) {
            throw new Exception("This Services are not of the category of this register Group");
        }

        lock (_lock) {
            if (!_serviceTypesToUnregister.TryGetValue(unregisterType, out var list)) {
                throw new Exception("Service is not unregistering");
            }

            list.Remove(serviceType);

            if (list.Count == 0) {
                foreach (var (key, value) in _serviceTypesToUnregister) {
                    value.Remove(unregisterType);
                }

                _serviceTypesToUnregisterTasks[unregisterType].SetResult();

                _serviceTypesToUnregister.Remove(unregisterType);
                _serviceTypesToUnregisterTasks.Remove(unregisterType);
            }
        }
    }
}