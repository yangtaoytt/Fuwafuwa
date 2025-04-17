namespace Fuwafuwa.Core.New;

public abstract class ServiceException : Exception {
    protected ServiceException(string message) : base(message) {}

    protected ServiceException(string message, Exception innerException) : base(message, innerException) {}

    protected ServiceException() {}
}

public class StartServiceRepeatedlyException : ServiceException;

public class ReceiveServiceDataException : ServiceException;

public class ReceiveServiceDataBeforeStartException : ServiceException;

public class StartServiceFailedException : ServiceException {
    public StartServiceFailedException(Exception innerException) : base("", innerException) {}
}

public class ThreadIndexOutOfRangeException : ServiceException {
    public ThreadIndexOutOfRangeException(ushort threadIndex) : base($"Thread index {threadIndex} is out of range.") {}
}