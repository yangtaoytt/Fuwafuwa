using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level2;

namespace Fuwafuwa.Core.ServiceCore.Level3;

public interface IInputCore<TSharedData, in TInitData>
    : IFinalAbleServiceCore<InputPackagedData, TSharedData, TInitData> where TSharedData : ISharedDataWrapper {
    public Task<List<Certificate>> ProcessData(InputPackagedData data, TSharedData sharedData, Logger2Event? logger);
}