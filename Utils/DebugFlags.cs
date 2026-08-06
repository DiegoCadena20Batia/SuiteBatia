
namespace BatiaSuite.Utils;

public static class DebugFlags {
#if DEBUG
    public static bool EsDebug => true;
#else
    public static bool EsDebug => false;
#endif
}