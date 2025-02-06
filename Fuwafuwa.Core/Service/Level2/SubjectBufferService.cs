using System.Diagnostics;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.Collector.Implements;
using Fuwafuwa.Core.Data.Collector.Interface;
using Fuwafuwa.Core.Data.RegisterData.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWapper.Implement;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Service.Level2;

public class
    SubjectBufferService :
    AServiceWithRegister<SubjectBufferCore, NullServiceData, SubjectData, NullSharedDataWrapper<object>, object,
        SubjectBufferService, SubjectBufferService>,
    IPrimitiveService<SubjectBufferService, NullSharedDataWrapper<object>, object, SubjectBufferService> {
    private readonly Dictionary<Subject, ISubjectTaskCollector> _taskCollectors = new();

    private SubjectBufferService(Logger2Event? logger) : base(logger) { }

    protected override async Task ProcessData(NullServiceData serviceData, SubjectData subjectData, SimpleSharedDataWrapper<Register> register,
        NullSharedDataWrapper<object> sharedData) {
        
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

            var taskAgentChannelList = register.Execute(reg => reg.Value.GetTypeChannel(typeof(ITaskAgentAttribute)));
            Debug.Assert(taskAgentChannelList.Count == 1);
            var taskAgentChannel = taskAgentChannelList[0];

            await taskAgentChannel.Writer.WriteAsync(
                (new TaskAgentData(taskSet), new NullSubjectData(), new NullRegisterData())
            );
        }
    }

    public static SubjectBufferService CreateService(Logger2Event? logger, SubjectBufferService? uniqueService = null) {
        return new SubjectBufferService(logger);
    }

    public static void FinalPrimitive(NullSharedDataWrapper<object> sharedData, Logger2Event? logger,
        SubjectBufferService? uniqueService = null) {
        SubjectBufferCore.Final(sharedData ,logger);
    }

    public static NullSharedDataWrapper<object> InitServicePrimitive(object initData, SubjectBufferService? uniqueService = null) {
        return new NullSharedDataWrapper<object>(initData);
    }
}