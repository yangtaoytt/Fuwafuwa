using Fuwafuwa.Core.Attributes.ReceiveRegisterBool.Implements;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;

namespace Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;

public interface IProcessorAttribute : IServiceAttribute;

public abstract class
    IProcessorAttribute<TServiceAttribute, TServiceData> :
    Certificate.IServiceAttribute<TServiceAttribute, TServiceData>, IReceiveTrue,
    IProcessorAttribute where TServiceData : IProcessorData
    where TServiceAttribute : IProcessorAttribute<TServiceAttribute, TServiceData>, new();