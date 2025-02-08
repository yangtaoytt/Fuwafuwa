using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level2;

namespace Fuwafuwa.Core.ServiceCore.Level3;

public interface IProcessorCore<TServiceData, TSharedData, in TInitData>
    : IFinalAbleServiceCore<TServiceData, TSharedData, TInitData>
    where TServiceData : IProcessorData where TSharedData : ISharedDataWrapper {
    public Task<List<Certificate>> ProcessData(TServiceData data, TSharedData sharedData, Logger2Event? logger);
}