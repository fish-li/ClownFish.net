namespace ClownFish.ImClients.Impls;

internal static partial class DingdingUtils
{
    // 匹配示例：
    // 1[StepId]: ##f2a0cee99ecb45d38ce940304c329074
    // 1[StepId]: # f2a0cee99ecb45d38ce940304c329074 
    // 1[StepId]: ## f2a0cee99ecb45d38ce940304c329074 
    // 1[StepId]: ### f2a0cee99ecb45d38ce940304c329074 
    // 结果全是：f2a0cee99ecb45d38ce940304c329074    
    //private static readonly Regex s_regex = new Regex(@"#+\s?(?<title>[^\n]+)\s?\n", RegexOptions.Compiled | RegexOptions.Multiline);


    [GeneratedRegex(@"#+\s?(?<title>[^\n]+)\s?\n", RegexOptions.Multiline, "en-US")]
    private static partial Regex GetTitleRegex();

    /// <summary>
    /// 钉钉个SB玩意需要在发送Markdown时指定一个标题文本用于列表显示
    /// </summary>
    /// <param name="markdown"></param>
    /// <returns></returns>
    public static string GetMarkdonwTitle(string markdown)
    {
        if( markdown.IsNullOrEmpty() )
            return null;

        Match m = GetTitleRegex().Match(markdown);
        if( m.Success )
            return m.Groups["title"].Value;

        if( markdown.Length <= 50 )
            return markdown;

        return markdown.Substring(0, 50);
    }
}
