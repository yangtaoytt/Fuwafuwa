namespace Fuwafuwa.Core.Utils;

static class Util {
    public static bool Is(Type type, Type baseType) {
        return baseType.IsAssignableFrom(type);
    }
}