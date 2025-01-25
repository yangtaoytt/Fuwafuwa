using Fuwafuwa.Core.Container.Base;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Service.Implements;

namespace Fuwafuwa.Core.Container.Implements;

public class SubjectBufferContainer : BaseContainerWithRegister<SubjectBufferService, EmptyData, SubjectInfo,
    SubjectBufferContainer> {
    public SubjectBufferContainer(int processorCount, DelSetDistribute setter) : base(processorCount, setter) { }
}