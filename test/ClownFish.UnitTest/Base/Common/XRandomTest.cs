namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class XRandomTest
{
    private static readonly string s_textTemplate = @"
x-null: #{xx}#
xx-guid: {guid}
xx-guid32: {guid32}
xx-guid36: {guid36}
xx-char: {char}
xx-char0: {char0}
xx-char1: {char1}
xx-char2: {char2}
xx-char3: {char3}
xx-char4: {char4}
xx-char5: {char5}
xx-int: {int}
xx-int0: {int0}
xx-int1: {int1}
xx-int2: {int2}
xx-int5: {int5}
xx-int9: {int9}
xx-int10: {int10}
xx-time1: {now}
xx-time2: {5秒前}
xx-time3: {5分钟前}
xx-time4: {5小时前}
xx-time5: {5天前}
";


    [TestMethod]
    public void Test1()
    {
        XRandom rand = new XRandom();
        string result = rand.FillTemplate(s_textTemplate);
        Console.WriteLine(result);

        List<NameValue> list = result.ToKVList(new char[] { '\r', '\n' }, ':');
        Dictionary<string, string> dict = list.ToDictionary2(list.Count, k => k.Name, v => v.Value);

        Assert.IsTrue(dict["x-null"] == "#{xx}#");

        Assert.IsTrue(dict["xx-guid"].Length == 36);
        Assert.IsTrue(dict["xx-guid32"].Length == 32);
        Assert.IsTrue(dict["xx-guid36"].Length == 36);

        Assert.IsTrue(dict["xx-char"].Length == 1);
        Assert.IsTrue(dict["xx-char0"] == "{char0}");   // 不处理
        Assert.IsTrue(dict["xx-char1"].Length == 1);
        Assert.IsTrue(dict["xx-char2"].Length == 2);
        Assert.IsTrue(dict["xx-char3"].Length == 3);
        Assert.IsTrue(dict["xx-char4"].Length == 4);
        Assert.IsTrue(dict["xx-char5"].Length == 5);

        Assert.IsTrue(int.Parse(dict["xx-int"]) > 0);
        Assert.IsTrue(dict["xx-int0"] == "{int0}");   // 不处理
        Assert.IsTrue(int.Parse(dict["xx-int1"]) > 0);
        Assert.IsTrue(dict["xx-int1"].Length == 1);
        Assert.IsTrue(int.Parse(dict["xx-int2"]) > 0);
        Assert.IsTrue(dict["xx-int2"].Length == 2);
        Assert.IsTrue(int.Parse(dict["xx-int5"]) > 0);
        Assert.IsTrue(dict["xx-int5"].Length == 5);
        Assert.IsTrue(int.Parse(dict["xx-int9"]) > 0);
        Assert.IsTrue(dict["xx-int9"].Length == 9);
        Assert.IsTrue(int.Parse(dict["xx-int10"]) > 0);
        Assert.IsTrue(dict["xx-int10"].Length >= 9);

        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time1"])).TotalSeconds < 3);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time2"])).TotalSeconds >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time3"])).TotalMinutes >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time4"])).TotalHours >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time5"])).TotalDays >= 5);
    }
}
