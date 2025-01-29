using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Attributes.ServiceAttribute.Level1;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Test.TestImplement.Attribute.Processor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Input;

public class StringInput : IInputCore<object, object> {
    public Task<List<Certificate>> ProcessData(InputPackagedData data, object sharedData,Lock sharedDataLock, Logger2Event? logger) {
        var inputMessage = (string)data.PackagedObject!;

        var stringData = new StringData(inputMessage);

        logger?.Debug(this, stringData.Data + " Into StringInput");

        return Task.FromResult(new List<Certificate> {
            IReadString.GetInstance().GetCertificate(stringData)
        });
    }

    public static IServiceAttribute<InputPackagedData> GetServiceAttribute() {
        return IInputAttribute.GetInstance();
    }

    public static object Init(object initData) {
        return new object();
    }

    public static void Final(object sharedData, Logger2Event? logger) { }
}