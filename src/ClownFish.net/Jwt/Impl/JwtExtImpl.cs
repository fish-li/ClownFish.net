namespace ClownFish.Jwt.Impl;

internal class JwtExtImpl : JwtBase
{
    private readonly IJwtAlgorithm2 _impl = null;

    public JwtExtImpl(IJwtAlgorithm2 impl)
    {
        _impl = impl;
    }

    public override string Name => _impl.Name;

    protected override string GetHeader()
    {
        return _impl.GetHeaderJson().Base64UrlEncode();
    }

    protected override string GetSignature(object secret, byte[] bytesToSign)
    {
        return _impl.GetSignature(secret, bytesToSign);
    }

    protected override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        _impl.ValidSignature(secret, bytesToSign, signature);
    }
}


internal static class JwtExtMananger
{
    private static readonly TSafeDictionary<string, JwtExtImpl> s_dict = new TSafeDictionary<string, JwtExtImpl>();

    public static void RegisterAlgorithmImpl(IJwtAlgorithm2 jwtAlgorithm)
    {
        if( jwtAlgorithm == null )
            throw new ArgumentNullException(nameof(jwtAlgorithm));

        string name = jwtAlgorithm.Name;
        if( name.IsNullOrEmpty() )
            throw new ValidationException2("jwtAlgorithm.Name is null");

        string header = jwtAlgorithm.GetHeaderJson();
        if( header.IsNullOrEmpty() )
            throw new ValidationException2("jwtAlgorithm.GetHeaderJson() return null");

        s_dict[name] = new JwtExtImpl(jwtAlgorithm);
    }

    public static JwtExtImpl GetImpl(string name)
    {
        return s_dict.TryGet(name);
    }
}