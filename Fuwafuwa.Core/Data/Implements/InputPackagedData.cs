using Fuwafuwa.Core.Data.Interface;

namespace Fuwafuwa.Core.Data.Implements;

public class InputPackagedData : IData {
    public InputPackagedData(object? packagedObject) {
        PackagedObject = packagedObject;
    }

    public object? PackagedObject { get; init; }
}