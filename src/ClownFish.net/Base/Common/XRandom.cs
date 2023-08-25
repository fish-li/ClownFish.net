namespace ClownFish.Base;

/// <summary>
/// 获取随机值的工具类
/// </summary>
public sealed class XRandom
{
    private readonly Random _random = new Random();

    /// <summary>
    /// 获取一个随机值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetValue(string name = null)
    {
        if( string.IsNullOrEmpty(name) )
            return Guid.NewGuid().ToString("N");

        if( name == "guid32" )
            return Guid.NewGuid().ToString("N");

        if( name == "guid" || name == "guid36" )
            return Guid.NewGuid().ToString();

        if( name.StartsWith0("char") )
            return GetCharN(name);

        if( name.StartsWith0("int") )
            return GetIntN(name);

        return GetTime(name, DateTime.Now);
    }

    /// <summary>
    /// 填充模板中的随机值占位标记
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public string FillTemplate(string template)
    {
        string[] names = TextTemplate.GetArgumentNames(template);

        if( names.IsNullOrEmpty() )
            return template;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append(template);

            foreach( var name in names ) {
                string value = this.GetValue(name);
                if( value.HasValue() )
                    sb.Replace("{" + name + "}", value);
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    private string GetIntN(string name)
    {
        if( name.Length == 3 )  // int
            return _random.Next(1, int.MaxValue).ToString();

        int len = name.Substring(3).TryToUInt();   // int999 => 999
        if( len <= 0 )
            return null;

        if( len <= 1 )
            return _random.Next(1, 9).ToString();

        if( len > 9 )
            return _random.Next(100000000, int.MaxValue).ToString();

        int min = (int)Math.Pow(10, len - 1);
        int max = (int)Math.Pow(10, len) - 1;

        return _random.Next(min, max).ToString();
    }

    private string GetCharN(string name)
    {
        string text = Guid.NewGuid().ToString("N");

        if( name.Length == 4 ) // char
            return text.Substring(0, 1);

        int len = name.Substring(4).TryToUInt();   // char999 => 999
        if( len <= 0 )
            return null;

        if( len <= 1 )
            return text.Substring(0, 1);

        if( len >= 32 )
            return text;

        return text.Substring(0, len);
    }

    /// <summary>
    /// 根据名称获取对应的时间值
    /// </summary>
    /// <param name="name"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    public static string GetTime(string name, DateTime now)
    {
        if( name == "now" )
            return now.ToTimeString();

        if( name.EndsWith0("秒前") ) {
            int ago = name.Substring(0, name.Length - 2).TryToUInt();
            if( ago > 0 )
                return now.AddSeconds(0 - ago).ToTimeString();
        }

        if( name.EndsWith0("分钟前") ) {
            int ago = name.Substring(0, name.Length - 3).TryToUInt();
            if( ago > 0 )
                return now.AddMinutes(0 - ago).ToTimeString();
        }

        if( name.EndsWith0("小时前") ) {
            int ago = name.Substring(0, name.Length - 3).TryToUInt();
            if( ago > 0 )
                return now.AddHours(0 - ago).ToTimeString();
        }

        if( name.EndsWith0("天前") ) {
            int ago = name.Substring(0, name.Length - 2).TryToUInt();
            if( ago > 0 )
                return now.AddDays(0 - ago).ToTimeString();
        }

        return null;
    }
}
