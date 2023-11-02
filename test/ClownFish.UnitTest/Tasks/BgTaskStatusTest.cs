using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Tasks;
#if NETCOREAPP
using ClownFish.Tasks;
[TestClass]
public class BgTaskStatusTest
{
    [TestMethod]
    public void Test1()
    {
        BgTaskStatus instance = new BgTaskStatus();

        PropertyInfo[] ps = typeof(BgTaskStatus).GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach( PropertyInfo pi in ps ) {
            object value = pi.GetValue(instance, null);
            pi.SetValue(instance, value, null);
        }

        instance.Status = 0;
        Assert.AreEqual("等待中", instance.GetStatusText());

        instance.Status = 1;
        Assert.AreEqual("运行中", instance.GetStatusText());

        instance.Status = 2;
        Assert.AreEqual("已停止", instance.GetStatusText());

        instance.Status = 3;
        Assert.AreEqual("未知", instance.GetStatusText());

        instance.Status = -1;
        Assert.AreEqual("未知", instance.GetStatusText());


        instance.LastStatus = 0;
        Assert.AreEqual("成功", instance.GetLastStatusText());

        instance.LastStatus = 1;
        Assert.AreEqual("运行中", instance.GetLastStatusText());

        instance.LastStatus = 2;
        Assert.AreEqual("失败", instance.GetLastStatusText());

        instance.LastStatus = 3;
        Assert.AreEqual("未知", instance.GetLastStatusText());

        instance.LastStatus = -1;
        Assert.AreEqual("未知", instance.GetLastStatusText());
    }
}
#endif
