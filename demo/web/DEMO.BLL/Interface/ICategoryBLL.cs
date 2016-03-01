using System;

namespace DEMO.BLL
{
	public interface ICategoryBLL
	{
		void Delete(int categoryId);
		System.Collections.Generic.List<DEMO.Model.Category> GetList();
		void Insert(DEMO.Model.Category category);
		void Update(DEMO.Model.Category category);
	}
}
