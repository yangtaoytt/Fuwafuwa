using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Data.ExecuteDataSet;

public class ExecuteDataSet {
    private readonly Dictionary<Type, SortedSet<AExecutorData>> _tasks;

    public ExecuteDataSet() {
        _tasks = new Dictionary<Type, SortedSet<AExecutorData>>();
    }

    public ExecuteDataSet(AExecutorData task) {
        _tasks = new Dictionary<Type, SortedSet<AExecutorData>>();
        AddTask(task);
    }

    public ExecuteDataSet(List<AExecutorData> tasks) {
        _tasks = new Dictionary<Type, SortedSet<AExecutorData>>();
        foreach (var task in tasks) {
            AddTask(task);
        }
    }

    public void AddTask(AExecutorData task) {
        if (!_tasks.ContainsKey(task.ExecutorAttribute)) {
            _tasks[task.ExecutorAttribute] = new SortedSet<AExecutorData>();
        }

        _tasks[task.ExecutorAttribute].Add(task);
    }

    public void Combine(ExecuteDataSet taskSet) {
        foreach (var (key, value) in taskSet._tasks) {
            if (!_tasks.ContainsKey(key)) {
                _tasks[key] = new SortedSet<AExecutorData>();
            }

            foreach (var task in value) {
                _tasks[key].Add(task);
            }
        }
    }

    public Dictionary<Type, List<AExecutorData>> GetTasks() {
        var result = new Dictionary<Type, List<AExecutorData>>();
        foreach (var (key, value) in _tasks) {
            result.Add(key, new List<AExecutorData>());

            var topTask = value.First();
            var priorityStrategy = topTask.Priority.Strategy;
            switch (priorityStrategy) {
                case PriorityStrategy.Unique:
                    result[key].Add(topTask);
                    break;
                case PriorityStrategy.Share:
                    result[key]
                        .AddRange(
                            value.Where(task => task.Priority.Strategy == PriorityStrategy.Share)
                        );
                    break;
                default:
                    throw new Exception("Unknown priority strategy");
            }
        }

        return result;
    }
}