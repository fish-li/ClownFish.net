namespace ClownFish.UnitTest.Base.Reflection;

[TestClass]
public class MethodTest
{

    [TestMethod]
    public void Test_FastInvoke()
    {
        MethodInfo method = typeof(StaticObject).GetMethod(nameof(StaticObject.Add), BindingFlags.Static | BindingFlags.Public);
        object value = method.FastInvoke(null, 10, 20);
        Assert.IsTrue(value.GetType() == typeof(int));
        Assert.AreEqual(30, (int)value);


        value = method.FastInvoke2(null, 20, 30);
        Assert.IsTrue(value.GetType() == typeof(int));
        Assert.AreEqual(50, (int)value);


        XxxObject xxx = new XxxObject(2);
        MethodInfo refMethod = typeof(XxxObject).GetMethod(nameof(XxxObject.M1), BindingFlags.Instance | BindingFlags.Public);
        MethodDelegate methodDelegate = DynamicMethodFactory.CreateMethod(refMethod);

        object[] args = new object[] { 2, 0 };
        methodDelegate.Invoke(xxx, args);
        Assert.AreEqual(2, args[1]);

    }




    [TestMethod]
    public void Test_MethodInfo_FastInvoke()
    {
        Random rand = new Random();
        List<DataObject> list = new List<DataObject>();

        for( int i = 0; i < 10000; i++ ) {
            DataObject data = new DataObject();
            data.InputA = rand.Next(3, 100);
            data.InputB = rand.Next(5, 200);
            data.Result = data.InputA + data.InputB;
            list.Add(data);
        }


        for( int i = 0; i < 1000; i++ ) {
            ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 50 };
            Parallel.ForEach(list, parallelOptions, x => {
                MethodInfo method = typeof(StaticObject).GetMethod("Add");
                int result = (int)method.FastInvoke(null, new object[] { x.InputA, x.InputB });
                Assert.AreEqual(x.Result, result);
            });
        }
    }


    [TestMethod]
    public void Test_DynamicMethodFactory_Error()
    {
        DataObject data = new DataObject();

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = EmitExtensions.FastInvoke2((MethodInfo)null, data, null);
        });
    }


    [TestMethod]
    public void Test_MethodInvokerFactory_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = MethodInvokerFactory.CreateMethodInvokerWrapper((MethodInfo)null);
        });


        MyAssert.IsError<NotSupportedException>(() => {
            MethodInfo method = typeof(XxxObject).GetMethod(nameof(XxxObject.M1), BindingFlags.Instance | BindingFlags.Public);
            _ = MethodInvokerFactory.CreateMethodInvokerWrapper(method);
        });
    }


    [TestMethod]
    public void Test_Extensions_ArgumentNullException()
    {
        MethodInfo methodInfo = null;
        object obj = new object();

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = methodInfo.FastInvoke(obj, null);
        });
    }

}
