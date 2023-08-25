//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace ClownFish.Data.SqlClient
//{
//	/// <summary>
//	/// SQLSERVER 2005 分页工具类
//	/// </summary>
//	public static class Sql2005Paging
//    {
//		// 匹配字符串：") as RowIndex,"
//		private static readonly Regex s_pagingRegex = new Regex(@"\)\s*as\s*rowindex\s*,", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);


//		private static void CreatePagedQuery(XmlCommand xmlCommand, PagingInfo pageInfo
//			, out CPQuery query1
//			, out CPQuery query2)
//		{
//			//--需要配置的SQL语句
//			//select row_number() over (order by UpCount asc) as RowIndex, 
//			//    Title, Tag, [Description], Creator, CreateTime, UpCount, ReadCount, ReplyCount
//			//from   CaoItem
//			//where CreateTime < @CreateTime

//			//--在运行时，将会生成下面二条SQL

//			//select * from (
//			    // ------------------ 原SQL语句 ------------------
//			    //select row_number() over (order by UpCount asc) as RowIndex, 
//			    //    Title, Tag, [Description], Creator, CreateTime, UpCount, ReadCount, ReplyCount
//			    //from   CaoItem
//			    //where CreateTime < @CreateTime
//			    // ------------------ 原SQL语句 ------------------
//			//) as t1
//			//where  RowIndex > (@PageSize * @PageIndex) and RowIndex <= (@PageSize * (@PageIndex+1))


//			//select  count(*) from   ( select 
//			    // ------------------ 原SQL语句 ------------------
//			    // ----- 去掉 select row_number() over (order by UpCount asc) as RowIndex,
//			    //    Title, Tag, [Description], Creator, CreateTime, UpCount, ReadCount, ReplyCount
//			    //from   CaoItem as p 
//			    //where CreateTime < @CreateTime
//			    // ------------------ 原SQL语句 ------------------
//			//) as t1

//			if( xmlCommand == null )
//				throw new ArgumentNullException("xmlCommand");
//			if( pageInfo == null )
//				throw new ArgumentNullException("pageInfo");

//            // 为了方便得到 count 的语句，先直接定位 ") as RowIndex," 
//            // 然后删除这之前的部分，将 select  count(*) from   (select 加到SQL语句的前面。
//            // 所以，这里就检查SQL语句是否符合要求。

//            //string flag = ") as RowIndex,";
//            //int p = xmlCommandText.IndexOf(flag, StringComparison.OrdinalIgnoreCase);
//            //if( p <= 0 )
//            //    throw new InvalidOperationException("XML中配置的SQL语句不符合分页语句的要求。");

//            DbCommand dbCommand = xmlCommand.Command;
//			string commandText = dbCommand.CommandText;

//			Match match = s_pagingRegex.Match(commandText);
//			if( match.Success == false )
//				throw new InvalidOperationException("XML中配置的SQL语句不符合分页语句的要求。");

//			int p = match.Index;

//			// 获取当前命令的参数集合，给第一个CPQuery使用
//			DbParameter[] parameters1 = dbCommand.Parameters.Cast<DbParameter>().ToArray();

//			// 克隆参数数组，因为参数对象只能属于一个命令对象。
//			DbParameter[] parameters2 = dbCommand.CloneParameters();

//			// 断开参数对象与原命令的关联。
//			xmlCommand.Command.Parameters.Clear();


//			// 生成 SELECT 命令
//			string selectCommandText = string.Format(@"select * from ( {0} ) as t1 
//where  RowIndex > (@PageSize * @PageIndex) and RowIndex <= (@PageSize * (@PageIndex+1))", commandText);

//			// 生成第1个查询
//			query1 = xmlCommand.Context.CPQuery.Create(selectCommandText, parameters1);

//			DbParameter pageIndexParameter = xmlCommand.Command.CreateParameter();
//			pageIndexParameter.ParameterName = "@PageIndex";
//			pageIndexParameter.DbType = System.Data.DbType.Int32;
//			pageIndexParameter.Value = pageInfo.PageIndex;
//			query1.Command.Parameters.Add(pageIndexParameter);


//			DbParameter pageSizeParameter = xmlCommand.Command.CreateParameter();
//			pageSizeParameter.ParameterName = "@PageSize";
//			pageSizeParameter.DbType = System.Data.DbType.Int32;
//			pageSizeParameter.Value = pageInfo.PageSize;
//			query1.Command.Parameters.Add(pageSizeParameter);



//			// 生成 COUNT 命令
//			string getCountText = string.Format("select  count(*) from   (select {0}  ) as t1",
//							commandText.Substring(p + match.Length));

//			// 生成第2个查询
//			query2 = xmlCommand.Context.CPQuery.Create(getCountText, parameters2);
//		}

//		/// <summary>
//		/// 分页查询，返回实体列表
//		/// </summary>
//		/// <typeparam name="T">实体类型</typeparam>
//		/// <param name="command">XmlCommand实例引用</param>
//		/// <param name="pageInfo">分页信息</param>
//		/// <returns>实体集合</returns>
//		public static List<T> ToPageList<T>(XmlCommand command, PagingInfo pageInfo) where T : class, new()
//		{
//			CPQuery query1 = null;
//			CPQuery query2 = null;

//			CreatePagedQuery(command, pageInfo, out query1, out query2);

//			List<T> list = query1.ToList<T>();
//			pageInfo.TotalRows = query2.ExecuteScalar<int>();

//			return list;
//		}

//		/// <summary>
//		/// 分页查询，返回实体列表
//		/// </summary>
//		/// <typeparam name="T">实体类型</typeparam>
//		/// <param name="command">XmlCommand实例引用</param>
//		/// <param name="pageInfo">分页信息</param>
//		/// <returns>实体集合</returns>
//		public static async Task<List<T>> ToPageListAsync<T>(XmlCommand command, PagingInfo pageInfo) where T : class, new()
//		{
//			CPQuery query1 = null;
//			CPQuery query2 = null;

//			CreatePagedQuery(command, pageInfo, out query1, out query2);

//			List<T> list = await query1.ToListAsync<T>();
//			pageInfo.TotalRows = await query2.ExecuteScalarAsync<int>();

//			return list;
//		}

//		/// <summary>
//		/// 分页查询，返回DataTable
//		/// </summary>
//		/// <param name="command"></param>
//		/// <param name="pageInfo"></param>
//		/// <returns></returns>
//		public static DataTable ToPageTable(XmlCommand command, PagingInfo pageInfo)
//		{
//			CPQuery query1 = null;
//			CPQuery query2 = null;

//			CreatePagedQuery(command, pageInfo, out query1, out query2);

//			System.Data.DataTable table = query1.ToDataTable();
//			pageInfo.TotalRows = query2.ExecuteScalar<int>();

//			return table;
//		}


//		/// <summary>
//		/// 分页查询，返回DataTable
//		/// </summary>
//		/// <param name="command"></param>
//		/// <param name="pageInfo"></param>
//		/// <returns></returns>
//		public static async Task<DataTable> ToPageTableAsync(XmlCommand command, PagingInfo pageInfo)
//		{
//			CPQuery query1 = null;
//			CPQuery query2 = null;

//			CreatePagedQuery(command, pageInfo, out query1, out query2);

//			DataTable table = await query1.ToDataTableAsync();
//			pageInfo.TotalRows = await query2.ExecuteScalarAsync<int>();

//			return table;
//		}

//	}
//}
