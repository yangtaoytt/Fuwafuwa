using Fuwafuwa.Core.Attributes.Bool.Implements;
using Fuwafuwa.Core.Attributes.Group.Implements;

namespace Fuwafuwa.Core.ServiceRegister;

public class ExecutorGroup : ServiceRegisterGroup<IExecutorGroup, IExecutorGroup<ITrue>> { }