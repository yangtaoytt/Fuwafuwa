using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;

namespace Fuwafuwa.Core.Container.Level3;

public class
    SubjectBufferContainer : BaseContainerWithRegister<SubjectBufferService, NullServiceData, SubjectData, object> {
    public SubjectBufferContainer(int serviceCount, DelSetDistribute setter,Logger2Event? logger) : base(serviceCount, setter, logger) { }
}