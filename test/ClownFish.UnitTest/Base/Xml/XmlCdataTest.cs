namespace ClownFish.UnitTest.Base.Serializer;

public class Xcdata
{
    public int A { get; set; }

    public XmlCdata Text { get; set; }
}


[TestClass]
public class XmlCdataTest
{
    [TestMethod]
    public void Test()
    {
        Xcdata data = new Xcdata();
        data.Text = "abc";
        Assert.AreEqual("abc", data.Text.Value);
        Assert.AreEqual("abc", data.Text.ToString());

        data.Text.Value = "12345";
        Assert.AreEqual("12345", data.Text.Value);
        Assert.AreEqual("12345", data.Text.ToString());

        Assert.IsFalse(data.Text.Equals((XmlCdata)null));

        Assert.IsFalse(data.Text.Equals((object)null));
        Assert.IsFalse(data.Text.Equals(123));

        data.Text.Value = null;
        Assert.IsTrue(data.Text.GetHashCode() > 0);

        XmlCdata a = new XmlCdata();
        XmlCdata a2 = new XmlCdata();
        XmlCdata b = a;
        XmlCdata c = null;
        XmlCdata d = null;
        XmlCdata e = new XmlCdata() { Value = "abc" };

        Assert.IsTrue(a == b);
        Assert.IsTrue(a == a2);

        Assert.IsFalse(a == c);
        Assert.IsFalse(c == b);
        Assert.IsTrue(c == d);

        Assert.IsTrue(a != e);
    }

    [TestMethod]
    public void Test_XmlCdata_ToXml_FromXml()
    {
        Xcdata m = new Xcdata { A = 2, Text = "123456789" };

        string xml = m.ToXml();

        Xcdata m2 = xml.FromXml<Xcdata>();

        Assert.AreEqual(2, m2.A);
    }


    [TestMethod]
    public void Test_XmlCdata_implicit()
    {
        XmlCdata data = "abc";      // 隐式类型转换
        Assert.AreEqual("abc", data.Value);
        Assert.AreEqual("abc", data.ToString());

        var schema = (data as IXmlSerializable).GetSchema();
        Assert.IsNull(schema);

        string s = data;
        Assert.AreEqual("abc", s);
    }

    [TestMethod]
    public void Test_XmlCdata_Equals()
    {
        XmlCdata data = "abc";      // 隐式类型转换
        XmlCdata data2 = new XmlCdata("abc");

        Assert.IsTrue(data == data2);
        Assert.IsTrue(data.Equals(data2));

        object a = data;
        object b = data2;
        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(a.GetHashCode() == b.GetHashCode());

        XmlCdata data3 = "ab";
        Assert.IsTrue(data != data3);
    }

}
