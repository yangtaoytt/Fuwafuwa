using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level0;

namespace Fuwafuwa.Core.ServiceCore.Level0;

public interface IServiceCore<TServiceData> where TServiceData : IServiceData {
    public static abstract IServiceAttribute<TServiceData> GetServiceAttribute();
}