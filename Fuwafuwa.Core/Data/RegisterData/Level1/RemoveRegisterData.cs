using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Data.RegisterData.Level1;

public class RemoveRegisterData : IRegisterData {
    public RemoveRegisterData((Type attributeType, Type serviceType) serviceType,
        ServiceRegisterGroup.RegisterUpdateConfirmDelegate unregisterConfirmDelegate) {
        ServiceType = serviceType;
        UnregisterConfirmDelegate = unregisterConfirmDelegate;
    }

    public (Type attributeType, Type serviceType) ServiceType { get; init; }


    public ServiceRegisterGroup.RegisterUpdateConfirmDelegate UnregisterConfirmDelegate { get; init; }
}