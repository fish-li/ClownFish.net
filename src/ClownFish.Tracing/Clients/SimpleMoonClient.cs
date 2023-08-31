//namespace ClownFish.Tracing.Clients;

//internal class SimpleMoonClient : IConfigClient
//{
//    private static string s_configServiceUrl;
//    private static readonly int s_httpTimeout = LocalSettings.GetUInt("Nebula_ConfigClient_HttpTimeout", 15_000);

//    public static readonly SimpleMoonClient Instance = new SimpleMoonClient();

//    internal static bool Init()
//    {
//        s_configServiceUrl = LocalSettings.GetSetting("ConfigServiceUrl", false).TrimEnd('/');
//        return s_configServiceUrl.HasValue();
//    }


//    public string GetSetting(string name)
//    {
//        HttpOption httpOption = new HttpOption {
//            Url = s_configServiceUrl + "/v20/api/moon/setting/get?name=" + name.UrlEncode(),
//            Timeout = s_httpTimeout
//        };

//        return httpOption.GetResult(HttpRetry.Create());
//    }

//    public DbConfig GetAppDbConfig(string name)
//    {
//        HttpOption httpOption = new HttpOption {
//            Url = s_configServiceUrl + $"/v20/api/moon/database/appdb?name=" + name.UrlEncode(),
//            Timeout = s_httpTimeout
//        };

//        return httpOption.GetResult<DbConfig>(HttpRetry.Create());
//    }

//    public DbConfig GetTntDbConfig(string connType, string tenantId, string flag)
//    {
//        throw new NotImplementedException();
//    }

//    public string GetConfigFile(string filename)
//    {
//        throw new NotImplementedException();
//    }
//}
