using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.Subjects;
using Fuwafuwa.Test.TestImplement.Attribute.Executor;
using Fuwafuwa.Test.TestImplement.Attribute.Processor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Processor;

public class AnotherStringProcessor : IProcessorCore<StringData, object, object> {
    public static IServiceAttribute<StringData> GetServiceAttribute() {
        return IReadString.GetInstance();
    }

    public static object Init(object initData) {
        return new object();
    }

    public Task<List<Certificate>> ProcessData(StringData data, object sharedData, Logger2Event? logger) {
        logger?.Debug(this, data.Data + " Into AnotherStringProcessor");
        return Task.FromResult(
            new List<Certificate> {
                IWriteToConsole.GetInstance()
                    .GetCertificate(
                        new WriteToConsoleData(new Priority(3, PriorityStrategy.Share),
                            data.Data + "<from AnotherStringProcessor>")
                    )
            }
        );
    }

    public static void Final(object sharedData, Logger2Event? logger) { }
}