namespace ClownFish.Jwt.Impl;

internal static class RsaUtils
{
    internal static string GetSignature(HashAlgorithmName hashName, X509Certificate2 x509, byte[] bytesToSign)
    {
        using RSA privateKey = x509.GetRSAPrivateKey();
        if( privateKey == null )
            throw new ArgumentException("证书没有包含私钥!");

        byte[] result = privateKey.SignData(bytesToSign, hashName, RSASignaturePadding.Pkcs1);

        return NbJwtBase64UrlEncoder.Encode(result);
    }


    internal static void ValidSignature(HashAlgorithmName hashName, X509Certificate2 x509, byte[] bytesToSign, string signature)
    {
        using RSA publicKey = x509.GetRSAPublicKey();
        if( publicKey == null )
            throw new ArgumentException("证书没有包含公钥!");

        try {
            byte[] signBytes = NbJwtBase64UrlEncoder.Decode(signature);
            if( publicKey.VerifyData(bytesToSign, signBytes, hashName, RSASignaturePadding.Pkcs1) == false )
                throw new SignatureVerificationException("Jwt Token Invalid signature");
        }
        catch( CryptographicException ) {
            throw new SignatureVerificationException("Jwt Token Invalid signature,2");
        }
    }

}

internal static class EcdUtils
{
    internal static string GetSignature(HashAlgorithmName hashName, X509Certificate2 x509, byte[] bytesToSign)
    {
        using ECDsa privateKey = x509.GetECDsaPrivateKey();
        if( privateKey == null )
            throw new ArgumentException("证书没有包含私钥!");

        byte[] result = privateKey.SignData(bytesToSign, hashName);

        return NbJwtBase64UrlEncoder.Encode(result);
    }


    internal static void ValidSignature(HashAlgorithmName hashName, X509Certificate2 x509, byte[] bytesToSign, string signature)
    {
        using ECDsa publicKey = x509.GetECDsaPublicKey();
        if( publicKey == null )
            throw new ArgumentException("证书没有包含公钥!");

        try {
            byte[] signBytes = NbJwtBase64UrlEncoder.Decode(signature);
            if( publicKey.VerifyData(bytesToSign, signBytes, hashName) == false )
                throw new SignatureVerificationException("Jwt Token Invalid signature");
        }
        catch( CryptographicException ) {
            throw new SignatureVerificationException("Jwt Token Invalid signature,2");
        }
    }
}

