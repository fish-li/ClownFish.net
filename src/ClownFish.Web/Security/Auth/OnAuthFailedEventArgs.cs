namespace ClownFish.Web.Security.Auth;
public sealed class OnAuthFailedEventArgs : EventArgs
{
    public string RequestId { get; set; }

    public string Url { get; set; }

    public string Token { get; set; }

    public string Reason { get; set; }

}
