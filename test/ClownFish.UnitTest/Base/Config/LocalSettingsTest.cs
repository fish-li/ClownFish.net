namespace ClownFish.UnitTest.Base.Config;

[TestClass]
public class LocalSettingsTest
{
    [TestMethod]
    public void TestGetSetting()
    {
        Assert.AreEqual("abcd", LocalSettings.GetSetting("key1", true));

        Assert.AreEqual("abcd", LocalSettings.GetSetting("key1", "aaaaaaaaaaaa"));
        Assert.AreEqual("aaaaaaaaaaaa", LocalSettings.GetSetting("key1111", "aaaaaaaaaaaa"));

        NameValue nv = LocalSettings.GetSetting<NameValue>("key3", true);
        Assert.AreEqual("FishDbServer", nv.Name);
        Assert.AreEqual("admin", nv.Value);

        Assert.AreEqual("FishDev", LocalSettings.GetSetting("RUNTIME_ENVIRONMENT", true));
    }


    [TestMethod]
    public void TestGetInt()
    {
        Assert.AreEqual(1234, LocalSettings.GetInt("key2", 222));
        Assert.AreEqual(222, LocalSettings.GetInt("key22", 222));
        Assert.AreEqual(-222, LocalSettings.GetInt("key22", -222));

        Assert.AreEqual(1234, LocalSettings.GetUInt("key2", 222));
        Assert.AreEqual(222, LocalSettings.GetUInt("key22", 222));
    }


    [TestMethod]
    public void Test_GetBool()
    {
        Assert.IsTrue(LocalSettings.GetBool("bool_1"));
        Assert.IsTrue(LocalSettings.GetBool("bool_2"));

        Assert.IsFalse(LocalSettings.GetBool("bool_3"));
        Assert.IsFalse(LocalSettings.GetBool("bool_4"));

        Assert.IsFalse(LocalSettings.GetBool("key1"));
        Assert.IsTrue(LocalSettings.GetBool("bool_4", 1));

        Assert.IsTrue(LocalSettings.GetBool("bool_xx", 1));
    }


    [TestMethod]
    public void Test_Error()
    {
        MyAssert.IsError<ArgumentNullException>(()=> {
            _= LocalSettings.GetSetting("");
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = LocalSettings.GetSetting("key-not-found", true);
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = LocalSettings.GetInt("key1");
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = LocalSettings.GetUInt("key1");
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = LocalSettings.GetUInt("key2b");
        });
    }

    [TestMethod]
    public void Test_ILocalSettings()
    {
        LocalSettings.SetImpl(new XLocalSettingsImpl());
        Assert.AreEqual("key_test_setting_xx", LocalSettings.GetSetting("key_test_setting"));

        LocalSettings.SetImpl(null);
        Assert.AreEqual("123456789", LocalSettings.GetSetting("key_test_setting"));
    }
}

public sealed class XLocalSettingsImpl : ILocalSettings
{
    public string GetSetting(string name, bool checkExist)
    {
        return name + "_xx";
    }
}