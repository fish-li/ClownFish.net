global using ClownFish.EntityProxyGen.Tests;

using System;

namespace ClownFish.EntityProxyGen.Tests;

[AttributeUsage(AttributeTargets.Class , Inherited = false)]
internal class XmlRootAttribute : Attribute
{
    public XmlRootAttribute(string elementName)
    {
        ElementName = elementName;
    }
    public string ElementName { get; }  
}
