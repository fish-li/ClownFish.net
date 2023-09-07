namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class ByteBufferTest
{
    [TestMethod]
    public void Test()
    {
        using( ByteBuffer buffer = new ByteBuffer(32) ) {
            Assert.IsTrue(buffer.Buffer.Length >= 32);
        }

        MyAssert.IsError<ArgumentOutOfRangeException>(()=> {
            _ = new ByteBuffer(0);
        });
    }
}
