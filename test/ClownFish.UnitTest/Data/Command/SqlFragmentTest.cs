namespace ClownFish.UnitTest.Data.Command;

[TestClass]
public class SqlFragmentTest
{
    [TestMethod]
    public void Test1()
    {
        SqlFragment s1 = new SqlFragment("xxx");
        Assert.AreEqual("xxx", s1.Value);

        MyAssert.IsError<ArgumentNullException>(() => {
            SqlFragment s2 = new SqlFragment("");
        });
    }
}
