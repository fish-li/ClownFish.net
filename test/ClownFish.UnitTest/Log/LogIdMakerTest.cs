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
        string a2 = LogIdMakerV1.Instance.GetNewId();
        Assert.IsTrue(a2.Length == 49);
        DateTime now2 = LogIdMaker.ExtractTime(a2).Value;
        Assert.IsTrue((now - now2).TotalSeconds < 1);

        Assert.IsNull(LogIdMakerV1.Instance.ExtractTime(""));
        Assert.IsNull(LogIdMakerV1.Instance.ExtractTime("xxxxxxxxxxxxxxxx"));
    }


    [TestMethod]
    public void Test_V2()
    {
        Assert.IsNull(LogIdMakerV2.Instance.ExtractTime(""));
        Assert.IsNull(LogIdMakerV2.Instance.ExtractTime("xxxxxxxxxxxxxxxx"));
        Assert.IsNull(LogIdMakerV2.Instance.ExtractTime(new string('?', 24)));

        int runCount = 50_0000;
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
                string id = LogIdMakerV2.Instance.GetNewId(now);

                // 检查是否有重复
                dict.AddValue(id, i);

                DateTime time = LogIdMakerV2.Instance.ExtractTime(id).Value;

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
        Assert.IsNull(LogIdMaker.ExtractTime(""));
        Assert.IsNull(LogIdMaker.ExtractTime("xxxxxxxxx"));

        Assert.IsNotNull(LogIdMakerV1.Instance.GetNewId());
        Assert.IsNotNull(LogIdMakerV2.Instance.GetNewId());

        //Console.WriteLine(LogIdMakerV2.Instance.ExtractTime(new string('a', 24)).Value.ToTimeString());
        //Console.WriteLine(LogIdMakerV2.Instance.ExtractTime(new string('z', 24)).Value.ToTimeString());

        Assert.IsNotNull(LogIdMaker.ExtractTime(new string('a', 24)));
        Assert.IsNotNull(LogIdMaker.ExtractTime(new string('a', 24)));
    }
}
