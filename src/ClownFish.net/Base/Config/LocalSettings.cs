namespace ClownFish.Base;


/// <summary>
/// 读取配置的接口
/// </summary>
public interface ILocalSettings
{
    /// <summary>
    /// 获取一个配置参数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    string GetSetting(string name, bool checkExist);
}


internal sealed class DefaultLocalSettingsImpl : ILocalSettings
{
    public static readonly DefaultLocalSettingsImpl Instance = new DefaultLocalSettingsImpl();

    public string GetSetting(string name, bool checkExist)
    {
        if( string.IsNullOrEmpty(name) )
            throw new ArgumentNullException(nameof(name));

        // 1，从环境变量中读取
        string value = EnvironmentVariables.Get(name);
        if( value != null ) {
            if( value.Length > 0 ) {
                return value;
            }
            else {
                // 允许将环境变量的值设置为 "" 空字符串，这样可以覆盖（屏蔽）低级别的配置项，
                // 例如：app.config 中设置了 key1=abc
                //       实际运行时，不希望 key1 起作用，希望将它的值设置为 null
                //       那么可以在部署时，添加环境变量 'key1='   来实现
                return null;
            }
        }

        // 2，从内存中读取
        value = MemoryConfig.GetSetting(name);
        if( string.IsNullOrEmpty(value) == false )
            return value;

        // 3，从Windows注册表中读取配置参数
        value = WinRegSetting.GetSetting(name);
        if( string.IsNullOrEmpty(value) == false )
            return value;

        // 4，从AppConfig中读取
        if( AppConfig.Inited ) {  // 防止死循环调用
            value = AppConfig.GetSetting(name);
            if( string.IsNullOrEmpty(value) == false )
                return value;
        }


        if( checkExist )
            throw new ConfigurationErrorsException("没有找到参数项，Name：" + name);
        else
            return null;
    }
}

/// <summary>
/// 供应用程序在运行时获取配置的工具类。
/// 
/// 参数项的读取顺序：环境变量，MemoryConfig, AppConfig
/// </summary>
public static class LocalSettings
{
    private static ILocalSettings s_instance = DefaultLocalSettingsImpl.Instance;

    /// <summary>
    /// 设置实现方式
    /// </summary>
    /// <param name="instance"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void SetImpl(ILocalSettings instance)
    {
        s_instance = instance ?? DefaultLocalSettingsImpl.Instance;
    }

    /// <summary>
    /// 获取一个与指定名称匹配的配置参数值。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static string GetSetting(string name, bool checkExist = false)
    {
        return s_instance.GetSetting(name, checkExist);
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
