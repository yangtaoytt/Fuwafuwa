using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

/// <summary>
///     A service strategy that uses static threads without async/await and task.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public class StaticThreadWithoutAsyncStrategy<TService> : AServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private readonly DistributionData _distributionData;
    private readonly Channel<IServiceData<TService, object>> _internalMainChannel;
    private readonly List<Channel<IServiceData<TService, object>>> _subThreadChannels;

    private readonly List<bool> _subThreadRunning = [];
    private readonly ushort _threadNumber;
    private bool _isRunning;

    private bool _mainThreadRunning;

    public StaticThreadWithoutAsyncStrategy(ushort threadNumber) {
        _threadNumber = threadNumber;

        _internalMainChannel = Channel.CreateUnbounded<IServiceData<TService, object>>();

        _subThreadChannels = [];

        _distributionData = new DistributionData(0, threadNumber);
    }

    protected override void StartInternal() {
        _isRunning = true;
        try {
            ThreadPool.QueueUserWorkItem(_ => RunMainThread());
            for (var i = 0; i < _threadNumber; ++i) {
                _subThreadRunning.Add(false);
                var channel = Channel.CreateUnbounded<IServiceData<TService, object>>();
                _subThreadChannels.Add(channel);
                var idx = i;
                ThreadPool.QueueUserWorkItem(_ => RunSubThread(idx, channel));
            }
        } catch (Exception e) {
            ShutDown();
            throw new StartServiceFailedException(e);
        } finally {
            Final();
        }
    }

    public override void Receive(IServiceData<TService, object> serviceData) {
        if (_internalMainChannel.Writer.TryWrite(serviceData)) {
            return;
        }

        throw new ReceiveServiceDataException();
    }

    /// <summary>
    ///     Run a sub thread to process data from its channel.
    /// </summary>
    /// <param name="runningFlagIdx">The index of its finish flag in the _subThreadRunning.</param>
    /// <param name="channel">The channel to pass data to sub thread.</param>
    private void RunSubThread(int runningFlagIdx, Channel<IServiceData<TService, object>> channel) {
        _subThreadRunning[runningFlagIdx] = true;
        while (_isRunning) {
            try {
                if (channel.Reader.TryRead(out var data)) {
                    WorkOnData(data);
                }
            } catch (Exception e) {
                Logger2Event.Instance.Warning(this, $"Error:[{e.Message}] from *{e.Source}*.");
            }
        }

        _subThreadRunning[runningFlagIdx] = false;
    }

    /// <summary>
    ///     Run the main thread to distribute data to sub threads.
    /// </summary>
    private void RunMainThread() {
        _mainThreadRunning = true;
        while (_isRunning) {
            try {
                if (!_internalMainChannel.Reader.TryRead(out var data)) {
                    continue;
                }

                var threadIndex = data.Distribute(_distributionData);
                if (threadIndex >= _threadNumber) {
                    throw new ThreadIndexOutOfRangeException(threadIndex);
                }

                _distributionData.LastThreadId = threadIndex;
                if (!_subThreadChannels[threadIndex].Writer.TryWrite(data)) {
                    throw new ReceiveServiceDataException();
                }
            } catch (Exception e) {
                Logger2Event.Instance.Error(this,
                    $"StaticThreadWithoutAsyncStrategy<{typeof(TService).Name}> encountered an error while processing service data: \n{e}");
            }
        }

        _mainThreadRunning = false;
    }

    public override void ShutDown() {
        if (_isRunning == false) {
            throw new NoStartForStrategyException();
        }

        _isRunning = false;
        while (_mainThreadRunning) { }

        while (_subThreadRunning.Contains(true)) { }
    }
}