using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level3;

public class
    ProcessorContainer<TProcessorCore, TServiceData, TSharedData, TInitData> : BaseContainerWithRegister<TProcessorCore,
    ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>, TServiceData,
    SubjectDataWithCommand, TSharedData, TInitData>
    where TServiceData : IProcessorData
    where TSharedData : new()
    where TProcessorCore : IProcessorCore<TServiceData, TSharedData, TInitData>, new() {
    public ProcessorContainer(int serviceCount, DelSetDistribute setter, (Register, TInitData) initData,
        Logger2Event? logger) : base(serviceCount, setter, initData, logger) { }
}