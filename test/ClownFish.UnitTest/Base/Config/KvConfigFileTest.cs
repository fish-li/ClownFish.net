using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class KvConfigFileTest
{
    [TestMethod]
    public void Test_1()
    {
        string localEnvFilePath = Path.Combine(AppContext.BaseDirectory, "files/_local2.env");

        Dictionary<string, string> dict = new Dictionary<string, string>();
        int count = KvConfigFile.LoadFromFile(localEnvFilePath, dict);
        CheckResult(count, dict);


        string text = RetryFile.ReadAllText(localEnvFilePath);

        Dictionary<string, string> dict2 = new Dictionary<string, string>();
        int count2 = KvConfigFile.LoadFromText(text, dict2);
        CheckResult(count2, dict2);


        void CheckResult(int count, Dictionary<string, string> dict)
        {
            Assert.AreEqual(4, count);
            Assert.AreEqual(4, dict.Count);

            Assert.IsTrue(dict.ContainsKey("env_test_0"));
            Assert.IsTrue(dict.ContainsKey("env_test_1"));
            Assert.IsTrue(dict.ContainsKey("env_test_2"));
            Assert.IsTrue(dict.ContainsKey("env_test_3"));
            Assert.IsFalse(dict.ContainsKey("env_test_4"));

            Assert.AreEqual("", dict["env_test_0"]);
            Assert.AreEqual("4ff568ad140b45328bb2e30072abaa76", dict["env_test_1"]);
            Assert.AreEqual("e6667554a0744f35892b4a7bb8ae45c7", dict["env_test_2"]);
            Assert.AreEqual("830418a8b7ae408a953d99829d291ecc", dict["env_test_3"]);
        }
    }

    [TestMethod]
    public void Test_2()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        Assert.AreEqual(-1, KvConfigFile.LoadFromFile("./xxxxxxxxxxx.abc", dict));


        string localEnvFilePath = Path.Combine(AppContext.BaseDirectory, "files/_local2.env");
        Dictionary<string, string> nullDict = null;
        Assert.AreEqual(-2, KvConfigFile.LoadFromFile(localEnvFilePath, nullDict));


        Assert.AreEqual(-1, KvConfigFile.LoadFromText("", dict));
        Assert.AreEqual(-2, KvConfigFile.LoadFromText("aaa=11", nullDict));

    }

}
