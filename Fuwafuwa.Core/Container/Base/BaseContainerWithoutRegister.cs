using Fuwafuwa.Core.Container.Abstract;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.Service.Abstract;

namespace Fuwafuwa.Core.Container.Base;

public abstract class
    BaseContainerWithoutRegister<TService, TData, TPrimaryInfo, TSubClassType> : APublicChannelContainer<TService, TData
    , TPrimaryInfo, object, TSubClassType> where TService : AService<TData, TPrimaryInfo, object>, new()
    where TData : IData
    where TPrimaryInfo : IPrimaryInfo
    where TSubClassType : BaseContainerWithoutRegister<TService, TData, TPrimaryInfo, TSubClassType> {
    protected BaseContainerWithoutRegister(int processorCount, DelSetDistribute setter) :
        base(processorCount, setter) { }

    protected override object GetInitObject(object initObject) {
        return initObject;
    }

    protected override Task HandleOtherData(DataObject<IData, IPrimaryInfo> otherDataObject) {
        throw new Exception("This container does not support other data.");
    }
}