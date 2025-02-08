using System.Threading.Channels;
using Fuwafuwa.Core.Container.Level0;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.ServiceCore.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level1;

public abstract class
    APublicChannelContainer<TServiceCore, TService, TServiceData, TSubjectData, TSharedData, TInitData, TNextService> :
    AContainer<
        TServiceCore, TService, TServiceData,
        TSubjectData, TSharedData, TInitData, TNextService>,
    IRegistrableContainer
    where TServiceData : IServiceData
    where TSharedData : new()
    where TSubjectData : ISubjectData
    where TServiceCore : IServiceCore<TServiceData>, new()
    where TNextService : class, IService<TNextService, TSharedData, TInitData, TService>
    where TService : AService<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData, TNextService,
        TService> {
    protected APublicChannelContainer(int serviceCount, DelSetDistribute setter, TInitData initData,
        Logger2Event? logger = null) : base(serviceCount, setter, initData, logger) { }

    public Channel<(IServiceData, ISubjectData, IRegisterData)> MainChannel => InternalMainChannel;
}