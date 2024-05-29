using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ClownFish.Web.Security.Auth;

/// <summary>
/// 终端客户端的登录身份
/// </summary>
public class EndClientUserInfo : IUserInfo
{
    /// <summary>
    /// 租户ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// 租户CODE
    /// </summary>
    public string TenantCode { get; set; }

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// 运行时ID，用于日志分析时判断客户端有没有重新启动
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 客户端的程序名称
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 客户端版本
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 客户端的权限角色，供服务端授权使用
    /// </summary>
    public string ClientRole { get; set; }

    /// <summary>
    /// 当前的机器名
    /// </summary>
    public string HostName { get; set; }

    /// <summary>
    /// 操作系统类别，例如 1: windows, 2: linux, 3: others
    /// </summary>
    [DefaultValue(0)]
    public int OsKind { get; set; }

    /// <summary>
    /// 操作系统名称
    /// </summary>
    public string OsName { get; set; }

    /// <summary>
    /// CPU指令类别
    /// </summary>
    public string CpuKind { get; set; }

    /// <summary>
    /// 当前时区
    /// </summary>
    public string TimeZone { get; set; }

    /// <summary>
    /// 当前时区
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    /// 部署方式，此字段支持按位操作，类似于 Enum-Flags。
    /// 取值 1: docker, 2: SingleFile, 3: docker+SingleFile
    /// </summary>
    [DefaultValue(0)]
    public int DeployMode { get; set; }

    /// <summary>
    /// 扩展用户信息（可选）
    /// </summary>
    public string ClientData { get; set; }

    /// <summary>
    /// 扩展数据（可选）
    /// </summary>
    public string ExtData { get; set; }

    /// <summary>
    /// 特殊标记（可选）
    /// </summary>
    [DefaultValue(0)]
    public int GrayFlag { get; set; }


    public void Validate()
    {
        if( this.TenantId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(TenantId));

        if( this.ClientId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(ClientId));
    }

    [JsonIgnore]
    public string UserId => this.ClientId;
    [JsonIgnore]
    public string UserName => this.AppName ?? "UnknownClient";
    [JsonIgnore]
    public string UserRole => this.ClientRole ?? "Client";


    /// <summary>
    /// 创建一个新实例，并填充【部分】字段
    /// </summary>
    /// <returns></returns>
    public static EndClientUserInfo CreateNew()
    {
        return new EndClientUserInfo {
            AppId = EnvUtils.AppRuntimeId,
            AppName = EnvUtils.GetAppName(),
            HostName = EnvUtils.GetHostName(),
            OsKind = OsUtils.IsWindows ? 1 : (OsUtils.IsLinux ? 2 : 3),
            OsName = OsUtils.GetOsName(),
            CpuKind = RuntimeInformation.ProcessArchitecture.ToString(),
            DeployMode = ((EnvUtils.IsInDocker ? 1 : 0) | (AsmHelper.IsSingleFileDeploy ? 2 : 0)),
            TimeZone = MyTimeZone.CurrentTZ,
            Culture = System.Globalization.CultureInfo.CurrentCulture?.Name
        };
    }
}
