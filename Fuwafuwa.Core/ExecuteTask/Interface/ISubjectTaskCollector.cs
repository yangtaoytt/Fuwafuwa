using Fuwafuwa.Core.Data.PrimaryInfo.Implements;

namespace Fuwafuwa.Core.ExecuteTask.Interface;

public interface ISubjectTaskCollector {
    public void Collect(SubjectInfo subjectInfo);
    public bool CheckFinished();
    public ExecuteTaskSet? GetTaskSet();
}