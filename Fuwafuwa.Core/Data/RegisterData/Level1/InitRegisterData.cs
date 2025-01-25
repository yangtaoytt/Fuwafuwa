using Fuwafuwa.Core.Data.RegisterData.Level0;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Data.RegisterData.Level1;

public class InitRegisterData : IRegisterData {
    public InitRegisterData(Register initRegister,
        ServiceRegisterGroup.RegisterInitConfirmDelegate initConfirmDelegate) {
        InitRegister = initRegister;
        InitConfirmDelegate = initConfirmDelegate;
    }

    public Register InitRegister { get; init; }

    public ServiceRegisterGroup.RegisterInitConfirmDelegate InitConfirmDelegate { get; init; }
}