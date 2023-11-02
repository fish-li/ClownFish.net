using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETCOREAPP
namespace ClownFish.UnitTest.MQ.Messages;
[TestClass]
public class HttpContextAloneTest
{
    [TestMethod]
    public void Test()
    {
        MyAssert.IsError<NotImplementedException>(() => {
            _ = HttpContextAlone.Instance.OriginalHttpContext;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            _ = HttpContextAlone.Instance.Request;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            _ = HttpContextAlone.Instance.Response;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            _ = HttpContextAlone.Instance.User;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            HttpContextAlone.Instance.User = null;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            _ = HttpContextAlone.Instance.SkipAuthorization;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            HttpContextAlone.Instance.SkipAuthorization = true;
        });

        MyAssert.IsError<NotImplementedException>(() => {
            _ = HttpContextAlone.Instance.Items;
        });
    }
}
#endif
