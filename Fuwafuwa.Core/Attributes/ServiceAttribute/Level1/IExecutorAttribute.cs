using Fuwafuwa.Core.Attributes.ReceiveRegisterBool.Implements;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;

namespace Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;

public interface IExecutorAttribute : IServiceAttribute;

public abstract class
    IExecutorAttribute<TServiceAttribute, TServiceData> :
    Certificate.IServiceAttribute<TServiceAttribute, TServiceData>, IReceiveFalse, IExecutorAttribute
    where TServiceData : AExecutorData
    where TServiceAttribute : IExecutorAttribute<TServiceAttribute, TServiceData>, new();