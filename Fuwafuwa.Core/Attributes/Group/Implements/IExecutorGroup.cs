using Fuwafuwa.Core.Attributes.Bool.Abstract;
using Fuwafuwa.Core.Attributes.Group.Abstract;

namespace Fuwafuwa.Core.Attributes.Group.Implements;

public interface IExecutorGroup : IServiceGroup { }

public interface IExecutorGroup<TBool> : IExecutorGroup, IServiceGroup<TBool> where TBool : IBool { }