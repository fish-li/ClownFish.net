namespace ClownFish.Jwt.Impl;

internal sealed class JwtHMACSHA256 : JwtBase
{
    public const string AlgorithmName = "HS256";
    public static readonly JwtHMACSHA256 Instance = new JwtHMACSHA256();

    private static readonly string s_headerText = JwtHeader.Create(AlgorithmName).ToJson().Base64UrlEncode();

    public override string Name => AlgorithmName;

    protected override string GetHeader()
    {
        return s_headerText;
    }

    protected override string GetSignature(object secret, byte[] bytesToSign)
    {
        byte[] value = HashHelper.HMACSHA256((byte[])secret, bytesToSign);
        return NbJwtBase64UrlEncoder.Encode(value);
    }

    protected override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        string value = GetSignature(secret, bytesToSign);
        if( value != signature)
            throw new SignatureVerificationException("Jwt Token Invalid signature");
    }
}
