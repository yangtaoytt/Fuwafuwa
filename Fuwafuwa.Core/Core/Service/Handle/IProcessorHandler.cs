using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.Handle;

/// <summary>
/// This interface defines a handler for processor-specific service data,
/// which helps to make the calling of service's data handler method automatically.
/// </summary>
/// <typeparam name="TService">The corresponding service of data.</typeparam>
/// <typeparam name="TServiceData">The data type to handle.</typeparam>
/// <typeparam name="TResult">The return type of the call.</typeparam>
public interface IProcessorHandler<TService, in TServiceData, out TResult> 
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