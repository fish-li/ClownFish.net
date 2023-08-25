using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Xml;

namespace ClownFish.Base.DEMO
{
    class Serialize_DEMO
    {
        public void Json_序列化_反序列化()
        {
            var obj = new NameValue { Name = "aaaa", Value = "123" };

            // 将对象序列化成JSON字符串
            string json = obj.ToJson();

            // 将JSON字符串反序列化为指定的类型
            var object2 = json.FromJson<NameValue>();
        }


        public void Xml_序列化_反序列化()
        {
            var obj = new NameValue { Name = "aaaa", Value = "123" };

            // 将对象序列化成XML字符串
            string xml = obj.ToXml();

            // 将XML字符串反序列化为指定的类型
            var object2 = xml.FromXml<NameValue>();
        }


        /// <summary>
        /// 模拟一个配置类型
        /// </summary>
        public class X_RunOption { /* 这里省略具体的参数成员 */ }

        public void Xml_配置文件_序列化_反序列化()
        {
            X_RunOption option = new X_RunOption();
            string file = @"d:\aa\xx.config";

            // 将对象序列化成XML配置文件
            XmlHelper.XmlSerializeToFile(option, file);

            // 将XML配置文件反序列化为指定的类型
            option = XmlHelper.XmlDeserializeFromFile<X_RunOption>(file);
        }


    }
}
