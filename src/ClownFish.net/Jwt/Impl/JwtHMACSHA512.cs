namespace ClownFish.Jwt.Impl;

internal sealed class JwtHMACSHA512 : JwtBase
{
    public const string AlgorithmName = "HS512";
    public static readonly JwtHMACSHA512 Instance = new JwtHMACSHA512();

    private static readonly string s_headerText = JwtHeader.Create(AlgorithmName).ToJson().Base64UrlEncode();

    public override string Name => AlgorithmName;

    protected override string GetHeader()
    {
        return s_headerText;
    }

    protected override string GetSignature(object secret, byte[] bytesToSign)
    {
        byte[] input = HashHelper.HMACSHA512((byte[])secret, bytesToSign);
        return NbJwtBase64UrlEncoder.Encode(input);
    }

    protected override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        string value = GetSignature(secret, bytesToSign);
        if( value != signature )
            throw new SignatureVerificationException("Jwt Token Invalid signature");
    }
}
