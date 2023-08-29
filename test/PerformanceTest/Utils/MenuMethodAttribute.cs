namespace PerformanceTest.Utils;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class  MenuMethodAttribute : Attribute
{
    public string Title { get; set; }

    public string Group { get; set; }

    internal string Index { get; set; }

    internal MethodInfo Method { get; set; }
}
