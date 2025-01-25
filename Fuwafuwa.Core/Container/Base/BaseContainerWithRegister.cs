using Fuwafuwa.Core.Container.Abstract;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.Service.Abstract;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Base;

public class
    BaseContainerWithRegister<TService, TData, TPrimaryInfo, TSubClassType> : APublicChannelContainer<TService, TData,
    TPrimaryInfo, Register, TSubClassType> where TService : AService<TData, TPrimaryInfo, Register>, new()
    where TData : IData
    where TPrimaryInfo : IPrimaryInfo
    where TSubClassType : BaseContainerWithRegister<TService, TData, TPrimaryInfo, TSubClassType> {
    private Register? _register;
    protected BaseContainerWithRegister(int processorCount, DelSetDistribute setter) : base(processorCount, setter) { }

    protected override Register GetInitObject(Register initObject) {
        return _register = initObject;
    }

    protected override Task HandleOtherData(DataObject<IData, IPrimaryInfo> otherDataObject) {
        var info = otherDataObject.PrimaryInfo;

        switch (info) {
            case AddServiceInfo addServiceInfo:
                _register!.ServiceTypes.TryAdd(
                    addServiceInfo.ServiceType,
                    addServiceInfo.DataChannel
                );
                break;
            case InitServiceInfo initServiceInfo:
                _register!.ServiceTypes.Clear();
                foreach (var (key, value) in initServiceInfo.InitRegister.ServiceTypes) {
                    _register.ServiceTypes.TryAdd(key, value);
                }

                break;
            case RemoveServiceInfo removeServiceInfo:
                _register!.ServiceTypes.TryRemove(
                    removeServiceInfo.ServiceType,
                    out _
                );
                removeServiceInfo.UnregisterConfirmDelegate(removeServiceInfo.ServiceType, GetServiceType());
                break;
            default:
                throw new Exception("strange data type");
        }

        return Task.CompletedTask;
    }
}