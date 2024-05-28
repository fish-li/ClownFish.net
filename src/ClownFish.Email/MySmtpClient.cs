using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace ClownFish.Email;


internal class MySmtpClient : SmtpClient
{

    public MySmtpClient() { }


    public MySmtpClient(IProtocolLogger protocolLogger) : base(protocolLogger)
    {

    }


    protected override void OnSenderNotAccepted(MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
    {
        // 忽略收件人不存在的错误
    }


    protected override void OnRecipientNotAccepted(MimeMessage message, MailboxAddress mailbox, SmtpResponse response)
    {
        // 忽略收件人不存在的错误
    }
}
