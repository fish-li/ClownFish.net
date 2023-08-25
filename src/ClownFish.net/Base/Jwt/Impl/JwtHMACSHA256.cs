namespace ClownFish.Base.Jwt.Impl;

internal sealed class JwtHMACSHA256 : JwtBase
{
    private static readonly string s_headerJson = NbJwtBase64UrlEncoder.Encode(Encoding.UTF8.GetBytes(new JwtHeader {
                                                                                                            Type = "JWT",
                                                                                                            Algorithm = "HS256"
                                                                                                        }.ToJson()));

    protected override string GetHeader()
    {
        return s_headerJson;
    }

    protected override string GetSignature(byte[] secretKey, byte[] bytesToSign)
    {
        byte[] input = HashHelper.HMACSHA256(secretKey, bytesToSign);
        return NbJwtBase64UrlEncoder.Encode(input);
    }
}
