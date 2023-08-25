using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class EnvUtilsTest
{
    [TestMethod]
    public void Test_1()
    {
        Assert.AreEqual(EvnKind.Dev, EnvUtils.CurEvnKind);
        Assert.IsTrue(EnvUtils.IsDevEnv);
        Assert.IsFalse(EnvUtils.IsProdEnv);
        Assert.IsFalse(EnvUtils.IsTestEnv);

    }

    [TestMethod]
    public void Test_GetEvnKind()
    {
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind(""));
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("prod"));
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("PROD"));
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("Product"));
        Assert.AreEqual(EvnKind.Prod, EnvUtils.GetEvnKind("Product_2"));

        Assert.AreEqual(EvnKind.Test, EnvUtils.GetEvnKind("Test"));
        Assert.AreEqual(EvnKind.Test, EnvUtils.GetEvnKind("Test_2"));

        Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("dev"));
        Assert.AreEqual(EvnKind.Dev, EnvUtils.GetEvnKind("xxx"));
    }

    [TestMethod]
    public void Test_CheckApplicationName()
    {
        EnvUtils.CheckApplicationName("aa11_bb");
        EnvUtils.CheckApplicationName("aa11.bb");

        MyAssert.IsError<ArgumentNullException>(() => {
            EnvUtils.CheckApplicationName("");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11-bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11/bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11 bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11+bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("aa11~bb");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            EnvUtils.CheckApplicationName("中文汉字");
        });
    }
}
