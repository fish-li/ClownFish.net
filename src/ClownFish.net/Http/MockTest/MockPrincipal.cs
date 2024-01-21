namespace ClownFish.Http.MockTest;

/// <summary>
/// 
/// </summary>
public class MockPrincipal : IPrincipal
{
    /// <summary>
    /// 
    /// </summary>
    public IIdentity Identity { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public bool IsInRole(string role)
    {
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
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

/// <summary>
/// 
/// </summary>
public class MockIdentity : IIdentity
{
    /// <summary>
    /// 
    /// </summary>
    public string AuthenticationType => "MOCK";

    /// <summary>
    /// 
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Name => "TestUser";
}
