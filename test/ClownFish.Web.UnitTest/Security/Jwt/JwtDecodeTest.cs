using ClownFish.Jwt;
using ClownFish.Web.Security.Auth;

namespace ClownFish.Web.UnitTest.Security.Jwt;

[TestClass]
public class JwtDecodeTest
{
    private static readonly JwtProvider s_jwtV3 = JwtProviderTest.CreateJwtProvider("HS256", true, true, true);

    [TestMethod]
    public void Test_1()
    {
        string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJVc2VyIjp7IiR0eXBlIjoiVXJhbnVzLkRhdGEuUHViLkRUTy5BZ2VudENsaWVudEluZm8sIFVyYW51cy5EYXRhIiwiVGVuYW50SWQiOiJteTU2MWY3OGM2MTgxNmYiLCJBZ2VudElkIjoidHM1ZjQ0ZDQwOWJlOTBmIiwiQXBwSWQiOiJhZ2VudCIsIlVzZXJSb2xlIjoiQWdlbnRYQ2xpZW50In0sIklzc3VlVGltZSI6NjM3OTM0NzUzODE4ODY4NDc1LCJFeHBpcmF0aW9uIjo2Mzc5MzQ4NjE4MTg4Njg0NzV9.gUoiFWqwx0aLl6n7P2TOCq_bzDmsG6fNTPXhSxiRMnE";
//{
//  "User": {
//    "$type": "Uranus.Data.Pub.DTO.AgentClientInfo, Uranus.Data",
//    "TenantId": "my561f78c61816f",
//    "AgentId": "ts5f44d409be90f",
//    "AppId": "agent",
//    "UserRole": "AgentXClient"
//  },
//  "IssueTime": 637934753818868500,
//  "Expiration": 637934861818868500
//}

        string json = s_jwtV3.DecodePayload(token, null);  // 不做校验
        LoginTicket ticket = s_jwtV3.DecodeJson(token, json, false);

        Assert.IsInstanceOfType(ticket.User, typeof(UnknownUserInfo));

        UnknownUserInfo user = (UnknownUserInfo)ticket.User;
        Assert.AreEqual("my561f78c61816f", user.GetValue("TenantId"));
        Assert.AreEqual("ts5f44d409be90f", user.GetValue("AgentId"));
    }


    [TestMethod]
    public void Test_2()
    {
        string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJVc2VyIjp7IiR0eXBlIjoiRGVtby5XZWJTaXRlQXBwLkNvbnRyb2xsZXJzLlhBZ2VudENsaWVudEluZm8sIERlbW8uV2ViU2l0ZUFwcCIsIlRlbmFudElkIjoiRklTSERFVi1XSU4xMCIsIkFnZW50SWQiOiJDbGllbnQtMiIsIlVzZXJSb2xlIjoiWEFnZW50Q2xpZW50SW5mbyJ9LCJJc3N1ZVRpbWUiOjYzNzkzNTEwMTg5NDAxNzUxNCwiRXhwaXJhdGlvbiI6NjM3OTM2MzAxODk0MDE3NTE0fQ.A0iVvyjC-gUdAn0qqSt7z4-p9zDR2Ex6HcM3tDb6M_w";
        //{
        //  "User": {
        //    "$type": "Demo.WebSiteApp.Controllers.XAgentClientInfo, Demo.WebSiteApp",
        //    "TenantId": "FISHDEV-WIN10",
        //    "AgentId": "Client-2",
        //    "UserRole": "XAgentClientInfo"
        //  },
        //  "IssueTime": 637935101894017500,
        //  "Expiration": 637936301894017500
        //}

        byte[] secretKeyBytes = JwtProviderTest.GetSecretKeyBytes();
        string json = s_jwtV3.DecodePayload(token, secretKeyBytes);
        LoginTicket ticket = s_jwtV3.DecodeJson(token, json, false);

        Assert.IsNotNull(ticket);
        Assert.IsInstanceOfType(ticket.User, typeof(UnknownUserInfo));

        UnknownUserInfo user = (UnknownUserInfo)ticket.User;
        Assert.AreEqual("FISHDEV-WIN10", user.GetValue("TenantId"));
        Assert.AreEqual("Client-2", user.GetValue("AgentId"));
    }


    [TestMethod]
    public void Test_3()
    {
        string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpZCI6IjM5ZWRhODc2LTgyNTctMmI1Yi04Y2MyLWNmZGExZmQxN2RmMCIsIl9mbGFnIjoxNjU4MzE0NzQyLCJhY2NvdW50IjoieWFuZ21jIiwiY29kZSI6IndoeXciLCJncm91cF9pZHMiOm51bGwsImN1c3RvbWl6ZV9yb2xlcyI6W10sImV4dGVybmFsX3VzZXJfaWQiOm51bGwsImV4dGVuZF95bF9wYXJhbXMiOiIifQ.1Nxt9GAfimYMWK2eRlZl1mNxAt95rMnzKdFAdG6rlKQ";
        string json = JwtUtils.Decode(token, null, null); // 密钥为空，所以“算法”参数无所谓
        Console.WriteLine(json);

        Assert.IsTrue(json.Contains("39eda876-8257-2b5b-8cc2-cfda1fd17df0"));
        Assert.IsTrue(json.Contains("1658314742"));
        Assert.IsTrue(json.Contains("yangmc"));
        Assert.IsTrue(json.Contains("whyw"));
        Assert.IsTrue(json.Contains("group_ids"));
    }


    [TestMethod]
    public void Test_4()
    {
        string token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjIzQjRCRkZGMTI3QzA5QTNFNzg5OTlCRTlERjYxQ0MyNzQ3NTdGQjUiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJJN1NfX3hKOENhUG5pWm0tbmZZY3duUjFmN1UifQ.eyJuYmYiOjE2NTgzMTE0NDMsImV4cCI6MTY1ODQ4NDI0MywiaXNzIjoiaHR0cHM6Ly9hdXRoLm1pbmd5dWFueXVuLmNvbSIsImF1ZCI6WyJodHRwczovL2F1dGgubWluZ3l1YW55dW4uY29tL3Jlc291cmNlcyIsImRlZmF1bHRBcGkiLCJyZGMtcmVzb3VyY2VjZW50ZXItc2VydmljZSIsInJkYy1maWxlLXNlcnZpY2UiLCJyZGMtZXhjaGFuZ2UtYWRtaW4tc2VydmljZSJdLCJjbGllbnRfaWQiOiJyZGMiLCJzdWIiOiIwZDZmMDA1Yy03YjNhLTQ3NzctYmJhNC1lNDA3YmFlMjVmN2YiLCJhdXRoX3RpbWUiOjE2NTgzMTE0NDMsInVzZXIiOiJ7XCJOYW1lXCI6XCLmnajmlY_otoVcIixcIlVzZXJOYW1lXCI6XCJ5YW5nbWNcIixcIkVtYWlsXCI6XCJ5YW5nbWNAbWluZ3l1YW55dW4uY29tXCIsXCJQaG9uZVwiOlwiMTMzNzc4ODM1ODdcIixcIkF2YXRhckZpbGVOYW1lXCI6XCJodHRwczovL3BrZy5taW5neXVhbnl1bi5jb20vVXNlci9BdmF0YXJzL2JkMDM0YWRiLTM1ZWYtNGRjNS1hZmI2LTk5MzRiODU4NTRiYy5qcGdcIixcIkROXCI6XCJDTj3mnajmlY_otoUsT1U9SVRTTeS6p-WTgee7hCxPVT3mnI3liqHkupHkuovkuJrpg6gsT1U95aSp6ZmF5bmz5Y-wLE9VPTAzLeaYjua6kOS6kSxPVT0wMi3mmI7mupDpm4blm6IsT1U955So5oi3566h55CGLE9VPeaYjua6kOi9r-S7tixEQz1teXNvZnQsREM9Y29tLERDPWNuXCIsXCJJc091dGVyXCI6ZmFsc2V9IiwibmFtZSI6InlhbmdtYyIsInByZWZlcnJlZF91c2VybmFtZSI6InlhbmdtYyIsImVtYWlsIjoieWFuZ21jQG1pbmd5dWFueXVuLmNvbSIsImF2YXRhcl9maWxlX25hbWUiOiJodHRwczovL3BrZy5taW5neXVhbnl1bi5jb20vVXNlci9BdmF0YXJzL2JkMDM0YWRiLTM1ZWYtNGRjNS1hZmI2LTk5MzRiODU4NTRiYy5qcGciLCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiZGVmYXVsdEFwaSIsInJkYy1yZXNvdXJjZWNlbnRlci1zZXJ2aWNlIiwicmRjLWZpbGUtc2VydmljZSIsInJkYy1leGNoYW5nZS1hZG1pbi1zZXJ2aWNlIl0sImFtciI6WyJwd2QiXX0.b-o1MBMyGRu17zyEWtyzN4s0YMwl7K8FQfgiIuJ5aSdG8P-6ODplhZMJ1RWptPBuyTLqBdI3UtblYlztjyWhPffUn8c8qZFmAAPdSh6KF5_CR92JXhkjs_h1bqrKFTf-cvVE-BqmvrN3bBc7K64jnuTfWDCI6fqAA9qrTxdO8rDlGsuh50j4rWE8SW21EtAQNu08N99vxzqCdRgJI1lVZJn75f-7eK0QIRzhGRChQ2yezvyJIRTBuaNLVBgPBdcDQ_2RpS2OaxH9J3JQmHGN5sIRwqt7lt_-wjxrpvEE6Ocm9z-rJcigvKoAtmjcHc6OQtdgYzO_O5uUix1xg3iooQ";
        string json = JwtUtils.Decode(token, null, null);
        Console.WriteLine(json);

        Assert.IsTrue(json.Contains("nbf"));
        Assert.IsTrue(json.Contains("1658311443"));
        Assert.IsTrue(json.Contains("exp"));
        Assert.IsTrue(json.Contains("1658484243"));
        Assert.IsTrue(json.Contains("sub"));
        Assert.IsTrue(json.Contains("0d6f005c-7b3a-4777-bba4-e407bae25f7f"));
        Assert.IsTrue(json.Contains("auth_time"));
        Assert.IsTrue(json.Contains("1658311443"));
        Assert.IsTrue(json.Contains("preferred_username"));
        Assert.IsTrue(json.Contains("avatar_file_name"));
        Assert.IsTrue(json.Contains("openid"));
    }


    [TestMethod]
    public void Test_5()
    {
        // 这是一个老版本产生的 token
        string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJEYXRhIjoie1wiVGVuYW50SWRcIjpcInlrZl81YmViZDQ2ODM1ZTBkXCIsXCJVc2VySWRcIjpcIjczMzgyOVwiLFwiVXNlck5hbWVcIjpcIuaZuuaFp-W3peeoi-eUqOaItzNcIixcIlVzZXJSb2xlXCI6XCJ2aXNpdG9yXCJ9IiwiRGF0YVR5cGUiOiJOZWJ1bGEuQ29tbW9uLlNlY3VyaXR5LldlYlVzZXJJbmZvLCBOZWJ1bGEubmV0IiwiSXNzdWVUaW1lIjo2MzgyMjI2OTEwMzc0ODU3NDksIkV4cGlyYXRpb24iOjYzODIzNTY1MTAzNzQ4NTc0OX0.DfdTRa1HGBHgeKvgKHakn1lU8uJFTtRKUPD4QmZhUsw";

        //{
        //  "Data": "{\"TenantId\":\"ykf_5bebd46835e0d\",\"UserId\":\"733829\",\"UserName\":\"智慧工程用户3\",\"UserRole\":\"visitor\"}",
        //  "DataType": "Nebula.Common.Security.WebUserInfo, Nebula.net",
        //  "IssueTime": 638222691037485700,
        //  "Expiration": 638235651037485700
        //}

        string json = s_jwtV3.DecodePayload(token, null);  // 不做校验
        LoginTicket ticket = s_jwtV3.DecodeJson(token, json, false);

        Assert.IsNotNull(ticket);
        Assert.IsNull(ticket.User);


        //LoginTicket ticket2 = AuthenticationManager.DecodeToken(token);
        //Assert.IsNotNull(ticket2);
        //Assert.IsNull(ticket2.User);
    }
}
