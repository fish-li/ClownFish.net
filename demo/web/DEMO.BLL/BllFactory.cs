using System;
using System.Configuration;


namespace DEMO.BLL
{
	enum DataSoureceKind
	{
		XmlFile,
		StoreProcedure,
		XmlCommand
	}

	/// <summary>
	/// 创建BLL的一个简单的工厂。
	/// </summary>
	public static class BllFactory
	{
		private static DataSoureceKind s_kind;

		static BllFactory()
		{
			string kind = ConfigurationManager.AppSettings["DataSoureceKind"];

			// 这里不用parse方法，以免配置错误造成程序不能运行。
			if( kind == "StoreProcedure" )
				s_kind = DataSoureceKind.StoreProcedure;
			else if( kind == "XmlCommand" )
				s_kind = DataSoureceKind.XmlCommand;
			else
				s_kind = DataSoureceKind.XmlFile;
		}

		public static ICategoryBLL GetCategoryBLL()
		{
			if( s_kind == DataSoureceKind.XmlFile )
				return new CategoryXmlBLL();

			// 说明：对于StoreProcedure和XmlCommand来说，C#代码是一样的。
			return new CategorySqlBLL();
		}

		public static ICustomerBLL GetCustomerBLL()
		{
			if( s_kind == DataSoureceKind.XmlFile )
				return new CustomerXmlBLL();

			return new CustomerSqlBLL();
		}

		public static IProductBLL GetProductBLL()
		{
			if( s_kind == DataSoureceKind.XmlFile )
				return new ProductXmlBLL();

			return new ProductSqlBLL();
		}

		public static IOrderBLL GetOrderBLL()
		{
			if( s_kind == DataSoureceKind.XmlFile )
				return new OrderXmlBLL();

			return new OrderSqlBLL();
		}

		public static IDatabase GetDatabase()
		{
			if( s_kind == DataSoureceKind.StoreProcedure )
				return new SqlServerDb();

			if( s_kind == DataSoureceKind.XmlCommand )
				return new SqlServerDb2();

			return new XmlDb();
		}
	}
}
