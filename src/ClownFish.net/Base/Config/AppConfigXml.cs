using ClownFish.Base.Config.Models;

namespace ClownFish.Base.Config;

internal static class AppConfigXml
{
    public static AppConfiguration LoadFile(string filePath)
    {
        var xmlObject = XmlHelper.XmlDeserializeFromFile<XmlAppConfiguration>(filePath);
        return xmlObject?.ToAppConfiguration() ?? new AppConfiguration();
    }


    public static AppConfiguration LoadXml(string xml)
    {
        if( xml.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(xml));

        var xmlObject = XmlHelper.XmlDeserialize<XmlAppConfiguration>(xml);
        return xmlObject?.ToAppConfiguration() ?? new AppConfiguration();
    }


}

