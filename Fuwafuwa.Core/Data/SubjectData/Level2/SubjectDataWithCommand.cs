using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Data.SubjectData.Level2;

public class SubjectDataWithCommand : Level1.SubjectData {
    public SubjectDataWithCommand(Level1.SubjectData? parent, int? index, int? siblingCount, Subject subject,
        ExecuteDataSet.ExecuteDataSet applyTasks, int index4Child, int siblingCount4Child) : base(parent, index,
        siblingCount, subject,
        applyTasks) {
        Index4Child = index4Child;
        SiblingCount4Child = siblingCount4Child;
    }

    public int Index4Child { get; init; }
    public int SiblingCount4Child { get; init; }
}