namespace ClownFish.Base;

internal static class ConstValues
{
    internal static readonly string CurrentVersion;

    public static readonly string HttpClientUserAgent;

    static ConstValues()
    {
        CurrentVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(ConstValues).Assembly.Location).FileVersion;

        HttpClientUserAgent = "ClownFish.HttpClient/" + CurrentVersion;
    }
}
