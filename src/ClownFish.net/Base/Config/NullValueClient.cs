namespace ClownFish.Base;

internal class NullValueClient : IConfigClient
{
    public static readonly NullValueClient Instance = new NullValueClient();

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
