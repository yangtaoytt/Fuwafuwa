using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.Subjects;
using Fuwafuwa.Test.TestImplement.Attribute.Executor;
using Fuwafuwa.Test.TestImplement.Attribute.Processor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Processor;

public class AnotherStringProcessor : BaseProcessService<StringData, object> {
    public override IServiceAttribute<StringData> GetServiceAttribute() {
        return IReadString.GetInstance();
    }

    protected override Task<List<Certificate>> ProcessData(StringData data, object sharedData) {
        // Console.WriteLine(data.Data + " Into AnotherStringProcessor");

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
}