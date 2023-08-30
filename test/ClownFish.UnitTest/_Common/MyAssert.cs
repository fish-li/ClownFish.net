using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest._Common;

public static class MyAssert
{
    public static void AreEqual(object a, object b)
    {
        if( a == null && b == null )
            return;

        Assert.IsNotNull(a);
        Assert.IsNotNull(b);

        Assert.IsTrue(a.GetType() == b.GetType());

        string json1 = a.ToJson();
        string json2 = b.ToJson();
        Assert.AreEqual(json1, json2);
    }


    public static void AreEqual(byte[] a, byte[] b)
    {
        Assert.IsTrue(a.IsEqual(b));
    }

    public static void AreEqual<T>(IEquatable<T> a, IEquatable<T> b)
    {
        Assert.IsTrue(a.Equals(b));
    }


    /// <summary>
    /// 比较二段文本是否相等。
    /// 这个方法主要是为了解决 坑爹的【换行符】问题。
    /// 有些文件被 git 修改了换行符，导致直接多行文本比较的不匹配！
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static void SqlAreEqual(string a, string b)
    {
        if( a == null && b == null )
            return;

        Assert.IsNotNull(a);
        Assert.IsNotNull(b);

        string a2 = a.Trim().Replace("\r", "").Replace("\n", "");
        string b2 = b.Trim().Replace("\r", "").Replace("\n", "");
        Assert.AreEqual(a2, b2);
    }


    public static Exception IsError<T>(Action action) where T : Exception
    {
        Exception exception = null;
        try {
            action();
        }
        catch( Exception ex ) {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(T));

        return exception;
    }


    public static async Task IsErrorAsync<T>(Func<Task> action) where T : Exception
    {
        Exception exception = null;
        try {
            await action();
        }
        catch( Exception ex ) {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(T));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">外层异常类型</typeparam>
    /// <typeparam name="T2">InnerException类型</typeparam>
    /// <param name="action"></param>
    public static void IsError<T, T2>(Action action) where T : Exception
    {
        Exception exception = null;
        try {
            action();
        }
        catch( Exception ex ) {
            exception = ex;
        }

        Assert.IsNotNull(exception);
        Assert.IsNotNull(exception.InnerException);

        Assert.IsInstanceOfType(exception, typeof(T));
        Assert.IsInstanceOfType(exception.InnerException, typeof(T2));
    }
}
