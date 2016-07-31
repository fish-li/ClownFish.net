using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.PerformanceTest
{
	public class OrderInfo : Entity
	{
		public virtual int OrderID { get; set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual decimal SumMoney { get; set; }
        public virtual string Comment { get; set; }
        public virtual bool Finished { get; set; }
        public virtual int ProductID { get; set; }
        public virtual decimal UnitPrice { get; set; }
        public virtual int Quantity { get; set; }
        public virtual string ProductName { get; set; }
        public virtual int CategoryID { get; set; }
        public virtual string Unit { get; set; }
        public virtual string Remark { get; set; }

        // 注意：客户信息有可能会是DBNull
        public virtual int? CustomerID { get; set; }
        public virtual string CustomerName { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string Address { get; set; }
        public virtual string PostalCode { get; set; }
        public virtual string Tel { get; set; }
	}
}
