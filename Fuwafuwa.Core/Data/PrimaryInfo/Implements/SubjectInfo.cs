using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.ExecuteTask;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Data.PrimaryInfo.Implements;

// send to child, pair with Data
public class SubjectInfo : IPrimaryInfo {
    public SubjectInfo(SubjectInfo? parent, int? index, int? siblingCount, Subject subject,
        ExecuteTaskSet applyTasks) {
        Parent = parent;
        Index = index;
        SiblingCount = siblingCount;
        Subject = subject;
        ApplyTasks = applyTasks;
    }

    // my parent
    public SubjectInfo? Parent { get; init; }

    // of me
    public int? Index { get; init; }

    // of me
    public int? SiblingCount { get; init; }

    public Subject Subject { get; init; }

    public ExecuteTaskSet ApplyTasks { get; init; }
}