using System.Diagnostics;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Service.Level2;

public class TaskAgentService : AServiceWithRegister<TaskAgentCore, TaskAgentData, NullSubjectData, object, object> {
    protected override async Task ProcessData(TaskAgentData serviceData, NullSubjectData subjectData, Register register,
        object sharedData) {
        Logger?.Debug(this, "ProcessTaskAgentData");
        var taskSet = serviceData.ExecuteTaskSet;
        var tasks = taskSet.GetTasks();

        foreach (var (type, taskList) in tasks) {
            var channelList = register.GetTypeChannel(type);

            Debug.Assert(channelList.Count == 1);
            var channel = channelList[0];

            foreach (var executorData in taskList) {
                await channel.Writer.WriteAsync(
                    (executorData, new NullSubjectData(), new NullRegisterData())
                );
            }
        }
    }

    protected override object SubInit(object initData) {
        return new object();
    }

    public override void Final(InitTuple<Register, object> sharedData, Logger2Event? logger) {
        base.Final(sharedData, logger);
        SubjectBufferCore.Final(sharedData, logger);
    }
}