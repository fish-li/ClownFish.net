namespace ClownFish.Jwt.Impl;

#if NET461_OR_GREATER|| NETCOREAPP

/// <summary>
/// 
/// </summary>
public sealed class JwtECD512 : JwtBase
{
    internal const string AlgorithmName = "ES512";
    internal static readonly JwtECD512 Instance = new JwtECD512();

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
        return EcdUtils.GetSignature(HashAlgorithmName.SHA512, x509, bytesToSign);
    }

    /// <inheritdoc/>
    public override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        X509Certificate2 x509 = (X509Certificate2)secret;
        EcdUtils.ValidSignature(HashAlgorithmName.SHA512, x509, bytesToSign, signature);
    }
}

#endif
