using Fuwafuwa.Core.Attributes.Bool.Abstract;

namespace Fuwafuwa.Core.Attributes.Group.Abstract;

public interface IServiceGroup { }

public interface IServiceGroup<T> where T : IBool { }