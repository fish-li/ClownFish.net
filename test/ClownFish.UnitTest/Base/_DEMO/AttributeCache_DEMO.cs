namespace ClownFish.Base.DEMO;

class AttributeCache_DEMO
{
    // AttributeCache 是什么？ 有什么作用？
    // =================================================

    // 用一个示例来说明AttributeCache有什么用。

    // [AllowHttp]
    // public void Method1() {}

    // =================================================

    // 如果我们希望用 [AllowHttp] 来查找某些标记的方法，那么是需要在编译前就在代码中写好的，
    // 由编译器将各种 Attribute 编译到程序集的元数据中，
    // 在运行时，我们就可以使用 .NET 提供的反射API来读取，用来做一些过滤判断。

    // 这样做唯一的问题在于：需要在编译前将各种Attribute固定下来。
    // 如果后期需要增加，只能进入开发流程：拉代码，增加Attribute，编译，测试，发包发布
    // 想实现在运行时调整就完全不可能了。


    // AttributeCache 可以实现上面那种想法：在运行时调整。
    // 具体实现步骤：
    // 1、先调用 AttributeCache.Register<T>(...) 来注册标记，例如：
    //    AllowHttpAttribute a = new AllowHttpAttribute();
    //    MehtodInfo method1 = typeof(XXX).GetMethod("Test1");
    //    AttributeCache.Register<AllowHttpAttribute>(method1, a);

    // 2、读取 Attribute 时，用新的API来替代.net framework 提供的API，例如：
    // method1.GetMyAttribute<AllowHttpAttribute>();        // 这个API定义在 ClownFish.Base中

    // 这样就可以实现在不修改代码的条件下，实现 Attribute 注入。


}
