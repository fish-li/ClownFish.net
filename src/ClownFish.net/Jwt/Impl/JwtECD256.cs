namespace ClownFish.Jwt.Impl;

internal class JwtECD256 : JwtBase
{
    public const string AlgorithmName = "ES256";
    public static readonly JwtECD256 Instance = new JwtECD256();

    private static readonly string s_headerText = JwtHeader.Create(AlgorithmName).ToJson().Base64UrlEncode();

    public override string Name => AlgorithmName;

    protected override string GetHeader()
    {
        return s_headerText;
    }

    protected override string GetSignature(object secret, byte[] bytesToSign)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        return EcdUtils.GetSignature(HashAlgorithmName.SHA256, x509, bytesToSign);
    }

    protected override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        EcdUtils.ValidSignature(HashAlgorithmName.SHA256, x509, bytesToSign, signature);
    }
}
