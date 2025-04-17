using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.New.Data;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New;

public abstract class ServiceWithRegister<TService> : 
    AStaticThreadService<TService>,
    N_IProcessorHandler<ServiceWithRegister<TService>, AddRegisterData<TService>, bool>, 
    N_IProcessorHandler<ServiceWithRegister<TService>, RemoveRegisterData<TService>, bool>,
    N_IProcessorHandler<ServiceWithRegister<TService>, InitRegisterData<TService>, bool>
    where TService : ServiceWithRegister<TService> {
    protected readonly Register Register;


    protected ServiceWithRegister(ushort threadNumber) : base(threadNumber) {
        Register = new Register();
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

public class InitRegisterData<TService> : AProcessorData<InitRegisterData<TService>, bool, ServiceWithRegister<TService> >
    where TService : ServiceWithRegister<TService> {
    public readonly Dictionary<string, IServiceReference> Services;

    public InitRegisterData(Dictionary<string, IServiceReference> services) : base(new PollingDistributor()) {
        Services = services;
    }
    public override  InitRegisterData<TService> Implement() {
        return this;
    }
}

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