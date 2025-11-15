using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategry;

public class DynamicThreadStrategy<TService>: AServiceStrategy<TService> 
    where TService : AStrategyService<TService> {
    
    private readonly List<Task> _tasks = [];
    private bool _isRunning;

    protected override void StartInternal() {
        _isRunning = true;
    }

    public override void Receive(IServiceData<TService, object> serviceData) {
        if (_isRunning) {
            _tasks.Add(Task.Run(()=>{
                WorkOnData(serviceData);
            }));
        }
    }
    public override void ShutDown() {
        _isRunning = false;
        Task.WaitAll(_tasks.ToArray());
        Final();
    }
}