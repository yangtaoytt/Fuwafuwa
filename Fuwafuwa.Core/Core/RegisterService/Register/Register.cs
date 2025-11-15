using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.RegisterService.Register;

/// <summary>
///     The service register that manages service registrations and buffers.
/// </summary>
public class Register {
    private readonly Lock _lock = new();
    private readonly Dictionary<string, IRegisterBuffer> _registerBuffers = [];
    private Dictionary<string, IServiceReference> _registerServices = [];

    /// <summary>
    ///     The method to copy the registered services.
    /// </summary>
    /// <returns>The copy dic of this register.</returns>
    public Dictionary<string, IServiceReference> CopyRegisterServices() {
        lock (_lock) {
            return new Dictionary<string, IServiceReference>(_registerServices);
        }
    }

    /// <summary>
    ///     Creates or retrieves a register buffer for the specified service type.
    ///     Should be called by services that want to use registered services, and cache them.
    ///     User can use the buffer to get the service reference without locking the register every time.
    /// </summary>
    /// <typeparam name="TService">The type of service.</typeparam>
    /// <returns>The buffer of service.</returns>
    public RegisterBuffer<TService> CreateRegisterBuffer<TService>()
        where TService : class, IService<TService> {
        lock (_lock) {
            var serviceName = typeof(TService).Name;

            if (_registerBuffers.TryGetValue(serviceName, out var value)) {
                return (RegisterBuffer<TService>)value;
            }

            var registerBuffer = new RegisterBuffer<TService>(SearchService<TService>(serviceName));
            _registerBuffers.Add(serviceName, registerBuffer);

            return registerBuffer;
        }
    }

    /// <summary>
    ///     The method to add a service to the register.
    ///     Returns false if the service already exists.
    ///     Called by the service register manager when a service is added.
    /// </summary>
    /// <param name="serviceName">The name of service.</param>
    /// <param name="service">The service to be added.</param>
    /// <returns>The flag of result.</returns>
    public bool AddService(string serviceName, IServiceReference service) {
        lock (_lock) {
            if (!_registerServices.TryAdd(serviceName, service)) {
                return false;
            }

            if (_registerBuffers.TryGetValue(serviceName, out var buffer)) {
                buffer.ResetService(service);
            }

            return true;
        }
    }

    /// <summary>
    ///     Removes a service from the register by its name.
    ///     Called by the service register manager when a service is removed.
    /// </summary>
    /// <param name="serviceName">The service's name.</param>
    public void RemoveService(string serviceName) {
        lock (_lock) {
            if (_registerServices.ContainsKey(serviceName)) {
                _registerServices.Remove(serviceName);
            }

            if (_registerBuffers.TryGetValue(serviceName, out var buffer)) {
                buffer.ResetService(null);
            }
        }
    }

    /// <summary>
    ///     Initializes the register with a set of services.
    ///     Called by the service register manager when initializing.
    /// </summary>
    /// <param name="registerServices">The init or new service dic.</param>
    public void InitService(Dictionary<string, IServiceReference> registerServices) {
        lock (_lock) {
            _registerServices = registerServices;
            foreach (var (serviceName, registerBuffer) in _registerBuffers) {
                registerBuffer.ResetService(null);
                if (_registerServices.TryGetValue(serviceName, out var serviceReference)) {
                    registerBuffer.ResetService(serviceReference);
                }
            }
        }
    }

    /// <summary>
    ///     Finds a service by its name and casts it to the specified type.
    ///     Returns null if the service is not found or cannot be cast.
    /// </summary>
    /// <param name="serviceName">The name of service.</param>
    /// <typeparam name="TService">The return service type.</typeparam>
    /// <returns>The service.</returns>
    private TService? SearchService<TService>(string serviceName)
        where TService : class, IService<TService> {
        if (_registerServices.TryGetValue(serviceName, out var service)) {
            return (TService)service;
        }

        return null;
    }
}