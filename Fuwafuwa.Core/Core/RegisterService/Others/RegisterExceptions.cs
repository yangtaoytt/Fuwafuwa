namespace Fuwafuwa.Core.Core.RegisterService.Others;

/// <summary>
///     The base exception class for register service related exceptions.
/// </summary>
public abstract class RegisterExceptions : Exception {
    protected RegisterExceptions(string message) : base(message) { }

    protected RegisterExceptions(string message, Exception innerException) : base(message, innerException) { }

    protected RegisterExceptions() { }
}

/// <summary>
///     Thrown when trying to register a service that is already registered.
/// </summary>
public class ServiceRegisterDuplicateException : RegisterExceptions {
    public ServiceRegisterDuplicateException(string serviceName) : base($"Service {serviceName} already registers.") { }
}

/// <summary>
///     Thrown when trying to access a service that is not registered.
/// </summary>
public class ServiceRegisterNotFoundException : RegisterExceptions {
    public ServiceRegisterNotFoundException(string serviceName) : base($"Service {serviceName} not registers.") { }
}