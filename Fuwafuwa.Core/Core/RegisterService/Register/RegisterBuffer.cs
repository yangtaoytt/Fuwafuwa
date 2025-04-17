using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

interface IRegisterBuffer {
    void ResetService(IServiceReference? service);
}

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

    public TService? GetService() {
        lock (_lock) {
            return _service;
        }
    }

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