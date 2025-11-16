using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

public class DynamicThreadStrategy <TService> : AServiceStrategy<TService>
    where TService : AStrategyService<TService>{
    private readonly List<Task> _tasks = [];

    protected override void ReceiveInternal(IServiceData<TService, object> serviceData) {
        _tasks.Add(Task.Run(() => {
            try {
                WorkOnData(serviceData);
            } catch (Exception e) {
                Logger2Event.Instance.Error(this,
                    $"DynamicThreadStrategy<{typeof(TService).Name}> encountered an error while processing service data: \n{e}");
            }
        }));
    }

    protected override void WaitForCompletionInternal() {
        Task.WaitAll(_tasks.ToArray());
    }

    protected override void ResumeInternal() { }
}