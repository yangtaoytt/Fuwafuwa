using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategy.ThreadSafeServiceStrategy;

/// <summary>
///     A service strategy that creates a new thread for each incoming service data.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public class DynamicThreadStrategy<TService> : AThreadSafeServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private readonly List<Task> _tasks = [];

    protected override void ReceiveInternal(IServiceData<TService, object> serviceData) {
        _tasks.Add(Task.Run(() => {
            try {
                WorkOnData(serviceData);
            } catch (Exception e) {
                Logger2Event.Instance.Error(this, "Error processing service data in DynamicThreadStrategy: " + e);
            }
        }));
    }

    protected override void WaitForCompletionInternal() {
        Task.WaitAll(_tasks.ToArray());
    }

    protected override void ResumeInternal() { }
}