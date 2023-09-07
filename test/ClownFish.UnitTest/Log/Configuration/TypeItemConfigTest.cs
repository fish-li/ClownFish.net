namespace ClownFish.UnitTest.Log.Configuration;

[TestClass]
public class TypeItemConfigTest
{
    [TestMethod]
    public void Test()
    {
        TypeItemConfig config = new TypeItemConfig {
            DataType = "abc",
            Writers = "xml,json"
        };

        Assert.AreEqual("abc => xml,json", config.ToString());
    }
}
