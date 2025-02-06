namespace Fuwafuwa.Core.Data.SharedDataWrapper.ReferenceBoxType;

public class Reference<T> {
    public Reference(T value) {
        Value = value;
    }
    public T Value { get; set; }
}