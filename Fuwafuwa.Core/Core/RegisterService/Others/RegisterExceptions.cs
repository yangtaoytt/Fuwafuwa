namespace Fuwafuwa.Core.New;

public abstract class RegisterExceptions : Exception {
    
    protected RegisterExceptions(string message) : base(message) {}
    
    protected RegisterExceptions(string message, Exception innerException) : base(message, innerException) {}
    
    protected RegisterExceptions() {}
}

public class ServiceRegisterDuplicateException : RegisterExceptions {
    public ServiceRegisterDuplicateException(string serviceName) : base($"Service {serviceName} already registers.") {}
}
public class ServiceRegisterNotFoundException : RegisterExceptions {
    public ServiceRegisterNotFoundException(string serviceName) : base($"Service {serviceName} not registers.") {}
}