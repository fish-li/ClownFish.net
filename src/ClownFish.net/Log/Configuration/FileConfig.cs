namespace ClownFish.Log.Configuration;

/// <summary>
/// 日志文件相关配置
/// </summary>
public sealed class FileConfig
{
    /// <summary>
    /// 日志文件的根目录
    /// </summary>
    [XmlAttribute]
    public string RootPath { get; set; }

    /// <summary>
    /// 单个文件的最大长度（单位：MB），默认值：500M
    /// </summary>
    [XmlAttribute]
    public string MaxLength { get; set; }


    /// <summary>
    /// 一个目录下的文件数量，如果超过则删除最老的文件。
    /// </summary>
    [XmlAttribute]
    public int MaxCount { get; set; }


    internal void CheckOrSetDefault()
    {
        if( string.IsNullOrEmpty(this.RootPath) )
            this.RootPath = "Logs";

        if( string.IsNullOrEmpty(this.MaxLength) )
            this.MaxLength = "500M";

        if( this.MaxCount < 0 )
            this.MaxCount = 0;
    }
}
