using Fuwafuwa.Core.Container.Level1;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.Container.Level2;

public abstract class
    BaseContainerWithEmpty<TServiceCore, TService, TServiceData, TSubjectData, TSharedData, TInitData,TNextService> :
    APublicChannelContainer<TServiceCore, TService,
        TServiceData, TSubjectData, ValueTuple<TSharedData>, TInitData,
        AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData,TNextService,TService>>
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TServiceCore : IServiceCore<TServiceData>, new()
    where TService : AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData,TNextService,TService>
    where TNextService : class, IPrimitiveService<TNextService, TSharedData, TInitData, TService> {

    protected BaseContainerWithEmpty(int serviceCount, DelSetDistribute setter, TInitData initData,
        Logger2Event? logger = null) : base(serviceCount, setter, initData, logger) { }

    protected override Task HandleOtherData(IServiceData serviceData, ISubjectData subjectData,
        IRegisterData registerData) {
        throw new Exception("This container does not support other data.");
    }
}