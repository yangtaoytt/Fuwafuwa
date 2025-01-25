using Fuwafuwa.Core.Data.ServiceData.Level1;

namespace Fuwafuwa.Test.TestImplement.Data;

public class StringData : IProcessorData {
    public StringData(string data) {
        Data = data;
    }

    public string Data { get; }
}