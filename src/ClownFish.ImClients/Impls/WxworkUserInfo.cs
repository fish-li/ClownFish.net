namespace ClownFish.ImClients.Impls;

/// <summary>
/// 企业微信的用户信息
/// </summary>
public sealed class WxworkUserInfo
{
    /// <summary>
    /// UserId
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// UserName
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 岗位
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 头像-大
    /// </summary>
    public string PhotoBig { get; set; }

    /// <summary>
    /// 头像-小
    /// </summary>
    public string PhotoSmall { get; set; }

    /// <summary>
    /// 入职时间
    /// </summary>
    public string HireDate { get; set; }

    /// <summary>
    /// 员工编号
    /// </summary>
    public string UserCode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string UserCode2 { get; set; }


    /// <summary>
    /// 将WxworkResult转换成WxworkUserInfo
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator WxworkUserInfo(ImShitResult result)
    {
        return new WxworkUserInfo {
            UserId = result.Json["userid"]?.ToString(),
            UserName = result.Json["name"]?.ToString(),
            Position = result.Json["position"]?.ToString(),
            Mobile = result.Json["mobile"]?.ToString(),
            Email = result.Json["email"]?.ToString(),
            Status = (int)result.Json["status"],
            PhotoBig = result.Json["avatar"]?.ToString(),
            PhotoSmall = result.Json["thumb_avatar"]?.ToString(),
            UserCode = GetAttr(result, "员工编号"),
            HireDate = GetAttr(result, "入职时间"),
            UserCode2 = GetAttr(result, "xxxx")
        };

        string GetAttr(ImShitResult result, string name)
        {
            JArray attrs = result.Json["attrs"] as JArray;

            if( attrs == null )
                return null;

            foreach( var x in attrs ) {
                if( x["name"]?.ToString() == name )
                    return x["value"].ToString();
            }
            return null;
        }

    }
}
