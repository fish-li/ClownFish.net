using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.DTO;

namespace ClownFish.UnitTest.DTO;
[TestClass]
public class WebUserInfoTest
{
    [TestMethod]
    public void Test1()
    {
        WebUserInfo user1 = new WebUserInfo();
        user1.TenantId = "1111111111111111";
        user1.TenantCode = "2222222222222222";
        user1.UserId = "333333333";
        user1.UserCode = "44444444";
        user1.UserName = "55555555";
        user1.UserRole = "admin";
        user1.UserType = "6666666666666";
        user1.UserData = "77777777777777777";
        user1.ExtData = "88888888888888";
        user1.GrayFlag = 3;

        Assert.IsNotNull(user1.TenantId);
        Assert.IsNotNull(user1.TenantCode);
        Assert.IsNotNull(user1.UserId);
        Assert.IsNotNull(user1.UserCode);
        Assert.IsNotNull(user1.UserName);
        Assert.IsNotNull(user1.UserRole);
        Assert.IsNotNull(user1.UserType);
        Assert.IsNotNull(user1.UserData);
        Assert.IsNotNull(user1.ExtData);
        Assert.AreEqual(3, user1.GrayFlag);
        Assert.AreEqual("TenantId=1111111111111111;UserId=333333333;UserName=55555555;UserRole=admin", user1.ToString());
        user1.Validate();
    }

    [TestMethod]
    public void Test2()
    {
        WebUserInfo user1 = new WebUserInfo();

        MyAssert.IsError<ValidationException2>(() => {
            user1.Validate();
        });

        user1.TenantId = "1111111111111111";

        MyAssert.IsError<ValidationException2>(() => {
            user1.Validate();
        });

        user1.UserId = "222222222222222222";

        MyAssert.IsError<ValidationException2>(() => {
            user1.Validate();
        });

        user1.UserRole = "admin";
        user1.Validate();

        Assert.AreEqual(user1.UserName, user1.UserId);
    }
}
