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
    public class JwtProviderTest
    {
        private static readonly JwtProvider s_jwtV3 = CreateJwtProvider(null, true, true, true);

        internal static JwtProvider CreateJwtProvider(string algorithmName, bool useShortTypeName, bool verifyTokenExpiration, bool useShortTime)
        {
            JwtOptions opt = new JwtOptions {
                HashAlgorithmName = algorithmName,
                ShortTypeName = useShortTypeName,
                VerifyTokenExpiration = verifyTokenExpiration,
                ShortTime = useShortTime,
                LoadUnknownUser = LocalSettings.GetBool("ClownFish_Authentication_LoadUnknownUserType"),
                SecretKeyBytes = GetSecretKeyBytes()
            };

            return new JwtProvider(opt);
        }

        internal static byte[] GetSecretKeyBytes()
        {
            string secretKey = Settings.GetSetting("ClownFish_Authentication_SecretKey", true);
            return Encoding.UTF8.GetBytes(secretKey);
        }


        [TestMethod]
        public void Test_ShortTypeName()
        {
            DateTime exp = DateTime.Now.AddDays(1);
            string token1 = s_jwtV3.CreateToken(JwtTest.WebUser, DateTime.Now, exp);

            JwtProvider jwtV2 = JwtProviderTest.CreateJwtProvider(null, false, true, true);
            string token2 = jwtV2.CreateToken(JwtTest.WebUser, DateTime.Now, exp);

            Console.WriteLine(token1);
            Console.WriteLine(token2);
            Assert.IsTrue(token2.Length > token1.Length);


            string json1 = s_jwtV3.DecodePayload(token1);
            string json2 = s_jwtV3.DecodePayload(token2);
            Console.WriteLine(json1);
            Console.WriteLine(json2);

            Assert.IsTrue(json1.Contains("\"$type\":\"WebUserInfo\""));
            Assert.IsTrue(json2.Contains("\"$type\":\"ClownFish.Web.Security.Auth.WebUserInfo, ClownFish.Web\""));            
        }


        [TestMethod]
        public void Test_ShortTypeName_Decode()  // 验证：长名称 生成， 短名称 解析
        {
            DateTime exp = DateTime.Now.AddDays(1);
            JwtProvider jwtV2 = JwtProviderTest.CreateJwtProvider(null, false, true, true);

            string token1 = jwtV2.CreateToken(JwtTest.WebUser, DateTime.Now, exp);
            Console.WriteLine(token1);

            string json = s_jwtV3.DecodePayload(token1);
            Console.WriteLine(json);
            Assert.IsTrue(json.Contains("\"User\":{\"$type\":\"ClownFish.Web.Security.Auth.WebUserInfo, ClownFish.Web\""));

            LoginTicket ticket = s_jwtV3.DecodeToken(token1);
            Assert.IsNotNull(ticket);
            Console.WriteLine(ticket.ToJson(JsonStyle.Indented));
            Assert.IsInstanceOfType(ticket.User, typeof(WebUserInfo));
        }

        [TestMethod]
        public void Test_Expiration()
        {
            // 生成一个 过期的 Token
            string token = s_jwtV3.CreateToken(JwtTest.WebUser, DateTime.Now, DateTime.Now.AddHours(-5));
            Assert.IsNull(s_jwtV3.DecodeToken(token));
            Assert.IsNull(s_jwtV3.DecodeToken(token));

            // 不校验时间，就能还原 token
            JwtProvider jwtHelper2 = JwtProviderTest.CreateJwtProvider(null, false, false, true);
            Assert.IsNotNull(jwtHelper2.DecodeToken(token));
            Assert.IsNotNull(jwtHelper2.DecodeToken2(token));
        }

        [TestMethod]
        public void Test_ShortTime()
        {
            DateTime exp = new DateTime(5555, 5, 3, 7, 8, 9);
            string token1 = s_jwtV3.CreateToken(JwtTest.WebUser, DateTime.Now, exp);

            JwtProvider jwtV2 = JwtProviderTest.CreateJwtProvider(null, true, true, false);
            string token2 = jwtV2.CreateToken(JwtTest.WebUser, DateTime.Now, exp);

            Console.WriteLine(token1);
            Console.WriteLine(token2);
            Assert.IsTrue(token2.Length > token1.Length);


            string json1 = s_jwtV3.DecodePayload(token1);
            string json2 = s_jwtV3.DecodePayload(token2);
            Console.WriteLine(json1);
            Console.WriteLine(json2);

            Assert.IsTrue(json1.Contains("\"Expiration\":55550503070809"));
            Assert.IsTrue(json2.Contains("\"Expiration\":1752778048890000000"));
        }


        [TestMethod]
        public void Test_ShortTime_Decode1()
        {
            // 生成一个 长时间戳 的Token，且已过期，模拟【上一个版本】产生的Token
            // 此时的时间戳值将会比较长，因此必须按长时间戳来识别

            JwtProvider jwtHelper = JwtProviderTest.CreateJwtProvider(null, true, true, false);
            DateTime exp = DateTime.Now.AddYears(-1);

            string token1 = jwtHelper.CreateToken(JwtTest.WebUser, DateTime.Now, exp);
            Console.WriteLine(token1);

            LoginTicket ticket = s_jwtV3.DecodeToken(token1);
            Assert.IsNull(ticket);

            string json = s_jwtV3.DecodePayload(token1, null);
            LoginTicket ticket2 = s_jwtV3.DecodeJson(json, false);
            Console.WriteLine(ticket2.ToJson(JsonStyle.Indented));
            Assert.IsInstanceOfType(ticket2.User, typeof(WebUserInfo));
        }

        [TestMethod]
        public void Test_ShortTime_Decode2()
        {
            // 生成一个 长时间戳 的Token，模拟【上一个版本】产生的Token
            // 此时的时间戳值将会比较长，因此必须按长时间戳来识别

            JwtProvider jwt = JwtProviderTest.CreateJwtProvider(null, true, true, false);
            DateTime exp = DateTime.Now.AddYears(1);

            string token1 = jwt.CreateToken(JwtTest.WebUser, DateTime.Now, exp);
            Console.WriteLine(token1);

            LoginTicket ticket = s_jwtV3.DecodeToken(token1);
            Assert.IsNotNull(ticket);
            Console.WriteLine(ticket.ToJson(JsonStyle.Indented));
            Assert.IsInstanceOfType(ticket.User, typeof(WebUserInfo));
        }

        [TestMethod]
        public void Test_FormatException()
        {
            Assert.IsNull(s_jwtV3.DecodePayload("token-xxxxxxxxxxxxx"));
            Assert.IsNull(s_jwtV3.DecodeToken("token-xxxxxxxxxxxxx"));

            Assert.IsNull(s_jwtV3.DecodeJson("", true));
            Assert.IsNull(s_jwtV3.DecodeJson("json-xxxxxxxxxxx", true));
            Assert.IsNull(s_jwtV3.DecodeJson("{aa: 1, bb: 2}", true));
        }



        [TestMethod]
        public void Test_SignatureVerificationException()
        {
            string token = s_jwtV3.CreateToken(JwtTest.WebUser, 1000);
            Assert.IsNotNull(s_jwtV3.DecodeToken(token));
            Assert.IsNull(s_jwtV3.DecodeToken(token + "xx"));
        }



        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                s_jwtV3.CreateToken(null, 100);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                s_jwtV3.DecodeToken(null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                s_jwtV3.DecodePayload(null);
            });
        }

    }
}
