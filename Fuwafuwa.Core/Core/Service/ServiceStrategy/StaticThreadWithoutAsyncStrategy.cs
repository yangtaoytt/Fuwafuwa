using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

public class StaticThreadWithoutAsyncStrategy <TService> : AServiceStrategy<TService>
    where TService : AStrategyService<TService>{
    private int _interval;
    
     private readonly ushort _threadNumber;
    private DistributionData? _distributionData;
    
    private Channel<IServiceData<TService, object>>? _internalMainChannel;
    private readonly List<Channel<IServiceData<TService, object>>?> _subThreadChannels;
    
    private bool _isMainShouldRun;
    private bool _isSubThreadShouldRun;
    private bool _mainThreadRunning;
    private readonly List<bool> _subThreadRunning;

    public StaticThreadWithoutAsyncStrategy(ushort threadNumber, int interval) {
        _interval = interval;
        _threadNumber = threadNumber;
        _subThreadChannels = [];
        _subThreadRunning = [];
        for(int i = 0; i < threadNumber; ++i) {
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
        throw new Exception("Failed to write service data to main channel.");
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
            Task.Run(() => { RunSubThread(idx,channel); });
        }
        _internalMainChannel = Channel.CreateUnbounded<IServiceData<TService, object>>();
        _isMainShouldRun = true;
        Task.Run(RunMainThread);
    }
    private void RunMainThread() {
        _mainThreadRunning = true;
        try {
            while (_isMainShouldRun) {
                if (_internalMainChannel!.Reader.TryRead(out var data)) {
                    var threadIndex = data!.Distribute(_distributionData!);
                    if (threadIndex >= _threadNumber) {
                        throw new ThreadIndexOutOfRangeException(threadIndex);
                    }

                    _distributionData!.LastThreadId = threadIndex;
                    if (!_subThreadChannels[threadIndex]!.Writer.TryWrite(data)) {
                        throw new ReceiveServiceDataException();
                    }
                }

                Thread.Sleep(_interval);
            }

            while (_internalMainChannel!.Reader.TryRead(out var data)) {
                var threadIndex = data!.Distribute(_distributionData!);
                if (threadIndex >= _threadNumber) {
                    throw new ThreadIndexOutOfRangeException(threadIndex);
                }

                _distributionData!.LastThreadId = threadIndex;
                if (!_subThreadChannels[threadIndex]!.Writer.TryWrite(data)) {
                    throw new ReceiveServiceDataException();
                }
            }
        } catch (Exception e) {
            Logger2Event.Instance.Warning(this, $"Error:[{e.Message}] from *{e.Source}*.");
        }
        _mainThreadRunning = false;
    }
    private void RunSubThread(int idx, Channel<IServiceData<TService, object>> channel) {
        try {
            _subThreadRunning[idx] = true;
            while (_isSubThreadShouldRun) {
                if (_subThreadChannels[idx]!.Reader.TryRead(out var data)) {
                    WorkOnData(data!);
                }
                Thread.Sleep(_interval);
            }
            while (_subThreadChannels[idx]!.Reader.TryRead(out var data)) {
                WorkOnData(data!);
            }
            _subThreadRunning[idx] = false;
        }catch (Exception e) {
            Logger2Event.Instance.Error(this,
                $"StaticThreadStrategy<{typeof(TService).Name}> encountered an error while processing service data: \n{e}");
        }
    }
}