using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

abstract class N_AProcessorData<TService, TResult,TServiceData> : N_IServiceData<TService,TServiceData>
    where TService : N_IService<TService> , N_IProcessorHandle<TService,TServiceData,TResult>
    where TServiceData : N_AProcessorData<TService, TResult,TServiceData> {
    
    private readonly TaskCompletionSource<TResult> _taskSource = new();
    
    public void Accept(TService service) {
        TResult result;
        try {
            result = service.Handle(Implement());
        } catch (Exception e) {
            _taskSource.TrySetException(e);
            return;
        }

        _taskSource.TrySetResult(result);
    }

    public abstract ushort Distribute(N_DistributionData distributionData);
    public abstract TServiceData Implement();

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