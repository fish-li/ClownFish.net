namespace ClownFish.Web.UnitTest.Utils;

internal sealed class NullHttpContext : NHttpContext
{
    internal static readonly NullHttpContext Instance = new NullHttpContext();

    public override object OriginalHttpContext => throw new NotImplementedException();

    public override NHttpRequest Request => throw new NotImplementedException();

    public override NHttpResponse Response => throw new NotImplementedException();

    public override IPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override bool SkipAuthorization { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override XDictionary Items => throw new NotImplementedException();

}
