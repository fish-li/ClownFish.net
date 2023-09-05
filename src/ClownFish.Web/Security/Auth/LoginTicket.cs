namespace ClownFish.Web.Security.Auth;

/// <summary>
/// JWT中保存的登录凭证
/// </summary>
public sealed class LoginTicket
{
    /// <summary>
    /// 用户对象
    /// </summary>
    public IUserInfo User { get; set; }

    /// <summary>
    /// 凭证的发布者
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// 凭证创建时间
    /// </summary>
    public long IssueTime { get; set; }

    /// <summary>
    /// 凭证有效截止时间
    /// </summary>
    public long Expiration { get; set; }


    // 说明：Token中的时间值有2种格式：
    //     1, datetime.Ticks       , 例如：638113896030207100
    //     2, datetime.ToNumber()  , 例如：20220207092259



    internal bool VerifyExpiration()
    {
        if( this.Expiration > 99991231235959 ) {  // 判断是哪种时间格式
            if( this.Expiration < DateTime.Now.Ticks )
                return false;
        }
        else {
            if( this.Expiration < DateTime.Now.ToNumber() )
                return false;
        }

        return true;
    }

    /// <summary>
    /// 凭证是否需要续期
    /// </summary>
    /// <returns></returns>
    internal bool IsNeedRefresh()
    {
        bool isLongTime = this.Expiration > 99991231235959;

        // 计算【一半有效期】的时间点
        DateTime issueTime = isLongTime ? new DateTime(this.IssueTime) : this.IssueTime.ToDateTime();
        DateTime expirationTime = isLongTime ? new DateTime(this.Expiration) : this.Expiration.ToDateTime();

        int expirationSeconds = (int)(expirationTime - issueTime).TotalSeconds;

        DateTime halfExpiration = issueTime.AddSeconds(expirationSeconds / 2);

        // 如果当前时间  大于 【一半有效期】，就需要自动续期
        return DateTime.Now > halfExpiration;
    }


    internal int GetSeconds()
    {
        if( this.Expiration > 99991231235959 )
            return (int)(new DateTime(this.Expiration) - new DateTime(this.IssueTime)).TotalSeconds;
        else
            return (int)(this.Expiration.ToDateTime() - this.IssueTime.ToDateTime()).TotalSeconds;
    }

}
