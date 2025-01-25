using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Data.SubjectData.Level1;

public class SubjectData : ISubjectData {
    // my parent
    public SubjectData(SubjectData? parent, int? index, int? siblingCount, Subject subject,
        ExecuteDataSet.ExecuteDataSet applyTasks) {
        Parent = parent;
        Index = index;
        SiblingCount = siblingCount;
        Subject = subject;
        ApplyTasks = applyTasks;
    }

    public SubjectData? Parent { get; init; }

    // of me
    public int? Index { get; init; }

    // of me
    public int? SiblingCount { get; init; }

    public Subject Subject { get; init; }

    public ExecuteDataSet.ExecuteDataSet ApplyTasks { get; init; }
}