using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.Subjects;
using Fuwafuwa.Test.TestImplement.Attribute.Executor;
using Fuwafuwa.Test.TestImplement.Attribute.Processor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Processor;

public class StringProcessor : BaseProcessService<StringData, object> {
    protected override Task<List<Certificate>> ProcessData(StringData data, object sharedData) {
        // Console.WriteLine(data.Data + " Into StringProcessor");
        return Task.FromResult(
            new List<Certificate> {
                IWriteToConsole.GetInstance()
                    .GetCertificate(
                        new WriteToConsoleData(new Priority(1, PriorityStrategy.Share),
                            data.Data + "<from StringProcessor>")
                    )
            }
        );
    }

    public override IServiceAttribute<StringData> GetServiceAttribute() {
        return IReadString.GetInstance();
    }
}