using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Reflection;
[TestClass]
public class ReflectionUtilsTest
{
    [TestMethod]
    public void Test_Get()
    {
        NameValue v1 = new NameValue("key1", "abc");
        Assert.AreEqual("key1", v1.Get<string>("Name"));
        Assert.AreEqual("abc", v1.Get<string>("Value"));

        MyAssert.IsError<ArgumentNullException>(() => {
            ReflectionUtils.Get<string>(null, "Name");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ReflectionUtils.Get<string>(v1, "");
        });

        MyAssert.IsError<ArgumentOutOfRangeException>(() => {
            ReflectionUtils.Get<string>(v1, "xxx");
        });


        ReflectionUtilsTestClass instance = new ReflectionUtilsTestClass();
        Assert.AreEqual(23, instance.Get<int>("Count"));
        Assert.AreEqual("abcd", instance.Get<string>("Name"));
    }


    [TestMethod]
    public void Test_CallStaticMethod()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ReflectionUtils.CallStaticMethod(null, "Name");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ReflectionUtils.CallStaticMethod("xxxxxx", "");
        });

        Assert.AreEqual(-1, ReflectionUtils.CallStaticMethod("xxxxxxxxxxxxxxxxxxxxxx", "Name"));
        Assert.AreEqual(-2, ReflectionUtils.CallStaticMethod("ClownFish.UnitTest.Base.Reflection.ReflectionUtilsTest, ClownFish.UnitTest", "Name"));

        Assert.AreEqual(1, ReflectionUtils.CallStaticMethod("ClownFish.UnitTest.Base.Reflection.ReflectionUtilsTest, ClownFish.UnitTest", "M1"));

    }


    private static void M1()
    {
        // 什么也不做
    }


    [TestMethod]
    public async Task Test_CallMethod()
    {
        ReflectionUtilsTestClass instance = new ReflectionUtilsTestClass();

        MethodInfo m1 = typeof(ReflectionUtilsTestClass).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
        int result1 = (int) await ReflectionUtils.CallMethod(instance, m1, new object[] { 2, 3 });
        Assert.AreEqual(5, result1);

        MethodInfo m2 = typeof(ReflectionUtilsTestClass).GetMethod("Show", BindingFlags.Instance | BindingFlags.Public);
        object result2 = await ReflectionUtils.CallMethod(instance, m2, new object[] { "aaaaaaaa" });
        Assert.IsNull(result2);

        MethodInfo m3 = typeof(ReflectionUtilsTestClass).GetMethod("AddAsync", BindingFlags.Instance | BindingFlags.Public);
        int result3 = (int)await ReflectionUtils.CallMethod(instance, m3, new object[] { 2, 3 });
        Assert.AreEqual(15, result3);

        MethodInfo m4 = typeof(ReflectionUtilsTestClass).GetMethod("ShowAsync", BindingFlags.Instance | BindingFlags.Public);
        object result4 = await ReflectionUtils.CallMethod(instance, m4, new object[] { "bbbbbbbbbb" });
        Assert.IsNull(result4);

        MethodInfo m5 = typeof(ReflectionUtilsTestClass).GetMethod("StaticAdd", BindingFlags.Static | BindingFlags.Public);
        int result5 = (int)await ReflectionUtils.CallMethod(null, m5, new object[] { 2, 3 });
        Assert.AreEqual(25, result5);

        MethodInfo m6 = typeof(ReflectionUtilsTestClass).GetMethod("StaticAddAsync", BindingFlags.Static | BindingFlags.Public);
        int result6 = (int)await ReflectionUtils.CallMethod(null, m6, new object[] { 2, 3 });
        Assert.AreEqual(35, result6);

        await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
            MethodInfo mm = null;
            await ReflectionUtils.CallMethod(instance, mm, new object[] { 2, 3 });
        });
    }
}



internal sealed class ReflectionUtilsTestClass
{
    public int Count = 23;

    public string Name = "abcd";

    public int Add(int a, int b) => a + b;

    public void Show(string s) => Console.WriteLine(s);

    public Task<int> AddAsync(int a, int b) => Task.FromResult(a + b + 10);

    public Task ShowAsync(string s)
    {
        Console.WriteLine(s);
        return Task.CompletedTask;
    }

    public static int StaticAdd(int a, int b) => a + b + 20;

    public static Task<int> StaticAddAsync(int a, int b) => Task.FromResult(a + b + 30);
}