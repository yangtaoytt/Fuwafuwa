using Fuwafuwa.Core.New.Data;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

public class ServiceRegisterManageHandler {
    private delegate Task AddAction(string serviceName, IServiceReference service);
    private delegate Task RemoveAction(string serviceName);
    
    private readonly Lock _lock = new();
    private readonly Dictionary<string, (AddAction addAction,RemoveAction removeAction)> _serviceAction = new();
    private readonly Register _register = new();

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
                    async (name) => {
                        var removeServiceData = new RemoveRegisterData<TService>(name);
                        await removeServiceData.Send(service);
                    }));

                _register.AddService(serviceName, service);
                var initServiceData = new InitRegisterData<TService>(_register.CopyRegisterServices());
                initServiceData.Send(service);
            }
        });
    }
    
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
}
