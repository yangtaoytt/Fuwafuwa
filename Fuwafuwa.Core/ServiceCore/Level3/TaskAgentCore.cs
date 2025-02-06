using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWapper.Implement;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level1;
using Fuwafuwa.Core.ServiceCore.Level2;

namespace Fuwafuwa.Core.ServiceCore.Level3;

public class TaskAgentCore : IFinalAbleServiceCore<TaskAgentData, NullSharedDataWrapper<object>, object> {
    public static IServiceAttribute<TaskAgentData> GetServiceAttribute() {
        return ITaskAgentAttribute.GetInstance();
    }
    public static NullSharedDataWrapper<object> Init(object initData) {
        return new NullSharedDataWrapper<object>(initData);
    }
    public static void Final(NullSharedDataWrapper<object> sharedData, Logger2Event? logger) { }
}