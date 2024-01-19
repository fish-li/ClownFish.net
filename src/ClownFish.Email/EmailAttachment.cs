namespace ClownFish.Email;

internal class EmailAttachment
{
    public string Name { get; set; }

    public byte[] Content { get; set; }

    public string ContentType { get; set; }
}
