using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DEMO.Model;

namespace DEMO.BLL
{

	public sealed class CategoryXmlBLL : ICategoryBLL
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Insert(Category category)
		{
			int maxId = WebSiteDB.MyNorthwind.Categories.Max(x => x.CategoryID);
			category.CategoryID = maxId + 1;
			WebSiteDB.MyNorthwind.Categories.Add(category);
		}

		public void Delete(int categoryId)
		{
			WebSiteDB.MyNorthwind.Categories = (
				from c in WebSiteDB.MyNorthwind.Categories
				where c.CategoryID != categoryId
				select c).ToList();
		}

		public void Update(Category category)
		{
			Category dest = WebSiteDB.MyNorthwind.Categories.FirstOrDefault(
								c => c.CategoryID == category.CategoryID);
			if( dest != null )
				dest.CategoryName = category.CategoryName;
		}


		public List<Category> GetList()
		{
			return WebSiteDB.MyNorthwind.Categories;
		}

	}
}
