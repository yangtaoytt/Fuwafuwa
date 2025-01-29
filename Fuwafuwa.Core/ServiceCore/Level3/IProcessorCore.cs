using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level2;

namespace Fuwafuwa.Core.ServiceCore.Level3;

public interface IProcessorCore<TServiceData, TSharedData, TInitData>
    : IFinalAbleServiceCore<TServiceData, TSharedData, TInitData>
    where TServiceData : IProcessorData {
    public Task<List<Certificate>> ProcessData(TServiceData data, TSharedData sharedData,Lock sharedDataLock, Logger2Event? logger);
}