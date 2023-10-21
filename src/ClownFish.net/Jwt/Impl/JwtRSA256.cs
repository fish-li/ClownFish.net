namespace ClownFish.Jwt.Impl;

internal class JwtRSA256 : JwtBase
{
    public const string AlgorithmName = "RS256";
    public static readonly JwtRSA256 Instance = new JwtRSA256();

    private static readonly string s_headerText = JwtHeader.Create(AlgorithmName).ToJson().Base64UrlEncode();

    public override string Name => AlgorithmName;

    protected override string GetHeader()
    {
        return s_headerText;
    }

    protected override string GetSignature(object secret, byte[] bytesToSign)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        return RsaUtils.GetSignature(HashAlgorithmName.SHA256, x509, bytesToSign);
    }

    protected override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        RsaUtils.ValidSignature(HashAlgorithmName.SHA256, x509, bytesToSign, signature);
    }
}
