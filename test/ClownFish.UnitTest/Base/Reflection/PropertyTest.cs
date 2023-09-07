namespace ClownFish.UnitTest.Base.Reflection;

[TestClass]
public class PropertyTest
{
    [TestMethod]
    public void Test_Instance_FastGetSet()
    {
        DataObject data = (DataObject)typeof(DataObject).FastNew();
        data.InputA = 10;

        PropertyInfo property = typeof(DataObject).GetProperty(nameof(data.InputA), BindingFlags.Instance | BindingFlags.Public);
        object value = property.FastGetValue(data);
        Assert.IsTrue(value.GetType() == typeof(int));
        Assert.AreEqual(10, (int)value);

        property.FastSetValue(data, 20);
        value = property.FastGetValue(data);
        Assert.AreEqual(20, (int)value);


        property.FastSetValue2(data, 30);
        value = property.FastGetValue2(data);
        Assert.AreEqual(30, (int)value);
    }


    [TestMethod]
    public void Test_static_FastGetSet()
    {
        StaticObject.Result = 10;

        PropertyInfo property = typeof(StaticObject).GetProperty(nameof(StaticObject.Result), BindingFlags.Static | BindingFlags.Public);
        object value = property.FastGetValue(null);
        Assert.IsTrue(value.GetType() == typeof(int));
        Assert.AreEqual(10, (int)value);

        property.FastSetValue(null, 20);
        value = property.FastGetValue(null);
        Assert.AreEqual(20, (int)value);


        property.FastSetValue2(null, 30);
        value = property.FastGetValue2(null);
        Assert.AreEqual(30, (int)value);
    }


    [TestMethod]
    public void Test_StaticFactory_GetSet()
    {
        PropertyInfo property = typeof(StaticObject).GetProperty(nameof(StaticObject.Result), BindingFlags.Static | BindingFlags.Public);
        IGetValue getter = GetterSetterFactory.CreatePropertyGetterWrapper(property);
        ISetValue setter = GetterSetterFactory.CreatePropertySetterWrapper(property);

        StaticObject.Result = 123584;
        Assert.AreEqual(123584, (int)getter.Get(null));
        setter.Set(null, 100);
        Assert.AreEqual(100, (int)getter.Get(null));
    }


    [TestMethod]
    public void Test_InstanceFactory_GetSet()
    {
        PropertyInfo property = typeof(InstanceObject).GetProperty(nameof(InstanceObject.Result), BindingFlags.Instance | BindingFlags.Public);
        IGetValue getter = GetterSetterFactory.CreatePropertyGetterWrapper(property);
        ISetValue setter = GetterSetterFactory.CreatePropertySetterWrapper(property);

        InstanceObject obj = new InstanceObject(123);

        obj.Result = 123584;
        Assert.AreEqual(123584, (int)getter.Get(obj));
        setter.Set(obj, 100);
        Assert.AreEqual(100, (int)getter.Get(obj));
    }

    [TestMethod]
    public void Test_Static_Wrapper_GetSet()
    {
        PropertyInfo property = typeof(StaticObject).GetProperty(nameof(StaticObject.Result), BindingFlags.Static | BindingFlags.Public);
        StaticGetterWrapper<int> getter = new StaticGetterWrapper<int>(property);
        StaticSetterWrapper<int> setter = new StaticSetterWrapper<int>(property);


        StaticObject.Result = 123584;
        Assert.AreEqual(123584, (int)getter.GetValue());
        setter.SetValue(100);
        Assert.AreEqual(100, (int)getter.GetValue());
    }


    [TestMethod]
    public void Test_Instance_Wrapper_GetSet()
    {
        PropertyInfo property = typeof(InstanceObject).GetProperty(nameof(InstanceObject.Result), BindingFlags.Instance | BindingFlags.Public);
        GetterWrapper<InstanceObject, int> getter = new GetterWrapper<InstanceObject, int>(property);
        SetterWrapper<InstanceObject, int> setter = new SetterWrapper<InstanceObject, int>(property);

        InstanceObject obj = new InstanceObject(123);

        obj.Result = 123584;
        Assert.AreEqual(123584, (int)getter.GetValue(obj));
        setter.SetValue(obj, 100);
        Assert.AreEqual(100, (int)getter.GetValue(obj));
    }

    [TestMethod]
    public void Test_Factory_Error()
    {
        PropertyInfo pStaticWrite = typeof(StaticObject).GetProperty("XWrite");
        PropertyInfo pStaticRead = typeof(StaticObject).GetProperty("XRead");
        PropertyInfo pInstanceWrite = typeof(InstanceObject).GetProperty("XWrite");
        PropertyInfo pInstanceRead = typeof(InstanceObject).GetProperty("XRead");

        PropertyInfo pInstanceItem = typeof(InstanceObject).GetProperty("Item");

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = GetterSetterFactory.CreatePropertyGetterWrapper(null);
        });
        MyAssert.IsError<NotSupportedException>(() => {                
            _ = GetterSetterFactory.CreatePropertyGetterWrapper(pInstanceWrite);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            _ = GetterSetterFactory.CreatePropertyGetterWrapper(pInstanceItem);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            _ = GetterSetterFactory.CreatePropertySetterWrapper(null);
        });
        MyAssert.IsError<NotSupportedException>(() => {                
            _ = GetterSetterFactory.CreatePropertySetterWrapper(pInstanceRead);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            _ = GetterSetterFactory.CreatePropertySetterWrapper(pInstanceItem);
        });


    }

    [TestMethod]
    public void Test_Static_Wrapper_Error()
    {
        PropertyInfo pStaticWrite = typeof(StaticObject).GetProperty("XWrite");
        PropertyInfo pStaticRead = typeof(StaticObject).GetProperty("XRead");
        PropertyInfo pInstanceWrite = typeof(InstanceObject).GetProperty("XWrite");
        PropertyInfo pInstanceRead = typeof(InstanceObject).GetProperty("XRead");

        PropertyInfo pInstanceItem = typeof(InstanceObject).GetProperty("Item");

        // -----------------------------------------------------------------------

        MyAssert.IsError<ArgumentNullException>(() => {
            StaticGetterWrapper<int> getter = new StaticGetterWrapper<int>(null);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            StaticGetterWrapper<int> getter = new StaticGetterWrapper<int>(pInstanceWrite);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            StaticGetterWrapper<int> getter = new StaticGetterWrapper<int>(pInstanceItem);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            StaticSetterWrapper<int> getter = new StaticSetterWrapper<int>(null);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            StaticSetterWrapper<int> getter = new StaticSetterWrapper<int>(pInstanceRead);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            StaticSetterWrapper<int> getter = new StaticSetterWrapper<int>(pInstanceItem);
        });
    }

    [TestMethod]
    public void Test_Instance_Wrapper_Error()
    {
        PropertyInfo pStaticWrite = typeof(StaticObject).GetProperty("XWrite");
        PropertyInfo pStaticRead = typeof(StaticObject).GetProperty("XRead");
        PropertyInfo pInstanceWrite = typeof(InstanceObject).GetProperty("XWrite");
        PropertyInfo pInstanceRead = typeof(InstanceObject).GetProperty("XRead");

        PropertyInfo pInstanceItem = typeof(InstanceObject).GetProperty("Item");

        // -----------------------------------------------------------------------

        MyAssert.IsError<ArgumentNullException>(() => {
            GetterWrapper<InstanceObject, int> getter = new GetterWrapper<InstanceObject, int>(null);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            GetterWrapper<InstanceObject, int> getter = new GetterWrapper<InstanceObject, int>(pInstanceWrite);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            GetterWrapper<InstanceObject, int> getter = new GetterWrapper<InstanceObject, int>(pInstanceItem);
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            SetterWrapper<InstanceObject, int> getter = new SetterWrapper<InstanceObject, int>(null);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            SetterWrapper<InstanceObject, int> getter = new SetterWrapper<InstanceObject, int>(pInstanceRead);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            SetterWrapper<InstanceObject, int> getter = new SetterWrapper<InstanceObject, int>(pInstanceItem);
        });
    }

    [TestMethod]
    public void Test_Wrapper_ArgumentNullException()
    {
        PropertyInfo pInstance = typeof(InstanceObject).GetProperty("Result");

        InstanceObject instance = new InstanceObject(123);

        // -----------------------------------------------------------------------

        GetterWrapper<InstanceObject, int> instanceGetter = new GetterWrapper<InstanceObject, int>(pInstance);
        SetterWrapper<InstanceObject, int> instanceSetter = new SetterWrapper<InstanceObject, int>(pInstance);

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = instanceGetter.GetValue(null);
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = ((IGetValue)instanceGetter).Get(null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            instanceSetter.SetValue(null, 2);
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            ((ISetValue)instanceSetter).Set(null, 2);
        });

    }


    [TestMethod]
    public void Test_DynamicMethodFactory_Error()
    {
        PropertyInfo pStaticWrite = typeof(StaticObject).GetProperty("XWrite");
        PropertyInfo pStaticRead = typeof(StaticObject).GetProperty("XRead");
        PropertyInfo pInstanceWrite = typeof(InstanceObject).GetProperty("XWrite");
        PropertyInfo pInstanceRead = typeof(InstanceObject).GetProperty("XRead");
        PropertyInfo pInstanceItem = typeof(InstanceObject).GetProperty("Item");

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DynamicMethodFactory.CreatePropertyGetter(null);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            _ = DynamicMethodFactory.CreatePropertyGetter(pInstanceWrite);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            _ = DynamicMethodFactory.CreatePropertyGetter(pInstanceItem);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = DynamicMethodFactory.CreatePropertySetter(null);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            _ = DynamicMethodFactory.CreatePropertySetter(pInstanceRead);
        });
        MyAssert.IsError<NotSupportedException>(() => {
            _ = DynamicMethodFactory.CreatePropertySetter(pInstanceItem);
        });

        DataObject data = new DataObject();

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = EmitExtensions.FastGetValue2((PropertyInfo)null, data);
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            EmitExtensions.FastSetValue2((PropertyInfo)null, data, 22);
        });
    }


    [TestMethod]
    public void Test_Extensions_ArgumentNullException()
    {
        PropertyInfo propertyInfo = null;
        object obj = new object();

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = propertyInfo.FastGetValue(obj);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            propertyInfo.FastSetValue(obj, (object)3);
        });

    }
}
