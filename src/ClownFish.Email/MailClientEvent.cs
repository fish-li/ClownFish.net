using MimeKit;

namespace ClownFish.Email;

internal static class MailClientEvent
{
    /// <summary>
    /// 每次发送一条消息后触发
    /// </summary>
    public static event EventHandler<SendMailEventArgs> OnSendMail;

    internal static void SendMail(MailClient client, bool isAsync, MimeMessage mail, DateTime start, Exception ex)
    {
        SendMailEventArgs e = null;

        EventHandler<SendMailEventArgs> handler = OnSendMail;
        if( handler != null ) {
            if( e == null )
                e = new SendMailEventArgs {
                    IsAsync = isAsync,
                    Mail = mail,
                    StartTime = start,
                    EndTime = DateTime.Now,
                    Exception = ex
                };
            handler(client, e);
        }

    }

}



/// <summary>
/// SendMailEventArgs
/// </summary>
public sealed class SendMailEventArgs : EventArgs
{
    /// <summary>
    /// IsAsync
    /// </summary>
    public bool IsAsync { get; internal set; }
    /// <summary>
    /// Mail Message
    /// </summary>
    public MimeMessage Mail { get; internal set; }

    /// <summary>
    /// StartTime
    /// </summary>
    public DateTime StartTime { get; internal set; }

    /// <summary>
    /// EndTime
    /// </summary>
    public DateTime EndTime { get; internal set; }

    /// <summary>
    /// 与执行相关的异常对象
    /// </summary>
    public Exception Exception { get; internal set; }
}
