using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace ClownFish.Email;

/// <summary>
/// 邮件发送工具
/// </summary>
public sealed class MailClient
{
    private readonly EmailMessage _message = new EmailMessage();

    internal EmailMessage EmailData => _message; 

    private SmtpConfig _config;

    private bool _isHtmlBody = false;

    /// <summary>
    /// 日志文件路径，可以不设置（不记录日志）。
    /// </summary>
    public string LogFilePath { get; set; }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="config"></param>
    public MailClient(SmtpConfig config)
    {
        Init(config);
    }


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="settingName">配置服务中的SmtpConfig参数名称</param>
    public MailClient(string settingName)
    {
        SmtpConfig config = Settings.GetSetting<SmtpConfig>(settingName, true);
        Init(config);
    }

    private void Init(SmtpConfig config)
    {
        if( config == null )
            throw new ArgumentNullException(nameof(config));

        config.Validate();
        _config = config;
    }

    /// <summary>
    /// 设置邮件的收件人
    /// </summary>
    /// <param name="receivers"></param>
    public MailClient SetReceivers(params string[] receivers)
    {
        if( receivers.IsNullOrEmpty() == false ) {
            //_message.Receivers = receivers.Select(x => new MailboxAddress(null, x)).ToList();
            _message.Receivers = receivers.Select(x => MailboxAddress.Parse(x)).ToList();
        }
        return this;
    }

    /// <summary>
    /// 设置邮件的收件人
    /// </summary>
    /// <param name="receivers"></param>
    public MailClient SetReceivers(params NameValue[] receivers)
    {
        if( receivers.IsNullOrEmpty() == false ) {
            _message.Receivers = receivers.Select(x => CreateMailboxAddress(x)).ToList();
        }
        return this;
    }


    private static MailboxAddress CreateMailboxAddress(NameValue x)
    {
        if( x.Name.HasValue() )
            return new MailboxAddress(x.Name, x.Value);
        else
            return new MailboxAddress(null, x.Value);
    }

    /// <summary>
    /// 设置邮件的抄送人
    /// </summary>
    /// <param name="cc"></param>
    /// <returns></returns>
    public MailClient SetCC(params string[] cc)
    {
        if( cc.IsNullOrEmpty() == false ) {
            _message.CC = cc.Select(x => MailboxAddress.Parse(x)).ToList();
        }
        return this;
    }

    /// <summary>
    /// 设置邮件的抄送人
    /// </summary>
    /// <param name="cc"></param>
    /// <returns></returns>
    public MailClient SetCC(params NameValue[] cc)
    {
        if( cc.IsNullOrEmpty() == false ) {
            _message.CC = cc.Select(x => CreateMailboxAddress(x)).ToList();
        }
        return this;
    }

    /// <summary>
    /// 添加一个邮件附件
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="content"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public MailClient AddAttachment(string filename, byte[] content, string contentType)
    {
        EmailAttachment attachment = new EmailAttachment {
            Name = filename,
            Content = content,
            ContentType = contentType
        };

        if( _message.Attachments == null )
            _message.Attachments = new List<EmailAttachment>();

        _message.Attachments.Add(attachment);
        return this;
    }


    /// <summary>
    /// 添加一个邮件附件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public MailClient AddAttachment(string filePath, string contentType)
    {
        byte[] content = RetryFile.ReadAllBytes(filePath);
        string filename = Path.GetFileName(filePath);

        return this.AddAttachment(filename, content, contentType);
    }

    /// <summary>
    /// 设置邮件主题
    /// </summary>
    /// <param name="subject"></param>
    /// <returns></returns>
    public MailClient SetSubject(string subject)
    {
        _message.Subject = subject ?? string.Empty;
        return this;
    }

    /// <summary>
    /// 设置邮件主体内容
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public MailClient SetBody(string body)
    {
        _message.Body = body ?? string.Empty;
        return this;
    }


    /// <summary>
    /// 设置邮件主体内容
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public MailClient SetHtmlBody(string html)
    {
        _message.Body = html ?? string.Empty;
        _isHtmlBody = true;
        return this;
    }


    private MimeMessage GetMimeMessage()
    {
        _message.Validate();

        MimeMessage mail = new MimeMessage();

        foreach( var x in _message.Receivers )
            mail.To.Add(x);


        if( _message.CC.IsNullOrEmpty() == false ) {
            foreach( var x in _message.CC )
                mail.Cc.Add(x);
        }

        mail.Subject = _message.Subject;

        TextPart bodyPart = _isHtmlBody
                            ? new TextPart(TextFormat.Html) { Text = _message.Body }
                            : new TextPart(TextFormat.Plain) { Text = _message.Body };

        Multipart multipart = new Multipart("mixed");
        multipart.Add(bodyPart);


        if( _message.Attachments.IsNullOrEmpty() == false ) {

            foreach( EmailAttachment atmt in _message.Attachments ) {
                if( atmt.Name.IsNullOrEmpty() || atmt.Content.IsNullOrEmpty() )
                    continue;

                MemoryStream ms = new MemoryStream(atmt.Content);

                string mediaType = atmt.ContentType.IfEmpty("application/octet-stream");

                MimePart attachment = new MimePart(mediaType) {
                    Content = new MimeContent(ms),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64
                };
                attachment.ContentDisposition.Parameters.Add(Encoding.UTF8, "filename", atmt.Name);
                multipart.Add(attachment);
            }
        }

        mail.Body = multipart;
        return mail;
    }


    private SmtpClient CreateClient()
    {
        if( string.IsNullOrEmpty(this.LogFilePath) ) {
            return new SmtpClient();
        }
        else {
            ProtocolLogger logger = new ProtocolLogger(this.LogFilePath);
            return new SmtpClient(logger);
        }
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    public void Send(CancellationToken cancellationToken = default(CancellationToken))
    {
        using MimeMessage mail = GetMimeMessage();

        mail.From.Add(MailboxAddress.Parse(_config.UserName));

        using( SmtpClient client = CreateClient() ) {

            DateTime start = DateTime.Now;
            Exception lastException = null;

            try {
                SecureSocketOptions options = _config.IsSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
                client.Connect(_config.Host, _config.Port, options, cancellationToken);
                client.Authenticate(_config.UserName, _config.Password, cancellationToken);

                client.Send(mail, cancellationToken);
                client.Disconnect(true);
            }
            catch( Exception ex ) {
                lastException = ex;
                throw;
            }
            finally {
                MailClientEvent.SendMail(this, false, mail, start, lastException);
            }
        }
    }


    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <returns></returns>
    public async Task SendAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        using MimeMessage mail = GetMimeMessage();

        mail.From.Add(MailboxAddress.Parse(_config.UserName));

        using( SmtpClient client = CreateClient() ) {

            DateTime start = DateTime.Now;
            Exception lastException = null;
            try {
                SecureSocketOptions options = _config.IsSSL ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
                await client.ConnectAsync(_config.Host, _config.Port, options, cancellationToken);
                await client.AuthenticateAsync(_config.UserName, _config.Password, cancellationToken);

                await client.SendAsync(mail, cancellationToken);
                await client.DisconnectAsync(true);
            }
            catch( Exception ex ) {
                lastException = ex;
                throw;
            }
            finally {
                MailClientEvent.SendMail(this, true, mail, start, lastException);
            }
        }
    }



}
