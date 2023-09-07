namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class TextTemplateTest
{
    private static readonly string s_httpTemplate = @"
POST http://www.abc.com/test/callback.aspx?data={enc(data.cn)}&v={rand} HTTP/1.1
x-header1: qqqqqqqqqqqqq
x-header2: wwwwwwwwwwwww
x-xx1: {data.xx1}
x-xx2: {enc(data.xx2)}
x-xx3: {data.xx333}
x-client-app: TestApp2
x-client-reqid: {guid}
x-client-reqid32: {guid32}
x-client-url: http://www.fish-test.com/aaa/bb.aspx
Content-Type: application/json; charset=utf-8
x-null: #{xx}#
xx-guid: {rand.guid}
xx-guid32: {rand.guid32}
xx-guid36: {rand.guid36}
xx-char: {rand.char}
xx-char0: {rand.char0}
xx-char1: {rand.char1}
xx-char2: {rand.char2}
xx-char3: {rand.char3}
xx-char4: {rand.char4}
xx-char5: {rand.char5}
xx-int: {rand.int}
xx-int0: {rand.int0}
xx-int1: {rand.int1}
xx-int2: {rand.int2}
xx-int5: {rand.int5}
xx-int9: {rand.int9}
xx-int10: {rand.int10}
xx-time1: {rand.now}
xx-time2: {rand.5秒前}
xx-time3: {rand.5分钟前}
xx-time4: {rand.5小时前}
xx-time5: {rand.5天前}

{data}
";



    private void CheckResult(string text)
    {
        HttpOption httpOption = HttpOption.FromRawText(text);

        string url = "http://www.abc.com/test/callback.aspx?data=%e4%b8%ad%e6%96%87%e6%b1%89%e5%ad%97%7e!%40%23%24%23%25&v=";
        Assert.IsTrue(httpOption.Url.StartsWith(url));
        Assert.AreEqual("223", httpOption.Headers["x-xx1"]);
        Assert.AreEqual("abcd", httpOption.Headers["x-xx2"]);
        Assert.AreEqual("{data.xx333}", httpOption.Headers["x-xx3"]);   // 不处理
        Assert.IsTrue(Guid.TryParse(httpOption.Headers["x-client-reqid"], out Guid guid1));
        Assert.AreEqual(32, httpOption.Headers["x-client-reqid32"].Length);
        Assert.AreEqual("#{xx}#", httpOption.Headers["x-null"]);       // 不处理
        Assert.AreEqual("{\"xx1\":223,\"xx2\":\"abcd\",\"cn\":\"中文汉字~!@#$#%\"}", httpOption.GetPostData().ToString());

        Assert.IsTrue(httpOption.Headers["xx-guid"].Length == 36);
        Assert.IsTrue(httpOption.Headers["xx-guid32"].Length == 32);
        Assert.IsTrue(httpOption.Headers["xx-guid36"].Length == 36);

        Assert.IsTrue(httpOption.Headers["xx-char"].Length == 1);
        Assert.IsTrue(httpOption.Headers["xx-char0"] == "{rand.char0}");   // 不处理
        Assert.IsTrue(httpOption.Headers["xx-char1"].Length == 1);
        Assert.IsTrue(httpOption.Headers["xx-char2"].Length == 2);
        Assert.IsTrue(httpOption.Headers["xx-char3"].Length == 3);
        Assert.IsTrue(httpOption.Headers["xx-char4"].Length == 4);
        Assert.IsTrue(httpOption.Headers["xx-char5"].Length == 5);

        Assert.IsTrue(int.Parse(httpOption.Headers["xx-int"]) > 0);
        Assert.IsTrue(httpOption.Headers["xx-int0"] == "{rand.int0}");   // 不处理
        Assert.IsTrue(int.Parse(httpOption.Headers["xx-int1"]) > 0);
        Assert.IsTrue(httpOption.Headers["xx-int1"].Length == 1);
        Assert.IsTrue(int.Parse(httpOption.Headers["xx-int2"]) > 0);
        Assert.IsTrue(httpOption.Headers["xx-int2"].Length == 2);
        Assert.IsTrue(int.Parse(httpOption.Headers["xx-int5"]) > 0);
        Assert.IsTrue(httpOption.Headers["xx-int5"].Length == 5);
        Assert.IsTrue(int.Parse(httpOption.Headers["xx-int9"]) > 0);
        Assert.IsTrue(httpOption.Headers["xx-int9"].Length == 9);
        Assert.IsTrue(int.Parse(httpOption.Headers["xx-int10"]) > 0);
        Assert.IsTrue(httpOption.Headers["xx-int10"].Length >= 9);

        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(httpOption.Headers["xx-time1"])).TotalSeconds < 3);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(httpOption.Headers["xx-time2"])).TotalSeconds >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(httpOption.Headers["xx-time3"])).TotalMinutes >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(httpOption.Headers["xx-time4"])).TotalHours >= 5);
        Assert.IsTrue(DateTime.Now.Subtract(DateTime.Parse(httpOption.Headers["xx-time5"])).TotalDays >= 5);
    }


    [TestMethod]
    public void Test1()
    {
        object data = new { xx1 = 223, xx2 = "abcd", cn = "中文汉字~!@#$#%" };

        TextTemplate template = new TextTemplate();
        string text = template.Populate(s_httpTemplate, data.ToDictionary());

        Console2.WriteLine(text);
        CheckResult(text);
    }

    [TestMethod]
    public void Test2()
    {
        object data = new { xx1 = 223, xx2 = "abcd", cn = "中文汉字~!@#$#%" };

        TextTemplate template = new TextTemplate();
        string text = template.Populate(s_httpTemplate, data.ToJson());

        CheckResult(text);
    }

    [TestMethod]
    public void Test3()
    {
        string[] names = TextTemplate.GetArgumentNames("xxxxxxxxxxx");
        Assert.IsNotNull(names);
        Assert.AreEqual(0, names.Length);

        TextTemplate template = new TextTemplate();
        Assert.IsNull(template.Populate(null, "xxxxx"));
        Assert.AreEqual("", template.Populate("", "xxxxx"));

        IDictionary<string, object> data = null;
        Assert.IsNull(template.Populate(null, data));
        Assert.AreEqual("", template.Populate("", data));
        Assert.AreEqual("xxx", template.Populate("xxx", data));

        data = new Dictionary<string, object>(0);
        data["key1"] = "xx";
        Assert.AreEqual("xxx", template.Populate("xxx", data));

    }


    [TestMethod]
    public void Test4()
    {
        string text = @"
GET http://localhost/data/query/ea/rt_pert_db_instance/page_life?start={enc(rand.30分钟前)}&end={enc(rand.now)}&instance_id=1 HTTP/1.1
x-client-app: AiLabClientUI
x-client-reqid: {rand.guid32}
x-client-url: NULL
x-tenantId: {data.tenantId}
";

        var data = new { tenantId = "123456789" };

        TextTemplate template = new TextTemplate();
        string result = template.Populate(text, data.ToDictionary());
        Console.WriteLine(result);

        HttpOption http = HttpOption.FromRawText(result);

        Uri uri = new Uri(http.Url);
        string query = uri.Query.TrimStart('?');

        List<NameValue> list = query.ToKVList('&', '=');

        NameValue x1 = list.First(x => x.Name == "start");
        Assert.IsTrue(DateTime.TryParse(x1.Value.UrlDecode(), out DateTime xx1));

        NameValue x2 = list.First(x => x.Name == "end");
        Assert.IsTrue(DateTime.TryParse(x2.Value.UrlDecode(), out DateTime xx2));

        Assert.AreEqual("123456789", http.Headers["x-tenantId"]);
    }


#if NETCOREAPP
    [TestMethod]
    public void Test_dynamic()
    {
        dynamic data2 = new ExpandoObject();
        data2.xx1 = 223;
        data2.xx2 = "abcd";
        data2.cn = "中文汉字~!@#$#%";

        TextTemplate template = new TextTemplate();
        string text = template.Populate(s_httpTemplate, data2);

        CheckResult(text);
    }
#endif


}
