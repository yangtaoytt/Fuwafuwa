using System;
using System.Collections.Generic;
using System.Linq;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Data.ExecuteDataSet;

public class ExecuteDataSet
{
    // 修改数据结构为嵌套的SortedDictionary
    private readonly Dictionary<Type, SortedDictionary<Priority, List<AExecutorData>>> _tasks;

    public ExecuteDataSet()
    {
        _tasks = new Dictionary<Type, SortedDictionary<Priority, List<AExecutorData>>>();
    }

    public ExecuteDataSet(AExecutorData task) : this()
    {
        AddTask(task);
    }

    public ExecuteDataSet(List<AExecutorData> tasks) : this()
    {
        foreach (var task in tasks)
        {
            AddTask(task);
        }
    }

    public void AddTask(AExecutorData task)
    {
        // 获取或创建类型对应的SortedDictionary
        if (!_tasks.TryGetValue(task.ExecutorAttribute, out var priorityDict))
        {
            priorityDict = new SortedDictionary<Priority, List<AExecutorData>>();
            _tasks[task.ExecutorAttribute] = priorityDict;
        }

        // 获取或创建优先级对应的List
        if (!priorityDict.TryGetValue(task.Priority, out var taskList))
        {
            taskList = new List<AExecutorData>();
            priorityDict.Add(task.Priority, taskList);
        }

        // 将任务添加到对应优先级的List
        taskList.Add(task);
    }

    public void Combine(ExecuteDataSet otherSet)
    {
        foreach (var (type, otherDict) in otherSet._tasks)
        {
            if (!_tasks.TryGetValue(type, out var currentDict))
            {
                currentDict = new SortedDictionary<Priority, List<AExecutorData>>();
                _tasks[type] = currentDict;
            }

            foreach (var (priority, otherList) in otherDict)
            {
                if (!currentDict.TryGetValue(priority, out var currentList))
                {
                    currentList = new List<AExecutorData>();
                    currentDict[priority] = currentList;
                }
                currentList.AddRange(otherList);
            }
        }
    }

    public Dictionary<Type, List<AExecutorData>> GetTasks()
    {
        var result = new Dictionary<Type, List<AExecutorData>>();

        foreach (var (type, priorityDict) in _tasks)
        {
            var combinedList = new List<AExecutorData>();
            
            // 获取最高优先级组
            var highestPriorityGroup = priorityDict.First();
            var strategy = highestPriorityGroup.Key.Strategy;

            // 根据策略处理
            switch (strategy)
            {
                case PriorityStrategy.Unique:
                    // 取最高优先级的第一个任务（即使该优先级有多个任务）
                    combinedList.Add(highestPriorityGroup.Value.First());
                    break;
                
                case PriorityStrategy.Share:
                    // 合并所有标记为Share的优先级组
                    foreach (var group in priorityDict.Where(g => g.Key.Strategy == PriorityStrategy.Share))
                    {
                        combinedList.AddRange(group.Value);
                    }
                    break;
                
                default:
                    throw new InvalidOperationException($"未知的优先级策略: {strategy}");
            }

            result[type] = combinedList;
        }

        return result;
    }
}
