using System.Xml.Linq;

namespace ClownFish.Base;

/// <summary>
/// 构造阿里云WebAPI查询字符串的工具类
/// </summary>
public sealed class AliWebApiQueryBuilder
{
    private readonly Dictionary<string, string> _args = new Dictionary<string, string>(32);
    private readonly string _accessKeyId;
    private readonly string _accessKeySecret;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="accessKeyId"></param>
    /// <param name="accessKeySecret"></param>
    public AliWebApiQueryBuilder(string accessKeyId, string accessKeySecret)
    {
        if( accessKeyId.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(accessKeyId));

        if( accessKeySecret.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(accessKeySecret));

        _accessKeyId = accessKeyId;
        _accessKeySecret = accessKeySecret;

        // 添加公共参数
        this.AddPubArgs();
    }

    private void AddPubArgs()
    {
        // https://help.aliyun.com/document_detail/2392107.html

        _args["AccessKeyId"] = _accessKeyId;
        _args["SignatureMethod"] = "HMAC-SHA1";
        _args["SignatureNonce"] = Guid.NewGuid().ToString("N");
        _args["SignatureVersion"] = "1.0";
        _args["Timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        _args["Format"] = "JSON";
        //_args["Version"] = "2017-05-25";    // 不同的API使用不同的版本号，所以这里不指定，放在调用时指定
    }

    /// <summary>
    /// 添加一个参数
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public AliWebApiQueryBuilder AddParam(string name, string value)
    {
        if( name.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(name));

        _args[name] = value ?? string.Empty;
        return this;
    }

    /// <summary>
    /// 生成满足阿里云的要求的WebAPi调用查询字符串
    /// </summary>
    /// <param name="httpMethod"></param>
    /// <returns></returns>
    public string GetQueryString(string httpMethod)
    {
        // 步骤二：根据参数Key排序（顺序）
        string sortQueryString = string.Join("&",
                _args.OrderBy(x => x.Key, StringComparer.Ordinal)
                     .Select(x => x.Key + "=" + SbEncode(x.Value)));

        // 步骤三：构造待签名的请求串
        string stringToSign = httpMethod + "&" + SbEncode("/") + "&" + SbEncode(sortQueryString);

        // 步骤四：签名
        string sign = GetSign(_accessKeySecret, stringToSign);

        // 获取完整的查询字符串
        return "Signature=" + SbEncode(sign) + "&" + sortQueryString;
    }

    private static string SbEncode(string text)
    {
        return Uri.EscapeDataString(text).Replace("+", "%20").Replace("*", "%2A").Replace("%7E", "~");
    }

    private static string GetSign(string accessSecret, string stringToSign)
    {
        using( HMACSHA1 hmac = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret + "&")) ) {
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign));
            return Convert.ToBase64String(hashValue);
        }
    }
}
