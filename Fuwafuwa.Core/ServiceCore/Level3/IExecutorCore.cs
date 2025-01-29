using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level2;

namespace Fuwafuwa.Core.ServiceCore.Level3;

public interface
    IExecutorCore<TServiceData, TSharedData, TInitData> : IFinalAbleServiceCore<TServiceData, TSharedData, TInitData>
    where TSharedData : new()
    where TServiceData : IServiceData {
    public Task ExecuteTask(TServiceData data, TSharedData sharedData, Logger2Event? logger);
}