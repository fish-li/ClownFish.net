using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class AsmHelperTest
{
    [TestMethod]
    public void Test_ForeachXmlFiles()
    {
        List<string> files = new List<string>();

        AsmHelper.ForeachXmlFiles(x=>  files.Add(x));

        Assert.IsTrue(files.Count > 0);
    }


}
