
namespace DEMO.Model
{
	/// <summary>
	/// 表示一个客户对象的实体类
	/// </summary>
	public sealed class Customer : MyDataItem
	{
		public int CustomerID { get; set; }
		public string CustomerName { get; set; }
		public string ContactName { get; set; }
		public string Address { get; set; }
		public string PostalCode { get; set; }
		public string Tel { get; set; }

		public override string IsValid()
		{
			if( string.IsNullOrEmpty(CustomerName) )
				return "客户名称不能为空。";

			return null;
		}
	}
}