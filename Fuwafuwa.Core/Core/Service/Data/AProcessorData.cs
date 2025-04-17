using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

public abstract class AProcessorData<TServiceData, TResult,TService> : AServiceData< TService,  TServiceData>
    where TService : IService<TService> , N_IProcessorHandler<TService,TServiceData,TResult>
    where TServiceData : AProcessorData<TServiceData, TResult,TService> {
    
    private readonly TaskCompletionSource<TResult> _taskSource = new();

    protected AProcessorData(IDistributor distributor) : base(distributor) { }

    public override void Accept(TService service) {
        TResult result;
        try {
            result = service.Handle(Implement());
        } catch (Exception e) {
            _taskSource.TrySetException(e);
            return;
        }

        _taskSource.TrySetResult(result);
    }

    /// <summary>
    /// Sends the data to the service and returns the result Task.
    /// </summary>
    /// <param name="service">The reference of the corresponding service.</param>
    /// <returns>The task which contains the process result of service.</returns>
    public Task<TResult> Send(TService service) {
        service.Receive(this);
        return _taskSource.Task;
    }
}