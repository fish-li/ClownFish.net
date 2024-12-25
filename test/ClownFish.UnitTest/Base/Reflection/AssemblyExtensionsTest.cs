namespace ClownFish.UnitTest.Base.Reflection;

[TestClass]
public class AssemblyExtensionsTest
{
    [TestMethod]
    public void Test_GetAttributes_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            Assembly assembly2 = null;
            _ = assembly2.GetAttributes<PreApplicationStartMethodAttribute>();
        });


        Assembly assembly = typeof(AssemblyExtensionsTest).Assembly;

        MyAssert.IsError<RuntimeReflectionException>(() => {
            FileNotFoundException error = new FileNotFoundException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetAttributes<PreApplicationStartMethodAttribute>();
        });


        MyAssert.IsError<RuntimeReflectionException>(() => {
            FileLoadException error = new FileLoadException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetAttributes<PreApplicationStartMethodAttribute>();
        });


        MyAssert.IsError<RuntimeReflectionException>(() => {
            TypeLoadException error = new TypeLoadException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetAttributes<PreApplicationStartMethodAttribute>();
        });
    }


    [TestMethod]
    public void Test_GetPublicTypes_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            Assembly assembly2 = null;
            _ = assembly2.GetPublicTypes();
        });


        Assembly assembly = typeof(AssemblyExtensionsTest).Assembly;

        MyAssert.IsError<RuntimeReflectionException>(() => {
            FileNotFoundException error = new FileNotFoundException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetPublicTypes();
        });


        MyAssert.IsError<RuntimeReflectionException>(() => {
            FileLoadException error = new FileLoadException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetPublicTypes();
        });


        MyAssert.IsError<RuntimeReflectionException>(() => {
            TypeLoadException error = new TypeLoadException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetPublicTypes();
        });

        MyAssert.IsError<RuntimeReflectionException>(() => {
            ReflectionTypeLoadException error = new ReflectionTypeLoadException(null, null, "xxx");
            TestHelper.SetException(error);
            _ = assembly.GetPublicTypes();
        });

        MyAssert.IsError<RuntimeReflectionException>(() => {
            NotSupportedException error = new NotSupportedException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetPublicTypes();
        });
    }


    [TestMethod]
    public void Test_GetAllTypes_Error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            Assembly assembly2 = null;
            _ = assembly2.GetAllTypes();
        });


        Assembly assembly = typeof(AssemblyExtensionsTest).Assembly;

        MyAssert.IsError<RuntimeReflectionException>(() => {
            FileNotFoundException error = new FileNotFoundException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetAllTypes();
        });


        MyAssert.IsError<RuntimeReflectionException>(() => {
            FileLoadException error = new FileLoadException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetAllTypes();
        });


        MyAssert.IsError<RuntimeReflectionException>(() => {
            TypeLoadException error = new TypeLoadException("xxx");
            TestHelper.SetException(error);
            _ = assembly.GetAllTypes();
        });

        MyAssert.IsError<RuntimeReflectionException>(() => {
            ReflectionTypeLoadException error = new ReflectionTypeLoadException(null, null, "xxx");
            TestHelper.SetException(error);
            _ = assembly.GetAllTypes();
        });

    }


    [TestMethod]
    public void Test_ReadResAsText()
    {
        Assembly assembly = typeof(AssemblyExtensionsTest).Assembly;
        string text = assembly.ReadResAsText("ClownFish.UnitTest.Base.Reflection._test1.txt");
        Assert.AreEqual(".NET Framework 4", text);

        MyAssert.IsError<ArgumentNullException>(()=> {
            _ = assembly.ReadResAsText("");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            Assembly asm = null;
            _ = asm.ReadResAsText("ClownFish.UnitTest.Base.Reflection._test1.txt");
        });
    }


    [TestMethod]
    public void Test_ReadResAsBytes()
    {
        Assembly assembly = typeof(AssemblyExtensionsTest).Assembly;
        byte[] bytes = assembly.ReadResAsBytes("ClownFish.UnitTest.Base.Reflection._test1.txt");
        string text = Encoding.ASCII.GetString(bytes);
        Assert.AreEqual(".NET Framework 4", text);



        MyAssert.IsError<ArgumentNullException>(() => {
            _ = assembly.ReadResAsBytes("");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            Assembly asm = null;
            _ = asm.ReadResAsBytes("ClownFish.UnitTest.Base.Reflection._test1.txt");
        });
    }

}
