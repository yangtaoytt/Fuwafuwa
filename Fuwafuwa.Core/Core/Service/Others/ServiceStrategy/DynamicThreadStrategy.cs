using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

/// <summary>
///     Uses dynamic threads to process incoming service data.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public class DynamicThreadStrategy<TService> : AServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private readonly List<Task> _tasks = [];
    private bool _isRunning;

    protected override void StartInternal() {
        _isRunning = true;
    }

    public override void Receive(IServiceData<TService, object> serviceData) {
        if (_isRunning) {
            _tasks.Add(Task.Run(() => {
                try {
                    WorkOnData(serviceData);
                } catch (Exception e) {
                    Logger2Event.Instance.Error(this,
                        $"DynamicThreadStrategy<{typeof(TService).Name}> encountered an error while processing service data: \n{e}");
                }
            }));
        }
    }

    public override void ShutDown() {
        _isRunning = false;
        Task.WaitAll(_tasks.ToArray());
        Final();
    }
}