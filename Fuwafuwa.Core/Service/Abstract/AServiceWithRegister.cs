using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Service.Abstract;

public abstract class AServiceWithRegister<TData, TPrimaryInfo> : AService<TData, TPrimaryInfo, Register>
    where TData : IData where TPrimaryInfo : IPrimaryInfo {
    protected Register? Register;

    protected override void Init(Register initObject) {
        Register = initObject;
    }

    protected override bool InitCheck() {
        return Register != null;
    }

    public override Register CreateInitObject() {
        return new Register();
    }
}