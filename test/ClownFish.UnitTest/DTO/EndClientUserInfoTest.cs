using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.DTO;

namespace ClownFish.UnitTest.DTO;
[TestClass]
public class EndClientUserInfoTest
{
    [TestMethod]
    public void Test1()
    {
        EndClientUserInfo client = EndClientUserInfo.CreateNew();
        client.TenantId = "111111111";
        client.TenantCode = "22222222222";
        client.ClientId = "333333";
        client.ClientRole = "admin";
        client.Version = "1.0.0.0";
        client.DeployMode = 2;
        client.RunMode = 200;
        client.GrayFlag = 2;
        client.ClientData = "22222222222222222222222";
        client.ExtData = "3333333333333333333333333";
        client.Ip = "126.0.0.1";
        client.Cluster = "Cluster123";

        Assert.IsNotNull(client.TenantId);
        Assert.IsNotNull(client.TenantCode);
        Assert.IsNotNull(client.ClientId);
        Assert.IsNotNull(client.AppId);
        Assert.IsNotNull(client.AppName);
        Assert.IsNotNull(client.Version);
        Assert.IsNotNull(client.ClientRole);
        Assert.IsNotNull(client.HostName);
        Assert.AreEqual(1, client.OsKind);
        Assert.IsNotNull(client.OsName);
        Assert.IsNotNull(client.CpuKind);
        Assert.IsNotNull(client.TimeZone);
        Assert.IsNotNull(client.Culture);
        Assert.AreEqual(2, client.DeployMode);
        Assert.AreEqual(200, client.RunMode);
        Assert.AreEqual(2, client.GrayFlag);
        Assert.IsNotNull(client.ClientData);
        Assert.IsNotNull(client.ExtData);
        Assert.IsNotNull(client.Ip);
        Assert.IsNotNull(client.Cluster);

        Assert.IsNotNull(client.ToString());

        Assert.IsNotNull(client.UserId);
        Assert.IsNotNull(client.UserName);
        Assert.IsNotNull(client.UserRole);

        client.Validate();

    }


    [TestMethod]
    public void Test2()
    {
        EndClientUserInfo client = new EndClientUserInfo();

        MyAssert.IsError<ValidationException2>(()=> {
            client.Validate();
        });

        client.TenantId = "1111111111111111";

        MyAssert.IsError<ValidationException2>(() => {
            client.Validate();
        });

        client.ClientId = "222222222222222222";

        client.Validate();
    }

}
