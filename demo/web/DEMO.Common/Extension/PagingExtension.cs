using System;
using System.Collections.Generic;
using System.Linq;
using ClownFish;

namespace DEMO.Common
{
	public static class PagingExtension
	{
		public static List<T> GetPagingList<T>(this IQueryable<T> query, PagingInfo pagingInfo)
		{
			if( query == null )
				throw new ArgumentNullException("query");

			pagingInfo.TotalRecords = query.Count();

			List<T> list = query.Skip(pagingInfo.PageIndex * pagingInfo.PageSize).Take(pagingInfo.PageSize).ToList();

			if( list == null || list.Count == 0 ) {
				if( pagingInfo.PageIndex > 0 && pagingInfo.TotalRecords > 0 ) {
					pagingInfo.PageIndex = 0;
					list = query.Skip(pagingInfo.PageIndex * pagingInfo.PageSize).Take(pagingInfo.PageSize).ToList();
				}
			}

			return list;
		}

		public static void CheckPagingInfoState(this PagingInfo pagingInfo)
		{
			if( pagingInfo.PageIndex > 0 )
				pagingInfo.PageIndex--;

			if( pagingInfo.PageSize < 1 )
				pagingInfo.PageSize = AppHelper.DefaultPageSize;
		}
	}


}