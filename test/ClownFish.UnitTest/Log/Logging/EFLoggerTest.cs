#if NETCOREAPP
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using ClownFish.Log.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Log.Logging;
[TestClass]
public class EFLoggerTest
{
    // 模拟 EntityFrameworkCore 发布事件
    private static readonly DiagnosticListener s_diagnosticSource = new DiagnosticListener("Microsoft.EntityFrameworkCore");

    static EFLoggerTest()
    {
        EFLogger.Init();
    }

    [TestMethod]
    public void Test1()
    {
        using DbContext db = DbContext.Create("sqlserver");
        db.BeginTransaction();

        using OprLogScope scope = OprLogScope.Start();

        PublishEvent(db);

        List<StepItem> steps = scope.GetStepItems();
        Assert.AreEqual(10, steps.Count);

        string details = scope.GetOprDetails();
        Console.WriteLine(details);
    }


    [TestMethod]
    public void Test2()
    {
        using DbContext db = DbContext.Create("sqlserver");
        db.BeginTransaction();

        PublishEvent(db);
    }

    private void PublishEvent(DbContext db)
    {
        object data1 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = false,
            Duration = TimeSpan.FromMilliseconds(111),
            Connection = db.Connection
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Connection.ConnectionOpened", data1);

        Thread.Sleep(50);

        object data2 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = false,
            Exception = ExceptionHelper.CreateException(),
            Duration = TimeSpan.FromMilliseconds(222),
            Connection = db.Connection
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Connection.ConnectionError", data2);


        Thread.Sleep(50);

        CPQuery query1 = db.CPQuery.Create("select * from table1 where id = @id", new { id = 2 });
        object data3 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = false,
            Duration = TimeSpan.FromMilliseconds(333),
            Command = query1.Command
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted", data3);


        Thread.Sleep(50);
        CPQuery query2 = db.CPQuery.Create("insert into * from table1 where id = @id", new { id = 2 });
        object data4 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = true,
            Duration = TimeSpan.FromMilliseconds(333),
            Command = query2.Command
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted", data4);


        Thread.Sleep(50);
        CPQuery query3 = db.CPQuery.Create("update * from table1 where id = @id", new { id = 2 });
        object data5 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = true,
            Duration = TimeSpan.FromMilliseconds(333),
            Command = query3.Command
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted", data5);


        Thread.Sleep(50);
        CPQuery query4 = db.CPQuery.Create("delete * from table1 where id = @id", new { id = 2 });
        object data6 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = true,
            Duration = TimeSpan.FromMilliseconds(333),
            Command = query4.Command
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted", data6);

        Thread.Sleep(50);
        CPQuery query5 = db.CPQuery.Create("exec * from table1 where id = @id", new { id = 2 });
        object data61 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = false,
            Duration = TimeSpan.FromMilliseconds(333),
            Command = query5.Command
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted", data61);


        Thread.Sleep(50);

        object data7 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = false,
            Exception = ExceptionHelper.CreateException(),
            Duration = TimeSpan.FromMilliseconds(444),
            Command = query1.Command
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Command.CommandError", data7);


        Thread.Sleep(50);

        object data8 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = false,
            Duration = TimeSpan.FromMilliseconds(555),
            Transaction = db.Transaction
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Transaction.TransactionCommitted", data8);

        object data9 = new {
            StartTime = DateTimeOffset.Now,
            IsAsync = false,
            Exception = ExceptionHelper.CreateException(),
            Duration = TimeSpan.FromMilliseconds(666),
            Transaction = db.Transaction
        };
        s_diagnosticSource.Write("Microsoft.EntityFrameworkCore.Database.Transaction.TransactionError", data9);

    }


    [TestMethod]
    public void Test_x1()
    {
        EFEventSubscriber x = new EFEventSubscriber();
        x.OnCompleted();
        x.OnError(null);
    }


    [TestMethod]
    public void Test_x2()
    {
        EFEventObserver x = new EFEventObserver();
        x.OnCompleted();
        x.OnError(null);
    }
}
#endif