using Fuwafuwa.Core.Container.Base;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Service.Implements;

namespace Fuwafuwa.Core.Container.Implements;

public class
    TaskAgentContainer : BaseContainerWithRegister<TaskAgentService, TaskAgentData, EmptyInfo, TaskAgentContainer> {
    public TaskAgentContainer(int processorCount, DelSetDistribute setter) : base(processorCount, setter) { }
}