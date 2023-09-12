namespace ClownFish.NRedis;

/// <summary>
/// Redis相关工具类
/// </summary>
public static class Redis
{
    // 参考链接：https://stackexchange.github.io/StackExchange.Redis/

    /// <summary>
    /// 默认连接配置名称
    /// </summary>
    public static string DefaultConnSettingName = "Redis_Connection";


    private static readonly object s_lock = new object();

    // 默认连接实例
    private static RedisClient s_client;

    // 多个连接实例的字典表
    private static TSafeDictionary<string, RedisClient> s_dict;

    private static RedisClient GetDefaultClient()
    {
        if( s_client == null ) {
            lock( s_lock ) {
                if( s_client == null ) {

                    s_client = new RedisClient(DefaultConnSettingName);
                }
            }
        }

        return s_client;
    }

    /// <summary>
    /// 获取 Redis 的 IDatabase 实例。  使用默认的连接配置"Redis.Connection"
    /// </summary>
    /// <param name="db"></param>
    /// <returns></returns>
    public static IDatabase GetDatabase(int db = 0)
    {
        return GetDefaultClient().GetDatabase(db);
    }


    /// <summary>
    /// 获取某个配置对应的RedisClient实例
    /// </summary>
    /// <param name="configName">在配置服务中的配置项名称，默认值："Redis.Connection"</param>
    /// <returns></returns>
    public static RedisClient GetClient(string configName = null)
    {
        if( configName.IsNullOrEmpty() || configName.Is(DefaultConnSettingName) )
            return GetDefaultClient();


        if( s_dict == null ) {
            lock( s_lock ) {
                if( s_dict == null ) {

                    s_dict = new TSafeDictionary<string, RedisClient>();
                }
            }
        }


        return s_dict.GetOrAdd(configName, x => { return new RedisClient(x); });
    }

}
