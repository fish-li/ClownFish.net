namespace ClownFish.Http.Clients.Elastic;

// ############ 设计意图 ############
// 在产生 elasticsearch Index-name 后缀时，
// 有些场景下的日志数据量，用 “1天”， “1小时” 来分割都不合适，
// 例如：一天的日志可能 3000 条，这样产生一个 index 文件，在 Kibana - Index Management 列表中，就会出现一大堆的 index
//      如果不按天，那就只能按 “月” 来分割，那么 “7天后过期删除” 就实现不了~~~
// 类似的，还有些日志量会大一些，但是用 “1小时” 为单位也不合适，只能按 “1天” 来分割，也不合适。

// 对于这此场景，我们需要的是：可以按 “5天”， “6小时” 来产生 index，这样会比较合理，其中 5,6 这样的数量是可自由选择的。
// 但是，在生成 index-name 时，用的是 DateTime.Now.ToString(_indexNameTimeFormat) 方法
// ToString 的 format 参数  并不能自由定义。
// 所以，这里的做法是，由程序定义 “特殊的format” 由程序自身来解析产生 index-name 后缀

// 这块的场景比较简单，先不考虑“外部扩展”了，类型全部是 internal


internal interface IEsIndexNameTimeFormat
{
    string TimeToString(DateTime time);
}

internal static partial class EsIndexNameTimeFormat
{
#if NET7_0_OR_GREATER
    [GeneratedRegex(@"^-(?<len>\d*)(?<unit>d|h)$", RegexOptions.None, "en-US")]
    private static partial Regex GetLenUnitRegex();
#else

    // 匹配字符串格式： "-5d" ,  "-4h"
    private static readonly Regex s_regex = new Regex(@"^-(?<len>\d*)(?<unit>d|h)$", RegexOptions.Compiled);
    private static Regex GetLenUnitRegex() => s_regex;
#endif

    public static IEsIndexNameTimeFormat GetImpl(string format)
    {
        if( format.IsNullOrEmpty() )
            return null;

        Match match = GetLenUnitRegex().Match(format);
        if( match.Success ) {
            string unit = match.Groups["unit"].Value;
            int len = match.Groups["len"].Value.TryToInt();

            if( unit == "d" ) {
                if( len >= 2 && len <= 15 )
                    return new EsIndexNameTimeFormatNdayImpl(len);
            }

            else if( unit == "h" ) {
                if( len >= 2 && len <= 12 )
                    return new EsIndexNameTimeFormatNhourImpl(len);
            }

            Console2.Warnning($"######## 注意：指定的EsIndexNameTimeFormat是无效的，将按.NET默认方式来处理，参数值：'{format}'");
        }

        return new EsIndexNameTimeFormatDefaultImpl(format);
    }
}

internal sealed class EsIndexNameTimeFormatDefaultImpl : IEsIndexNameTimeFormat
{
    private readonly string _format;

    public EsIndexNameTimeFormatDefaultImpl(string format)
    {
        _format = format;
    }

    public string TimeToString(DateTime time)
    {
        return time.ToString(_format);
    }
}

internal sealed class EsIndexNameTimeFormatNdayImpl : IEsIndexNameTimeFormat
{
    private readonly int _nday;

    public EsIndexNameTimeFormatNdayImpl(int nday)
    {
        _nday = nday;
    }

    public string TimeToString(DateTime time)
    {
        string mm = time.Month.ToString("D2");
        int dd = (time.Day / _nday) + 1;
        return $"-{time.Year}{mm}-{dd}";
    }
}

internal sealed class EsIndexNameTimeFormatNhourImpl : IEsIndexNameTimeFormat
{
    private readonly int _nhour;

    public EsIndexNameTimeFormatNhourImpl(int nhour)
    {
        _nhour = nhour;
    }

    public string TimeToString(DateTime time)
    {
        string mm = time.Month.ToString("D2");
        string dd = time.Day.ToString("D2");
        int hh = (time.Hour / _nhour) + 1;
        return $"-{time.Year}{mm}{dd}-{hh}";
    }
}



