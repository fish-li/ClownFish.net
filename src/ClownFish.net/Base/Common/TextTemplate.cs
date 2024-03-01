namespace ClownFish.Base;

/// <summary>
/// 简单模板工具类
/// </summary>
public sealed class TextTemplate
{
    private static readonly Regex s_regex = new Regex(@"\{(?<name>[\w\.\(\)]+)\}", RegexOptions.Compiled);

    private IDictionary<string, object> _data;
    private string _json;


    /// <summary>
    /// 获取模板中包含的参数名称
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static string[] GetArgumentNames(string template)
    {
        if( template.IsNullOrEmpty() )
            return Empty.Array<string>();

        MatchCollection matchs = s_regex.Matches(template);
        if( matchs.Count == 0 )
            return Empty.Array<string>();

        List<string> list = new List<string>(matchs.Count);
        foreach( Match m in matchs ) {
            list.Add(m.Groups["name"].Value.Trim());
        }
        string[] names = list.Distinct().ToArray();

        //string[] names = (from m in matchs select m.Groups["name"].Value.Trim()).Distinct().ToArray();

        return names;
    }


    /// <summary>
    /// 填充模板
    /// </summary>
    /// <param name="template"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public string Populate(string template, IDictionary<string, object> data)
    {
        if( template.IsNullOrEmpty() || data == null )
            return template;

        string[] names = GetArgumentNames(template);
        if( names.Length == 0 )
            return template;


        _data = data;

        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append(template);

            foreach( var name in names ) {
                string value = GetFixValue(name) ?? GetRandValue(name) ?? GetPropertyValue(name) ?? ExecuteFuncation(name);

                if( value != null ) {
                    sb.Replace("{" + name + "}", value);
                }
            }

            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    /// <summary>
    /// 填充模板
    /// </summary>
    /// <param name="template"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    public string Populate(string template, string json)
    {
        if( template.IsNullOrEmpty() || json.IsNullOrEmpty() )
            return template;

        ExpandoObject data;

        try {
            data = json.FromJson<ExpandoObject>();
        }
        catch( Exception ex ) {
            throw new ArgumentException("Argument [json] is not json string", nameof(json), ex);
        }

        _json = json;

        return Populate(template, data);
    }


    private string GetFixValue(string name)
    {
        // 先处理一些固定的占位符
        switch( name ) {
            case "data":
                return _json ?? _data.ToJson();

            default:
               return XRandom.GetValue(name);
        }
    }



    private string GetRandValue(string name)
    {
        if( name.StartsWith("rand.", StringComparison.Ordinal) ) {
            string name2 = name.Substring(5);   // rand.xxx => xxx
            return XRandom.GetValue(name2);
        }

        return null;
    }

    private string GetPropertyValue(string name)
    {
        if( name.StartsWith("data.", StringComparison.Ordinal) ) {

            string propertyName = name.Substring(5);   // data.xxx => xxx
            object obj = _data.TryGet(propertyName);
            if( obj == null )
                return null;

            string value = obj.ToString();
            return value;
        }

        return null;
    }


    private string ExecuteFuncation(string name)
    {
        if( name.StartsWith("enc(", StringComparison.Ordinal) || name.EndsWith(')') )
            return ExecuteUrlEncode(name);


        return null;
    }

    private string ExecuteUrlEncode(string name)
    {
        string express = name.Substring(4, name.Length - 5);   // enc(xxx) => xxx
        string value = GetFixValue(express) ?? GetRandValue(express) ?? GetPropertyValue(express);

        if( value == null )
            return null;

        return System.Web.HttpUtility.UrlEncode(value);
    }

}
