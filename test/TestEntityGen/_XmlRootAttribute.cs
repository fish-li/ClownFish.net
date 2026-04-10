global using ClownFish.ProxyEntityGen.Tests;

using System;

namespace ClownFish.ProxyEntityGen.Tests;

[AttributeUsage(AttributeTargets.Class , Inherited = false)]
internal class XmlRootAttribute : Attribute
{
    public XmlRootAttribute(string elementName)
    {
        ElementName = elementName;
    }
    public string ElementName { get; }  
}
