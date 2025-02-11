using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level2;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Service.Level2;

public class TaskAgentService : AServiceWithRegister<TaskAgentCore, TaskAgentData, NullSubjectData,
        NullSharedDataWrapper<object>, object, TaskAgentService, TaskAgentService>,
    IService<TaskAgentService, NullSharedDataWrapper<object>, object, TaskAgentService> {
    private TaskAgentService(Logger2Event? logger) : base(logger) { }

    public static TaskAgentService CreateService(Logger2Event? logger, TaskAgentService? uniqueService = null) {
        return new TaskAgentService(logger);
    }

    public static void Final(NullSharedDataWrapper<object> sharedData, Logger2Event? logger,
        TaskAgentService? uniqueService = null) {
        SubjectBufferCore.Final(sharedData, logger);
    }

    public static NullSharedDataWrapper<object> InitService(object initData, TaskAgentService? uniqueService = null) {
        return new NullSharedDataWrapper<object>(initData);
    }

    protected override async Task ProcessData(TaskAgentData serviceData, NullSubjectData subjectData,
        SimpleSharedDataWrapper<Register> register,
        NullSharedDataWrapper<object> sharedData) {
        Logger?.Debug(this, "ProcessTaskAgentData");
        var taskSet = serviceData.ExecuteTaskSet;
        var tasks = taskSet.GetTasks();

        foreach (var (type, taskList) in tasks) {
            var channelList = register.Execute(reg => reg.Value.GetTypeChannel(type));

            if (channelList.Count == 0) {
                continue;
            }

            var channel = channelList[0];

            foreach (var executorData in taskList) {
                await channel.Writer.WriteAsync(
                    (executorData, new NullSubjectData(), new NullRegisterData())
                );
            }
        }
    }
}