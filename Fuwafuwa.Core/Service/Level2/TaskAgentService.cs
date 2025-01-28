using System.Diagnostics;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Service.Level2;

public class TaskAgentService : AServiceWithRegister<TaskAgentData, NullSubjectData, object> {
    protected override async Task ProcessData(TaskAgentData serviceData, NullSubjectData subjectData, Register register,
        object sharedData, Logger2Event? logger) {
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

    public override IServiceAttribute<TaskAgentData> GetServiceAttribute() {
        return new ITaskAgentAttribute();
    }
}