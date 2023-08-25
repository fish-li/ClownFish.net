using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Xml
{
	public class TXmlObject
	{
		public int A { get; set; }

		public string B { get; set; }
	}

	[TestClass]
	public class XmlHelperTest
	{
		[TestMethod]
		public void Test1()
        {
			TXmlObject obj = new TXmlObject {
				A = 5,
				B = "Fish Li"
			};

			string xml = obj.ToXml();

			TXmlObject obj2 = xml.FromXml<TXmlObject>();
			Assert.AreEqual(obj.A, obj2.A);
			Assert.AreEqual(obj.B, obj2.B);

			TXmlObject obj3 = (TXmlObject)xml.FromXml(typeof(TXmlObject));
			Assert.AreEqual(obj.A, obj3.A);
			Assert.AreEqual(obj.B, obj3.B);


			string filename = "temp/test_xmlhelper.xml";
			XmlHelper.XmlSerializeToFile(obj, filename);
			Assert.IsTrue(File.Exists(filename));

			TXmlObject obj4 = XmlHelper.XmlDeserializeFromFile<TXmlObject>(filename);
			Assert.AreEqual(obj.A, obj4.A);
			Assert.AreEqual(obj.B, obj4.B);

			//File.Delete(filename);
		}


		[TestMethod]
		public void Test_XmlSerializerObject()
		{
			MyAssert.IsError<ArgumentNullException>(() => {
				_ = XmlHelper.XmlSerializerObject(null);
			});

			TXmlObject obj = new TXmlObject {
				A = 5,
				B = "Fish Li"
			};
			string result = XmlHelper.XmlSerializerObject(obj);
			Console.WriteLine(result);

			string xml = @"
<TXmlObject>
    <A>5</A>
    <B>Fish Li</B>
</TXmlObject>";

			Assert.AreEqual(TrimString(xml), TrimString(result));			
		}


		[TestMethod]
		public void Test_Error()
        {
			MyAssert.IsError<ArgumentNullException>(() => {
				_ = XmlHelper.XmlSerialize(null, Encoding.UTF8);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				_ = XmlHelper.XmlSerialize("xx", null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				XmlHelper.XmlSerializeToFile("xx", null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				_ = XmlHelper.XmlDeserialize((Stream)null, typeof(TXmlObject));
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				XmlHelper.XmlDeserialize(new MemoryStream(), (Type)null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				_= XmlHelper.XmlDeserialize(string.Empty, typeof(TXmlObject), Encoding.UTF8);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				_ = XmlHelper.XmlDeserialize("xx", (Type)null, Encoding.UTF8);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				_ = XmlHelper.XmlDeserialize("xx", typeof(TXmlObject), (Encoding)null);
			});

			MyAssert.IsError<ArgumentNullException>(() => {
				_ = XmlHelper.XmlDeserializeFromFile<TXmlObject>(string.Empty);
			});

			MyAssert.IsError<InvalidDataException>(() => {
				_ = XmlHelper.XmlDeserializeFromFile<TXmlObject>("xx.xml");
			});
		}

		private string TrimString(string s)
		{
			if( string.IsNullOrEmpty(s) )
				return s;

			return s.Replace("\r", "").Replace("\n", "").Replace(" ", "").Replace("\t", "");
		}
	}
}
