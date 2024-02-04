using ClownFish.Jwt;

namespace ClownFish.UnitTest.Jwt;

[TestClass]
public class NbJwtBase64UrlEncoderTest
{
    [TestMethod]
    public void Test_1()
    {
        string s1 = "中文汉字";
        string s2 = ",!@#$@#*%^%&#%$!@#$--超级NB!";

        StringBuilder sb = new StringBuilder(s1);
        for(int i=0;i<1000;i++ ) {
            sb.Append(s2);

            string s3 = sb.ToString();
            
            string s4 = NbJwtBase64UrlEncoder.Encode(s3.ToUtf8Bytes());
            string s5 = NbJwtBase64UrlEncoder.Decode(s4).ToUtf8String();
            Assert.AreEqual(s3, s5);

            string s6 = s3.Base64UrlEncode();
            string s7 = s3.ToUtf8Bytes().Base64UrlEncode();
            Assert.AreEqual(s7, s6);
        }
    }


    [TestMethod]
    public void Test_null()
    {
        Assert.AreEqual(string.Empty, NbJwtBase64UrlEncoder.Encode((byte[])null));
        Assert.AreEqual(string.Empty, NbJwtBase64UrlEncoder.Encode(Empty.Array<byte>()));
        Assert.IsTrue(NbJwtBase64UrlEncoder.Decode(null).Length ==0);
        Assert.IsTrue(NbJwtBase64UrlEncoder.Decode("").Length == 0);
    }
}
