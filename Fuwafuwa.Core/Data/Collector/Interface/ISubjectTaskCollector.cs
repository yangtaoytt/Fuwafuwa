namespace Fuwafuwa.Core.Data.Collector.Interface;

public interface ISubjectTaskCollector {
    public void Collect(SubjectData.Level1.SubjectData subjectData);
    public bool CheckFinished();
    public ExecuteDataSet.ExecuteDataSet? GetTaskSet();
}