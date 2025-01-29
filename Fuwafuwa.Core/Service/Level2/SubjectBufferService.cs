using System.Diagnostics;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.Collector.Implements;
using Fuwafuwa.Core.Data.Collector.Interface;
using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Service.Level2;

public class
    SubjectBufferService : AServiceWithRegister<SubjectBufferCore, NullServiceData, SubjectData, object, object> {
    private readonly Dictionary<Subject, ISubjectTaskCollector> _taskCollectors = new();

    protected override async Task ProcessData(NullServiceData serviceData, SubjectData subjectData, Register register,
        object sharedData) {
        Logger?.Debug(this, "ProcessSubjectBufferData");
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

    protected override object SubInit(object initData) {
        return new object();
    }

    public override void Final(InitTuple<Register, object> sharedData, Logger2Event? logger) {
        base.Final(sharedData, logger);
        SubjectBufferCore.Final(sharedData, logger);
    }
}