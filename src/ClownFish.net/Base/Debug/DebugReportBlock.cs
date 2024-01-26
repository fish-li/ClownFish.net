namespace ClownFish.Base;

/// <summary>
/// 表示一个诊断信息块
/// </summary>
public sealed class DebugReportBlock
{
    /// <summary>
    /// 显示次序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 块标题
    /// </summary>
    public string Category { get; set; }

    internal List<string> Lines { get; set; }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="line"></param>
    public void AppendLine(string line)
    {
        if( line.IsNullOrEmpty() )
            return;

        if( this.Lines == null )
            this.Lines = new List<string>();

        this.Lines.Add(line);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    public void GetText(StringBuilder sb)
    {
        sb.AppendLineRN($"##### {this.Category ?? "title"} #####");

        if( this.Lines != null ) {
            foreach( var line in Lines )
                sb.AppendLineRN(line);
        }
    }

    /// <summary>
    /// 获取整块信息
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return this.Category ?? "title";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string ToString2()  // for test
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            this.GetText(sb);
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }
}
