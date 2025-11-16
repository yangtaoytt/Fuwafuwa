using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

public class StaticThreadStrategy<TService> : AServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private readonly ushort _threadNumber;
    private DistributionData? _distributionData;
    
    private Channel<IServiceData<TService, object>>? _internalMainChannel;
    private readonly List<Channel<IServiceData<TService, object>>?> _subThreadChannels;
    
    private Task? _mainThreadTask;
    private readonly List<Task?> _subThreadTasks;

    public StaticThreadStrategy(ushort threadNumber) {
        _threadNumber = threadNumber;
        _subThreadChannels = [];
        _subThreadTasks = [];
        for(int i = 0; i < threadNumber; ++i) {
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
        throw new Exception("Failed to write service data to main channel.");
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
    private async Task RunMainThread() {
        try {
            await foreach (var data in _internalMainChannel!.Reader.ReadAllAsync()) {
                var threadIndex = data.Distribute(_distributionData);
                if (threadIndex >= _threadNumber) {
                    throw new ThreadIndexOutOfRangeException(threadIndex);
                }

                _distributionData.LastThreadId = threadIndex;
                if (!_subThreadChannels[threadIndex]!.Writer.TryWrite(data)) {
                    throw new ReceiveServiceDataException();
                }
            }
        } catch (OperationCanceledException) { } catch (Exception e) {
            Logger2Event.Instance.Warning(this, $"Error:[{e.Message}] from *{e.Source}*.");
        }
    }
    private async Task RunSubThread(Channel<IServiceData<TService, object>> channel) {
        try {
            await foreach (var data in channel.Reader.ReadAllAsync()) {
                WorkOnData(data);
            }
        } catch (OperationCanceledException) { } catch (Exception e) {
            Logger2Event.Instance.Error(this,
                $"StaticThreadStrategy<{typeof(TService).Name}> encountered an error while processing service data: \n{e}");
        }
    }
    
    
}