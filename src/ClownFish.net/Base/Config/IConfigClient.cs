namespace ClownFish.Base;
/// <summary>
/// 配置服务的调用接口
/// </summary>
public interface IConfigClient
{
    /// <summary>
    /// 获取一个配置参数项
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string GetSetting(string name);

    /// <summary>
    /// 获取一个 应用库 的 数据库连接参数
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    DbConfig GetAppDbConfig(string name);

    /// <summary>
    /// 获取一个 租户库 的 数据库连接参数
    /// </summary>
    /// <param name="connType"></param>
    /// <param name="tenantId"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    DbConfig GetTntDbConfig(string connType, string tenantId, string flag);

    /// <summary>
    /// 获取一个配置文件的内容
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    string GetConfigFile(string filename);
}
