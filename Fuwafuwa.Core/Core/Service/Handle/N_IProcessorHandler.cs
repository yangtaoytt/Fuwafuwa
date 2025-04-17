using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

public interface N_IProcessorHandler<TService, in TServiceData, out TResult> 
    where TService : IService<TService>
    where TServiceData : IServiceData<TService, TServiceData> {
    /// <summary>
    /// Handles the data.
    /// The function contains the specific logic to handle the data.
    /// </summary>
    /// <param name="data">The specific data.</param>
    /// <returns>The result</returns>
    TResult Handle(TServiceData data);
}