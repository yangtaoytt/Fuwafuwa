using Fuwafuwa.Core.ExecuteTask.Abstract;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.ExecuteTask;

public class ExecuteTaskSet {
    private readonly Dictionary<Type, SortedSet<BaseExecuteTask>> _tasks;

    public ExecuteTaskSet() {
        _tasks = new Dictionary<Type, SortedSet<BaseExecuteTask>>();
    }

    public ExecuteTaskSet(BaseExecuteTask task) {
        _tasks = new Dictionary<Type, SortedSet<BaseExecuteTask>>();
        AddTask(task);
    }

    public ExecuteTaskSet(List<BaseExecuteTask> tasks) {
        _tasks = new Dictionary<Type, SortedSet<BaseExecuteTask>>();
        foreach (var task in tasks) {
            AddTask(task);
        }
    }

    public void AddTask(BaseExecuteTask task) {
        if (!_tasks.ContainsKey(task.ExecutorAttributeType)) {
            _tasks[task.ExecutorAttributeType] = new SortedSet<BaseExecuteTask>();
        }

        _tasks[task.ExecutorAttributeType].Add(task);
    }

    public void Combine(ExecuteTaskSet taskSet) {
        foreach (var (key, value) in taskSet._tasks) {
            if (!_tasks.ContainsKey(key)) {
                _tasks[key] = new SortedSet<BaseExecuteTask>();
            }

            foreach (var task in value) {
                _tasks[key].Add(task);
            }
        }
    }

    public Dictionary<Type, List<BaseExecuteTask>> GetTasks() {
        var result = new Dictionary<Type, List<BaseExecuteTask>>();
        foreach (var (key, value) in _tasks) {
            result.Add(key, new List<BaseExecuteTask>());

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