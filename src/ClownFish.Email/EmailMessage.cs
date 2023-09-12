using MimeKit;

namespace ClownFish.EMail;

internal class EmailMessage
{
    public List<MailboxAddress> Receivers { get; set; }

    public List<MailboxAddress> CC { get; set; }

    public List<EmailAttachment> Attachments { get; set; }

    public string Subject { get; set; }

    public string Body { get; set; }


    internal long GetSize()
    {
        long sum = this.Body.Length;

        if( this.Attachments.HasValue() ) {
            foreach( var x in this.Attachments )
                sum += x.Content.Length;
        }

        return sum;
    }


    internal void Validate()
    {
        if( this.Receivers.IsNullOrEmpty() )
            throw new ValidationException2("Receivers 不能为空");

        if( this.Subject.IsNullOrEmpty() )
            throw new ValidationException2("Subject 不能为空");

        if( this.Body.IsNullOrEmpty() )
            throw new ValidationException2("Body 不能为空");
    }

}
