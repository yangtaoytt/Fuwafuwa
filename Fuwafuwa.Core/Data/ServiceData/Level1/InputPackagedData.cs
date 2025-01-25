using Fuwafuwa.Core.Data.ServiceData.Level0;

namespace Fuwafuwa.Core.Data.ServiceData.Level1;

public class InputPackagedData : IServiceData {
    public InputPackagedData(object? packagedObject) {
        PackagedObject = packagedObject;
    }

    public object? PackagedObject { get; init; }
}