namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class DataSpliterTest
{
    [TestMethod]
    public void Test1()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 97));
        list.Add(new string('a', 98));
        list.Add(new string('a', 99));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(97, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(98, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }


    [TestMethod]
    public void Test2()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 20));
        list.Add(new string('a', 30));
        list.Add(new string('a', 40));

        list.Add(new string('a', 98));
        list.Add(new string('a', 120));
        list.Add(new string('a', 30));
        list.Add(new string('a', 100));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(92, line.Length);  // 20+1+30+1+40

        line = spliter.GetNextPart();
        Assert.AreEqual(98, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(120, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(30, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(100, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }


    [TestMethod]
    public void Test3()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 100));
        list.Add(new string('a', 100));
        list.Add(new string('a', 99));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(100, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(100, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }


    [TestMethod]
    public void Test4()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 110));

        list.Add(new string('a', 10));
        list.Add(new string('a', 20));
        list.Add(new string('a', 30));

        list.Add(new string('a', 40));
        list.Add(new string('a', 99));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(110, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(62, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(40, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }


    [TestMethod]
    public void Test5()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 100));
        list.Add(new string('a', 100));
        list.Add(new string('a', 100));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(100, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(100, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(100, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }


    [TestMethod]
    public void Test6()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 101));
        list.Add(new string('a', 101));
        list.Add(new string('a', 101));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(101, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(101, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(101, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }

    [TestMethod]
    public void Test7()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 99));
        list.Add(new string('a', 99));
        list.Add(new string('a', 99));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }



    [TestMethod]
    public void Test8()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 50));
        list.Add(new string('a', 49));

        list.Add(new string('a', 1));

        list.Add(new string('a', 99));

        list.Add(new string('a', 1));
        list.Add(new string('a', 98));

        list.Add(new string('a', 98));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(100, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(1, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(100, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(98, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }


    [TestMethod]
    public void Test8b()
    {
        List<string> list = new List<string>();
        list.Add(new string('a', 50));

        list.Add(new string('a', 49));
        list.Add(new string('a', 1));

        list.Add(new string('a', 99));

        list.Add(new string('a', 1));

        list.Add(new string('a', 98));

        list.Add(new string('a', 98));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\r\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(50, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(52, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(1, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(98, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(98, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }


    [TestMethod]
    public void Test9()
    {
        List<string> list = new List<string>();
        list.Add(string.Empty);
        list.Add(string.Empty);
        list.Add(new string('a', 99));
        list.Add(string.Empty);
        list.Add(new string('a', 99));
        list.Add(string.Empty);
        list.Add(string.Empty);
        list.Add(new string('a', 99));

        DataSpliter<string> spliter = new DataSpliter<string>(list, x => x, 100, "\n");

        string line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.AreEqual(99, line.Length);

        line = spliter.GetNextPart();
        Assert.IsNull(line);
    }


    [TestMethod]
    public void Test10()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            DataSpliter<string> spliter = new DataSpliter<string>(null, x => x, 100, "\n");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            DataSpliter<string> spliter = new DataSpliter<string>(new List<string>(), null, 100, "\n");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            DataSpliter<string> spliter = new DataSpliter<string>(new List<string>(), x => x, 0, "\n");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            DataSpliter<string> spliter = new DataSpliter<string>(new List<string>(), x => x, -1, "\n");
        });
    }
}
