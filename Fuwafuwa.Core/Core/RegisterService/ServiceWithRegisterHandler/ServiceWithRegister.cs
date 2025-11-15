using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Handle;
using Fuwafuwa.Core.Core.Service.Others.Distributor;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Core.Service.ServiceStrategry;

namespace Fuwafuwa.Core.Core.RegisterService.ServiceWithRegisterHandler;

/// <summary>
/// The base class for services that support registration functionality.
/// This class provides methods to add, remove, and initialize registered services.
/// </summary>
/// <typeparam name="TService">The subclass of this class.</typeparam>
public abstract class ServiceWithRegister<TService> : 
    AStrategyService<TService>,
    IProcessorHandler<ServiceWithRegister<TService>, AddRegisterData<TService>, bool>, 
    IProcessorHandler<ServiceWithRegister<TService>, RemoveRegisterData<TService>, bool>,
    IProcessorHandler<ServiceWithRegister<TService>, InitRegisterData<TService>, bool>
    where TService : ServiceWithRegister<TService> {
    protected readonly Register.Register Register;
    
    protected ServiceWithRegister(IServiceStrategy<TService> serviceStrategy) : base(serviceStrategy) {
        Register = new Register.Register();
    }
    
    public bool Handle(AddRegisterData<TService> data) {
        Register.AddService(data.ServiceName, data.Service);
        return true;
    }

    public bool Handle(RemoveRegisterData<TService> data) {
        Register.RemoveService(data.ServiceName);
        return true;
    }

    public bool Handle(InitRegisterData<TService> data) {
        Register.InitService(data.Services);
        return true;
    }
}

/// <summary>
/// The data structure for initializing registered services.
/// </summary>
/// <typeparam name="TService">The service with register.</typeparam>
public class InitRegisterData<TService> : AProcessorData<InitRegisterData<TService>, bool, ServiceWithRegister<TService> >
    where TService : ServiceWithRegister<TService> {
    public readonly Dictionary<string, IServiceReference> Services;

    public InitRegisterData(Dictionary<string, IServiceReference> services) : base(new PollingDistributor()) {
        Services = services;
    }
    public override InitRegisterData<TService> Implement() {
        return this;
    }
}

/// <summary>
/// The data structure for adding a registered service.
/// </summary>
/// <typeparam name="TService">The service with register.</typeparam>
public class AddRegisterData<TService> : AProcessorData<AddRegisterData<TService>,bool,ServiceWithRegister<TService>> 
    where TService : ServiceWithRegister<TService> {
    public readonly string ServiceName;
    public readonly IServiceReference Service;

    public AddRegisterData(string serviceName, IServiceReference service) : base(new PollingDistributor()) {
        ServiceName = serviceName;
        Service = service;
    }
    
    public override AddRegisterData<TService> Implement() {
        return this;
    }
}

/// <summary>
/// The data structure for removing a registered service.
/// </summary>
/// <typeparam name="TService">The service with register.</typeparam>
public class RemoveRegisterData<TService>: AProcessorData<RemoveRegisterData<TService>, bool, ServiceWithRegister<TService>> 
    where TService : ServiceWithRegister<TService> {
    public readonly string ServiceName;
    public RemoveRegisterData(string serviceName) : base(new PollingDistributor()) {
        ServiceName = serviceName;
    }
    
    public override RemoveRegisterData<TService> Implement() {
        return this;
    }
}