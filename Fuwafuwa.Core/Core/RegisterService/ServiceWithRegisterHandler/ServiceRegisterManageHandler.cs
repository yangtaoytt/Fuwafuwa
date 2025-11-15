using Fuwafuwa.Core.Core.RegisterService.Others;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.RegisterService.ServiceWithRegisterHandler;

/// <summary>
///     The handler that manages service registration and notifies services of changes.
/// </summary>
public class ServiceRegisterManageHandler {
    private readonly Lock _lock = new();
    private readonly Register.Register _register = new();
    private readonly Dictionary<string, (AddAction addAction, RemoveAction removeAction)> _serviceAction = new();

    /// <summary>
    ///     The method to add a service and notify all registered services.
    /// </summary>
    /// <param name="service">The service to add.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns>The task of the async progress.</returns>
    public Task AddServiceAsync<TService>(ServiceWithRegister<TService> service)
        where TService : ServiceWithRegister<TService> {
        var serviceName = typeof(TService).Name;

        return Task.Run(() => {
            lock (_lock) {
                if (_serviceAction.ContainsKey(serviceName)) {
                    throw new ServiceRegisterDuplicateException(serviceName);
                }

                foreach (var (_, action) in _serviceAction) {
                    action.addAction(serviceName, service);
                }

                _serviceAction.Add(serviceName, (
                    async (name, reference) => {
                        var addServiceData = new AddRegisterData<TService>(name, reference);
                        await addServiceData.Send(service);
                    },
                    async name => {
                        var removeServiceData = new RemoveRegisterData<TService>(name);
                        await removeServiceData.Send(service);
                    }));

                _register.AddService(serviceName, service);
                var initServiceData = new InitRegisterData<TService>(_register.CopyRegisterServices());
                initServiceData.Send(service);
            }
        });
    }

    /// <summary>
    ///     The method to remove a service and notify all registered services.
    /// </summary>
    /// <param name="service">The service to remove.</param>
    /// <typeparam name="TService">The type of service.</typeparam>
    /// <returns>The task of the async progress.</returns>
    public Task RemoveServiceAsync<TService>(ServiceWithRegister<TService> service)
        where TService : ServiceWithRegister<TService> {
        var serviceName = typeof(TService).Name;
        return Task.Run(() => {
            lock (_lock) {
                if (!_serviceAction.ContainsKey(serviceName)) {
                    throw new ServiceRegisterNotFoundException(serviceName);
                }

                _serviceAction.Remove(serviceName);

                foreach (var (_, action) in _serviceAction) {
                    action.removeAction(serviceName);
                }

                _register.RemoveService(serviceName);
                var removeInitServiceData = new InitRegisterData<TService>(new Dictionary<string, IServiceReference>());
                removeInitServiceData.Send(service);
            }
        });
    }

    private delegate Task AddAction(string serviceName, IServiceReference service);

    private delegate Task RemoveAction(string serviceName);
}