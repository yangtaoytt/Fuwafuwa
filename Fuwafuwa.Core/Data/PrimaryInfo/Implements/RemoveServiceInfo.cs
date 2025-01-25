using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Data.PrimaryInfo.Implements;

public class RemoveServiceInfo : IPrimaryInfo {
    public RemoveServiceInfo(Type serviceType,
        IServiceRegisterGroup.UnregisterConfirmDelegate unregisterConfirmDelegate) {
        ServiceType = serviceType;
        UnregisterConfirmDelegate = unregisterConfirmDelegate;
    }

    public Type ServiceType { get; init; }


    public IServiceRegisterGroup.UnregisterConfirmDelegate UnregisterConfirmDelegate { get; init; }
}