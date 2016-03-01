using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using ClownFish.Web;
using DEMO.Model.AjaxPK;
using ClownFish.Base.Xml;




// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html

namespace DEMO.Controllers.AjaxPK
{
	public class DemoPkService
	{
		[Action(Verb = "post")]
		public int Add(int a, int b)
		{
			return a + b;
		}

		[Action(Verb = "get")]
		public int Add(int a, int b, VoidType c)
		{
			return a + b + 10;
		}


		[Action]
		public string AddCustomer(Customer customer)
		{
			// 简单地返回一个XML字符串。
			// 告诉客户端：服务端收到了什么样的数据。
			return XmlHelper.XmlSerialize(customer, Encoding.UTF8);
		}


		[Action]
		public string BatchAddCustomer(BatchSubmitCustomer input)
		{
			List<Customer> list = new List<Customer>();
			for( int i = 0; i < input.Name.Length; i++ ) {
				if( input.Name[i].Length > 0 && TryToInt(input.Age[i]) > 0 && input.Tel[i].Length > 0 ) {
					Customer c = new Customer {
						Name = input.Name[i],
						Age = TryToInt(input.Age[i]),
						Address = input.Address[i],
						Tel = input.Tel[i],
						Email = input.Email[i]
					};
					list.Add(c);
				}
			}

			return XmlHelper.XmlSerialize(list, Encoding.UTF8);
		}


		[Action]
		public string BatchAddCustomer2(string[] name, string[] age, string[] address, string[] tel, string[] email)
		{
			return BatchAddCustomer(new BatchSubmitCustomer {
				Name = name,
				Address = address,
				Age = age,
				Email = email,
				Tel = tel
			});
		}


		private static int TryToInt(string s)
		{
			int value = 0;
			int.TryParse(s, out value);
			return value;
		}


		[Action]
		public string AddProduct(Product product)
		{
			// 简单地返回一个XML字符串。
			// 告诉客户端：服务端收到了什么样的数据。
			return XmlHelper.XmlSerialize(product, Encoding.UTF8);
		}
	}

}

