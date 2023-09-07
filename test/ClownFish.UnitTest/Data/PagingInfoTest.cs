namespace ClownFish.UnitTest.Data;

[TestClass]
public class PagingInfoTest
{
    [TestMethod]
    public void Test()
    {
        PagingInfo p1 = new PagingInfo {
            PageIndex = 5,
            PageSize = 10,
            TotalRows = 33
        };

        Assert.AreEqual(4, p1.CalcPageCount());


        PagingInfo p2 = new PagingInfo {
            PageIndex = 5,
            PageSize = 0,
            TotalRows = 33
        };

        Assert.AreEqual(0, p2.CalcPageCount());


        PagingInfo p3 = new PagingInfo {
            PageIndex = 5,
            PageSize = 10,
            TotalRows = 0
        };

        Assert.AreEqual(0, p3.CalcPageCount());
    }
}
