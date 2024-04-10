namespace ClownFish.ImClients;

/// <summary>
/// NotifyUtils
/// </summary>
public sealed class NotifyUtils
{
    internal static readonly string ImAppAuthConfigName = "ImApp_Auth_Config";
    private static bool s_initOK = false;

    /// <summary>
    /// 
    /// </summary>
    public static void Init()
    {
        if( s_initOK )
            return;

        var config = LocalSettings.GetSetting<ImAppAuthConfig>(ImAppAuthConfigName, false);
        if( config == null ) {
            Console2.Info($"没有发现配置参数 [{ImAppAuthConfigName}]，所有经过IM的通知功能将不会启用！");
            return;
        }
        else {
            // 如果有配置企业微信，就检查配置是否有效。
            config.Validate();
            s_initOK = true;
        }
    }

    /// <summary>
    /// 发送一条文本消息给某个用户。 典型使用场景：给 用户A 发送验证码。
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="text"></param>
    public static void SendTextToUser(string userId, string text)
    {
        if( s_initOK == false || userId.IsNullOrEmpty() || text.IsNullOrEmpty() )
            return;

        try {
            ImAppMsgClient client = new ImAppMsgClient(ImAppAuthConfigName);
            client.SendText(userId, text);
        }
        catch( Exception ex ) {
            // 这只有异常只能忽略
            Console2.Error("调用 WxWorkNotify.SendTextToUser() 失败。", ex);
        }
    }
}
