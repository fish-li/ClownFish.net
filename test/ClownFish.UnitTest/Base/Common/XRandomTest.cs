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
xx-char6: {char35}
xx-char7: {char-5}
xx-int: {int}
xx-int0: {int0}
xx-int1: {int1}
xx-int2: {int2}
xx-int5: {int5}
xx-int9: {int9}
xx-int10: {int10}
xx-int11: {int-5}
xx-time1: {now}
xx-time2: {5秒前}
xx-time3: {5分钟前}
xx-time4: {5小时前}
xx-time5: {5天前}
xx-time6: {-5天前}
xx-time7: {2月前}
xx-value01: {rand}
xx-value02: {now}
xx-value03: {昨天}
xx-value04: {今天}
xx-value05: {明天}
xx-value06: {月初}
xx-value07: {下月初}
xx-value08: {季度初}
xx-value09: {下季度初}
xx-value10: {年初}
xx-value11: {明年初}
xx-value12: {周一}
xx-value13: {下周一}
xx-local1: {LocalSetting_key1}
xx-local2: {LocalSetting_key2}
xx-local3: {LocalSetting_keyxx}
";


    [TestMethod]
    public void Test1()
    {
        string result = XRandom.FillTemplate(s_textTemplate);
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
        Assert.IsTrue(dict["xx-char7"] == "{char-5}");   // 不处理

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
        Assert.IsTrue(dict["xx-int11"] == "{int-5}");   // 不处理

        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time1"])).TotalSeconds < 3);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time2"])).TotalSeconds >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time3"])).TotalMinutes >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time4"])).TotalHours >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(dict["xx-time5"])).TotalDays >= 5);
        Assert.IsTrue(dict["xx-time6"] == "{-5天前}");   // 不处理
    }


    [TestMethod]
    public void Test2()
    {
        XRandom.RegisterValueGetter("USERID", GetCurrentUserId);

        string useId = XRandom.GetValue("USERID");
        Console.WriteLine(useId);

        Assert.IsNotNull(useId);
        Assert.IsTrue(useId.StartsWith0("u"));


        string sqltemplate = "select * from table1 where create_time >= '{今天}' and create_time < '{明天}' and userid = '{USERID}'";
        string sql = XRandom.FillTemplate(sqltemplate);
        Console.WriteLine(sql);
        Assert.IsFalse(sql.Contains("今天"));
        Assert.IsFalse(sql.Contains("明天"));
        Assert.IsFalse(sql.Contains("USERID"));
    }



    private static string GetCurrentUserId()
    {
        // 例如（不能运行）：return HttpPipelineContext.Get().HttpContext.User.Identity.Name;
        return "u" + DateTime.Now.Ticks.ToString();
    }

    [TestMethod]
    public void Test_error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            Func<string> getter = () => "aaaa";
            XRandom.RegisterValueGetter(null, getter);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            Func<string> getter = null;
            XRandom.RegisterValueGetter("key", getter);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            Func<string, string> getter = null;
            XRandom.RegisterValueGetter(getter);
        });


        string s1 = XRandom.GetValue();   // guid32
        Assert.IsTrue(s1.Length == 32);


        string template = "aaaaaaaaaaaaa";
        string s2 = XRandom.FillTemplate(template);
        Assert.AreEqual(template, s2);
    }

    [TestMethod]
    public void Test_GetTime()
    {
        string s1 = XRandom.GetTime("2天前");
        Assert.IsNotNull(s1);
    }

}
