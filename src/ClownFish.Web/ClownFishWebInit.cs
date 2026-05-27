using System.Diagnostics.CodeAnalysis;
using ClownFish.Jwt;

namespace ClownFish.Web;

public static class ClownFishWebInit
{
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AuthOptions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ClownFishWebOptions))]
    public static void InitOptions()
    {
        AuthOptions.Init();
        DebugReport.RegisterOptionsType(typeof(ClownFish.Web.Security.Auth.AuthOptions));
        DebugReport.RegisterOptionsType(typeof(ClownFish.Web.ClownFishWebOptions));
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JwtOptions))]
    public static void InitAuth()
    {
        string hashName = Settings.GetSetting("ClownFish_JwtToken_AlgorithmName");
        if( hashName.IsNullOrEmpty() ) {
            hashName = JwtUtils.DefaultAlgorithm;
            Console2.Info("####################### 没有指定参数 ClownFish_JwtToken_AlgorithmName, ClownFish 将使用默认值：" + hashName);
        }

        JwtOptions jwtOptions = new JwtOptions {
            AlgorithmName = hashName,
            IssuerName = LocalSettings.GetSetting("ClownFish_JwtToken_IssuerName").IfEmpty(EnvUtils.GetAppName()),
            ShortTime = LocalSettings.GetBool("ClownFish_JwtToken_ShortTimeFormat", 1),
            ShortTypeName = LocalSettings.GetBool("ClownFish_JwtToken_ShortTypeName", 1),
            LoadUnknownUser = LocalSettings.GetBool("ClownFish_Authentication_LoadUnknownUserType", 0),
            VerifyTokenExpiration = LocalSettings.GetBool("ClownFish_JwtToken_VerifyExpiration", 1)
        };

        Console2.Info($"Jwt-Options: HashName={hashName}; ShortTimeFormat={jwtOptions.ShortTime}; ShortTypeName={jwtOptions.ShortTypeName}; LoadUnknownUserType={jwtOptions.LoadUnknownUser}; VerifyExpiration={jwtOptions.VerifyTokenExpiration}");

        // HMACSHA 系列HASH算法，它们只需要一个密钥就可以了
        if( jwtOptions.AlgorithmName.StartsWith0("HS") ) {
            string secretKey = Settings.GetSetting("ClownFish_Authentication_SecretKey");
            if( secretKey.IsNullOrEmpty() ) {
                Console2.Info("####################### 没有指定参数 ClownFish_Authentication_SecretKey, ClownFish 将使用随机密钥！");
                secretKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
            }

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
