using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Service.Level2;

namespace Fuwafuwa.Core.Container.Level3;

public class
    ExecutorContainer<TService, TServiceData, TSharedData> : BaseContainerWithEmpty<TService, TServiceData,
    NullSubjectData,
    TSharedData> where TService : BaseExecutorService<TServiceData, TSharedData>, new()
    where TSharedData : new()
    where TServiceData : IServiceData {
    public ExecutorContainer(int serviceCount, DelSetDistribute setter) : base(serviceCount, setter) { }
}