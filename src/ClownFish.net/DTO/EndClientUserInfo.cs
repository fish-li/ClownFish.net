using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ClownFish.DTO;

/// <summary>
/// 终端客户端的登录身份
/// </summary>
public class EndClientUserInfo : IUserInfo, IValidate
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
    /// 当前机器的 ip
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// 当前机器所在的集群标识
    /// </summary>
    public string Cluster { get; set; }

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
    /// CPU指令架构
    /// </summary>
    public string CpuKind { get; set; }    

    /// <summary>
    /// 当前时区
    /// </summary>
    public string TimeZone { get; set; }

    /// <summary>
    /// 当前文化
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    /// 部署方式，此字段支持按位操作，类似于 Enum-Flags。
    /// 从低到高位：b-1: 是否 docker, b-2: 是否 singlefile, b-3: 是否 k8s
    /// 典型取值 001: docker-run, 010: SingleFile, 011: docker+SingleFile, 111: k8s+docker+SingleFile
    /// </summary>
    [DefaultValue(0)]
    public int DeployMode { get; set; }

    /// <summary>
    /// 程序的运行方式。
    /// 0: not set, 
    /// 100: Windows Service, 101: Windows UI Application, 102: Windows Console Application
    /// 200: Linux Normal application, 201: Linux Systemd Service, 202: Linux SysV-Init Service, 203: Linux init.d service
    /// </summary>
    public int RunMode { get; set; }

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"TenantId={TenantId};ClientId={ClientId};HostName={HostName};OsKind={OsKind};DeployMode={DeployMode}";
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Validate()
    {
        if( this.TenantId.IsNullOrEmpty() )
            throw new ValidationException2("TenantId is empty.");

        if( this.ClientId.IsNullOrEmpty() )
            throw new ValidationException2("ClientId is empty.");
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
    public string UserId => this.ClientId;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
    public string UserName => this.AppName ?? "UnknownClient";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Xml.Serialization.XmlIgnore]
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
#if NETFRAMEWORK
            CpuKind = Environment.Is64BitOperatingSystem ? "X64" : "X86",
#else
            CpuKind = RuntimeInformation.OSArchitecture.ToString(),  // 指令架构有3个级别的：CPU/OS/Process，这里取OS级别
#endif
            DeployMode = ((EnvUtils.IsInDocker ? 1 : 0) | (AsmHelper.IsSingleFileDeploy ? 2 : 0) | (EnvUtils.IsInK8s ? 4 : 0)),
            TimeZone = MyTimeZone.CurrentTZ,
            Culture = System.Globalization.CultureInfo.CurrentCulture?.Name
        };
    }
}
