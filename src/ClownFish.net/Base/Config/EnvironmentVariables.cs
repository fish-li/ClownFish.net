using System.Collections.Generic;

namespace ClownFish.Base;

/// <summary>
/// 读取环境变量的工具类
/// </summary>
public static class EnvironmentVariables
{
    //========================================================================
    // 说明：由于历史原因，早期的配置参数名称的风格是：x.y.z
    // 后来感觉不方便：1，不能双击全选，2，“不符合”环境变量的约定命名风格，
    // 所以后面采用新的命名风格：x_y_z
    // 但是为了向后兼容，配置API要支持：用 x_y_z 的风格去读取 x.y.z 的配置参数
    // 因此，在API实现时，会在内存中保存2种风格的配置参数对象
    //========================================================================


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

        if( s_dict.TryGet("DOTNET_RUNNING_IN_CONTAINER").TryToBool() ) {
            CleanK8sHeaders(s_dict);
        }

        // 增加兼容KEY查找项
        List<string> names = s_dict.Where(x => x.Key.Contains('.')).Select(x => x.Key).ToList();
        foreach( var x in names ) {
            string key2 = x.GetConfName();
            string value = s_dict[x];
            s_dict[key2] = value;   // 供新命名风格查找
        }
    }


    internal static IEnumerable<KeyValuePair<string, string>> GetAll()
    {
        foreach( var x in s_dict ) {
            if( x.Key.Contains('.') == false )
                yield return new KeyValuePair<string, string>(x.Key, x.Value);
        }
    }

    internal static void Init()
    {
        // 不需要任何实现
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

    internal static void CleanK8sHeaders(Dictionary<string, string> dict)
    {
        // 清理一些无用的环境变量
        // 例如：在K8S环境中，会为每个服务增加7个变量：
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
        s_dict[name.GetConfName()] = value;
    }

}
