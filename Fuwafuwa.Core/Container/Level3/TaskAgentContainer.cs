using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level3;

public class TaskAgentContainer : BaseContainerWithRegister<TaskAgentCore, TaskAgentService, TaskAgentData,
    NullSubjectData, object, object> {
    public TaskAgentContainer(int serviceCount, DelSetDistribute setter, (Register, object) initData,
        Logger2Event? logger) : base(serviceCount, setter, initData, logger) { }
}