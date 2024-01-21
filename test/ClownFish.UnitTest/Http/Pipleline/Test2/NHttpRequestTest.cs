namespace ClownFish.UnitTest.Http.Pipleline.Test;

[TestClass]
public class NHttpRequestTest
{
    
    [TestMethod]
    public void Test_Read_Collection()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90&checkType=%E7%B3%BB%E7%BB%9F%E5%BA%94%E7%94%A8%E6%B0%B4%E5%B9%B3 HTTP/1.1
Content-Type: text/plain
x-client-app: HttpTest1
Cookie: c1=1111111; c2=22222222
Origin: https://www.abc.com

fabbd011e6804d82987a453e7902234c
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            MockHttpRequest request = (MockHttpRequest)mock.HttpContext.Request;

            Assert.AreEqual(2, request.QueryStringKeys.Length);
            Assert.AreEqual("my57972739adc90", request.QueryString("tenantId"));
            Assert.AreEqual("系统应用水平", request.QueryString("checkType"));

            Assert.AreEqual(4, request.HeaderKeys.Length);
            Assert.AreEqual("text/plain", request.Header("Content-Type"));
            Assert.AreEqual("HttpTest1", request.Header("x-client-app"));
            Assert.AreEqual("c1=1111111; c2=22222222", request.Header("Cookie"));
            Assert.AreEqual("https://www.abc.com", request.Header("Origin"));


            Assert.AreEqual("my57972739adc90", request.GetValue("tenantId"));
            Assert.AreEqual("text/plain", request.GetValue("Content-Type"));

            Assert.AreEqual("fabbd011e6804d82987a453e7902234c", request.GetBodyText());
        }
    }

    [TestMethod]
    public void Test_ContentLength_NoSet()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain

fabbd011e6804d82987a453e7902234c
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.AreEqual(-1, mock.HttpContext.Request.ContentLength);
            Assert.AreEqual(-1, mock.HttpContext.Request.GetBodyTextLength());
        }            
    }


    [TestMethod]
    public void Test_ContentLength_Normal()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
Content-Length: 12345678

fabbd011e6804d82987a453e7902234c
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.AreEqual(12345678, mock.HttpContext.Request.ContentLength);
            Assert.AreEqual(12345678, mock.HttpContext.Request.GetBodyTextLength());
        }

    }

    [TestMethod]
    public void Test_ContentLength_ContentType_IsNotText()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: application/xxxxxxxxx
Content-Length: 12345678

fabbd011e6804d82987a453e7902234c
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.AreEqual(12345678, mock.HttpContext.Request.ContentLength);
            Assert.AreEqual(0, mock.HttpContext.Request.GetBodyTextLength());
        }
    }


    [TestMethod]
    public void Test_ContentLength_NoBody()
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();
        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.AreEqual(-1, mock.HttpContext.Request.ContentLength);
            Assert.AreEqual(-1, mock.HttpContext.Request.GetBodyTextLength());
        }

    }


    [TestMethod]
    public void Test_ContentLength_HttpGet_SetLength()
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Length: 12345678
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.AreEqual(12345678, mock.HttpContext.Request.ContentLength);
            Assert.AreEqual(0, mock.HttpContext.Request.GetBodyTextLength());
        }
    }


    [TestMethod]
    public void Test_GetEncoding_NoSet()
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        Assert.AreEqual(Encoding.UTF8, request.GetEncoding());
    }


    [TestMethod]
    public void Test_GetEncoding_GB2312()
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain; charset=gb2312
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        Assert.AreEqual(Encoding.GetEncoding("GB2312"), request.GetEncoding());
    }


    [TestMethod]
    public void Test_GetEncoding_ErrorValue()
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain; charset=xxxxx
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        Assert.AreEqual(Encoding.UTF8, request.GetEncoding());
    }


    [TestMethod]
    public void Test_ReadBody_HttpGet()  // GET方法，不支持请求体
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        byte[] body = request.ReadBodyAsBytes();

        Assert.IsNotNull(body);
        Assert.AreEqual(0, body.Length);

        string text = request.ReadBodyAsText();
        Assert.AreEqual(string.Empty, text);
    }

    [TestMethod]
    public void Test_ReadBody_NoBody()   // 没有请求体
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        requestData.Body = null;

        MockHttpRequest request = requestData;

        byte[] body = request.ReadBodyAsBytes();

        Assert.IsNotNull(body);
        Assert.AreEqual(0, body.Length);

        string text = request.ReadBodyAsText();
        Assert.AreEqual(string.Empty, text);
    }

    [TestMethod]
    public void Test_ReadBody_CannotRead()   // “流”不能读
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        requestData.SetInputStream(new CannotReadStream());

        MockHttpRequest request = requestData;

        byte[] body = request.ReadBodyAsBytes();

        Assert.IsNotNull(body);
        Assert.AreEqual(0, body.Length);

        string text = request.ReadBodyAsText();
        Assert.AreEqual(string.Empty, text);
    }


    [TestMethod]
    public void Test_ReadBody_ReadError()  // 读“流”过程中出现异常
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        requestData.SetInputStream(new XDataStream());

        MockHttpRequest request = requestData;

        byte[] body = request.ReadBodyAsBytes();

        Assert.IsNotNull(body);
        Assert.AreEqual(0, body.Length);

        string text = request.ReadBodyAsText();
        Assert.AreEqual(string.Empty, text);
    }


    [TestMethod]
    public void Test_ReadBody_Normal()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain

4efc1f117a8e4fa1bf3f479ac088eae1
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        byte[] body = request.ReadBodyAsBytes();

        Assert.IsNotNull(body);
        Assert.AreEqual("4efc1f117a8e4fa1bf3f479ac088eae1", body.ToUtf8String());

        string text = request.ReadBodyAsText();
        Assert.AreEqual("4efc1f117a8e4fa1bf3f479ac088eae1", text);
    }


    private static readonly string s_inputText = "4efc1f117a8e4fa1bf3f479ac088eae1中文汉字！~！！#￥%……￥……￥#%&SDFWE%@$#";


    [TestMethod]
    public void Test_ReadBodyAsText_gzip()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: text/plain
Content-Encoding: gzip
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        requestData.Body = s_inputText.ToUtf8Bytes().ToGzip();

        MockHttpRequest request = requestData;

        string text = request.ReadBodyAsText();
        Assert.AreEqual(s_inputText, text);
    }


    [TestMethod]
    public void Test_ReadBodyAsText_binary()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
Content-Type: application/octet-stream
Content-Encoding: gzip
".Trim();

        MockRequestData requestData = MockRequestData.FromText(requestText);
        requestData.Body = s_inputText.ToUtf8Bytes().ToGzip();

        MockHttpRequest request = requestData;

        byte[] body = request.ReadBodyAsBytes().UnGzip();
        string text = body.ToUtf8String();

        Assert.AreEqual(s_inputText, text);
    }

    [TestMethod]
    public void Test_AccessHeaders()
    {
        string requestText = @"
POST http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
aa: 11
bb: 22

xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
".Trim();
        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        MyAssert.IsError<ArgumentNullException>(()=> {
            request.AccessHeaders(null);
        });

        StringBuilder sb = new StringBuilder();
        request.AccessHeaders((k, v) => sb.Append(k).Append('=').Append(v).Append(';'));

        string text = sb.ToString();
        Assert.AreEqual("aa=11;bb=22;", text);
    }


    [TestMethod]
    public void Test_Route()
    {
        string requestText = "GET http://www.abc.com:14752/page/123/2021-9-30.aspx?tenantId=my57972739adc90 HTTP/1.1";
        MockRequestData requestData = MockRequestData.FromText(requestText);
        MockHttpRequest request = requestData;

        string pattern = @"/page/(?<id>\w+)/(?<year>\w+)-(?<month>\w+)-(?<day>\w+).aspx";
        request.RegexMatch = Regex.Match(request.Path, pattern);

        Assert.AreEqual("123", request.Route("id"));
        Assert.AreEqual("2021", request.Route("year"));
        Assert.AreEqual("9", request.Route("month"));
        Assert.AreEqual("30", request.Route("day"));

        Assert.IsNull(request.Route("xxx"));
    }

    [TestMethod]
    public void Test_IsAuthenticated()
    {
        string requestText = @"
GET http://www.abc.com:14752/aaa/bb/ccc.aspx?tenantId=my57972739adc90 HTTP/1.1
".Trim();
        MockRequestData requestData = MockRequestData.FromText(requestText);
        using( MockHttpPipeline mock = new MockHttpPipeline(requestData) ) {

            Assert.IsFalse(mock.HttpContext.IsAuthenticated);

            mock.HttpContext.User = new MockPrincipal(null);
            Assert.IsFalse(mock.HttpContext.IsAuthenticated);

            mock.HttpContext.User = new MockPrincipal(false);
            Assert.IsFalse(mock.HttpContext.IsAuthenticated);

            mock.HttpContext.User = new MockPrincipal(true);
            Assert.IsTrue(mock.HttpContext.IsAuthenticated);
        }
    }
}
