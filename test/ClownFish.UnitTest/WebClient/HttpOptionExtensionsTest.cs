namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class HttpOptionExtensionsTest
{
    [TestMethod]
    public async Task Test()
    {
        HttpOption option1 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        _ = option1.GetResult();

        HttpOption option2 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        _ = option2.GetResult(Retry.Create());


        HttpOption option3 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };
        _ = await option3.GetResultAsync();


        HttpOption option4 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };
        _ = await option4.GetResultAsync(Retry.Create());
    }


    [TestMethod]
    public void Test_return_void()
    {
        HttpOption option = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        // 直接发送请求，不读取响应流
        option.GetResult<ClownFish.Base.Void>();
    }

    [TestMethod]
    public void Test_return_string()
    {
        HttpOption option1 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };
        string html = option1.GetResult();
        Assert.IsTrue(html.Contains("<!DOCTYPE html>"));


        HttpOption option2 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };
        HttpResult<string> result = option2.GetResult<HttpResult<string>>(Retry.Create());
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsTrue(result.Headers["Content-Type"].StartsWith("text/html"));
        Assert.IsTrue(result.Result.Contains("<!DOCTYPE html>"));
    }


    [TestMethod]
    public void Test_return_HttpWebResponse()
    {
        HttpOption option = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        using( HttpWebResponse response = option.GetResult<HttpWebResponse>() ) {
            Assert.IsNotNull(response);
            Assert.AreEqual(200, (int)response.StatusCode);

            byte[] bytes = response.GetResponseStream().ToArray();
            string html = Encoding.UTF8.GetString(bytes);
            Assert.IsTrue(html.Contains("<!DOCTYPE html>"));
        }
    }

    [TestMethod]
    public void Test_return_Stream()
    {
        HttpOption option1 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        using( Stream stream = option1.GetResult<Stream>() ) {
            Assert.IsNotNull(stream);
            byte[] bytes = stream.ToArray();
            string html = Encoding.UTF8.GetString(bytes);
            Assert.IsTrue(html.Contains("<!DOCTYPE html>"));
        }


        HttpOption option2 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };
        HttpResult<Stream> result = option2.GetResult<HttpResult<Stream>>();
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        byte[] bytes2 = result.Result.ToArray();
        string html2 = Encoding.UTF8.GetString(bytes2);
        Assert.IsTrue(html2.Contains("<!DOCTYPE html>"));
    }


    [TestMethod]
    public void Test_return_bytes()
    {
        HttpOption option1 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        byte[] bytes = option1.GetResult<byte[]>();
        Assert.IsNotNull(bytes);
        string html = Encoding.UTF8.GetString(bytes);
        Assert.IsTrue(html.Contains("<!DOCTYPE html>"));



        HttpOption option2 = new HttpOption {
            Url = "http://www.fish-test.com/test1.aspx"
        };

        HttpResult<byte[]> result = option2.GetResult<HttpResult<byte[]>>();
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        string html2 = Encoding.UTF8.GetString(result.Result);
        Assert.IsTrue(html2.Contains("<!DOCTYPE html>"));
    }


    [TestMethod]
    public async Task Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = HttpOptionExtensions.GetResult(null);
        });

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            _ = await HttpOptionExtensions.GetResultAsync(null);
        });
    }


    [TestMethod]
    public void Test_RemoteWebException()
    {
        HttpOption option = new HttpOption {
            Url = "http://www.fish-test.com/test1111111111.aspx",
            AllowAutoRedirect = false,
            KeepAlive = false
        };

        Exception exception = null;
        try {
            string html = option.GetResult();
        }
        catch(Exception ex ) {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(RemoteWebException));

        RemoteWebException ex2 = (RemoteWebException)exception;
        Assert.AreEqual(404, ex2.StatusCode);

        Assert.IsTrue(ex2.ResponseText.Contains("<!DOCTYPE html>"));
    }


    [TestMethod]
    public async Task Test_RemoteWebExceptionAsync()
    {
        HttpOption option = new HttpOption {
            Url = "http://www.fish-test.com/test1111111111.aspx"
        };

        Exception exception = null;
        try {
            string html = await option.GetResultAsync();
        }
        catch( Exception ex ) {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(RemoteWebException));

        RemoteWebException ex2 = (RemoteWebException)exception;
        Assert.AreEqual(404, ex2.StatusCode);

        Assert.IsTrue(ex2.ResponseText.Contains("<!DOCTYPE html>"));
    }
}
