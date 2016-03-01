using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ClownFish.Base;
using ClownFish.Base.Xml;
using ClownFish.Web;
using ClownFish.Web.UnitTest.Models;

namespace ClownFish.Web.UnitTest.Controllers
{
	public class ComplexDataTypeService : BaseController
	{
		[Action(OutFormat = SerializeFormat.Auto)]
		public Product Input_Product(Product p)
		{
			p.ProductID++;
			p.CategoryID++;
			p.Quantity++;
			p.ProductName += "_test";
			return p;
		}

		public int Input_JSON(string json)
		{
			Product p = JsonExtensions.FromJson<Product>(json);
			return p.ProductID;
		}

		public int Input_XML(string xml)
		{
			Product p = XmlExtensions.FromXml<Product>(xml);
			return p.ProductID;
		}

		public int Input_XmlDocument(XmlDocument xmlDocument)
		{
			string xml = xmlDocument.OuterXml;
			return Input_XML(xml);
		}


		[Action(OutFormat=SerializeFormat.Json)]
		public Product Input_2_Product(Product p1, Product p2)
		{
			Product p = new Product();
			p.CategoryID = p1.CategoryID + p2.CategoryID;
			p.ProductID = p1.ProductID + p2.ProductID;
			p.ProductName = p1.ProductName + p2.ProductName;
			p.Quantity = p1.Quantity + p2.Quantity;
			p.Remark = p1.Remark + p2.Remark;
			p.Unit = p1.Unit + p2.Unit;
			p.UnitPrice = p1.UnitPrice + p2.UnitPrice;
			return p;
		}

		public JsonResult Input_int_Product(int a, Product p)
		{
			p.ProductID++;
			p.CategoryID++;
			//p.Quantity++;
			p.ProductName += "_test";
			p.Quantity += a;
			return new JsonResult(p);
		}

	}
}
