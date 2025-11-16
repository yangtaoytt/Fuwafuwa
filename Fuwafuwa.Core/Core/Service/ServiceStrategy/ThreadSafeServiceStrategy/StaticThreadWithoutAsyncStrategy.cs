using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategy.ThreadSafeServiceStrategy;

/// <summary>
///     A service strategy that distributes incoming service data to a fixed number of threads without using async/await.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public class StaticThreadWithoutAsyncStrategy<TService> : AThreadSafeServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private readonly int _interval;
    private readonly List<Channel<IServiceData<TService, object>>?> _subThreadChannels;
    private readonly List<bool> _subThreadRunning;

    private readonly ushort _threadNumber;
    private DistributionData? _distributionData;

    private Channel<IServiceData<TService, object>>? _internalMainChannel;

    private bool _isMainShouldRun;
    private bool _isSubThreadShouldRun;
    private bool _mainThreadRunning;

    public StaticThreadWithoutAsyncStrategy(ushort threadNumber, int interval) {
        _interval = interval;
        _threadNumber = threadNumber;
        _subThreadChannels = [];
        _subThreadRunning = [];
        for (var i = 0; i < threadNumber; ++i) {
            _subThreadChannels.Add(null);
            _subThreadRunning.Add(false);
        }

        _isMainShouldRun = false;
        _isSubThreadShouldRun = false;
        _internalMainChannel = null;
        _mainThreadRunning = false;

        _distributionData = new DistributionData(0, threadNumber);
    }

    protected override void ReceiveInternal(IServiceData<TService, object> serviceData) {
        if (_internalMainChannel!.Writer.TryWrite(serviceData)) {
            return;
        }

        throw new AddData2ChannelException(typeof(TService), serviceData.GetType(),
            "Failed to write service data to main channel.");
    }

    protected override void WaitForCompletionInternal() {
        _internalMainChannel!.Writer.Complete();
        _isMainShouldRun = false;
        while (_mainThreadRunning) {
            Thread.Sleep(_interval);
        }

        _subThreadChannels.ForEach(channel => channel!.Writer.Complete());
        _isSubThreadShouldRun = false;
        while (_subThreadRunning.Contains(true)) {
            Thread.Sleep(_interval);
        }

        _isMainShouldRun = false;
        _internalMainChannel = null;
        _mainThreadRunning = false;

        _isSubThreadShouldRun = false;
        for (var i = 0; i < _threadNumber; ++i) {
            _subThreadChannels[i] = null;
            _subThreadRunning[i] = false;
        }

        _distributionData = null;
    }

    protected override void ResumeInternal() {
        _distributionData = new DistributionData(0, _threadNumber);
        _isSubThreadShouldRun = true;
        for (var i = 0; i < _threadNumber; ++i) {
            var channel = Channel.CreateUnbounded<IServiceData<TService, object>>();
            _subThreadChannels[i] = channel;
            var idx = i;
            Task.Run(() => { RunSubThread(idx); });
        }

        _internalMainChannel = Channel.CreateUnbounded<IServiceData<TService, object>>();
        _isMainShouldRun = true;
        Task.Run(RunMainThread);
    }

    /// <summary>
    ///     The main thread that distributes incoming service data to sub-threads.
    /// </summary>
    private void RunMainThread() {
        try {
            _mainThreadRunning = true;
            while (_isMainShouldRun) {
                if (_internalMainChannel!.Reader.TryRead(out var data)) {
                    var threadIndex = data.Distribute(_distributionData!);
                    if (threadIndex >= _threadNumber) {
                        throw new DistributionDataException(typeof(TService), data.GetType(),
                            $"Thread index[{threadIndex}] out of range.");
                    }

                    _distributionData!.LastThreadId = threadIndex;
                    if (!_subThreadChannels[threadIndex]!.Writer.TryWrite(data)) {
                        throw new AddData2ChannelException(typeof(TService), data.GetType());
                    }
                }

                Thread.Sleep(_interval);
            }

            while (_internalMainChannel!.Reader.TryRead(out var data)) {
                var threadIndex = data.Distribute(_distributionData!);
                if (threadIndex >= _threadNumber) {
                    throw new DistributionDataException(typeof(TService), data.GetType(),
                        $"Thread index[{threadIndex}] out of range.");
                }

                _distributionData!.LastThreadId = threadIndex;
                if (!_subThreadChannels[threadIndex]!.Writer.TryWrite(data)) {
                    throw new AddData2ChannelException(typeof(TService), data.GetType());
                }
            }

            _mainThreadRunning = false;
        } catch (Exception e) {
            Logger2Event.Instance.Error(this,
                "Error processing service data in StaticThreadWithoutAsyncStrategy: " + e);
        }
    }

    /// <summary>
    ///     The sub-thread that processes service data.
    /// </summary>
    /// <param name="idx">The sub thread's corresponding id.</param>
    private void RunSubThread(int idx) {
        try {
            _subThreadRunning[idx] = true;
            while (_isSubThreadShouldRun) {
                if (_subThreadChannels[idx]!.Reader.TryRead(out var data)) {
                    WorkOnData(data);
                }

                Thread.Sleep(_interval);
            }

            while (_subThreadChannels[idx]!.Reader.TryRead(out var data)) {
                WorkOnData(data);
            }

            _subThreadRunning[idx] = false;
        } catch (Exception e) {
            Logger2Event.Instance.Error(this, "Error processing service data in DynamicThreadStrategy: " + e);
        }
    }
}