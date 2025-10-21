namespace ClownFish.Jwt.Impl;

#if NET46_OR_GREATER|| NETCOREAPP

/// <summary>
/// 
/// </summary>
public sealed class JwtRSA256 : JwtBase
{
    internal const string AlgorithmName = "RS256";
    internal static readonly JwtRSA256 Instance = new JwtRSA256();

    private static readonly string s_headerText = JwtHeader.Create(AlgorithmName).ToJson().Base64UrlEncode();

    /// <inheritdoc/>
    public override string Name => AlgorithmName;

    /// <inheritdoc/>
    public override string GetHeader()
    {
        return s_headerText;
    }

    /// <inheritdoc/>
    public override string GetSignature(object secret, byte[] bytesToSign)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        return RsaUtils.GetSignature(HashAlgorithmName.SHA256, x509, bytesToSign);
    }


    /// <inheritdoc/>
    public override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        RsaUtils.ValidSignature(HashAlgorithmName.SHA256, x509, bytesToSign, signature);
    }
}

#endif