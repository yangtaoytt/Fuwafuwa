using Fuwafuwa.Core.Container.Level1;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.Container.Level2;

public abstract class
    BaseContainerWithEmpty<TServiceCore, TService, TServiceData, TSubjectData, TSharedData, TInitData> :
    APublicChannelContainer<TServiceCore, TService,
        TServiceData, TSubjectData, InitTuple<TSharedData>, TInitData>
    where TService : AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData>, new()
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TSharedData : new()
    where TServiceCore : IServiceCore<TServiceData>, new() {

    protected BaseContainerWithEmpty(int serviceCount, DelSetDistribute setter, TInitData initData, Lock sharedDataLock,
        Logger2Event? logger = null) : base(serviceCount, setter, initData, sharedDataLock, logger) { }

    protected override Task HandleOtherData(IServiceData serviceData, ISubjectData subjectData,
        IRegisterData registerData) {
        throw new Exception("This container does not support other data.");
    }
}