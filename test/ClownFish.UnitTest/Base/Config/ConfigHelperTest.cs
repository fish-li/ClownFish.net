namespace ClownFish.UnitTest.Base.Config;

[TestClass]
public class ConfigHelperTest
{
    [TestMethod]
    public void Test_GetFilePath()
    {
        string name = "abc.txt";
        Assert.IsTrue(PathUtils.GetFileAbsolutePath(name).EndsWith1(name));  // not found

        string name2 = "ClownFish.App.config";
        string path2 = PathUtils.GetFileAbsolutePath(name2);
        Assert.AreNotEqual(name2, path2);
        Assert.IsTrue(path2.EndsWith(name2));
    }


    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_GetFilePath_ArgumentNullException()
    {
        string path2 = PathUtils.GetFileAbsolutePath(null);
    }


    [TestMethod]
    public void Test_GetDirectoryPath()
    {
        string name = "abc";
        Assert.IsTrue(PathUtils.GetDirectoryAbsolutePath(name).EndsWith1(name));  // not found

        string name2 = "Logs";
        string path2 = PathUtils.GetDirectoryAbsolutePath(name2);
        Assert.AreNotEqual(name2, path2);
        Assert.IsTrue(path2.EndsWith(name2));
    }


    [ExpectedException(typeof(ArgumentNullException))]
    [TestMethod]
    public void Test_GetDirectoryPath_ArgumentNullException()
    {
        string path2 = PathUtils.GetDirectoryAbsolutePath(string.Empty);
    }
}
