using Fuwafuwa.Core.Data.PrimaryInfo.Implements;

namespace Fuwafuwa.Core.Container.Distributor;

public class HashDistributor<TPrimaryInfo> : IDistributor<TPrimaryInfo> where TPrimaryInfo : SubjectInfo {
    public int Distribute(int processorCount, TPrimaryInfo info) {
        var subjectId = info.Subject.UniqueId;

        return (int)(subjectId % processorCount);
    }
}