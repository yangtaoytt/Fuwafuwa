using Fuwafuwa.Core.Core.Service.Handle;
using Fuwafuwa.Core.Core.Service.Others.Distributor;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.Data;

/// <summary>
///     The abstract base class for processor service data.
///     The processor data can send to the service and get the result back.
///     The class implements the logic to find and call the corresponding service handler method,
///     and also implements the logic to return the result task to the caller.
/// </summary>
/// <typeparam name="TServiceData">Same as the IServiceData.</typeparam>
/// <typeparam name="TResult">The return type of this data.</typeparam>
/// <typeparam name="TService">The corresponding service which implements the handle of this data.</typeparam>
public abstract class AProcessorData<TServiceData, TResult, TService> :
    AServiceData<TService, TServiceData>
    where TService : IService<TService>, IProcessorHandler<TService, TServiceData, TResult>
    where TServiceData : AProcessorData<TServiceData, TResult, TService> {
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
    ///     Sends the data to the service and returns the result Task.
    ///     The reason you should pass the service to the data instead of passing the data to the service
    ///     is that the data knows the result type.
    ///     Otherwise, the service cannot return the correct result type.
    /// </summary>
    /// <param name="service">The reference of the corresponding service.</param>
    /// <returns>The task which contains the process result of service.</returns>
    public Task<TResult> Send(TService service) {
        service.Receive(this);
        return _taskSource.Task;
    }
}