namespace ClownFish.UnitTest.Base.Common;

[TestClass]
public class TSafeDictionaryGetOrAddTest
{
    private int _createCount = 0;

    private readonly int _taskCount = 20;
    private readonly ManualResetEvent _event = new ManualResetEvent(false);

    private readonly string[] _keys = new string[] { "11", "11", "22", "22", "33", "44", "55" };


    [TestMethod]
    public void Test_ConcurrentDictionary()
    {
        _createCount = 0;
        _event.Reset();

        ConcurrentDictionary<string, string> dict = new ConcurrentDictionary<string, string>();
                    
        Task[] tasks = new Task[_taskCount];

        for( int i = 0; i < _taskCount; i++ )
            tasks[i] = Task.Run(()=> {
                foreach( var key in _keys ) {
                    string value = dict.GetOrAdd(key, CreateFunc);
                    Assert.AreEqual(key + "_xx", value);
                }
            });

        _event.Set();
        Task.WaitAll(tasks);

        Assert.AreEqual(5, dict.Count);
        Assert.IsTrue(_createCount > 5);   // 注意这里，居然是 100，非常坑！

        Console.WriteLine(_createCount);
    }


    [TestMethod]
    public void Test_TSafeDictionary()
    {
        _createCount = 0;
        _event.Reset();

        TSafeDictionary<string, string> dict = new TSafeDictionary<string, string>();

        Task[] tasks = new Task[_taskCount];

        for( int i = 0; i < _taskCount; i++ )
            tasks[i] = Task.Run(() => {
                foreach( var key in _keys ) {
                    string value = dict.GetOrAdd(key, CreateFunc);
                    Assert.AreEqual(key + "_xx", value);
                }
            });

        _event.Set();
        Task.WaitAll(tasks);

        Assert.AreEqual(5, dict.Count);
        Assert.AreEqual(5, _createCount);   // 注意这里

        Console.WriteLine(_createCount);
    }


    [TestMethod]
    public void Test_TSafeDictionary2()
    {
        _createCount = 0;
        _event.Reset();

        TSafeDictionary2<string, string> dict = new TSafeDictionary2<string, string>();

        Task[] tasks = new Task[_taskCount];

        for( int i = 0; i < _taskCount; i++ )
            tasks[i] = Task.Run(() => {
                foreach( var key in _keys ) {
                    string value = dict.GetOrAdd(key, CreateFunc);
                    Assert.AreEqual(key + "_xx", value);
                }
            });

        _event.Set();
        Task.WaitAll(tasks);

        Assert.AreEqual(5, dict.Count);
        Assert.AreEqual(5, _createCount);   // 注意这里

        Console.WriteLine(_createCount);
    }


    private string CreateFunc(string x)
    {
        _event.WaitOne();

        System.Threading.Thread.Sleep(10);
        Interlocked.Increment(ref _createCount);
        return x + "_xx";
    }

}
