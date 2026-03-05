namespace ClownFish;

// 这个文件中的代码是为了兼容.NET Framework而添加的，没有任何实际功能，仅仅是为了让.NET Framework编译通过而已。

#if NETFRAMEWORK

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
internal sealed class UnconditionalSuppressMessageAttribute : Attribute
{
    public UnconditionalSuppressMessageAttribute(string category, string checkId) { }
}


[AttributeUsage(AttributeTargets.All, Inherited = false)]
internal sealed class RequiresUnreferencedCodeAttribute : Attribute
{
    public RequiresUnreferencedCodeAttribute(string message) { }
}


[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
internal sealed class DynamicDependencyAttribute : Attribute
{
    public DynamicDependencyAttribute(string memberSignature, Type type) { }
}



[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter |
    AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Method |
    AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct,
    Inherited = false)]
internal sealed class DynamicallyAccessedMembersAttribute : Attribute
{
    public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
    {
        MemberTypes = memberTypes;
    }

    public DynamicallyAccessedMemberTypes MemberTypes { get; }
}


[Flags]
internal enum DynamicallyAccessedMemberTypes
{
    None = 0,
    PublicParameterlessConstructor = 1,
    PublicConstructors = 3,
    NonPublicConstructors = 4,
    PublicMethods = 8,
    NonPublicMethods = 0x10,
    PublicFields = 0x20,
    NonPublicFields = 0x40,
    PublicNestedTypes = 0x80,
    NonPublicNestedTypes = 0x100,
    PublicProperties = 0x200,
    NonPublicProperties = 0x400,
    PublicEvents = 0x800,
    NonPublicEvents = 0x1000,
    Interfaces = 0x2000,
    NonPublicConstructorsWithInherited = 0x4004,
    NonPublicMethodsWithInherited = 0x8010,
    NonPublicFieldsWithInherited = 0x10040,
    NonPublicNestedTypesWithInherited = 0x20100,
    NonPublicPropertiesWithInherited = 0x40400,
    NonPublicEventsWithInherited = 0x81000,
    PublicConstructorsWithInherited = 0x100003,
    PublicNestedTypesWithInherited = 0x200080,
    AllConstructors = 0x104007,
    AllMethods = 0x8018,
    AllFields = 0x10060,
    AllNestedTypes = 0x220180,
    AllProperties = 0x40600,
    AllEvents = 0x81800,
    All = -1
}

#endif

