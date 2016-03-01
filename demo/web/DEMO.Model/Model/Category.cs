
namespace DEMO.Model
{
	/// <summary>
	/// 表示一个商品分类记录
	/// </summary>
	public sealed class Category : MyDataItem
	{
		public int CategoryID { get; set; }
		public string CategoryName { get; set; }

		public override string IsValid()
		{
			if( string.IsNullOrEmpty(this.CategoryName) )
				return "商品分类名称不能为空。";

			return null;
		}


	}
}