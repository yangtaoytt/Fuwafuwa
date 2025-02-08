using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level1;

namespace Fuwafuwa.Core.ServiceCore.Level2;

public interface
    IFinalAbleServiceCore<TServiceData, TSharedData, in TInitData> : IInitAbleServiceCore<TServiceData, TSharedData,
    TInitData> where TServiceData : IServiceData where TSharedData : ISharedDataWrapper {
    public static abstract void Final(TSharedData sharedData, Logger2Event? logger);
}