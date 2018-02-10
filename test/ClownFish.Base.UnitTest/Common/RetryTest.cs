using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Common
{
    [TestClass]
    public class RetryTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Retry_指定错误重试次数()
        {
            TestRetryTask1 task = new TestRetryTask1();

            string text = Retry.Create(-5, 10).Run(() => {
                return task.Exec1();
            });

            Assert.AreEqual(TestRetryTask1.Result, text);
        }


        [TestMethod]
        public void Test_Retry_不指定过滤器()
        {
            TestRetryTask1 task = new TestRetryTask1();

            string text = Retry.Create(5, 10).Run(() => {                
                return task.Exec1();
            });

            Assert.AreEqual(TestRetryTask1.Result, text);
        }


        [TestMethod]
        public void Test_Retry_指定全部过滤器()
        {
            TestRetryTask1 task = new TestRetryTask1();

            string text = Retry.Create(5, 10)
                .Filter<NotSupportedException>()
                .Filter<ArgumentOutOfRangeException>(ex => ex.ParamName == "name")
                .Filter<ArgumentNullException>(ex => ex.ParamName == "key")
                .Run(() => {                    
                    return task.Exec2();
                });

            Assert.AreEqual(TestRetryTask1.Result, text);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Retry_指定部分过滤器_将会出现异常()
        {
            TestRetryTask1 task = new TestRetryTask1();

            string text = Retry.Create(5, 10)
                    .Filter<NotSupportedException>()
                    .Filter<ArgumentOutOfRangeException>(ex => ex.ParamName == "name")
                      .Run(() => {                          
                          return task.Exec2();
                      });

            Assert.AreEqual(TestRetryTask1.Result, text);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Retry_指定部分过滤器_将会出现异常2()
        {
            TestRetryTask1 task = new TestRetryTask1();

            string text = Retry.Create(5, 10)
                    .Filter<NotSupportedException>()
                      .Run(() => {                          
                          return task.Exec2();
                      });

            Assert.AreEqual(TestRetryTask1.Result, text);
        }



        [TestMethod]
        public void Test_Retry_指定过滤器_指定回调方法()
        {
            int count = 0;
            int count2 = 0;
            TestRetryTask1 task = new TestRetryTask1();

            string text = Retry.Create(5, 10)
                    .Filter<NotSupportedException>()
                    .OnException((ex, n)=> { count++; })
                    .OnException((ex, n) => { count2+=n; })
                      .Run(() => {                          
                          return task.Exec3();
                      });

            Assert.AreEqual(TestRetryTask1.Result, text);
            Assert.AreEqual(3, count);
            Assert.AreEqual(6, count2);
        }

    }


    public class TestRetryTask1
    {
        private int _count = 0;


        public static readonly string Result = "123";


        public string Exec1()
        {
            _count++;

            if( _count < 3 )
                throw new InvalidOperationException();


            return Result;
        }


        public string Exec2()
        {
            _count++;

            if( _count == 1 )
                throw new NotSupportedException();

            if( _count == 2 )
                throw new ArgumentOutOfRangeException("name");

            if( _count == 3 )
                throw new ArgumentNullException("key");


            return Result;
        }


        public string Exec3()
        {
            _count++;

            if( _count <= 3 )
                throw new NotSupportedException();


            return Result;
        }

    }
}
