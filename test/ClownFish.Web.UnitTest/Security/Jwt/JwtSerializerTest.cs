using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.UnitTest;
using ClownFish.Web.Security.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Security.Jwt
{
    [TestClass]
    public class JwtSerializerTest
    {
        private static readonly JwtJsonSerializer s_longSerializer = new JwtJsonSerializer(false);
        private static readonly JwtJsonSerializer s_shortSerializer = new JwtJsonSerializer(true);

        [TestMethod]
        public void Test_1()
        {
            List<JwtJsonSerializer> list = new List<JwtJsonSerializer> { s_longSerializer , s_shortSerializer };
            foreach( var serializer in list ) {
                string json1 = serializer.Serialize(JwtTest.WebUser);
                WebUserInfo user1 = serializer.Deserialize<WebUserInfo>(json1);
                MyAssert.AreEqual(JwtTest.WebUser, user1);
            }
        }


        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = s_longSerializer.Serialize(null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = s_longSerializer.Deserialize<WebUserInfo>("");
            });
        }
    }
}
