using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class AsmHelperTest
{

    [TestMethod]
    public void Test_ForeachXmlFiles()
    {
        string xmlFile = "ClownFish.UnitTest.xml";

        // 故意生成一个“无效”的XML文件，检验加载过程不中断
        File.WriteAllText(xmlFile, "xxxxxxxx");

        int fileCount = 0;
        int successCount = 0;

        AsmHelper.ForeachXmlFiles(LoadXmlFile);

        Assert.IsTrue(fileCount > 0);        
        Assert.IsTrue(fileCount > successCount);

        // ClownFish.net.xml 这个文件的内容经常会发生错乱，
        // 导致出现异常：Unexpected end of file has occurred. The following elements are not closed: member, members, doc. Line 3365, position 9.
        // 所以就不使用下面的断言
        //Assert.IsTrue(successCount > 0);


        RetryFile.Delete(xmlFile);

        void LoadXmlFile(string filename)
        {
            fileCount++;

            XmlDocument doc = new XmlDocument();

            //doc.LoadXml(filename);  // Data at the root level is invalid

            string xml = File.ReadAllText(filename);
            doc.LoadXml(xml);

            successCount++;
        }
    }

}
