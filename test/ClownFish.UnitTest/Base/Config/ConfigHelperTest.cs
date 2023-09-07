namespace ClownFish.UnitTest.Base.Config;

[TestClass]
public class ConfigHelperTest
{
    [TestMethod]
    public void Test_GetFilePath()
    {
        string name = "abc.txt";
        Assert.AreEqual(name, ConfigHelper.GetFileAbsolutePath(name));  // not found

        string name2 = "ClownFish.App.config";
        string path2 = ConfigHelper.GetFileAbsolutePath(name2);
        Assert.AreNotEqual(name2, path2);
        Assert.IsTrue(path2.EndsWith(name2));
    }


    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_GetFilePath_ArgumentNullException()
    {
        string path2 = ConfigHelper.GetFileAbsolutePath(null);
    }


    [TestMethod]
    public void Test_GetDirectoryPath()
    {
        string name = "abc";
        Assert.AreEqual(name, ConfigHelper.GetDirectoryAbsolutePath(name));  // not found

        string name2 = "Logs";
        string path2 = ConfigHelper.GetDirectoryAbsolutePath(name2);
        Assert.AreNotEqual(name2, path2);
        Assert.IsTrue(path2.EndsWith(name2));
    }


    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_GetDirectoryPath_ArgumentNullException()
    {
        string path2 = ConfigHelper.GetDirectoryAbsolutePath(string.Empty);
    }
}
