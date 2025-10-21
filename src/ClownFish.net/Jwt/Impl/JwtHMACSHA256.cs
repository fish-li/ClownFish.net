namespace ClownFish.Jwt.Impl;

/// <summary>
/// 
/// </summary>
public sealed class JwtHMACSHA256 : JwtBase
{
    internal const string AlgorithmName = "HS256";
    internal static readonly JwtHMACSHA256 Instance = new JwtHMACSHA256();

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
        byte[] value = HashHelper.HMACSHA256((byte[])secret, bytesToSign);
        return NbJwtBase64UrlEncoder.Encode(value);
    }

    /// <inheritdoc/>
    public override void ValidSignature(object secret, byte[] bytesToSign, string signature)
    {
        string value = GetSignature(secret, bytesToSign);
        if( value != signature)
            throw new SignatureVerificationException("Jwt Token signature verify failed");
    }
}
