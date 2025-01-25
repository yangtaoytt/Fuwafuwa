using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Service.Level2;

namespace Fuwafuwa.Core.Container.Level3;

public class TaskAgentContainer : BaseContainerWithRegister<TaskAgentService, TaskAgentData, NullSubjectData, object> {
    public TaskAgentContainer(int serviceCount, DelSetDistribute setter) : base(serviceCount, setter) { }
}