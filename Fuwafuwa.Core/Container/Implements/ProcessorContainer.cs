using Fuwafuwa.Core.Container.Base;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Service.Base;

namespace Fuwafuwa.Core.Container.Implements;

public class
    ProcessorContainer<TService, TData> : BaseContainerWithRegister<TService, TData, SubjectInfoWithCommand,
    ProcessorContainer<TService, TData>> where TService : BaseProcessService<TData>, new()
    where TData : IData {
    public ProcessorContainer(int processorCount, DelSetDistribute setter) : base(processorCount, setter) { }
}