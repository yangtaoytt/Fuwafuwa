using Fuwafuwa.Core.Container.Level1;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Service.Level1;

namespace Fuwafuwa.Core.Container.Level2;

public abstract class
    BaseContainerWithEmpty<TService, TServiceData, TSubjectData, TSharedData> : APublicChannelContainer<TService,
    TServiceData, TSubjectData, InitTuple<TSharedData>>
    where TService : AServiceWithEmpty<TServiceData, TSubjectData, TSharedData>, new()
    where TServiceData : IServiceData
    where TSubjectData : ISubjectData
    where TSharedData : new() {
    protected BaseContainerWithEmpty(int serviceCount, DelSetDistribute setter) : base(serviceCount, setter) { }

    protected override Task HandleOtherData(IServiceData serviceData, ISubjectData subjectData,
        IRegisterData registerData) {
        throw new Exception("This container does not support other data.");
    }
}