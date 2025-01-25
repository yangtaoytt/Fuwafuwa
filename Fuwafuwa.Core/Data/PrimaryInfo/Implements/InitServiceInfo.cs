using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Data.PrimaryInfo.Implements;

public class InitServiceInfo : IPrimaryInfo {
    public InitServiceInfo(Register initRegister) {
        InitRegister = initRegister;
    }

    public Register InitRegister { get; init; }
}