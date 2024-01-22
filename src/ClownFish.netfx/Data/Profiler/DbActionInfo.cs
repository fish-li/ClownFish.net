namespace ClownFish.Data.Profiler;

/// <summary>
/// 
/// </summary>
public sealed class DbActionInfo
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly string OpenConnectionFlag = "<open connection>";

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan Time { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string SqlText { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool InTranscation { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ErrorMsg { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string SqlShowText { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<CommandParameter> Parameters { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string Serialize(DbActionInfo info)
    {
        if( info == null )
            throw new ArgumentNullException(nameof(info));

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(info);
        return GzipHelper.Compress(json);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="base64"></param>
    /// <returns></returns>
    public static DbActionInfo Deserialize(string base64)
    {
        if( string.IsNullOrEmpty(base64) )
            throw new ArgumentNullException(nameof(base64));

        string json = GzipHelper.Decompress(base64);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<DbActionInfo>(json);
    }

}

/// <summary>
/// 
/// </summary>
public sealed class CommandParameter
{/// <summary>
 /// 
 /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string DbType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Value { get; set; }
}
