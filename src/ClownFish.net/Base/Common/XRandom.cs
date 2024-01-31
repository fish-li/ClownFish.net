namespace ClownFish.Base;

/// <summary>
/// 随机值-变量 工具类
/// </summary>
public static class XRandom
{
    private static readonly TSafeDictionary<string, Func<string>> s_dictionary = new(128);
    private static readonly List<Func<string, string>> s_list = new(16);

    static XRandom()
    {
        RegisterValueGetter("rand", XRandomImpls.GetRand);
        RegisterValueGetter("guid32", XRandomImpls.GetGuid32);
        RegisterValueGetter("guid", XRandomImpls.GetGuid36);
        RegisterValueGetter("guid36", XRandomImpls.GetGuid36);

        RegisterValueGetter("now", XRandomImpls.GetNow);
        RegisterValueGetter("昨天", XRandomImpls.GetYesterday);
        RegisterValueGetter("今天", XRandomImpls.GetToday);
        RegisterValueGetter("明天", XRandomImpls.GetTomorrow);

        RegisterValueGetter("月初", XRandomImpls.GetMonth1);
        RegisterValueGetter("下月初", XRandomImpls.GetMonth2);
        RegisterValueGetter("季度初", XRandomImpls.GetQuarter1);
        RegisterValueGetter("下季度初", XRandomImpls.GetQuarter2);
        RegisterValueGetter("年初", XRandomImpls.GetYear1);
        RegisterValueGetter("明年初", XRandomImpls.GetYear2);
        RegisterValueGetter("周一", XRandomImpls.GetWeek1);
        RegisterValueGetter("下周一", XRandomImpls.GetWeek2);


        RegisterValueGetter(XRandomImpls.GetIntN);
        RegisterValueGetter(XRandomImpls.GetCharN);
        RegisterValueGetter(XRandomImpls.GetTime);
        RegisterValueGetter(XRandomImpls.GetLocalSetting);
    }

    /// <summary>
    /// 注册一个随机数【获取器】，用于根据指定的名称产生匹配的随机数据
    /// </summary>
    /// <param name="key">随机数或者变量的名称</param>
    /// <param name="getter">一个回调委托，它需要产生一个与 key参数对应的结果</param>
    public static void RegisterValueGetter(string key, Func<string> getter)
    {
        if( key.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(key));
        if( getter == null )
            throw new ArgumentNullException(nameof(getter));

        s_dictionary[key] = getter;
    }

    /// <summary>
    /// 注册一个随机数【获取器】，用于根据指定的名称产生匹配的随机数据
    /// </summary>
    /// <param name="getter">一个回调委托，它需要根据一个name参数来产生对应的结果，如果name不在支持的范围，必须返回null</param>
    public static void RegisterValueGetter(Func<string, string> getter)
    {
        if( getter == null )
            throw new ArgumentNullException(nameof(getter));

        s_list.Add(getter);
    }

    /// <summary>
    /// 获取一个随机值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetValue(string name = null)
    {
        if( string.IsNullOrEmpty(name) )
            return Guid.NewGuid().ToString("N");

        Func<string> getter = s_dictionary.TryGet(name);
        if( getter != null )
            return getter();


        foreach(var x in s_list) {
            string value = x.Invoke(name);
            if( value != null )
                return value;
        }

        return null;
    }

    /// <summary>
    /// 填充模板中的随机值占位标记
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static string FillTemplate(string template)
    {
        string[] names = TextTemplate.GetArgumentNames(template);

        if( names.IsNullOrEmpty() )
            return template;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append(template);

            foreach( var name in names ) {
                string value = GetValue(name);
                if( value.HasValue() )
                    sb.Replace("{" + name + "}", value);
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    /// 不建议调用
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string GetTime(string name) => XRandomImpls.GetTime(name);
}

internal static class XRandomImpls
{
    public static string GetRand() => DateTime.Now.Ticks.ToString();

    public static string GetGuid32() => Guid.NewGuid().ToString("N");

    public static string GetGuid36() => Guid.NewGuid().ToString();

    public static string GetNow() => DateTime.Now.ToTimeString();

    public static string GetToday() => DateTime.Today.ToDateString();

    public static string GetTomorrow() => DateTime.Today.AddDays(1).ToDateString();

    public static string GetYesterday() => DateTime.Today.AddDays(-1).ToDateString();

    public static string GetIntN(string name)
    {
        if( name.StartsWith0("int") == false )
            return null;

        Random random = new Random();

        if( name.Length == 3 )  // int
            return random.Next(1, int.MaxValue).ToString();

        int len = name.Substring(3).TryToInt();   // int999 => 999
        if( len <= 0 )
            return null;

        if( len <= 1 )
            return random.Next(1, 9).ToString();

        if( len > 9 )
            return random.Next(100000000, int.MaxValue).ToString();

        int min = (int)Math.Pow(10, len - 1);
        int max = (int)Math.Pow(10, len) - 1;

        return random.Next(min, max).ToString();
    }

    public static string GetCharN(string name)
    {
        if( name.StartsWith0("char") == false )
            return null;

        string text = Guid.NewGuid().ToString("N");

        if( name.Length == 4 ) // char
            return text.Substring(0, 1);

        int len = name.Substring(4).TryToInt();   // char999 => 999
        if( len <= 0 )
            return null;

        if( len <= 1 )
            return text.Substring(0, 1);

        if( len >= 32 )
            return text;

        return text.Substring(0, len);
    }

    public static string GetTime(string name)
    {
        if( name.EndsWith0("秒前") ) {
            int ago = name.Substring(0, name.Length - 2).TryToInt();
            return DateTime.Now.AddSeconds(0 - ago).ToTimeString();
        }

        if( name.EndsWith0("分钟前") ) {
            int ago = name.Substring(0, name.Length - 3).TryToInt();
            return DateTime.Now.AddMinutes(0 - ago).ToTimeString();
        }

        if( name.EndsWith0("小时前") ) {
            int ago = name.Substring(0, name.Length - 3).TryToInt();
            return DateTime.Now.AddHours(0 - ago).ToTimeString();
        }

        if( name.EndsWith0("天前") ) {
            int ago = name.Substring(0, name.Length - 2).TryToInt();
            return DateTime.Now.AddDays(0 - ago).ToTimeString();
        }

        if( name.EndsWith0("月前") ) {
            int ago = name.Substring(0, name.Length - 2).TryToInt();
            return DateTime.Now.AddMonths(0 - ago).ToTimeString();
        }

        return null;
    }

    public static string GetLocalSetting(string name)
    {
        if( name.StartsWith0("LocalSetting_") == false )
            return null;

        string name2 = name.Substring(13);
        return LocalSettings.GetSetting(name2);
    }

    public static string GetMonth1()     // 月初
    {
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        return firstDayOfMonth.ToDateString();
    }

    public static string GetMonth2()     // 下月初
    {
        DateTime now = DateTime.Now;
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
        return firstDayOfMonth.AddMonths(1).ToDateString();
    }

    //public static string GetMonth2()   // 月未
    //{
    //    DateTime now = DateTime.Now;
    //    DateTime endOfMonth = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
    //    return endOfMonth.ToDateString();
    //}

    public static string GetQuarter1()    // 季度初
    {
        DateTime now = DateTime.Now;
        int quarter = (now.Month - 1) / 3 + 1;
        DateTime firstDayOfQuarter = new DateTime(now.Year, 3 * quarter - 2, 1);
        return firstDayOfQuarter.ToDateString();
    }

    public static string GetQuarter2()    // 下季度初
    {
        DateTime now = DateTime.Now;
        int quarter = (now.Month - 1) / 3 + 1;
        DateTime firstDayOfQuarter = new DateTime(now.Year, 3 * quarter - 2, 1);
        return firstDayOfQuarter.AddMonths(3).ToDateString();
    }

    //public static string GetQuarter2()   // 季度未
    //{
    //    DateTime now = DateTime.Now;
    //    int quarter = (now.Month - 1) / 3 + 1;
    //    DateTime firstDayOfQuarter = new DateTime(now.Year, 3 * quarter - 2, 1);
    //    DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
    //    return lastDayOfQuarter.ToDateString();
    //}

    public static string GetYear1()     // 年初
    {
        DateTime now = DateTime.Now;
        return new DateTime(now.Year, 1, 1).ToDateString();
    }

    public static string GetYear2()   // 明年初
    {
        DateTime now = DateTime.Now;
        return new DateTime(now.Year, 1, 1).AddYears(1).ToDateString();
    }


    //public static string GetYear2()  // 年未
    //{
    //    DateTime now = DateTime.Now;
    //    return new DateTime(now.Year, 12, 31).ToDateString();
    //}


    public static string GetWeek1()
    {
        DateTime today = DateTime.Now;
        int daysUntilMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        DateTime monday = today.AddDays(-daysUntilMonday);
        return monday.ToDateString();
    }

    public static string GetWeek2()   // 下周一
    {
        DateTime today = DateTime.Now;
        int daysUntilMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        DateTime monday = today.AddDays(-daysUntilMonday);
        return monday.AddDays(7).ToDateString();
    }

    //public static string GetWeek2()    // 周二
    //{
    //    DateTime today = DateTime.Now;
    //    int daysUntilTuesday = ((int)DayOfWeek.Tuesday - (int)today.DayOfWeek + 7) % 7;
    //    DateTime tuesday = today.AddDays(daysUntilTuesday);
    //    return tuesday.ToDateString();
    //}

    //public static string GetWeek3()    // 周3
    //{
    //    DateTime today = DateTime.Now;
    //    int daysUntilWednesday = ((int)DayOfWeek.Wednesday - (int)today.DayOfWeek + 7) % 7;
    //    DateTime wednesday = today.AddDays(daysUntilWednesday);
    //    return wednesday.ToDateString();
    //}

    //public static string GetWeek4()     // 周4
    //{
    //    DateTime today = DateTime.Now;
    //    int daysUntilThursday = ((int)DayOfWeek.Thursday - (int)today.DayOfWeek + 7) % 7;
    //    DateTime thursday = today.AddDays(daysUntilThursday);
    //    return thursday.ToDateString();
    //}

    //public static string GetWeek5()
    //{
    //    DateTime today = DateTime.Now;
    //    int daysUntilFriday = ((int)DayOfWeek.Friday - (int)today.DayOfWeek + 7) % 7;
    //    DateTime friday = today.AddDays(daysUntilFriday);
    //    return friday.ToDateString();
    //}

    //public static string GetWeek6()
    //{
    //    DateTime today = DateTime.Now;
    //    int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
    //    DateTime saturday = today.AddDays(daysUntilSaturday);
    //    return saturday.ToDateString();
    //}

    //public static string GetWeek7()
    //{
    //    DateTime today = DateTime.Now;
    //    int daysUntilSunday = ((int)DayOfWeek.Sunday - (int)today.DayOfWeek + 7) % 7;
    //    DateTime sunday = today.AddDays(daysUntilSunday);
    //    return sunday.ToDateString();
    //}
}