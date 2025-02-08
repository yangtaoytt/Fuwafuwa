using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.ServiceCore.Level1;

public interface IInitAbleServiceCore<TServiceData, out TSharedData, in TInitData> : IServiceCore<TServiceData>
    where TServiceData : IServiceData where TSharedData : ISharedDataWrapper {
    public static abstract TSharedData Init(TInitData initData);
}