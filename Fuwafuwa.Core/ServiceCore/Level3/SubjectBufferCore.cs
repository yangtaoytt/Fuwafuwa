using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level2;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level2;

namespace Fuwafuwa.Core.ServiceCore.Level3;

public class SubjectBufferCore : IFinalAbleServiceCore<NullServiceData, NullSharedDataWrapper<object>, object> {
    public static IServiceAttribute<NullServiceData> GetServiceAttribute() {
        return ISubjectBufferAttribute.GetInstance();
    }

    public static NullSharedDataWrapper<object> Init(object initData) {
        return new NullSharedDataWrapper<object>(initData);
    }

    public static void Final(NullSharedDataWrapper<object> sharedData, Logger2Event? logger) { }
}