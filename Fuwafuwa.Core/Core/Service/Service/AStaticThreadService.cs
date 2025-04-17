using System.Threading.Channels;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Log.LogEventArgs.Interface;
using Fuwafuwa.Core.New.Data;

namespace Fuwafuwa.Core.New.Serviece;

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

    /// <summary>
    /// Receives the service data.
    /// </summary>
    /// <param name="serviceData">The data which is belonged to this service.</param>
    /// <exception cref="ReceiveServiceDataBeforeStartException">Thrown when the method is called before the service starts.
    /// Will not shut down the service.</exception>
    /// <exception cref="ReceiveServiceDataException">Thrown when the writing to the channel fails.
    /// Will not shut down the service.</exception>
    public void Receive(IServiceData<TService,object> serviceData) {
        if (!_hasStarted) {
            throw new ReceiveServiceDataBeforeStartException();
        }
        
        if (_internalMainChannel.Writer.TryWrite(serviceData)) {
            return;
        }
        
        throw new ReceiveServiceDataException();
    }
    

    /// <summary>
    /// Start the service, both of sub thread and main thread of service.
    /// </summary>
    /// <exception cref="StartServiceRepeatedlyException">Thrown when start a started service.
    /// Will not shut down the service.</exception>
    /// <exception cref="StartServiceFailedException">Thrown when start fails cause of inner exception.
    /// Will shut down the service.</exception>
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

    /// <summary>
    /// Shut down the service by cancel the token source and wait for all tasks to complete.
    /// </summary>
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