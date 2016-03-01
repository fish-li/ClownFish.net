

namespace DEMO.Model
{
	/// <summary>
	/// 表示一个商品对象的实体类
	/// </summary>
	public sealed class Product : MyDataItem
	{
		public int ProductID { get; set; }
		public string ProductName { get; set; }
		public int CategoryID { get; set; }
		public string Unit { get; set; }
		public decimal UnitPrice { get; set; }
		public int Quantity { get; set; }
		public string Remark { get; set; }

		public override string IsValid()
		{
			if( string.IsNullOrEmpty(ProductName) )
				return "商品名称不能为空。";

			if( string.IsNullOrEmpty(Unit) )
				return "单位不能为空。";

			return null;
		}
	}


}
