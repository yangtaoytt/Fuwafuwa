using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;

namespace Fuwafuwa.Core.Container.Level3;

public class
    ProcessorContainer<TService, TServiceData, TSharedData> : BaseContainerWithRegister<TService, TServiceData,
    SubjectDataWithCommand, TSharedData>
    where TService : BaseProcessService<TServiceData, TSharedData>, new()
    where TServiceData : IProcessorData
    where TSharedData : new() {
    public ProcessorContainer(int serviceCount, DelSetDistribute setter,Logger2Event? logger) : base(serviceCount, setter, logger) { }
}