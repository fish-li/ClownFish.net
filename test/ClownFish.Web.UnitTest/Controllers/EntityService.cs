using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Controllers
{
	[Serializable]
	[DbEntity(Alias = "Products")]
	public class ProductX : Entity
	{
		[DbColumn(PrimaryKey = true)]
		public virtual int ProductID { get; set; }
		public virtual string ProductName { get; set; }
		public virtual int CategoryID { get; set; }
		public virtual string Unit { get; set; }
		public virtual decimal UnitPrice { get; set; }
		public virtual string Remark { get; set; }
		public virtual int Quantity { get; set; }
	}


	public class EntityService : BaseController
	{
		public string Action1([EntityProxy]ProductX p)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine();
			sb.AppendLine(p.GetType().FullName);

			string[] names = (p as IEntityProxy).GetChangeNames();
			object[] values = (p as IEntityProxy).GetChangeValues();

			Assert.AreEqual(names.Length, values.Length);

			for( int i = 0; i < names.Length; i++ )
				sb.AppendFormat("{0}={1}\r\n", names[i], values[i]);


			// 执行数据库操作
			p.Update();


			sb.AppendLine()
				.Append(ClownFish.Data.UnitTest.ClownFishDataEventSubscriber.LastExecuteSQL);

			return sb.ToString();

		}
	}
}
