using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#if NETCOREAPP

namespace ClownFish.UnitTest.Http.Utils;
internal static class ResponseExtensions
{
    // 这是一个内部的构造方法
    // internal HttpWebResponse(HttpResponseMessage _message, Uri requestUri, CookieContainer cookieContainer)
    // HttpWebResponse httpWebResponse = new HttpWebResponse(httpResponseMessage, _requestUri, _cookieContainer);

    private static readonly ConstructorInfo s_ctor = typeof(HttpWebResponse).GetConstructor(
                                                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                                                            null, new Type[] { typeof(HttpResponseMessage), typeof(Uri), typeof(CookieContainer) }, null);

    internal static HttpWebResponse ToHttpWebResponse(this HttpResponseMessage responseMessage, Uri requestUri = null, CookieContainer cookieContainer = null)
    {
        return (HttpWebResponse)s_ctor.Invoke(new object[] { responseMessage, requestUri, cookieContainer });
    }
}
#endif
