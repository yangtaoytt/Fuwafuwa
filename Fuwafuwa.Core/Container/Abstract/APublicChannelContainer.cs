using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.DataChannel;
using Fuwafuwa.Core.Service.Abstract;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Abstract;

public abstract class
    APublicChannelContainer<TService, TData, TPrimaryInfo, TInitType, TSubClassType> :
    AContainer<TService, TData, TPrimaryInfo, TInitType, TSubClassType>, IRegistrableContainer
    where TService : AService<TData, TPrimaryInfo, TInitType>, new()
    where TData : IData
    where TPrimaryInfo : IPrimaryInfo
    where TSubClassType : APublicChannelContainer<TService, TData, TPrimaryInfo, TInitType, TSubClassType> {
    protected APublicChannelContainer(int processorCount, DelSetDistribute setter) : base(processorCount, setter) { }

    public DataChannel<IData, IPrimaryInfo> MainChannel => InternalMainChannel;
}