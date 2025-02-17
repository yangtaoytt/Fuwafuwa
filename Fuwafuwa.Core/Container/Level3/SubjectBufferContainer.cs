using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level2;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level3;

public sealed class
    SubjectBufferContainer : BaseContainerWithRegister<SubjectBufferCore, SubjectBufferService, NullServiceData,
    SubjectData, NullSharedDataWrapper<object>, object, SubjectBufferService> {
    public SubjectBufferContainer(int serviceCount, DelSetDistribute setter,
        (SimpleSharedDataWrapper<Register>, object) initData, Logger2Event? logger = null) : base(serviceCount, setter,
        initData, logger) { }
}