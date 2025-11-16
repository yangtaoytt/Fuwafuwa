using Fuwafuwa.Core.Core.RegisterService.Others;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.RegisterService.Register;

/// <summary>
///     The interface for register buffers.
///     Used internally by the register to manage services.
/// </summary>
internal interface IRegisterBuffer {
    /// <summary>
    ///     The method to reset the service in the buffer.
    /// </summary>
    /// <param name="service">the new reference.</param>
    void ResetService(IServiceReference? service);
}

/// <summary>
///     The register buffer for a specific service type.
/// </summary>
/// <typeparam name="TService">The specific service type.</typeparam>
public class RegisterBuffer<TService> : IRegisterBuffer
    where TService : class, IService<TService> {
    private readonly AsyncSharedDataWrapper<Ref<TService?>> _service;

    public RegisterBuffer(TService? service) {
        _service = new AsyncSharedDataWrapper<Ref<TService?>>(new Ref<TService?>(service));
    }

    /// <summary>
    ///     Resets the service in the buffer.
    ///     This method is called by the register when a service is added or removed.
    ///     User should not call this method directly.
    /// </summary>
    /// <param name="service">The new service.</param>
    public void ResetService(IServiceReference? service) {
        if (service == null) {
            ResetTService(null).Wait();
            return;
        }

        if (service.GetType().Name == typeof(TService).Name) {
            ResetTService((TService)service).Wait();
        }
    }

    /// <summary>
    ///     The method to reset the service reference.
    /// </summary>
    /// <param name="service">New Service.</param>
    private async Task ResetTService(TService? service) {
        await _service.ExecuteAsync(serviceRef => {
            serviceRef.Value = service;
            return Task.CompletedTask;
        });
    }

    /// <summary>
    ///     The method to execute an async action with the service.
    /// </summary>
    /// <param name="asyncAction">The action on service.</param>
    public async Task ExecuteAsync(Func<TService?, Task> asyncAction) {
        await _service.ExecuteAsync(serviceRef => Task.FromResult(asyncAction.Invoke(serviceRef.Value)));
    }

    /// <summary>
    ///     The method to execute an async function with the service.
    /// </summary>
    /// <param name="asyncFunc">The action on service.</param>
    /// <typeparam name="TResult">The return Type in Task.</typeparam>
    /// <returns>The result of action.</returns>
    public async Task<TResult> ExecuteAsync<TResult>(Func<TService?, Task<TResult>> asyncFunc) {
        return await _service.ExecuteAsync(serviceRef => asyncFunc.Invoke(serviceRef.Value));
    }

    /// <summary>
    ///     The method to execute a synchronous action with the service.
    /// </summary>
    /// <param name="action">The action on service.</param>
    public void Execute(Action<TService?> action) {
        _service.ExecuteAsync(serviceRef => {
                action.Invoke(serviceRef.Value);
                return Task.CompletedTask;
            })
            .Wait();
    }

    /// <summary>
    ///     The method to execute a synchronous function with the service.
    /// </summary>
    /// <param name="func">The action on service.</param>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <returns>The result of action.</returns>
    public TResult Execute<TResult>(Func<TService?, TResult> func) {
        return _service.ExecuteAsync(serviceRef => Task.FromResult(func.Invoke(serviceRef.Value))).Result;
    }
}