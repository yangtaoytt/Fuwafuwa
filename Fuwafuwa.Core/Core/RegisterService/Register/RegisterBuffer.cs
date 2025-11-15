using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.RegisterService.Register;

/// <summary>
/// The interface for register buffers.
/// Used internally by the register to manage services.
/// </summary>
interface IRegisterBuffer {
    /// <summary>
    /// The method to reset the service in the buffer.
    /// </summary>
    /// <param name="service">the new reference.</param>
    void ResetService(IServiceReference? service);
}

/// <summary>
/// The register buffer for a specific service type.
/// </summary>
/// <typeparam name="TService">The specific service type.</typeparam>
public class RegisterBuffer<TService> : IRegisterBuffer
    where TService : class, IService<TService> {
    private readonly Lock _lock;
    private TService? _service;

    public RegisterBuffer(TService? service) {
        _service = service;
        _lock = new Lock();
    }
    
    private void ResetTService(TService? service) {
        lock (_lock) {
            _service = service;
        }
    }

    /// <summary>
    /// Gets the service from the buffer.
    /// This method is thread-safe.
    /// If the service is not available, it returns null.
    /// </summary>
    /// <returns>The service</returns>
    public TService? GetService() {
        lock (_lock) {
            return _service;
        }
    }

    /// <summary>
    /// Resets the service in the buffer.
    /// This method is called by the register when a service is added or removed.
    /// User should not call this method directly.
    /// </summary>
    /// <param name="service"></param>
    public void ResetService(IServiceReference? service) {
        if (service == null) {
            ResetTService(null);
            return;
        }
        if (service.GetType().Name == typeof(TService).Name) {
            ResetTService((TService)service);
        }
    }
}