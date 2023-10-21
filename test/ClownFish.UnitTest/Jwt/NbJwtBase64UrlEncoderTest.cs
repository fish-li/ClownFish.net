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
        }
    }


    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentException>(() => {
            _ = NbJwtBase64UrlEncoder.Encode((byte[])null);
        });

        MyAssert.IsError<ArgumentException>(() => {
            _ = NbJwtBase64UrlEncoder.Encode(Empty.Array<byte>());
        });

        MyAssert.IsError<ArgumentException>(() => {
            _ = NbJwtBase64UrlEncoder.Decode(null);
        });

        MyAssert.IsError<ArgumentException>(() => {
            _ = NbJwtBase64UrlEncoder.Decode("");
        });
    }
}
