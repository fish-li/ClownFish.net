namespace ClownFish.Base;

/// <summary>
/// 包含String类型相关的扩展方法
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// 等同于：string.IsNullOrEmpty(text)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string text)
    {
        return text == null || text.Length == 0;
    }

    /// <summary>
    /// 判断字符串是否包含内容（不是NULL且长度大于0）
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool HasValue(this string text)
    {
        return text != null && text.Length > 0;
    }

    /// <summary>
    /// 判断二个字符串是否相等，忽略大小写的比较方式。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool EqualsIgnoreCase(this string a, string b)
    {
        return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
    }

    /// <summary>
    /// 判断二个字符串是否相等，忽略大小写的比较方式。等同于 EqualsIgnoreCase方法。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool Is(this string a, string b)
    {
        return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
    }


    /// <summary>
    /// 以忽略大小写的方式调用 string.EndsWith
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool EndsWithIgnoreCase(this string a, string b)
    {
        if( a == null )
            return false;   // 这里不抛异常，可以减少不必要问题发生！

        return a.EndsWith(b, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 以 Ordinal 比较方式调用 string.EndsWith
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool EndsWith0(this string a, string b)
    {
        if( a == null )
            return false;   // 这里不抛异常，可以减少不必要问题发生！

        return a.EndsWith(b, StringComparison.Ordinal);
    }


    /// <summary>
    /// 以 OrdinalIgnoreCase 比较方式调用 string.EndsWith
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool EndsWith1(this string a, string b)
    {
        if( a == null )
            return false;   // 这里不抛异常，可以减少不必要问题发生！

        return a.EndsWith(b, StringComparison.OrdinalIgnoreCase);
    }

#if NETFRAMEWORK

    /// <summary>
    /// 确定此字符串实例的开头是否与指定的字符串匹配。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool StartsWith(this string a, char c)
    {
        if( a == null )
            return false;   // 这里不抛异常，可以减少不必要问题发生！

        if( a[0] == c ) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 确定此字符串实例的结尾是否与指定的字符串匹配。
    /// </summary>
    /// <param name="a"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool EndsWith(this string a, char c)
    {
        if( a == null )
            return false;   // 这里不抛异常，可以减少不必要问题发生！

        int length = a.Length;
        if( length != 0 && a[length - 1] == c ) {
            return true;
        }
        return false;
    }
#endif


    /// <summary>
    /// 以忽略大小写的方式调用 string.StartsWith
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool StartsWithIgnoreCase(this string a, string b)
    {
        if( a == null )
            return false;   // 这里不抛异常，可以减少不必要问题发生！

        return a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 以 Ordinal 比较方式调用 string.StartsWith
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool StartsWith0(this string a, string b)
    {
        if( a == null )
            return false;   // 这里不抛异常，可以减少不必要问题发生！

        return a.StartsWith(b, StringComparison.Ordinal);
    }

    /// <summary>
    /// 以 OrdinalIgnoreCase 比较方式调用 string.StartsWith
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool StartsWith1(this string a, string b)
    {
        if( a == null )
            return false;   // 这里不抛异常，可以减少不必要问题发生！

        return a.StartsWith(b, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 以忽略大小写的方式调用 string.IndexOf
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int IndexOfIgnoreCase(this string a, string b)
    {
        if( a.IsNullOrEmpty() )
            return -1;

        return a.IndexOf(b, StringComparison.OrdinalIgnoreCase);
    }


    /// <summary>
    /// 以 Ordinal 比较方式调用 string.IndexOf
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int IndexOf0(this string a, string b)
    {
        if( a.IsNullOrEmpty() )
            return -1;

        return a.IndexOf(b, StringComparison.Ordinal);
    }


    /// <summary>
    /// 以 OrdinalIgnoreCase 比较方式调用 string.IndexOf
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int IndexOf1(this string a, string b)
    {
        if( a.IsNullOrEmpty() )
            return -1;

        return a.IndexOf(b, StringComparison.OrdinalIgnoreCase);
    }


    /// <summary>
    /// 由“逗号”和“分号”组成的分隔符数组，即：  new char[] { ',', ';' };
    /// </summary>
    public static readonly char[] ItemSeparators = new char[] { ',', ';' };


    /// <summary>
    /// 将一个字符串按分隔符拆分成数组，
    /// 等效于 string.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries) 
    /// 且为每个拆分后的结果又做了Trim()操作。
    /// 如果字符串变量 IsNullOrEmpty，则返回 null
    /// </summary>
    /// <param name="str"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string[] ToArray(this string str, params char[] separator)
    {
        if( string.IsNullOrEmpty(str) )
            return Empty.Array<string>();

        if( separator.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(separator));

        return (from s in str.Split(separator)
                let u = s.Trim()
                where u.Length > 0
                select u).ToArray();
    }

    /// <summary>
    /// 将一个字符串按分隔符拆分成数组，
    /// 等效于 string.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) 
    /// 且为每个拆分后的结果又做了Trim()操作。
    /// 如果字符串变量 IsNullOrEmpty，则返回 null
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string[] ToArray2(this string str)
    {
        return ToArray(str, ItemSeparators);
    }

    /// <summary>
    /// 将一个字符串按分隔符拆分成列表，
    /// 等效于 string.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries).ToList()
    /// 且为每个拆分后的结果又做了Trim()操作。
    /// 如果字符串变量 IsNullOrEmpty，则返回 null
    /// </summary>
    /// <param name="str"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static List<string> ToList(this string str, params char[] separator)
    {
        if( string.IsNullOrEmpty(str) )
            return new List<string>(0);

        if( separator.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(separator));

        return (from s in str.Split(separator)
                let u = s.Trim()
                where u.Length > 0
                select u).ToList();
    }


    /// <summary>
    /// 将一个字符串按分隔符拆分成列表，
    /// 等效于 string.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList()
    /// 且为每个拆分后的结果又做了Trim()操作。
    /// 如果字符串变量 IsNullOrEmpty，则返回 null
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static List<string> ToList2(this string str)
    {
        return ToList(str, ItemSeparators);
    }


    /// <summary>
    /// 将一个字符串按分隔符拆分成 ImmutableHashSet&lt;string&gt;  ，
    /// 如果字符串变量 IsNullOrEmpty，则返回 ImmutableHashSet&lt;string&gt;.Empty
    /// </summary>
    /// <param name="str"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static HashSet<string> SplitToHashSet(this string str, params char[] separator)
    {
        if( string.IsNullOrEmpty(str) )
            return new HashSet<string>();

        if( separator.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(separator));

        return (from s in str.Split(separator)
                let u = s.Trim()
                where u.Length > 0
                select u).ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 将一个字符串按分隔符拆分成 ImmutableHashSet&lt;string&gt;  ，
    /// 如果字符串变量 IsNullOrEmpty，则返回 ImmutableHashSet&lt;string&gt;.Empty
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static HashSet<string> SplitToHashSet(this string str)
    {
        return SplitToHashSet(str, ItemSeparators);
    }




    /// <summary>
    /// 换行分隔符 { '\r', '\n' }
    /// </summary>
    public static readonly char[] LineSeparators = new char[] { '\r', '\n' };

    /// <summary>
    /// 等效于 string.Split(换行符, StringSplitOptions.RemoveEmptyEntries)
    /// 且为每个拆分后的结果又做了Trim()操作。
    /// </summary>
    /// <param name="str">要拆分的字符串</param>
    /// <returns></returns>
    public static string[] ToLines(this string str)
    {
        if( string.IsNullOrEmpty(str) )
            return Empty.Array<string>();

        return (from s in str.Split(LineSeparators)
                let u = s.Trim()
                where u.Length > 0
                select u).ToArray();
    }




    /// <summary>
    /// <para>拆分一个字符串行。如：a=1;b=2;c=3;d=4;</para>
    /// <para>此时可以调用: ToKVList("a=1;b=2;c=3;d=4;", ';', '=');</para>
    /// <para>说明：对于空字符串，方法也会返回一个空的列表。</para>
    /// </summary>
    /// <param name="line">包含所有项目组成的字符串行</param>
    /// <param name="separator1">每个项目之间的分隔符</param>
    /// <param name="separator2">每个项目内的分隔符</param>
    /// <returns>拆分后的结果列表</returns>
    public static List<NameValue> ToKVList(this string line, char[] separator1, char separator2)
    {
        if( string.IsNullOrEmpty(line) )
            return new List<NameValue>();

        if( separator1.IsNullOrEmpty() )
            return new List<NameValue>();

        string[] itemArray = line.Split(separator1, StringSplitOptions.RemoveEmptyEntries);
        List<NameValue> list = new List<NameValue>(itemArray.Length);

        //char[] separator2Array = new char[] { separator2 };

        foreach( string item in itemArray ) {
            //string[] parts = item.Trim().Split(separator2Array, StringSplitOptions.RemoveEmptyEntries);
            //if( parts.Length != 2 )
            //	throw new ArgumentException("要拆分的字符串的格式无效。");

            //list.Add(new NameValue { Name = parts[0], Value = parts[1] });

            // Cookie允许在一个名称下写多个子节点，例如：SRCHUSR=AUTOREDIR=0&GEOVAR=&DOB=20141216; _EDGE_V=1;
            // 如果使用上面的拆分方式，会抛出异常，
            // 所以，调整为，只判断有没有【分隔符】，而不管出现多次

            int p = item.IndexOf(separator2);
            if( p <= 0 )
                throw new ArgumentException("要拆分的字符串的格式无效。");

            list.Add(new NameValue {
                Name = item.Substring(0, p).Trim(),
                Value = item.Substring(p + 1).Trim()
            });
        }
        return list;
    }


    private static readonly char[] s_defaultKvSeparator1 = new char[] { ';' };
    private static readonly char[] s_defaultKvSeparator2 = new char[] { '&' };

    /// <summary>
    /// <para>拆分一个字符串行。如：a=1;b=2;c=3;d=4;</para>
    /// <para>此时可以调用: ToKVList("a=1;b=2;c=3;d=4;", ';', '=');</para>
    /// <para>说明：对于空字符串，方法也会返回一个空的列表。</para>
    /// </summary>
    /// <param name="line">包含所有项目组成的字符串行</param>
    /// <param name="separator1">每个项目之间的分隔符</param>
    /// <param name="separator2">每个项目内的分隔符</param>
    /// <returns>拆分后的结果列表</returns>
    public static List<NameValue> ToKVList(this string line, char separator1, char separator2)
    {
        if( separator1 == ';' )
            return ToKVList(line, s_defaultKvSeparator1, separator2);
        else if( separator1 == '&' )
            return ToKVList(line, s_defaultKvSeparator2, separator2);
        else
            return ToKVList(line, new char[] { separator1 }, separator2);
    }



    /// <summary>
    /// 按多行请求头的方式解析字符串，将结果转成NameValueCollection
    /// </summary>
    /// <param name="headerText"></param>
    /// <returns></returns>
    internal static NameValueCollection ToHeaderCollection(this string headerText)
    {
        if( headerText.IsNullOrEmpty() )
            return new NameValueCollection();

        var list = headerText.Trim().ToKVList(LineSeparators, ':');
        NameValueCollection collection = new NameValueCollection(list.Count, StringComparer.OrdinalIgnoreCase);

        foreach( var nv in list )
            collection.Add(nv.Name, nv.Value);

        return collection;
    }


    /// <summary>
    /// 将字符串的首个英文字母大写
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToTitleCase(this string text)
    {
        // 重新实现：CultureInfo.CurrentCulture.TextInfo.ToTitleCase
        // 那个方法太复杂了，重新实现一个简单的版本。

        if( text == null || text.Length < 2 )
            return text;

        char c = text[0];
        if( (c >= 'a') && (c <= 'z') )
            return ((char)(c - 32)).ToString() + text.Substring(1);
        else
            return text;
    }


    /// <summary>
    /// 将字符串转成byte[]，等效于：Encoding.UTF8.GetBytes(text);
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static byte[] GetBytes(this string text)
    {
        if( text == null )
            return null;

        return Encoding.UTF8.GetBytes(text);
    }


    /// <summary>
    /// 截取一个字符串，只保留部分长度，再显示原字符串的长度。
    /// 例如：SubstringN("长度为125的字符串", 5)  将得到： xxxxx...125 ，所以最后得到的结果可能比keepLength长。
    /// </summary>
    /// <param name="text"></param>
    /// <param name="keepLength"></param>
    /// <returns></returns>
    public static string SubstringN(this string text, int keepLength)
    {
        if( string.IsNullOrEmpty(text) || keepLength <= 0 )
            return text;

        if( text.Length <= keepLength )
            return text;

#if NETFRAMEWORK
        return text.Substring(0, keepLength) + "..." + text.Length.ToString();
#else
        return string.Concat(text.AsSpan(0, keepLength), "...", text.Length.ToString());
#endif
    }

    /// <summary>
    /// 尝试将一个字符串转成【整数】，如果失败就返回默认值
    /// </summary>
    /// <param name="text"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int TryToInt(this string text, int defaultValue = 0)
    {
        if( string.IsNullOrEmpty(text) )
            return defaultValue;

        int result = 0;
        if( int.TryParse(text, out result) == false )
            return defaultValue;

        return result;
    }


    /// <summary>
    /// 尝试将一个字符串转成【正整数】，如果失败就返回默认值
    /// </summary>
    /// <param name="text"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int TryToUInt(this string text, int defaultValue = 0)
    {
        if( string.IsNullOrEmpty(text) )
            return defaultValue;

        int result = 0;
        if( int.TryParse(text, out result) == false )
            return defaultValue;

        if( result < 0 )
            return defaultValue;

        return result;
    }


    /// <summary>
    /// 尝试将一个字符串转成【double】，如果失败就返回默认值
    /// </summary>
    /// <param name="text"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double TryToDouble(this string text, double defaultValue = 0d)
    {
        if( string.IsNullOrEmpty(text) )
            return defaultValue;

        double result;
        if( double.TryParse(text, out result) == false )
            return defaultValue;

        return result;
    }


    /// <summary>
    /// 尝试将一个字符串转成【long】，如果失败就返回默认值
    /// </summary>
    /// <param name="text"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long TryToLong(this string text, long defaultValue = 0L)
    {
        if( string.IsNullOrEmpty(text) )
            return defaultValue;

        long result;
        if( long.TryParse(text, out result) == false )
            return defaultValue;

        return result;
    }


    /// <summary>
    /// 尝试将一个字符串转成【布尔值】，如果失败就返回默认值
    /// </summary>
    /// <param name="text"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static bool TryToBool(this string text, bool defaultValue = false)
    {
        if( string.IsNullOrEmpty(text) )
            return defaultValue;

        if( text == "1" || text.Is("true") )
            return true;

        return false;
    }

    /// <summary>
    /// 检查一个字符串变量 IsNullOrEmpty，如果是则返回指定的默认值。
    /// </summary>
    /// <param name="text"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string IfEmpty(this string text, string defaultValue)
    {
        if( string.IsNullOrEmpty(text) )
            return defaultValue;

        return text;
    }

    /// <summary>
    /// 如果字符串为空则抛出异常
    /// </summary>
    /// <param name="text"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string IfEmptyThrow(this string text, string name)
    {
        if( string.IsNullOrEmpty(text) )
            throw new ArgumentNullException(name);

        return text;
    }


    /// <summary>
    /// 如果字符串为 NULL，就返回 string.Empty
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string NotNull(this string text)
    {
        return text ?? string.Empty;
    }

    /// <summary>
    /// 尽量将对象转成更容易看的字符串形式。
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToString2(this object value)
    {
        if( value == null )
            return string.Empty;

        if( value is IToString2 x2 )
            return x2.ToString2();

        if( value is bool flag )
            return flag ? "true" : "false";

        if( value is DateTime time )
            return time.ToTimeString();

        if( value is int x3 )
            return x3.ToWString();

        if( value is long x4 )
            return x4.ToWString();

        if( value is byte[] bb )
            return "byte[], len=" + bb.Length;

        if( value is System.Security.Cryptography.X509Certificates.X509Certificate2 x509 )
            return x509.Subject;

        if( value is System.Reflection.TargetInvocationException ex2 )
            return ex2.InnerException.ToString2();

        // 以后再完善

        return value.ToString();
    }



    /// <summary>
    /// 解析一个字节长度字符串，例如：100KB, 500M, 10G, 100B, 200
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static long ParseLength(this string text)
    {
        if( text.IsNullOrEmpty() )
            return 0;

        // 先把左边的数字部分找出来，非数字部分就认为是“单位”，
        // 允许没有“单位”，就当字节数来处理
        int p = -1;
        for( int i = 0; i < text.Length; i++ ) {
            if( char.IsDigit(text[i]) == false ) {
                p = i;
                break;
            }
        }

        if( p < 0 )  // 全是数字
            return long.Parse(text);

        if( p == 0 )  // 没有数字
            throw new ArgumentException("无效的长度参数：" + text);


        string unit = text.Substring(p);
        if( unit.Length > 2 )
            throw new ArgumentException("无效的长度参数：" + text);

        string value = text.Substring(0, p);

        return unit switch {
            "K" or "KB" => long.Parse(value) * 1024L,
            "M" or "MB" => long.Parse(value) * 1024L * 1024,
            "G" or "GB" => long.Parse(value) * 1024L * 1024 * 1024,
            "T" or "TB" => long.Parse(value) * 1024L * 1024 * 1024 * 1024,
            "P" or "PB" => long.Parse(value) * 1024L * 1024 * 1024 * 1024 * 1024,
            _ => throw new ArgumentOutOfRangeException("无效的长度参数：" + text)
        };
    }

    /// <summary>
    /// 判断字符串是不是有效的名称，即只包含：字母，数字，下划线。
    /// </summary>
    /// <param name="text"></param>
    public static void CheckName(this string text)
    {
        if( string.IsNullOrEmpty(text) )
            return;

        foreach( char ch in text ) {
            if( (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') || ch == '_' )
                continue;
            else
                throw new ArgumentOutOfRangeException(nameof(text), "参数包含了不允许的字符。");
        }
    }


    /// <summary>
    /// 仅供框架内部使用
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetConfName(this string name)
    {
        if( name.IsNullOrEmpty() || name.Contains('.') == false )
            return name;

        // x.y.z  =>  x_y_z
        return name.Replace('.', '_');
    }

}
