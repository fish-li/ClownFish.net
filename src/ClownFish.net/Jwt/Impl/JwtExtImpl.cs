namespace ClownFish.Jwt.Impl;

internal class JwtExtImpl : JwtBase
{
    private readonly IJwtAlgorithm2 _impl = null;

    public JwtExtImpl(IJwtAlgorithm2 impl)
    {
        _impl = impl;
    }

    public override string Name => _impl.Name;

    public override string GetHeader()
    {
        return _impl.GetHeaderJson().Base64UrlEncode();
    }

    public override string GetSignature(object secret, byte[] bytesToSign)
    {
        return _impl.GetSignature(secret, bytesToSign);
    }

    public override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        _impl.ValidSignature(secret, bytesToSign, signature);
    }
}
