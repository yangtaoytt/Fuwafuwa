using Fuwafuwa.Core.Attributes.Bool.Abstract;
using Fuwafuwa.Core.Attributes.Group.Abstract;

namespace Fuwafuwa.Core.Attributes.Group.Implements;

public interface IProcessorGroup : IServiceGroup { }

public interface IProcessorGroup<TBool> : IProcessorGroup, IServiceGroup<TBool> where TBool : IBool { }