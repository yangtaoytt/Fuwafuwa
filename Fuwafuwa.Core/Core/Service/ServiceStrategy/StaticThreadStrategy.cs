using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategy;

/// <summary>
///     Use a fixed number of threads to process service data.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public class StaticThreadStrategy<TService> : AServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private readonly DistributionData _distributionData;
    private readonly Channel<IServiceData<TService, object>> _internalMainChannel;
    private readonly List<Channel<IServiceData<TService, object>>> _subThreadChannels;

    private readonly List<Task> _subThreadTasks;


    private readonly ushort _threadNumber;
    private CancellationTokenSource _cancellationTokenSource;

    private Task _mainThreadTask;

    public StaticThreadStrategy(ushort threadNumber) {
        _threadNumber = threadNumber;

        _internalMainChannel = Channel.CreateUnbounded<IServiceData<TService, object>>();

        _subThreadTasks = [];
        _subThreadChannels = [];

        _distributionData = new DistributionData(0, threadNumber);
    }

    protected override void StartInternal() {
        _cancellationTokenSource = new CancellationTokenSource();
        try {
            _mainThreadTask = RunMainThread(_cancellationTokenSource.Token);
            for (var i = 0; i < _threadNumber; ++i) {
                var channel = Channel.CreateUnbounded<IServiceData<TService, object>>();
                _subThreadChannels.Add(channel);
                _subThreadTasks.Add(RunSubThread(_cancellationTokenSource.Token, channel));
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
    /// <param name="cancellationToken">The cancellation token to stop the thread.</param>
    /// <param name="channel">The channel to pass data to sub thread.</param>
    private async Task RunSubThread(CancellationToken cancellationToken,
        Channel<IServiceData<TService, object>> channel) {
        while (true) {
            try {
                await foreach (var data in channel.Reader.ReadAllAsync(cancellationToken)) {
                    WorkOnData(data);
                }
            } catch (OperationCanceledException) {
                break;
            } catch (Exception e) {
                Logger2Event.Instance.Warning(this, $"Error:[{e.Message}] from *{e.Source}*.");
            }
        }
    }

    /// <summary>
    ///     Run the main thread to distribute data to sub threads.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to stop the thread.</param>
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

    public override void ShutDown() {
        _cancellationTokenSource.Cancel();
        _mainThreadTask.Wait();
        Task.WaitAll(_subThreadTasks.ToArray());
    }
}