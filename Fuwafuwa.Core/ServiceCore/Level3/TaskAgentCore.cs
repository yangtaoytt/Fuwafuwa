using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level2;

namespace Fuwafuwa.Core.ServiceCore.Level3;

public class TaskAgentCore : IFinalAbleServiceCore<TaskAgentData, object, object> {
    public static IServiceAttribute<TaskAgentData> GetServiceAttribute() {
        return ITaskAgentAttribute.GetInstance();
    }

    public static object Init(object initData) {
        return new object();
    }

    public static void Final(object sharedData, Logger2Event? logger) { }
}