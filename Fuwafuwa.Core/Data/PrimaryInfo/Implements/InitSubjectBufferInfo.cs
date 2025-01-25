using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.DataChannel;

namespace Fuwafuwa.Core.Data.PrimaryInfo.Implements;

public class InitSubjectBufferInfo : IPrimaryInfo {
    public InitSubjectBufferInfo(DataChannel<IData, IPrimaryInfo> subjectBufferEnterPoint) {
        SubjectBufferEnterPoint = subjectBufferEnterPoint;
    }

    public DataChannel<IData, IPrimaryInfo> SubjectBufferEnterPoint { get; init; }
}