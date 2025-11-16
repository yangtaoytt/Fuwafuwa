using Fuwafuwa.Core.Core.Service.ServiceStrategy.ThreadSafeServiceStrategy;

namespace Fuwafuwa.Core.Core.Service.Others;

/// <summary>
///     Base exception for strategy-related errors.
/// </summary>
public class StrategyException : Exception {
    public StrategyException(Type serviceType, string msg)
        : base($"*serviceType={serviceType}* {msg}") { }
}

/// <summary>
///     Exception for invalid strategy state transitions.
/// </summary>
public class StrategyStateException : StrategyException {
    public StrategyStateException(Type serviceType, ServiceStrategyState current, ServiceStrategyState expect,
        string msg = "")
        : base(serviceType, $"Expect state [{expect}],but get state[{current}]. (Message: {msg})") { }

    public StrategyStateException(Type serviceType, ServiceStrategyState current, ServiceStrategyState expect1,
        ServiceStrategyState expect2, string msg = "")
        : base(serviceType, $"Expect state [{expect1} or {expect2}],but get state[{current}]. (Message: {msg})") { }
}

/// <summary>
///     Base exception for strategy-data-related errors.
/// </summary>
public class StrategyDataException : StrategyException {
    public StrategyDataException(Type serviceType, Type dataType, string msg)
        : base(serviceType, $"*dataType={dataType}* {msg}") { }
}

/// <summary>
///     Exception for failures when adding data to a channel.
/// </summary>
public class AddData2ChannelException : StrategyDataException {
    public AddData2ChannelException(Type serviceType, Type dataType, string msg = "")
        : base(serviceType, dataType, $"Failed to add data to channel. (Message: {msg})") { }
}

/// <summary>
///     Exception for errors in distribution data.
/// </summary>
public class DistributionDataException : StrategyDataException {
    public DistributionDataException(Type serviceType, Type dataType, string msg = "")
        : base(serviceType, dataType, $"Distribution data error. (Message: {msg})") { }
}