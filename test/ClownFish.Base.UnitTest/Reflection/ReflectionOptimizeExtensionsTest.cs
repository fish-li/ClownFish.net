using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest.Reflection
{
    [TestClass]
    public class ReflectionOptimizeExtensionsTest
    {
        [TestMethod]
        public void Test_MethodInfo_FastInvoke()
        {
            Random rand = new Random();
            List<TestData> list = new List<TestData>();

            for( int i = 0; i < 100000; i++ ) {
                TestData data = new TestData();
                data.InputA = rand.Next(3, 100);
                data.InputB = rand.Next(5, 200);
                data.Result = data.InputA + data.InputB;
                list.Add(data);
            }


            for( int i = 0; i < 1000; i++ ) {
                ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 50 };
                Parallel.ForEach(list, parallelOptions, x => {
                    MethodInfo method = typeof(TestObject).GetMethod("Add");
                    int result = (int)method.FastInvoke(null, new object[] { x.InputA, x.InputB });

                    try {
                        Assert.AreEqual(x.Result, result);
                    }
                    catch {
                        Console.WriteLine($"{x.InputA} + {x.InputB} != {x.Result}, => {result}");
                        throw;
                    }
                });
            }
        }


        public static class TestObject
        {
            public static int Add(int a, int b)
            {
                return a + b;
            }
        }

        public class TestData
        {
            public int InputA { get; set; }

            public int InputB { get; set; }

            public int Result { get; set; }
        }
    }




}
