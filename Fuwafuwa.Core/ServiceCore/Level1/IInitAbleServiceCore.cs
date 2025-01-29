using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.ServiceCore.Level1;

public interface IInitAbleServiceCore<TServiceData, out TSharedData, in TInitData> : IServiceCore<TServiceData>
    where TServiceData : IServiceData {
    public static abstract TSharedData Init(TInitData initData);
}