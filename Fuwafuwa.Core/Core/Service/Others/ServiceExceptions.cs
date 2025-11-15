namespace Fuwafuwa.Core.Core.Service.Others;

/// <summary>
/// The base exception for all service related exceptions.
/// </summary>
public abstract class ServiceException : Exception {
    protected ServiceException(string message) : base(message) {}

    protected ServiceException(string message, Exception innerException) : base(message, innerException) {}

    protected ServiceException() {}
}

/// <summary>
/// Thrown when trying to start a service that is already running.
/// </summary>
public class StartServiceRepeatedlyException : ServiceException;

/// <summary>
/// Thrown when service data cannot be received by the service.
/// </summary>
public class ReceiveServiceDataException : ServiceException;

/// <summary>
/// Thrown when service data is received before the service has been started.
/// </summary>
public class ReceiveServiceDataBeforeStartException : ServiceException;

/// <summary>
/// Thrown when a service fails to start due to an internal error.
/// </summary>
public class StartServiceFailedException : ServiceException {
    public StartServiceFailedException(Exception innerException) : base("", innerException) {}
}

/// <summary>
/// Thrown when a thread index is out of the valid range.
/// </summary>
public class ThreadIndexOutOfRangeException : ServiceException {
    public ThreadIndexOutOfRangeException(ushort threadIndex) : base($"Thread index {threadIndex} is out of range.") {}
}

public class NoServiceException : ServiceException {
    public NoServiceException() : base("No inner service available.") {}
}