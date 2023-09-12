namespace ClownFish.Rabbit;

internal static class RabbitCache
{
    // 其实用 RabbitOption 做缓存键也是可以正常运行的，但是有个小问题：
    // 有一种场景：不同的配置名称，对应的配置内容是一样的。 原本是希望连接独立的，
    // 但是在 RabbitOptionEqualityComparer 的实现中，没法区分，最后会导致共用连接。

    private static readonly TSafeDictionary<string, RabbitConnection> s_connectionCache
                    = new TSafeDictionary<string, RabbitConnection>(31, StringComparer.OrdinalIgnoreCase);

    private static readonly TSafeDictionary<string, RabbitOption> s_optionCache 
                        = new TSafeDictionary<string, RabbitOption>();


    static RabbitCache()
    {
        ClownFishInit.AppExitToken.Register(OnAppEnd);
    }

    private static void OnAppEnd()
    {
        // 关闭所有打开的长连接
        foreach( var kvp in s_connectionCache.Clone() ) {
            Console2.WriteLine($"Application exit, stop RabbitClient Connection: " + kvp.Key);
            kvp.Value.Dispose();
        }
        s_connectionCache.Clear();
    }

    public static RabbitConnection GetConnection(string settingName, string connectionName)
    {
        return s_connectionCache.GetOrAdd(connectionName, 
            x => {
                RabbitOption option = GetOption(settingName);
                string clientShowName = "RabbitClient-" + settingName;
                return new RabbitConnection(option, clientShowName);
        });
    }


    public static RabbitOption GetOption(string settingName)
    {
        if( settingName.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(settingName));

        return s_optionCache.GetOrAdd(settingName,
            x => {
                var option = Settings.GetSetting<RabbitOption>(x, true);
                option.Validate();
                return option;
            });
    }


}
