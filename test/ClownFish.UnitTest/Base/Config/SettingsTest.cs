namespace ClownFish.UnitTest.Base.Config;
[TestClass]
public class SettingsTest
{
    [TestMethod]
    public void Test_GetSetting()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = Settings.GetSetting("");
        });

        Assert.IsNull(Settings.GetSetting("xxxxxxxxxxxxxxx", false));
        Assert.IsNotNull(Settings.GetSetting("xxxxxxxxxxxxxxx", "abc"));
        Assert.IsNotNull(Settings.GetSetting("ConfigServiceUrl", true));
    }


    [TestMethod]
    public void Test_GetSettingT()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = Settings.GetSetting<NameValue>("");
        });

        Assert.IsNull(Settings.GetSetting<NameValue>("xxxxxxxxxxxxxxx", false));

        Assert.IsNotNull(Settings.GetSetting<NameValue>("key3", true));
    }

    [TestMethod]
    public void Test_GetBool()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = Settings.GetBool("");
        });

        Assert.IsFalse(Settings.GetBool("key1"));
        Assert.IsFalse(Settings.GetBool("key2"));

        Assert.IsFalse(Settings.GetBool("bool_4"));
        Assert.IsFalse(Settings.GetBool("bool_3"));

        Assert.IsTrue(Settings.GetBool("bool_1"));
        Assert.IsTrue(Settings.GetBool("bool_2"));
    }

    [TestMethod]
    public void Test_GetInt()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = Settings.GetInt("");
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = Settings.GetInt("key1");
        });

        Assert.AreEqual(1234, Settings.GetInt("key2"));        
        Assert.AreEqual(-1234, Settings.GetInt("key2b"));
    }

    [TestMethod]
    public void Test_GetUInt()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = Settings.GetUInt("");
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = Settings.GetUInt("key1");
        });

        MyAssert.IsError<ConfigurationErrorsException>(() => {
            _ = Settings.GetUInt("key2b");
        });

        Assert.AreEqual(1234, Settings.GetUInt("key2"));
    }


    [TestMethod]
    public void Test_ISettings()
    {
        Settings.SetImpl(new XSettingsImpl());
        Assert.AreEqual("key_test_setting_xx", Settings.GetSetting("key_test_setting"));

        Settings.SetImpl(null);
        Assert.AreEqual("123456789", Settings.GetSetting("key_test_setting"));
    }
}



public sealed class XSettingsImpl : ISettings
{
    public string GetSetting(string name, bool checkExist)
    {
        return name + "_xx";
    }
}