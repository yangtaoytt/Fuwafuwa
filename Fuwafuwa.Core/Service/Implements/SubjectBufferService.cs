using System.Diagnostics;
using Fuwafuwa.Core.Attributes.Implements;
using Fuwafuwa.Core.Data.DataObject;
using Fuwafuwa.Core.Data.Implements;
using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.Data.PrimaryInfo.Implements;
using Fuwafuwa.Core.Data.PrimaryInfo.Interface;
using Fuwafuwa.Core.ExecuteTask.Implements;
using Fuwafuwa.Core.ExecuteTask.Interface;
using Fuwafuwa.Core.Service.Abstract;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Service.Implements;

public class SubjectBufferService : AServiceWithRegister<EmptyData, SubjectInfo>, ISubjectBufferAttribute {
    private readonly Dictionary<Subject, ISubjectTaskCollector> _taskCollectors = new();

    protected override async Task ProcessDataObject(DataObject<EmptyData, SubjectInfo> dataObject) {
        var subjectInfo = dataObject.PrimaryInfo;
        var subject = subjectInfo.Subject;
        if (!_taskCollectors.TryGetValue(subject, out var collector)) {
            collector = new FractionCollector();
            _taskCollectors[subject] = collector;
        }

        collector.Collect(subjectInfo);
        if (collector.CheckFinished()) {
            var taskSet = collector.GetTaskSet()!;
            _taskCollectors.Remove(subject);

            var taskAgentChannelList = Register!.GetTypeChannel(typeof(ITaskAgentAttribute));
            Debug.Assert(taskAgentChannelList.Count == 1);
            var taskAgentChannel = taskAgentChannelList[0];

            await taskAgentChannel.Writer.WriteAsync(
                new DataObject<IData, IPrimaryInfo>(new TaskAgentData(taskSet), new EmptyInfo())
            );
        }
    }
}