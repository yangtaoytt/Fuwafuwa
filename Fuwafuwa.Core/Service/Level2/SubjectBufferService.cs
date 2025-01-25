using System.Diagnostics;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.Collector.Implements;
using Fuwafuwa.Core.Data.Collector.Interface;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceRegister;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Service.Level2;

public class SubjectBufferService : AServiceWithRegister<NullServiceData, SubjectData, object> {
    private readonly Dictionary<Subject, ISubjectTaskCollector> _taskCollectors = new();

    protected override async Task ProcessData(NullServiceData serviceData, SubjectData subjectData, Register register,
        object sharedData) {
        var subject = subjectData.Subject;
        if (!_taskCollectors.TryGetValue(subject, out var collector)) {
            collector = new FractionCollector();
            _taskCollectors[subject] = collector;
        }

        collector.Collect(subjectData);
        if (collector.CheckFinished()) {
            var taskSet = collector.GetTaskSet()!;
            _taskCollectors.Remove(subject);

            var taskAgentChannelList = register.GetTypeChannel(typeof(ITaskAgentAttribute));
            Debug.Assert(taskAgentChannelList.Count == 1);
            var taskAgentChannel = taskAgentChannelList[0];

            await taskAgentChannel.Writer.WriteAsync(
                (new TaskAgentData(taskSet), new NullSubjectData(), new NullRegisterData())
            );
        }
    }

    public override IServiceAttribute<NullServiceData> GetServiceAttribute() {
        return new ISubjectBufferAttribute();
    }
}