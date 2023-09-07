namespace ClownFish.WebApi;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class HttpMethodAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    protected HttpMethodAttribute(string name)
    {
        this.Name = name;
    }
}


/// <summary>
/// 
/// </summary>
public sealed class HttpGetAttribute : HttpMethodAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public HttpGetAttribute() : base("GET") { }
}

/// <summary>
/// 
/// </summary>
public sealed class HttpPostAttribute : HttpMethodAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public HttpPostAttribute() : base("POST") { }
}

/// <summary>
/// 
/// </summary>
public sealed class HttpDeleteAttribute : HttpMethodAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public HttpDeleteAttribute() : base("DELETE") { }
}

/// <summary>
/// 
/// </summary>
public sealed class HttpPutAttribute : HttpMethodAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public HttpPutAttribute() : base("PUT") { }
}

/// <summary>
/// 
/// </summary>
public sealed class HttpHeadAttribute : HttpMethodAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public HttpHeadAttribute() : base("HEAD") { }
}

/// <summary>
/// 
/// </summary>
public sealed class HttpOptionsAttribute : HttpMethodAttribute
{
    internal static readonly string MethodName = "OPTIONS";

    /// <summary>
    /// 
    /// </summary>
    public HttpOptionsAttribute() : base(MethodName) { }
}

/// <summary>
/// 
/// </summary>
public sealed class HttpPatchAttribute : HttpMethodAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public HttpPatchAttribute() : base("PATCH") { }
}
