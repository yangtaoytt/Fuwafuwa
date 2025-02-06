using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.SharedDataWapper.Implement;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Test.TestImplement.Attribute.Executor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Executor;

public class WriteToConsoleExecutor : IExecutorCore<WriteToConsoleData, NullSharedDataWrapper<object>, object> {
    public static IServiceAttribute<WriteToConsoleData> GetServiceAttribute() {
        return IWriteToConsole.GetInstance();
    }
    
    public static NullSharedDataWrapper<object> Init(object initData) {
        return new NullSharedDataWrapper<object>(initData);
    }
    public static void Final(NullSharedDataWrapper<object> sharedData, Logger2Event? logger) { }
    public Task ExecuteTask(WriteToConsoleData data, NullSharedDataWrapper<object> sharedData, Logger2Event? logger) {
        Console.WriteLine(data.Message);
        return Task.CompletedTask;
    }
}