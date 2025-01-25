using Fuwafuwa.Core.Container.Base;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Service.Abstract;

namespace Fuwafuwa.Core.Container.Implements;

public class
    ExecutorContainer<TService> : BaseContainerWithoutRegister<TService, ExecuteTaskData, EmptyInfo,
    ExecutorContainer<TService>> where TService : AService<ExecuteTaskData, EmptyInfo, object>, new() {
    public ExecutorContainer(int processorCount, DelSetDistribute setter) : base(processorCount, setter) { }
}