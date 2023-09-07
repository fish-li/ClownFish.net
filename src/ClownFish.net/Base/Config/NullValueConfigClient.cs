namespace ClownFish.Base;

internal class NullValueConfigClient : IConfigClient
{
    public static readonly NullValueConfigClient Instance = new NullValueConfigClient();

    public string GetSetting(string name)
    {
        return null;
    }

    public string GetConfigFile(string filename)
    {
        return null;
    }

    public DbConfig GetAppDbConfig(string name)
    {
        return null;
    }

    public DbConfig GetTntDbConfig(string connType, string tenantId, string flag)
    {
        return null;
    }

}
