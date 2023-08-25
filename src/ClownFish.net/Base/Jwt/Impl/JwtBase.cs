namespace ClownFish.Base.Jwt.Impl;

internal abstract class JwtBase
{
    protected abstract string GetHeader();

    protected abstract string GetSignature(byte[] secretKey, byte[] bytesToSign);



    public string Encode(string payload, byte[] secretKey)
    {
        if( payload.IsNullOrEmpty() ) 
            throw new ArgumentNullException(nameof(payload));
        

        string text1 = GetHeader();
        string text2 = NbJwtBase64UrlEncoder.Encode(payload.ToUtf8Bytes());
        string text3 = text1 + "." + text2;

        string signature = GetSignature(secretKey, text3.ToUtf8Bytes());
        return text3 + "." + signature;
    }


    public string Decode(string token, byte[] secretKey)
    {
        JwtParts jwt = new JwtParts(token);

        if( secretKey != null ) {
            Validate(jwt, secretKey);
        }

        return NbJwtBase64UrlEncoder.Decode(jwt.Payload).ToUtf8String();            
    }

    private void Validate(JwtParts jwt, byte[] secretKey)
    {
        // JWT-TOKEN在验证时，算法应该使用 header 中指定的算法，但是对于一个应用程序来说，它【不可能】同时使用多种签名算法
        // 反而从 header 中读取算法名称这种做法会引入安全漏洞，所以这里的设计是按Encode的方式来重新计算签名

        byte[] bytes3 = (jwt.Header + "." + jwt.Payload).ToUtf8Bytes();
        string signature = GetSignature(secretKey, bytes3);

        if( signature != jwt.Signature )
            throw new SignatureVerificationException("Jwt Token Invalid signature");
    }
}
