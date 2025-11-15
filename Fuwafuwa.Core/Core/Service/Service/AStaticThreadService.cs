using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.Service;

/// <summary>
/// The abstract class for services with static number of threads.
/// The number of threads is defined when constructing the service.
/// The service will create the threads when started.
/// And each thread will have its own channel to receive the service data.
/// The main thread will distribute the data to each sub thread according to the distribution logic defined in the service data.
/// The service data will be processed in the sub threads.
/// </summary>
/// <typeparam name="TService">The subclass of this class.</typeparam>
public abstract class AStaticThreadService<TService> : IService<TService> 
    where TService : AStaticThreadService<TService> {
    
    private readonly Channel<IServiceData<TService,object>> _internalMainChannel;
    private readonly ushort _threadNumber;
    
    private Task _mainThreadTask;
    private readonly List<Task> _subThreadTasks;
    
    private bool _hasStarted;

    private CancellationTokenSource _cancellationTokenSource; 
    
    private readonly DistributionData _distributionData;
    
    private readonly List<Channel<IServiceData<TService,object>>> _subThreadChannels;
    
    public AStaticThreadService(ushort threadNumber) {
        _internalMainChannel = Channel.CreateUnbounded<IServiceData<TService,object>>();
        _threadNumber = threadNumber;
        
        _subThreadTasks = [];
        
        _hasStarted = false;
        
        _distributionData = new DistributionData(0, threadNumber);
        
        _subThreadChannels = [];
    }
    
    public void Receive(IServiceData<TService,object> serviceData) {
        if (!_hasStarted) {
            throw new ReceiveServiceDataBeforeStartException();
        }
        
        if (_internalMainChannel.Writer.TryWrite(serviceData)) {
            return;
        }
        
        throw new ReceiveServiceDataException();
    }
    
    public TService Start() {
        if (_hasStarted) {
            throw new StartServiceRepeatedlyException();
        }
        _hasStarted = true;
        _cancellationTokenSource = new CancellationTokenSource();
        try {
            _mainThreadTask = RunMainThread(_cancellationTokenSource.Token);
            for (var i = 0; i < _threadNumber; ++i) {
                var channel = Channel.CreateUnbounded<IServiceData<TService,object>>();
                _subThreadChannels.Add(channel);
                _subThreadTasks.Add(RunSubThread(_cancellationTokenSource.Token, channel));
            }
        } catch (Exception e) {
            ShutDown();
            throw new StartServiceFailedException(e);
        } finally {
            Final();
        }
        return Implement();
    }

    public abstract TService Implement();
    
    public void ShutDown() {
        _cancellationTokenSource.Cancel();
        _mainThreadTask.Wait();
        Task.WaitAll(_subThreadTasks.ToArray());
    }
    
    private async Task RunMainThread(CancellationToken cancellationToken) {
        while (true) {
            try {
                await foreach (var data in _internalMainChannel.Reader.ReadAllAsync(cancellationToken)) {
                    var threadIndex = data.Distribute(_distributionData);
                    if (threadIndex >= _threadNumber) {
                        throw new ThreadIndexOutOfRangeException(threadIndex);
                    }

                    _distributionData.LastThreadId = threadIndex;
                    if (!_subThreadChannels[threadIndex].Writer.TryWrite(data)) {
                        throw new ReceiveServiceDataException();
                    }
                }
            } catch (OperationCanceledException) {
                break;
            } catch (Exception e) {
                Logger2Event.Instance.Warning(this, $"Error:[{e.Message}] from *{e.Source}*.");
            }
        }
    }
    
    private async Task RunSubThread(CancellationToken cancellationToken, Channel<IServiceData<TService,object>> channel) {
        while (true) {
            try {
                await foreach (var data in channel.Reader.ReadAllAsync(cancellationToken)) {
                    data.Accept(Implement());
                }
            } catch (OperationCanceledException) {
                break;
            }catch (Exception e) {
                Logger2Event.Instance.Warning(this, $"Error:[{e.Message}] from *{e.Source}*.");
            }
        }
    }

    protected virtual void Final() { }
}