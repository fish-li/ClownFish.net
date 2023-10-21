namespace ClownFish.Jwt.Impl;

internal class JwtRSA512 : JwtBase
{
    public const string AlgorithmName = "RS512";
    public static readonly JwtRSA512 Instance = new JwtRSA512();

    private static readonly string s_headerText = JwtHeader.Create(AlgorithmName).ToJson().Base64UrlEncode();

    public override string Name => AlgorithmName;

    protected override string GetHeader()
    {
        return s_headerText;
    }

    protected override string GetSignature(object secret, byte[] bytesToSign)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        return RsaUtils.GetSignature(HashAlgorithmName.SHA512, x509, bytesToSign);
    }

    protected override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        RsaUtils.ValidSignature(HashAlgorithmName.SHA512, x509, bytesToSign, signature);
    }

}
