namespace ClownFish.Base;

/// <summary>
/// 格式化相关的扩展工具类
/// </summary>
public static class FormatUtils
{
    /// <summary>
    /// 将一个数字格式为【万分位】的字符串
    /// </summary>
    /// <param name="number"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string ToWString(this long number, char separator = '\'')
    {
        string text = number.ToString();

        // 小于1W，不会出现分隔符，就直接返回
        if( number > -1000 && number < 10000 )
            return text;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            int p = 0;
            int len = text.Length;

            for( int i = len - 1; i >= 0; i-- ) {

                char c = text[i];

                if( c != '-' && p > 0 && p % 4 == 0 )
                    sb.Append(separator);

                sb.Append(c);
                p++;
            }

            char[] chars = new char[sb.Length];

            for( int i = 0; i < chars.Length; i++ )
                chars[i] = sb[chars.Length - 1 - i];

            return new string(chars);
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// 将一个数字格式为【万分位】的字符串
    /// </summary>
    /// <param name="number"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string ToWString(this int number, char separator = '\'')
    {
        return ToWString((long)number, separator);
    }


    /// <summary>
    /// 将一个金额数字格式为【万分位】的字符串，并保留4位小数
    /// </summary>
    /// <param name="number"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string ToWString(this decimal number, char separator = '\'')
    {
        string text = number.ToString("F4");  // 金额一般用4位小数，所以暂且固定下来 

        // 小于1W，不会出现分隔符，就直接返回
        if( number > -1000m && number < 10000m )
            return text;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            // 取整数部分
            string intText = text.Substring(0, text.Length - 5);

            int p = 0;
            int len = intText.Length;

            // 对整数部分的字符串做循环处理，方向是从右向左
            for( int i = len - 1; i >= 0; i-- ) {

                char c = intText[i];

                if( c != '-' && p > 0 && p % 4 == 0 )
                    sb.Append(separator);

                sb.Append(c);
                p++;
            }

            char[] chars = new char[sb.Length];

            for( int i = 0; i < chars.Length; i++ )
                chars[i] = sb[chars.Length - 1 - i];

            // 小数部分不做万分位处理，所以直接添加到结果中
            return new string(chars) + text.Substring(text.Length - 5);
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// 将一个数字格式为【千分位】的字符串
    /// </summary>
    /// <param name="number"></param>
    /// <param name="decimals"></param>
    /// <returns></returns>
    public static string ToKString(this long number, int decimals = 0)
    {
        if( decimals == 0 ) {
            return number.ToString("N0");
        }
        else if( decimals == 2 ) {
            return number.ToString("N2");
        }
        else {
            string format = "N" + decimals.ToString();
            return number.ToString(format);
        }
    }

    /// <summary>
    /// 将一个数字格式为【千分位】的字符串
    /// </summary>
    /// <param name="number"></param>
    /// <param name="decimals"></param>
    /// <returns></returns>
    public static string ToKString(this int number, int decimals = 0)
    {
        if( decimals == 0 ) {
            return number.ToString("N0");
        }
        else if( decimals == 2 ) {
            return number.ToString("N2");
        }
        else {
            string format = "N" + decimals.ToString();
            return number.ToString(format);
        }
    }




}
