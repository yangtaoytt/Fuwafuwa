namespace Fuwafuwa.Core.Utils;

public static class Util {
    public static bool Is(Type type, Type baseType) {
        return baseType.IsAssignableFrom(type);
    }
}