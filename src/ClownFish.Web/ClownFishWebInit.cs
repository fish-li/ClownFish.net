using ClownFish.Jwt;

namespace ClownFish.Web;

public static class ClownFishWebInit
{
    public static void Init(bool initAuth)
    {
        AuthOptions.Init();
        DebugReport.OptionList.Add(typeof(ClownFish.Web.Security.Auth.AuthOptions));
        DebugReport.OptionList.Add(typeof(ClownFish.Web.ClownFishWebOptions));

        if( initAuth )
            InitAuth();
    }

    internal static void InitAuth()
    {
        JwtOptions jwtOptions = new JwtOptions {
            AlgorithmName = LocalSettings.GetSetting("ClownFish_JwtToken_AlgorithmName").IfEmpty(JwtUtils.DefaultAlgorithm),
            IssuerName = EnvUtils.GetAppName(),
            ShortTime = LocalSettings.GetBool("ClownFish_JwtToken_ShortTimeFormat", 1),
            ShortTypeName = LocalSettings.GetBool("ClownFish_JwtToken_ShortTypeName", 1),
            LoadUnknownUser = LocalSettings.GetBool("ClownFish_Authentication_LoadUnknownUserType", 0),
            VerifyTokenExpiration = LocalSettings.GetBool("ClownFish_JwtToken_VerifyExpiration", 1)
        };

        // HMACSHA 系列HASH算法，它们只需要一个密钥就可以了
        if( jwtOptions.AlgorithmName.StartsWith0("HS") ) {
            string secretKey = LocalSettings.GetSetting("ClownFish_Authentication_SecretKey", "defaultkey_618475f68e044243a3352881f1ab5c60");
            jwtOptions.HashKeyBytes = Encoding.UTF8.GetBytes(secretKey);
        }
        else {
            // 非对称算法，它们需要一个X509证书
            jwtOptions.X509Cert = GetAuthX509Cert();
        }

        JwtProvider provider = new JwtProvider(jwtOptions);
        AuthenticationManager.Init(provider, null);
    }

    public static X509Certificate2 GetAuthX509Cert()  // nebula也要调用这个方法
    {
        // 允许用户指定一个 “包含密码和证书” 的配置文件
        // 注意：这个方法要给Nebula调用，所以获取配置时，使用 Settings 而不是 LocalSettings
        string filename = Settings.GetSetting("ClownFish_Authentication_X509Conf_FileName", true);
        string configValue = ConfigFile.GetFile(filename, true);
        return X509Finder.LoadFromConfigFile(configValue);
    }
}
