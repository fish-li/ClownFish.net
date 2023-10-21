using ClownFish.Jwt;

namespace ClownFish.UnitTest.Jwt;

[TestClass]
public class JwtPartsTest
{
    [TestMethod]
    public void Test_1()
    {
        JwtParts jwt = new JwtParts("aa.bb.cc");
        Assert.AreEqual("aa", jwt.Header);
        Assert.AreEqual("bb", jwt.Payload);
        Assert.AreEqual("cc", jwt.Signature);
    }


    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentException>(() => {
            _ = new JwtParts(null);
        });

        MyAssert.IsError<ArgumentException>(() => {
            _ = new JwtParts("");
        });

        MyAssert.IsError<ArgumentException>(() => {
            _ = new JwtParts("   ");
        });

        MyAssert.IsError<InvalidTokenPartsException>(() => {
            _ = new JwtParts("aa.bb");
        });

        MyAssert.IsError<InvalidTokenPartsException>(() => {
            _ = new JwtParts("aa.bb.cc.dd");
        });
    }
}
