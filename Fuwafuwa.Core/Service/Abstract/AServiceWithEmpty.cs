using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;

namespace Fuwafuwa.Core.Service.Abstract;

public abstract class AServiceWithEmpty<TData, TPrimaryInfo> : AService<TData, TPrimaryInfo, object>
    where TData : IData where TPrimaryInfo : IPrimaryInfo {
    protected override void Init(object initObject) { }

    protected override bool InitCheck() {
        return true;
    }

    public override object CreateInitObject() {
        return new object();
    }
}