using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Extensions;

[TestClass]
public class MethodExtensionsTest
{
    private static readonly BindingFlags s_flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    [TestMethod]
    public void Test_HasReturn()
    {
        Assert.IsTrue(typeof(MethodExtensions).GetMethod("HasReturn").HasReturn());
        Assert.IsTrue(typeof(MethodExtensions).GetMethod("IsTaskMethod").HasReturn());

        Assert.IsFalse(typeof(TestHelper).GetMethod("TryThrowException").HasReturn());
    }

    [TestMethod]
    public void Test_IsTaskMethod()
    {
        Assert.IsTrue(typeof(TestClass1).GetMethod(nameof(TestClass1.Task1), s_flags).IsTaskMethod());
        Assert.IsTrue(typeof(TestClass1).GetMethod(nameof(TestClass1.Task2), s_flags).IsTaskMethod());
#if NETCOREAPP
        Assert.IsFalse(typeof(TestClass1).GetMethod(nameof(TestClass1.Task3), s_flags).IsTaskMethod());
        Assert.IsFalse(typeof(TestClass1).GetMethod(nameof(TestClass1.Task4), s_flags).IsTaskMethod());
#endif
        Assert.IsFalse(typeof(TestClass1).GetMethod(nameof(TestClass1.Method1), s_flags).IsTaskMethod());
        Assert.IsFalse(typeof(TestClass1).GetMethod(nameof(TestClass1.Method2), s_flags).IsTaskMethod());        
    }
       

    [TestMethod]
    public void Test_GetTaskMethodResultType()
    {
        Assert.IsNull(typeof(TestClass1).GetMethod(nameof(TestClass1.Task1), s_flags).GetTaskMethodResultType());

        Assert.AreEqual(typeof(int),
                        typeof(TestClass1).GetMethod(nameof(TestClass1.Task2), s_flags).GetTaskMethodResultType());

#if NETCOREAPP
        Assert.IsNull(typeof(TestClass1).GetMethod(nameof(TestClass1.Task3), s_flags).GetTaskMethodResultType());
        Assert.IsNull(typeof(TestClass1).GetMethod(nameof(TestClass1.Task4), s_flags).GetTaskMethodResultType());
#endif

        Assert.AreEqual(typeof(DataSet),
                        typeof(BaseCommand).GetMethod("ToDataSetAsync", s_flags).GetTaskMethodResultType());

        // return Task
        Assert.AreEqual(null,
                        typeof(DbContext).GetMethod("OpenConnectionAsync", s_flags).GetTaskMethodResultType());

        // return Task<T>
        Assert.IsTrue(typeof(BaseCommand).GetMethod("ExecuteScalarAsync", s_flags).GetTaskMethodResultType().IsGenericParameter);
    }

#if NETCOREAPP

    [TestMethod]
    public void Test_IsValueTaskMethod()
    {
        Assert.IsFalse(typeof(TestClass1).GetMethod(nameof(TestClass1.Task1), s_flags).IsValueTaskMethod());
        Assert.IsFalse(typeof(TestClass1).GetMethod(nameof(TestClass1.Task2), s_flags).IsValueTaskMethod());

        Assert.IsTrue(typeof(TestClass1).GetMethod(nameof(TestClass1.Task3), s_flags).IsValueTaskMethod());
        Assert.IsTrue(typeof(TestClass1).GetMethod(nameof(TestClass1.Task4), s_flags).IsValueTaskMethod());

        Assert.IsFalse(typeof(TestClass1).GetMethod(nameof(TestClass1.Method1), s_flags).IsValueTaskMethod());
        Assert.IsFalse(typeof(TestClass1).GetMethod(nameof(TestClass1.Method2), s_flags).IsValueTaskMethod());
    }


    [TestMethod]
    public void Test_GetValueTaskMethodResultType()
    {
        Assert.IsNull(typeof(TestClass1).GetMethod(nameof(TestClass1.Task1), s_flags).GetValueTaskMethodResultType());
        Assert.IsNull(typeof(TestClass1).GetMethod(nameof(TestClass1.Task2), s_flags).GetValueTaskMethodResultType());
        Assert.IsNull(typeof(TestClass1).GetMethod(nameof(TestClass1.Task3), s_flags).GetValueTaskMethodResultType());

        Assert.AreEqual(typeof(int),
                        typeof(TestClass1).GetMethod(nameof(TestClass1.Task4), s_flags).GetValueTaskMethodResultType());

    }

#endif


    public class TestClass1
    {
        public Task Task1()
        {
            throw new NotImplementedException();
        }

        public Task<int> Task2()
        {
            throw new NotImplementedException();
        }

#if NETCOREAPP
        public ValueTask Task3()
        {
            throw new NotImplementedException();
        }

        public ValueTask<int> Task4()
        {
            throw new NotImplementedException();
        }
#endif

        public void Method1()
        {
            throw new NotImplementedException();
        }

        public int Method2()
        {
            throw new NotImplementedException();
        }
    }

}
