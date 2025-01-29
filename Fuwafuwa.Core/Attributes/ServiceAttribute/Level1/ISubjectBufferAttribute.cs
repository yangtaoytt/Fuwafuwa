using Fuwafuwa.Core.Attributes.ReceiveRegisterBool.Implements;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;

namespace Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;

public class ISubjectBufferAttribute : Certificate.IServiceAttribute<ISubjectBufferAttribute, NullServiceData>,
    IReceiveTrue;