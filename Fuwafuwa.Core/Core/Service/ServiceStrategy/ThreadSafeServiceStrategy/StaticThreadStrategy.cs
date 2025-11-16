using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategy.ThreadSafeServiceStrategy;

/// <summary>
///     A service strategy that distributes incoming service data to a fixed number of threads.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public class StaticThreadStrategy<TService> : AThreadSafeServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private readonly List<Channel<IServiceData<TService, object>>?> _subThreadChannels;
    private readonly List<Task?> _subThreadTasks;
    private readonly ushort _threadNumber;
    private DistributionData? _distributionData;

    private Channel<IServiceData<TService, object>>? _internalMainChannel;

    private Task? _mainThreadTask;

    public StaticThreadStrategy(ushort threadNumber) {
        _threadNumber = threadNumber;
        _subThreadChannels = [];
        _subThreadTasks = [];
        for (var i = 0; i < threadNumber; ++i) {
            _subThreadChannels.Add(null);
            _subThreadTasks.Add(null);
        }

        _internalMainChannel = null;
        _mainThreadTask = null;

        _distributionData = null;
    }

    protected override void ReceiveInternal(IServiceData<TService, object> serviceData) {
        if (_internalMainChannel!.Writer.TryWrite(serviceData)) {
            return;
        }

        throw new AddData2ChannelException(typeof(TService), serviceData.GetType());
    }

    protected override void WaitForCompletionInternal() {
        _internalMainChannel!.Writer.Complete();
        _mainThreadTask!.Wait();
        _subThreadChannels.ForEach(channel => channel!.Writer.Complete());
        Task.WaitAll(_subThreadTasks.ToArray()!);

        _internalMainChannel = null;
        _mainThreadTask = null;
        for (var i = 0; i < _threadNumber; ++i) {
            _subThreadChannels[i] = null;
            _subThreadTasks[i] = null;
        }

        _distributionData = null;
    }

    protected override void ResumeInternal() {
        _distributionData = new DistributionData(0, _threadNumber);
        for (var i = 0; i < _threadNumber; ++i) {
            var channel = Channel.CreateUnbounded<IServiceData<TService, object>>();
            _subThreadChannels[i] = channel;
            _subThreadTasks[i] = RunSubThread(channel);
        }

        _internalMainChannel = Channel.CreateUnbounded<IServiceData<TService, object>>();
        _mainThreadTask = RunMainThread();
    }

    /// <summary>
    ///     The main thread that distributes incoming service data to sub-threads.
    /// </summary>
    private async Task RunMainThread() {
        try {
            await foreach (var data in _internalMainChannel!.Reader.ReadAllAsync()) {
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
        } catch (Exception e) {
            Logger2Event.Instance.Error(this, "Error processing service data in DynamicThreadStrategy: " + e);
        }
    }

    /// <summary>
    ///     The sub-thread that processes service data.
    /// </summary>
    /// <param name="channel">The channel corresponding with sub-thread</param>
    private async Task RunSubThread(Channel<IServiceData<TService, object>> channel) {
        try {
            await foreach (var data in channel.Reader.ReadAllAsync()) {
                WorkOnData(data);
            }
        } catch (Exception e) {
            Logger2Event.Instance.Error(this, "Error processing service data in DynamicThreadStrategy: " + e);
        }
    }
}