namespace ClownFish.UnitTest.Http.Mock;

class MockPrincipal : IPrincipal
{
    public IIdentity Identity { get; private set; }

    public bool IsInRole(string role)
    {
        return false;
    }

    public MockPrincipal(bool? value)
    {
        if( value.HasValue == false )
            return;

        if( value.Value )
            this.Identity = new MockIdentity { IsAuthenticated = true };
        else
            this.Identity = new MockIdentity { IsAuthenticated = false };
    }
}

public class MockIdentity : IIdentity
{
    public string AuthenticationType => "MOCK";

    public bool IsAuthenticated { get; set; }

    public string Name => "TestUser";
}
