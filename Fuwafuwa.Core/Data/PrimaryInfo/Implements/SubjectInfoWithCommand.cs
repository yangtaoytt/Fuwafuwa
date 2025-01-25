using Fuwafuwa.Core.ExecuteTask;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Data.PrimaryInfo.Implements;

public class SubjectInfoWithCommand : SubjectInfo {
    public SubjectInfoWithCommand(SubjectInfo? parent, int? index, int? siblingCount, Subject subject,
        ExecuteTaskSet applyTasks, int index4Child, int siblingCount4Child) : base(parent, index, siblingCount, subject,
        applyTasks) {
        Index4Child = index4Child;
        SiblingCount4Child = siblingCount4Child;
    }

    public int Index4Child { get; init; }
    public int SiblingCount4Child { get; init; }
}