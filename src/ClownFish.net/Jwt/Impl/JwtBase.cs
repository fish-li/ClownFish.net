namespace ClownFish.Jwt.Impl;

internal abstract class JwtBase
{
    public abstract string Name { get; }

    protected abstract string GetHeader();

    protected abstract string GetSignature(object secret, byte[] bytesToSign);

    protected abstract void ValidSignature(object secret, byte[] bytesToSign, string signature);


    public string Encode(string payload, object secret)
    {
        if( payload.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(payload));


        string text1 = GetHeader();
        string text2 = payload.Base64UrlEncode();
        string text3 = text1 + "." + text2;

        string signature = GetSignature(secret, text3.ToUtf8Bytes());
        return text3 + "." + signature;
    }


    public string Decode(string token, object secret)
    {
        JwtParts jwt = new JwtParts(token);

        if( secret != null ) {
            // JWT-TOKEN在验证时，算法应该使用 header 中指定的算法，但是对于一个应用程序来说，它【不可能】同时使用多种签名算法
            // 反而从 header 中读取算法名称这种做法会引入安全漏洞，所以这里的设计是按Encode的方式来重新计算签名

            // 说明：正式环境中，JWT校验失败的可能性太低，所以取消下面的代码，节省不必要的性能损耗

            //JwtHeader header = jwt.Header.Base64UrlDecode().FromJson<JwtHeader>();
            //if( header == null || header.Algorithm.IsNullOrEmpty() )
            //    throw new InvalidTokenPartsException("token格式不正确3");

            //if( header.Algorithm != this.Name)
            //    throw new SignatureVerificationException($"Jwt Token signature algorithmName mismatch: {header.Algorithm} != {this.Name}");

            byte[] bytesToSign = (jwt.Header + "." + jwt.Payload).ToUtf8Bytes();
            ValidSignature(secret, bytesToSign, jwt.Signature);
        }

        return NbJwtBase64UrlEncoder.Decode(jwt.Payload).ToUtf8String();
    }

 
}
