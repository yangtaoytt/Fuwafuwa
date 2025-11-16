using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategy.SimpleServiceStrategy;

/// <summary>
///     The simple single thread service strategy.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public class SimpleServiceStrategy<TService> : ASimpleServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    protected override void ReceiveInternal(IServiceData<TService, object> serviceData) {
        WorkOnData(serviceData);
    }

    protected override void WaitForCompletionInternal() { }
    protected override void ResumeInternal() { }
}