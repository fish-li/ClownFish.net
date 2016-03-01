using System;
using System.Collections.Generic;
using System.Linq;
using DEMO.Model;
using ClownFish;


namespace DEMO.BLL
{

	public sealed class CategorySqlBLL : ICategoryBLL
	{
		public void Insert(Category category)
		{
			DbHelper.ExecuteNonQuery("InsertCategory", category);
		}

		public void Delete(int categoryId)
		{
			var parameters = new { ProductID = categoryId };
			DbHelper.ExecuteNonQuery("DeleteCategory", parameters);
		}

		public void Update(Category category)
		{
			DbHelper.ExecuteNonQuery("UpdateCategory", category);
		}

		public List<Category> GetList()
		{
			return DbHelper.FillList<Category>("GetCategories", null);
		}

	}
}
