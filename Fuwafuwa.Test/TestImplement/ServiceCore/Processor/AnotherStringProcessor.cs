using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.SharedDataWapper.Implement;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.Subjects;
using Fuwafuwa.Test.TestImplement.Attribute.Executor;
using Fuwafuwa.Test.TestImplement.Attribute.Processor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Processor;

public class AnotherStringProcessor : IProcessorCore<StringData, NullSharedDataWrapper<object>, object> {
    public static IServiceAttribute<StringData> GetServiceAttribute() {
        return IReadString.GetInstance();
    }
    
    public static NullSharedDataWrapper<object> Init(object initData) {
        return new NullSharedDataWrapper<object>(initData);
    }
    public static void Final(NullSharedDataWrapper<object> sharedData, Logger2Event? logger) { }
    public async Task<List<Certificate>> ProcessData(StringData data, NullSharedDataWrapper<object> sharedData, Logger2Event? logger) {
        await Task.CompletedTask;
        logger?.Debug(this, data.Data + " Into AnotherStringProcessor");
        return [
            IWriteToConsole.GetInstance()
                .GetCertificate(
                    new WriteToConsoleData(new Priority(3, PriorityStrategy.Share),
                        data.Data + "<from AnotherStringProcessor>")
                )
        ];
    }
}