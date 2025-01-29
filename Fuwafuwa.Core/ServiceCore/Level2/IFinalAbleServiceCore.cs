using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level1;

namespace Fuwafuwa.Core.ServiceCore.Level2;

public interface
    IFinalAbleServiceCore<TServiceData, TSharedData, TInitData> : IInitAbleServiceCore<TServiceData, TSharedData,
    TInitData> where TServiceData : IServiceData {
    public static abstract void Final(TSharedData sharedData, Logger2Event? logger);
}