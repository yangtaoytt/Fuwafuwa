using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Test.TestImplement.Attribute.Executor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Executor;

public class WriteToConsoleExecutor : IExecutorCore<WriteToConsoleData, object, object> {
    public static IServiceAttribute<WriteToConsoleData> GetServiceAttribute() {
        return IWriteToConsole.GetInstance();
    }

    public static object Init(object initData) {
        return new object();
    }

    public Task ExecuteTask(WriteToConsoleData data, object sharedData,Lock sharedDataLock, Logger2Event? logger) {
        Console.WriteLine(data.Message);
        return Task.CompletedTask;
    }

    public static void Final(object sharedData, Logger2Event? logger) { }
}