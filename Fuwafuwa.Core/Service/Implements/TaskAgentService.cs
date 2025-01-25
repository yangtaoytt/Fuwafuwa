using System.Diagnostics;
using Fuwafuwa.Core.Attributes.Implements;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.Service.Abstract;

namespace Fuwafuwa.Core.Service.Implements;

public class TaskAgentService : AServiceWithRegister<TaskAgentData, EmptyInfo>, ITaskAgentAttribute {
    protected override async Task ProcessDataObject(DataObject<TaskAgentData, EmptyInfo> dataObject) {
        var taskAgentData = dataObject.Data;
        var taskSet = taskAgentData.ExecuteTaskSet;
        var tasks = taskSet.GetTasks();

        foreach (var (type, taskList) in tasks) {
            var channelList = Register!.GetTypeChannel(type);

            Debug.Assert(channelList.Count == 1);
            var channel = channelList[0];

            foreach (var task in taskList) {
                await channel.Writer.WriteAsync(
                    new DataObject<IData, IPrimaryInfo>(new ExecuteTaskData(task), new EmptyInfo())
                );
            }
        }
    }
}