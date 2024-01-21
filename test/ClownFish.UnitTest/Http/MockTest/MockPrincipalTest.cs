using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.MockTest;
[TestClass]
public class MockPrincipalTest
{
    [TestMethod]
    public void Test1()
    {
        MockIdentity identity = new MockIdentity {
            IsAuthenticated = true
        };

        Assert.AreEqual("MOCK", identity.AuthenticationType);
        Assert.AreEqual("TestUser", identity.Name);
        Assert.IsTrue(identity.IsAuthenticated);
    }

    [TestMethod]
    public void Test2()
    {
        MockPrincipal principal = new MockPrincipal(true);

        Assert.IsFalse(principal.IsInRole("role1"));
        Assert.IsFalse(principal.IsInRole(""));

        Assert.IsNotNull(principal.Identity);
        Assert.AreEqual("MOCK", principal.Identity.AuthenticationType);
        Assert.AreEqual("TestUser", principal.Identity.Name);
        Assert.IsTrue(principal.Identity.IsAuthenticated);
    }
}
