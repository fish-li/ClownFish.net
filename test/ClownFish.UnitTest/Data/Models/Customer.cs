using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data.Models
{
	[Serializable]
	[DbEntity(Alias = "Customers")]
	public class Customer : Entity
	{
		[DbColumn(PrimaryKey = true, Identity = true)]
		public virtual int CustomerID { get; set; }
		public virtual string CustomerName { get; set; }
		public virtual string ContactName { get; set; }
		public virtual string Address { get; set; }
		public virtual string PostalCode { get; set; }
		public virtual string Tel { get; set; }
	}

}
