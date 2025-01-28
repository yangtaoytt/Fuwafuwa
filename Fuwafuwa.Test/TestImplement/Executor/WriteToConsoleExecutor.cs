using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Test.TestImplement.Attribute.Executor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Executor;

public class WriteToConsoleExecutor : BaseExecutorService<WriteToConsoleData, object> {

    public override IServiceAttribute<WriteToConsoleData> GetServiceAttribute() {
        return IWriteToConsole.GetInstance();
    }

    protected override Task ExecuteTask(WriteToConsoleData data, object sharedData, Logger2Event? logger) {
        Console.WriteLine(data.Message);
        return Task.CompletedTask;
    }
}