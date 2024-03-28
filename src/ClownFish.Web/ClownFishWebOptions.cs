namespace ClownFish.Web;
internal static class ClownFishWebOptions
{
    public static readonly int Framework_PerformanceThresholdMs = LocalSettings.GetInt("ASPNET_FrameworkBefore_PerformanceThresholdMs", 10);

    public static readonly bool DebugHttpLine = LocalSettings.GetBool("ClownFish_Aspnet_DebugHttpLine");

    public static readonly bool Show404Page = LocalSettings.GetBool("ClownFish_Aspnet_Show404Page");
    public static readonly bool DeleteUselessHeaders = LocalSettings.GetBool("ClownFish_Aspnet_DeleteUselessHeaders", 1);

    public static readonly bool LogExecutTime = LocalSettings.GetBool("ClownFish_Aspnet_LogExecutTime");

    public static readonly int MaxRequestBodySize = LocalSettings.GetUInt("AspNetCore_Kestrel_MaxRequestBodySize", 1080 * 1024);

    public static bool AlwaysShowFullException = LocalSettings.GetBool("ExceptionModule_AlwaysShowFullException", 1);







}
