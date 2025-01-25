using Fuwafuwa.Core.Attributes.Bool.Implements;
using Fuwafuwa.Core.Attributes.Group.Implements;

namespace Fuwafuwa.Core.Attributes.Implements;

public interface ISubjectBufferAttribute : IExecutorGroup<ITrue>, IProcessorGroup<IFalse> { }