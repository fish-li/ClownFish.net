using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Log;
[TestClass]
public class LogIdMakerTest
{
    [TestMethod]
    public void Test_V1()
    {
        DateTime time1 = new DateTime(2020, 1, 2, 3, 4, 5);
        string a1 = LogIdMakerV1.Instance.GetNewId(time1);
        Console.WriteLine(a1);

        DateTime time2 = LogIdMaker.ExtractTime(a1).Value;
        Assert.AreEqual(time1, time2);
        
        DateTime now = DateTime.Now;
        string a2 = LogIdMakerV1.Instance.GetNewId(now);
        Assert.IsTrue(a2.Length == 49);
        DateTime now2 = LogIdMaker.ExtractTime(a2).Value;
        Assert.IsTrue((now - now2).TotalSeconds < 1);

        Assert.IsNull(LogIdMakerV1.Instance.ExtractTime(""));
        Assert.IsNull(LogIdMakerV1.Instance.ExtractTime("xxxxxxxxxxxxxxxx"));
    }


    [TestMethod]
    public void Test_V2()
    {
        int runCount = 10_0000;
        Dictionary<string, int> sum = new Dictionary<string, int>();

        Thread[] threads = new Thread[20];
        for( int i = 0; i < threads.Length; i++ ) {
            Thread thread = new Thread(ThreadAction);
            thread.IsBackground = true;
            threads[i] = thread;
            thread.Start();
        }

        for( int i = 0; i < threads.Length; i++ ) {
            Thread thread = threads[i];
            thread.Join();
        }

        Assert.AreEqual(runCount * threads.Length, sum.Count);

        foreach( string id in sum.Select(x => x.Key).Take(100) ) {
            Console.WriteLine(id);
        }

        void ThreadAction(object xx)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();

            for( int i = 0; i < runCount; i++ ) {
                DateTime now = DateTime.Now;
                string id = LogIdMaker.GetNewId(now);
                Assert.AreEqual(24, id.Length);

                // 检查是否有重复
                dict.AddValue(id, i);

                DateTime time = LogIdMaker.ExtractTime(id).Value;

                Assert.IsTrue((now - time).TotalSeconds < 1);
            }

            lock( sum ) {
                foreach( var kv in dict ) {
                    sum.AddValue(kv.Key, kv.Value);
                }
            }
        }
    }

    [TestMethod]
    public void Test_ExtractTime()
    {
        Assert.IsNull(LogIdMakerV2.Instance.ExtractTime(""));
        Assert.IsNull(LogIdMakerV2.Instance.ExtractTime("xxxxxxxxxxxxxxxx"));
        Assert.IsNull(LogIdMakerV2.Instance.ExtractTime(new string('?', 24)));

        Assert.IsNull(LogIdMaker.ExtractTime(""));
        Assert.IsNull(LogIdMaker.ExtractTime("xxxxxxxxx"));

        DateTime now = DateTime.Now;
        string id1 = LogIdMakerV1.Instance.GetNewId(now);
        DateTime time1 = LogIdMakerV1.Instance.ExtractTime(id1).Value;

        string id2 = LogIdMakerV2.Instance.GetNewId(now);
        DateTime time2 = LogIdMakerV2.Instance.ExtractTime(id2).Value;

        Assert.IsTrue((now - time1).TotalSeconds < 1);
        Assert.IsTrue((now - time2).TotalSeconds < 1);

        //Console.WriteLine(LogIdMakerV2.Instance.ExtractTime(new string('a', 24)).Value.ToTimeString());
        //Console.WriteLine(LogIdMakerV2.Instance.ExtractTime(new string('z', 24)).Value.ToTimeString());

        Assert.IsNotNull(LogIdMaker.ExtractTime(new string('a', 24)));
        Assert.IsNotNull(LogIdMaker.ExtractTime(new string('a', 24)));
    }

#if NETCOREAPP
    [TestMethod]
    public void Test_GetNewIdV2()
    {
        DateTime now = DateTime.Now;

        Stopwatch stopwatch = Stopwatch.StartNew();

        for( int i = 0; i < 100_0000; i++ ) {
            string id = LogIdMakerV2.Instance.GetNewId(now);
        }

        stopwatch.Stop();
        Console.WriteLine(stopwatch.Elapsed.ToString());

        stopwatch.Restart();
        for( int i = 0; i < 100_0000; i++ ) {
            string id = LogIdMakerV2.Instance.GetNewId2(now);
        }

        stopwatch.Stop();
        Console.WriteLine(stopwatch.Elapsed.ToString());

        for( int i = 0; i < 30; i++ ) {
            Console.WriteLine(LogIdMakerV2.Instance.GetNewId(now));
            Console.WriteLine(LogIdMakerV2.Instance.GetNewId2(now));
        }
    }
#endif

}
