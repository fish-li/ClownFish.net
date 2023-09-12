﻿using ClownFish.UnitTest.Data.Models;

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
}
#endif
