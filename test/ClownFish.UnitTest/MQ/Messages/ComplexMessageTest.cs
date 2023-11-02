using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.MQ.Messages;

#if NETCOREAPP

[TestClass]
public class ComplexMessageTest
{
    [TestMethod]
    public void Test_ObjectMessage()
    {
        Product data = new Product {
            ProductID = 2,
            CategoryID = 3,
            ProductName = "中文汉字",
            UnitPrice = 22.33m,
            Quantity = 123,
            Unit = "个",
            Remark = "720 次提交 3 个分支 0 个标签 53.8 MB 文件 3.0 GB Storage"
        };
        string json = data.ToJson();

        ComplexMessage<Product> msg = new ComplexMessage<Product>(data);

        byte[] b1 = MessageBinSerializer.Instance.Serialize(msg);
        ComplexMessage<Product> msg2 = MessageBinSerializer.Instance.Deserialize<ComplexMessage<Product>>(b1);
        Assert.AreEqual(json, msg2.Body.ToJson());


        string text = MessageTextSerializer.Instance.Serialize(msg);
        ComplexMessage<Product> msg3 = MessageTextSerializer.Instance.Deserialize<ComplexMessage<Product>>(text);
        Assert.AreEqual(json, msg3.Body.ToJson());
    }


    [TestMethod]
    public void Test_TextMessage()
    {
        string data = "10月考勤请大家及时处理，未处理的考勤异常将按事假扣发工资，并无法事后补发";


        ComplexMessage<string> msg = new ComplexMessage<string>(data);

        byte[] b1 = MessageBinSerializer.Instance.Serialize(msg);
        ComplexMessage<string> msg2 = MessageBinSerializer.Instance.Deserialize<ComplexMessage<string>>(b1);
        Assert.AreEqual(data, msg2.Body);

        string text = MessageTextSerializer.Instance.Serialize(msg);
        ComplexMessage<string> msg3 = MessageTextSerializer.Instance.Deserialize<ComplexMessage<string>>(text);
        Assert.AreEqual(data, msg3.Body);
    }


    [TestMethod]
    public void Test_BytesMessage()
    {
        string data = "补卡归档DDL：2022年10月31日，请及时跟进审批，请及时跟进审批（提交考勤单后，请及时跟进考勤归档情况）";
        byte[] bb = data.GetBytes();

        ComplexMessage<byte[]> msg = new ComplexMessage<byte[]>(bb);

        byte[] b1 = MessageBinSerializer.Instance.Serialize(msg);
        ComplexMessage<byte[]> msg2 = MessageBinSerializer.Instance.Deserialize<ComplexMessage<byte[]>>(b1);
        Assert.IsTrue(bb.IsEqual(msg2.Body));

        string text = MessageTextSerializer.Instance.Serialize(msg);
        ComplexMessage<byte[]> msg3 = MessageTextSerializer.Instance.Deserialize<ComplexMessage<byte[]>>(text);
        Assert.IsTrue(bb.IsEqual(msg3.Body));
    }

    [TestMethod]
    public void Test_IMsgObject()
    {
        ComplexMessage<string> msg = new ComplexMessage<string>("aa");

        // 检验是否可取到数据，不抛异常就算测试通过
        Console.WriteLine(((IMsgObject)msg).GetTime());
        Console.WriteLine(((IMsgObject)msg).GetId());
    }

    [TestMethod]
    public void Test_ctor_error()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = new ComplexMessage<string>(null);
        });
    }


    [TestMethod]
    public void Test_Validate()
    {
        // throw new InvalidDataException("消息体为空，不能执行序列化。");
        MyAssert.IsError<InvalidDataException>(() => {
            ComplexMessage<string> msg = new ComplexMessage<string>("");
            msg.Validate();
        });

        // throw new InvalidDataException("没有指定消息头。");
        MyAssert.IsError<InvalidDataException>(() => {
            ComplexMessage<string> msg = new ComplexMessage<string>("aa");
            msg.Headers.Clear();
            msg.Validate();
        });

        // throw new InvalidDataException("消息体为null，不能执行序列化。");
        MyAssert.IsError<InvalidDataException>(() => {
            ComplexMessage<string> msg = new ComplexMessage<string>();
            msg.Headers["x1"] = "aa";
            msg.Validate();
        });
    }

    [TestMethod]
    public void Test_GetBodyAsString()
    {
        ComplexMessage<string> msg = new ComplexMessage<string>();
        Assert.AreEqual(string.Empty, msg.GetBodyAsString());


        ComplexMessage<string> msg2 = new ComplexMessage<string>("98b5cb37b39c41b6aef2b3a843698451");
        Assert.AreEqual("98b5cb37b39c41b6aef2b3a843698451", msg2.GetBodyAsString());

        byte[] bb = "中文汉字最优秀！".GetBytes();
        ComplexMessage<byte[]> msg3 = new ComplexMessage<byte[]>(bb);
        Assert.AreEqual("5Lit5paH5rGJ5a2X5pyA5LyY56eA77yB", msg3.GetBodyAsString());

        NameValue nv = new NameValue("aa", "bb");
        ComplexMessage<NameValue> msg4 = new ComplexMessage<NameValue>(nv);
        Assert.AreEqual("{\"Name\":\"aa\",\"Value\":\"bb\"}", msg4.GetBodyAsString());
    }

    [TestMethod]
    public void Test_StringToBodyObject()
    {
        ComplexMessage<string> msg = new ComplexMessage<string>();
        Assert.IsNull(msg.StringToBodyObject(null));

        Assert.AreEqual("98b5cb37b39c41b6aef2b3a843698451", msg.StringToBodyObject("98b5cb37b39c41b6aef2b3a843698451"));


        ComplexMessage<byte[]> msg3 = new ComplexMessage<byte[]>();
        byte[] bb = msg3.StringToBodyObject("5Lit5paH5rGJ5a2X5pyA5LyY56eA77yB");
        Assert.AreEqual("中文汉字最优秀！", Encoding.UTF8.GetString(bb));

        ComplexMessage<NameValue> msg4 = new ComplexMessage<NameValue>();
        NameValue nv = msg4.StringToBodyObject("{\"Name\":\"aa\",\"Value\":\"bb\"}");
        Assert.IsNotNull(nv);
    }


    [TestMethod]
    public void Test_LoadData()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            ComplexMessage<string> msg = new ComplexMessage<string>();
            (msg as ITextSerializer).LoadData("");
        });

        MyAssert.IsError<InvalidDataException>(() => {
            ComplexMessage<string> msg = new ComplexMessage<string>();
            (msg as ITextSerializer).LoadData("xxxxxxxxxxxxxxxxxxxxxx");
        });


        MyAssert.IsError<ArgumentNullException>(() => {
            ComplexMessage<string> msg = new ComplexMessage<string>();
            ReadOnlyMemory<byte> body = new byte[0];
            (msg as IBinarySerializer).LoadData(body);

        });
    }

}
#endif
