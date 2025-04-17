using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

public class Register {
    private Lock _lock;
    private Dictionary<string, IRegisterBuffer> _registerBuffers;
    private Dictionary<string, IServiceReference> _registerServices;
    public Register() {
        _registerBuffers = [];
        _registerServices = [];
        _lock = new Lock();
    }

    public Dictionary<string, IServiceReference> CopyRegisterServices() {
        lock (_lock) {
            return new Dictionary<string, IServiceReference>(_registerServices);
        }
    }
    
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
    
    public bool AddService(string serviceName,IServiceReference service) {
        lock (_lock) {
            if (_registerServices.ContainsKey(serviceName)) {
                return false;
            }
        
            _registerServices.Add(serviceName, service);
        
            if (_registerBuffers.TryGetValue(serviceName, out var buffer)) {
                buffer.ResetService(service);
            }
            return true;
        }
    }
    
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

    public void InitService(Dictionary<string, IServiceReference> registerServices) {
        lock (_lock) {
            _registerServices = registerServices;
            foreach (var (serviceName ,registerBuffer) in _registerBuffers) {
                registerBuffer.ResetService(null);
                if (_registerServices.TryGetValue(serviceName, out var serviceReference)) {
                    registerBuffer.ResetService(serviceReference);
                }
            }
        }
    }

    private TService? SearchService<TService>(string serviceName) 
    where TService : class, IService<TService> {
        if (_registerServices.TryGetValue(serviceName, out var service)) {
            return (TService)service;
        }
        return null;
    }
}