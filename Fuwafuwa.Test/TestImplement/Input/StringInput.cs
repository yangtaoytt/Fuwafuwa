using Fuwafuwa.Core.Attributes.ServiceAttribute.Level0;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Test.TestImplement.Attribute.Processor;
using Fuwafuwa.Test.TestImplement.Data;

namespace Fuwafuwa.Test.TestImplement.Input;

public class StringInput : BaseInputService<object> {
    protected override Task<List<Certificate>> ProcessData(InputPackagedData data, object sharedData) {
        var inputMessage = (string)data.PackagedObject;

        var stringData = new StringData(inputMessage);

        // Console.WriteLine(stringData.Data + " Into StringInput");

        return Task.FromResult(new List<Certificate> {
            IReadString.GetInstance().GetCertificate(stringData)
        });
    }
}