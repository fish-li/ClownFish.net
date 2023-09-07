using ClownFish.UnitTest.Data.EntityX;

namespace ClownFish.UnitTest.Log.Serialization;
[TestClass]
public class TextUtilsTest
{
    [TestMethod]
    public void Test_string()
    {
        Assert.AreEqual("", TextUtils.GetLogText(null));

        Assert.AreEqual("abc", TextUtils.GetLogText("abc"));
    }

    [TestMethod]
    public void Test_ToJson()
    {
        NameValue nv = new NameValue("key1", "abc");
        string text = TextUtils.GetLogText(nv);
        Console.WriteLine(text);
    }

    [TestMethod]
    public void Test_DbCommand()
    {
        using DbContext db = DbContext.Create("s1");
        var query = db.CPQuery.Create("select * from table1 where id=@id", new {id =5});

        string text = TextUtils.GetLogText(query.Command);
        Console.WriteLine(text);

        // 只要不抛异常就可以了
    }

    [TestMethod]
    public void Test_ILoggingObject()
    {
        X1 x = new X1("82ee28c0b1a74beeb4c2e797878eca6e");

        string text = TextUtils.GetLogText(x);
    }

    [TestMethod]
    public void Test_ITextSerializer()
    {
        X2 x = new X2("82ee28c0b1a74beeb4c2e797878eca6e");

        string text = TextUtils.GetLogText(x);
    }



#if NET6_0_OR_GREATER
    [TestMethod]
    public void Test_DbBatch()
    {
        List<ClownFish.UnitTest.Data.Models.Product> list = BatchInsertTest.CreateList(5);
        
        using DbContext dbContext = DbContext.Create("mysql");
        DbBatch batch = dbContext.Batch.CreateDbBatch(list, CurdKind.Insert);

        string text = TextUtils.GetLogText(batch);
        Console.WriteLine(text);
    }
#endif


    [TestMethod]
    public void Test_Error()
    {
        string text = TextUtils.GetLogText(new X3());
        Console.WriteLine(text);

        Assert.IsTrue(text.StartsWith0("## GetLogText(ClownFish.UnitTest.Log.Serialization.TextUtilsTest+X3) ERROR: System.NotImplementedException"));
        
    }

    [TestMethod]
    public void Test_GetErrorLogText()
    {
        Assert.AreEqual("", TextUtils.GetErrorLogText((Exception)null));

        Exception ex1 = ExceptionHelper.CreateException();
        string text1 = TextUtils.GetErrorLogText(ex1);
    }



    public class X1 : ILoggingObject
    {
        private readonly string _message;

        public X1(string message)
        {
            _message = message;
        }

        public string ToLoggingText() => _message;
    }

    public class X2 : ITextSerializer
    {
        private readonly string _message;

        public X2(string message)
        {
            _message = message;
        }

        public void LoadData(string text)
        {
            throw new NotImplementedException();
        }

        public string ToText() => _message;
    }

    public class X3 : ITextSerializer
    {
        public void LoadData(string text)
        {
            throw new NotImplementedException();
        }

        public string ToText()
        {
            throw new NotImplementedException();
        }
    }
}
