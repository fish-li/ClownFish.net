namespace ClownFish.Base;

/// <summary>
/// 读取环境变量的工具类
/// </summary>
public static class EnvironmentVariables
{
    /// <summary>
    /// 环境变量数据集合
    /// </summary>
    private static readonly Dictionary<string, string> s_dict = new Dictionary<string, string>(256, StringComparer.OrdinalIgnoreCase);

    static EnvironmentVariables()
    {
        // https://learn.microsoft.com/zh-cn/dotnet/api/system.environmentvariabletarget?view=net-6.0#remarks
        // 按文档说法，一般情况下，只需要读取 EnvironmentVariableTarget.Process 的环境变量就够了，

        // 但是，VS有个BUG，在单元测试项目的属性中添加的环境变量在运行时读取不到，
        // 所以为了方便使用环境变量，就直接把 Windows的 “系统/用户” 一起加载进来

        Fill(EnvironmentVariableTarget.Machine, s_dict);
        Fill(EnvironmentVariableTarget.User, s_dict);
        Fill(EnvironmentVariableTarget.Process, s_dict);

        if( s_dict.TryGet("DOTNET_RUNNING_IN_CONTAINER").TryToBool() && s_dict.ContainsKey("KUBERNETES_PORT") ) {
            int count1 = s_dict.Count;
            CleanK8sVariables(s_dict);    // 删除一些无用的环境变量

            int count2 = s_dict.Count;
            Console2.Info($"已忽略 {count1 - count2} 个K8S注入的环境变量");
        }


        // 从文件中加载环境变量
        // 注意：这里的文件名是固定的，并非像其它框架那样的名字，诸如： .env , .env.development , .env.production
        // 因为我认为：支持多个名字没有必要，反而会支持错误的配置方法！ 试问：你是要把这些文件签入代码仓库吗？？
        // 如果不是要签入代码仓库，就没有必要多个文件名，
        // 如果把 .env.production 这种文件签入代码仓库，那就非常非常SB了！！ 各种密钥全部都泄露了~~~

        // 事实上，生产环境用docker部署时，指定环境变量已经非常方便，根本不需要通过文件来指定
        // 目前通过文件来指定环境变量仅仅是为了方便开发阶段，因为有些与自己相关的敏感信息不适合放在【常规配置文件】中，
        // 此时可以把这些与自己相关的敏感参数放在 bin/_local.env，这样就不会签入仓库

        // 为什么要使用 _local.env 这件文件名，而不是  .env  ??
        // 回答：虽然有很多Linux下的程序都采用 .env 这个文件名，但是我认为这个文件名【不规范】！   所以不想跟风~~
        // app.evn 这个名字想过，但是想到或许未来会被MS采用，所以还是尽早避开了吧~~·

        string localEnvFilePath = Path.Combine(AppContext.BaseDirectory, "_local.env");
        int count3 = KvConfigFile.LoadFromFile(localEnvFilePath, s_dict);
        if( count3 > 0 ) {
            Console2.Info($"已从文件 {localEnvFilePath} 加载到 {count3} 个环境变量");
        }
    }

    /// <summary>
    /// 获取当前进程已加载的所有环境变量
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, string>> GetAll()
    {
        foreach( var x in s_dict ) {
            yield return new KeyValuePair<string, string>(x.Key, x.Value);
        }
    }

    internal static Dictionary<string, string> GetDictionary() => s_dict;   // 单元测试使用

    internal static void Init()
    {
        // 调用这个方法是为了触发 cctor
    }

    private static void Fill(EnvironmentVariableTarget target, Dictionary<string, string> dict)
    {
        foreach( DictionaryEntry kvp in Environment.GetEnvironmentVariables(target) ) {
            string key = kvp.Key?.ToString();

            if( key.IsNullOrEmpty() )
                continue;

            string value = kvp.Value?.ToString() ?? string.Empty;
            dict[key] = value;
        }
    }

    internal static void CleanK8sVariables(Dictionary<string, string> dict)
    {
        // 清理一些无用的K8S环境变量
        //KUBERNETES_PORT: tcp://172.21.0.1:443
        //KUBERNETES_PORT_443_TCP: tcp://172.21.0.1:443
        //KUBERNETES_PORT_443_TCP_ADDR: 172.21.0.1
        //KUBERNETES_PORT_443_TCP_PORT: 443
        //KUBERNETES_PORT_443_TCP_PROTO: tcp
        //KUBERNETES_SERVICE_HOST: 172.21.0.1
        //KUBERNETES_SERVICE_PORT: 443
        //KUBERNETES_SERVICE_PORT_HTTPS: 443

        // 而且，在K8S环境中，每个服务有7个环境变量来描述它的调用地址信息：
        // CONFIGSERVICE_PORT: tcp://172.21.0.119:80
        // CONFIGSERVICE_PORT_80_TCP: tcp://172.21.0.119:80
        // CONFIGSERVICE_PORT_80_TCP_ADDR: 172.21.0.119
        // CONFIGSERVICE_PORT_80_TCP_PORT: 80
        // CONFIGSERVICE_PORT_80_TCP_PROTO: tcp
        // CONFIGSERVICE_SERVICE_HOST: 172.21.0.119
        // CONFIGSERVICE_SERVICE_PORT: 80
        // 一个集群中，服务越多，这种无用的环境变量就越多，非常多，所以这里就把它们清理掉

        List<string> names = (from x in dict
                              let a = x.Key.LastIndexOf('_')
                              where a > 0
                              let m = x.Key.Substring(0, a + 1)  // 包含下划线
                              select m).ToList();

        foreach( var x in names ) {
            string v1 = dict.TryGet(x + "PORT");
            string v2 = dict.TryGet(x + "SERVICE_HOST");
            string v3 = dict.TryGet(x + "SERVICE_PORT");

            if( v1.HasValue() && v2.HasValue() && v3.HasValue() ) {

                List<string> list = (from z in dict
                                     where z.Key.StartsWith0(x)
                                     select z.Key).ToList();

                foreach( var d in list )
                    dict.Remove(d);
            }
        }
    }


    /// <summary>
    /// 从环境变量中读取一个配置参数值。
    /// </summary>
    /// <param name="name">参数名称，不区分大小写</param>
    /// <returns></returns>
    public static string Get(string name)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException(nameof(name));

        return s_dict.TryGet(name);
    }


    /// <summary>
    /// 修改内存中的环境变量参数值，【除非测试项目，否则不建议调用】
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void Set(string name, string value)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException(nameof(name));

        value = value ?? string.Empty;

        s_dict[name] = value;
    }

}
