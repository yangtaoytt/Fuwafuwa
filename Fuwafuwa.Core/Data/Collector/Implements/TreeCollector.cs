using System.Diagnostics;
using Fuwafuwa.Core.Data.Collector.Interface;

namespace Fuwafuwa.Core.Data.Collector.Implements;

public class TreeCollector : ISubjectTaskCollector {
    private readonly SubjectTaskTree _subjectTaskTree;

    public TreeCollector() {
        _subjectTaskTree = new SubjectTaskTree();
    }

    public void Collect(SubjectData.Level1.SubjectData subjectData) {
        var indexList = new List<int>();
        var siblingCountList = new List<int>();
        List<ExecuteDataSet.ExecuteDataSet> taskLists = new();

        var currentData = subjectData;
        while (currentData != null) {
            indexList.Insert(0, currentData.Index!.Value);
            siblingCountList.Insert(0, currentData.SiblingCount!.Value);
            taskLists.Insert(0, currentData.ApplyTasks);
            currentData = currentData.Parent;
        }

        _subjectTaskTree.Set(indexList, siblingCountList, taskLists);
    }

    public bool CheckFinished() {
        // Checks if the subject tree is finished
        return _subjectTaskTree.CheckFinished();
    }

    public ExecuteDataSet.ExecuteDataSet? GetTaskSet() {
        if (!CheckFinished()) {
            return null;
        }

        return _subjectTaskTree.GetTaskSet();
    }
}

public class SubjectTreeNode {
    public List<SubjectTreeNode> Children { get; set; } = new();
    public ExecuteDataSet.ExecuteDataSet? Value { get; set; }
}

public class SubjectTaskTree {
    public SubjectTreeNode FakeRoot { get; set; } = new();

    public void Set(List<int> position, List<int> siblingCount, List<ExecuteDataSet.ExecuteDataSet> taskList) {
        Debug.Assert(position.Count == siblingCount.Count);
        Debug.Assert(position.Count == taskList.Count);
        Debug.Assert(position.Count > 0);
        Debug.Assert(siblingCount[0] == 1);
        Debug.Assert(position[0] == 0);

        var currentParent = FakeRoot;
        for (var floor = 0; floor < position.Count; ++floor) {
            var index = position[floor];
            var count = siblingCount[floor];
            var task = taskList[floor];

            if (currentParent.Children.Count == 0) {
                for (var i = 0; i < count; ++i) {
                    currentParent.Children.Add(new SubjectTreeNode());
                }
            }

            Debug.Assert(count == currentParent.Children.Count);
            Debug.Assert(index < count);

            currentParent.Children[index].Value = task;
            currentParent = currentParent.Children[index];
        }
    }

    public bool CheckFinished() {
        return FakeRoot.Children.Count != 0 && CheckFinished(FakeRoot.Children[0]);
    }

    private bool CheckFinished(SubjectTreeNode node) {
        // the value is always before the children to be assigned, so only check the value when there is no children
        if (node.Children.Count == 0) {
            return node.Value != null;
        }

        var isFinished = true;
        foreach (var child in node.Children) {
            isFinished &= CheckFinished(child);
        }

        return isFinished;
    }

    public ExecuteDataSet.ExecuteDataSet? GetTaskSet() {
        if (!CheckFinished()) {
            return null;
        }

        return GetTaskSet(FakeRoot.Children[0]);
    }

    private ExecuteDataSet.ExecuteDataSet GetTaskSet(SubjectTreeNode node) {
        if (node.Children.Count == 0) {
            return node.Value!;
        }

        var result = new ExecuteDataSet.ExecuteDataSet();
        foreach (var child in node.Children) {
            result.Combine(GetTaskSet(child));
        }

        return result;
    }
}