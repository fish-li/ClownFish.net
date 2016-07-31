using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.UnitTest.Models
{
	[Serializable]
	public class Customer : Entity
	{
		public virtual int CustomerID { get; set; }
		public virtual string CustomerName { get; set; }
		public virtual string ContactName { get; set; }
		public virtual string Address { get; set; }
		public virtual string PostalCode { get; set; }
		public virtual string Tel { get; set; }
	}

}
