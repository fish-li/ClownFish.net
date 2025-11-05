using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Clients.Elastic;

namespace ClownFish.UnitTest.Http.Clients.Elastic;
[TestClass]
public class EsIndexNameTimeFormatTest
{
    [TestMethod]
    public void Test1()
    {
        DateTime time = new DateTime(2025, 3, 25, 19, 22, 33);

        IEsIndexNameTimeFormat format1 = EsIndexNameTimeFormat.GetImpl("-yyyyMMdd");
        Assert.AreEqual("-20250325", format1.TimeToString(time));

        IEsIndexNameTimeFormat format2 = EsIndexNameTimeFormat.GetImpl("-yyyyMM");
        Assert.AreEqual("-202503", format2.TimeToString(time));

        IEsIndexNameTimeFormat format3 = EsIndexNameTimeFormat.GetImpl("-yyyyMMdd-HH");
        Assert.AreEqual("-20250325-19", format3.TimeToString(time));
    }

    [TestMethod]
    public void Test2()
    {
        DateTime time = new DateTime(2025, 3, 25, 20, 22, 33);

        IEsIndexNameTimeFormat format1 = EsIndexNameTimeFormat.GetImpl("-3d");
        Assert.AreEqual("-202503-9", format1.TimeToString(time));

        IEsIndexNameTimeFormat format2 = EsIndexNameTimeFormat.GetImpl("-5d");
        Assert.AreEqual("-202503-6", format2.TimeToString(time));

        IEsIndexNameTimeFormat format3 = EsIndexNameTimeFormat.GetImpl("-3h");
        Assert.AreEqual("-20250325-7", format3.TimeToString(time));

        IEsIndexNameTimeFormat format4 = EsIndexNameTimeFormat.GetImpl("-5h");
        Assert.AreEqual("-20250325-5", format4.TimeToString(time));
    }

    [TestMethod]
    public void Test3()
    {
        DateTime time = new DateTime(2025, 3, 2, 2, 22, 33);

        IEsIndexNameTimeFormat format1 = EsIndexNameTimeFormat.GetImpl("-3d");
        Assert.AreEqual("-202503-1", format1.TimeToString(time));

        IEsIndexNameTimeFormat format2 = EsIndexNameTimeFormat.GetImpl("-5d");
        Assert.AreEqual("-202503-1", format2.TimeToString(time));

        IEsIndexNameTimeFormat format3 = EsIndexNameTimeFormat.GetImpl("-3h");
        Assert.AreEqual("-20250302-1", format3.TimeToString(time));

        IEsIndexNameTimeFormat format4 = EsIndexNameTimeFormat.GetImpl("-5h");
        Assert.AreEqual("-20250302-1", format4.TimeToString(time));
    }

    [TestMethod]
    public void Test4()
    {
        DateTime time = new DateTime(2025, 3, 25, 19, 22, 33);

        IEsIndexNameTimeFormat format1 = EsIndexNameTimeFormat.GetImpl("-1d");
        Assert.AreEqual("-125", format1.TimeToString(time));

        IEsIndexNameTimeFormat format2 = EsIndexNameTimeFormat.GetImpl("-17d");
        Assert.AreEqual("-1725", format2.TimeToString(time));

        IEsIndexNameTimeFormat format3 = EsIndexNameTimeFormat.GetImpl("-1h");
        Assert.AreEqual("-17", format3.TimeToString(time));

        IEsIndexNameTimeFormat format4 = EsIndexNameTimeFormat.GetImpl("-15h");
        Assert.AreEqual("-157", format4.TimeToString(time));

        Assert.IsNull(EsIndexNameTimeFormat.GetImpl(""));
    }
}
