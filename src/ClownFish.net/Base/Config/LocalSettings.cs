using System.Runtime.InteropServices;

namespace ClownFish.Base;

/// <summary>
/// 供应用程序在运行时获取配置的工具类。
/// 
/// 参数项的读取顺序：环境变量，App.config
/// </summary>
public static class LocalSettings
{
    internal static readonly string RegPath = @"HKEY_CURRENT_USER\SOFTWARE\ClownFish_LocalSettings\" + Path.GetFileNameWithoutExtension(AsmHelper.GetEntryAssembly().Location);

    /// <summary>
    /// 获取一个与指定名称匹配的配置参数值。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static string GetSetting(string name, bool checkExist = false)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException(nameof(name));

        // 1，从环境变量中读取
        string value = EnvironmentVariables.Get(name);
        if( string.IsNullOrEmpty(value) == false )
            return value;

        // 2，从内存中读取
        value = MemoryConfig.GetSetting(name);
        if( string.IsNullOrEmpty(value) == false )
            return value;

        // 3，从AppConfig中读取
        value = AppConfig.GetSetting(name);
        if( string.IsNullOrEmpty(value) == false )
            return value;

#if NETFRAMEWORK || NET6_0_OR_GREATER

        // 为了方便开发环境：不想把一些敏感参数
        // 1，写到 app.config (避免被提交到代码仓库)
        // 2，或者放在 “本机的环境变量”   (Windows的环境变量配置是全局的，各个程序的参数放在一起太乱了)
        //  所以就把这些参数放在注册表中，并使用各自的 “AppName” 分开存储

        if( EnvUtils.IsDevEnv && RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) {
            value = Microsoft.Win32.Registry.GetValue(RegPath, name, null)?.ToString();

            if( string.IsNullOrEmpty(value) == false )
                return value;
        }
#endif

        if( checkExist )
            throw new ConfigurationErrorsException("没有找到参数项，Name：" + name);
        else
            return null;
    }



    /// <summary>
    /// 获取一个与指定名称匹配的配置参数值。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="defaultVal"></param>
    /// <returns></returns>
    public static string GetSetting(string name, string defaultVal)
    {
        string value = GetSetting(name);
        if( string.IsNullOrEmpty(value) == false )
            return value;

        return defaultVal;
    }

    /// <summary>
    /// 获取一个与指定名称匹配的配置参数值，并转换成指定的类型对象。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static T GetSetting<T>(string name, bool checkExist = false) where T : class, new()
    {
        string value = GetSetting(name, checkExist);

        return value.ToObject<T>();
    }


    /// <summary>
    /// 读取指定的配置参数，并转换成BOOL类型
    /// </summary>
    /// <param name="name"></param>
    /// <param name="defaultValue">当配置参数不存在时的默认值，1：true，0：false</param>
    /// <returns></returns>
    public static bool GetBool(string name, int defaultValue = 0)
    {
        string value = GetSetting(name);
        if( value.IsNullOrEmpty() )
            return defaultValue == 1;

        return value == "1" || value.Is("true");
    }

    /// <summary>
    /// 获取一个与指定名称匹配的配置参数值，并转换成整数。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int GetInt(string name, int defaultValue = 0)
    {
        string value = GetSetting(name);

        // 允许不指定，就用默认值返回
        if( string.IsNullOrEmpty(value) )
            return defaultValue;

        int result = 0;
        if( int.TryParse(value, out result) )
            return result;

        // 如果有指定设置，就必须是正确的！
        throw new ConfigurationErrorsException($"{name} 对应的配置值 {value} 无效");
    }


    /// <summary>
    /// 获取一个与指定名称匹配的配置参数值，并转换成正整数。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int GetUInt(string name, int defaultValue = 0)
    {
        int value = GetInt(name, defaultValue);
        if( value >= 0 )
            return value;

        throw new ConfigurationErrorsException($"{name} 对应的配置值 {value} 无效");
    }




}
