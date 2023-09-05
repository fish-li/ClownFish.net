namespace ClownFish.Base;

// 4个“配置源”优先级的原因：
// App.config： 用于开发环境，避免暴露生产环境相关敏感信息。
// 配置服务：可用于在生产环境中【全局共享】的参数配置，它会覆盖开发环境的配置，用于保护敏感信息不泄漏
// 内存配置：从第三方配置服务中读取的设置，等同于“配置服务”
// 环境变量：每个应用【最终运行时】可指定的参数，用于覆盖全局共享的参数配置


/// <summary>
/// 供应用程序在运行时获取配置的工具类。
/// 参数项的读取顺序：环境变量，配置服务，App.config
/// </summary>
public static class Settings
{
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

        // 3，从配置服务中读取
        value = ConfigClient.Instance.GetSetting(name, false);
        if( string.IsNullOrEmpty(value) == false )
            return value;

        // 4，从AppConfig中读取
        value = AppConfig.GetSetting(name);
        if( string.IsNullOrEmpty(value) == false )
            return value;

        if( value.IsNullOrEmpty() && checkExist )
            throw new ConfigurationErrorsException("没有找到配置参数，Name：" + name);
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

        if( int.TryParse(value, out int result) )
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
        if( value > 0 )
            return value;

        throw new ConfigurationErrorsException($"{name} 对应的配置值 {value} 无效");
    }



}
