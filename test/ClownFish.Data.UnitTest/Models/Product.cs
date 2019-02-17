using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.UnitTest.Models
{
	[Serializable]
	[DbEntity(Alias = "Products")]
	public class Product : Entity
	{
		[DbColumn(PrimaryKey =true)]
		public virtual int ProductID { get; set; }
		public virtual string ProductName { get; set; }
		public virtual int CategoryID { get; set; }
		public virtual string Unit { get; set; }
		public virtual decimal UnitPrice { get; set; }
		public virtual string Remark { get; set; }
		public virtual int Quantity { get; set; }
	}



    /// <summary>
    /// 这个实体类型用于验证[DbColumn(Alias="xxxxxxxxxx")]
    /// </summary>
    public class Product2 : Entity
    {
        [DbColumn(Alias = "ProductID", PrimaryKey = true)]
        public virtual int PId { get; set; }

        [DbColumn(Alias = "ProductName")]
        public virtual string PName { get; set; }

        [DbColumn(Alias = "CategoryID")]
        public virtual int CID { get; set; }

        [DbColumn(Alias = "Unit")]
        public virtual string Unt { get; set; }

        [DbColumn(Alias = "UnitPrice")]
        public virtual decimal UPrice { get; set; }

        [DbColumn(Alias = "Remark")]
        public virtual string Remark2 { get; set; }

        [DbColumn(Alias = "Quantity")]
        public virtual int Quantity2 { get; set; }
    }

}
