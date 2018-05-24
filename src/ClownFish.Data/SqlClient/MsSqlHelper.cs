using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace ClownFish.Data.SqlClient
{

    // 如果直接取名 SqlHelper 担心会影响其它项目中的已有代码，
    // 因为这个名字被用滥了，所以为了避免名称冲突，这个类型取名 MsSqlHelper

    /// <summary>
    /// SQLSERVER相关的工具类
    /// </summary>
	public static class MsSqlHelper
	{
        #region 常用SQL脚本定义

        /// <summary>
        /// SQL脚本，用于获取某个SQLSERVER实例的数据库清单
        /// </summary>
        public static readonly string ScriptGetDataBaseNames = @"
SELECT dtb.name AS [Database_Name] FROM master.sys.databases AS dtb 
WHERE (CAST(case when dtb.name in ('master','model','msdb','tempdb') then 1 else dtb.is_distributor end AS bit)=0 
		and CAST(isnull(dtb.source_database_id, 0) AS bit)=0) 
ORDER BY [Database_Name] ASC";

        /// <summary>
        /// SQL脚本，用于获取某个数据库的数据表清单
        /// </summary>
        public static readonly string ScriptGetTableNames = @"
select name from ( SELECT obj.name AS [Name],  
	CAST( case when obj.is_ms_shipped = 1 then 1     
			when ( select major_id from sys.extended_properties          
					where major_id = obj.object_id and  minor_id = 0 and class = 1 and name = N'microsoft_database_tools_support')          
			is not null then 1  else 0 end  AS bit) AS [IsSystemObject] 
	FROM sys.all_objects AS obj where obj.type in (N'U') ) as tables 
	where [IsSystemObject] = 0 ORDER BY [Name] ASC";

        /// <summary>
        /// SQL脚本，用于获取某个数据库的视图清单
        /// </summary>
        public static readonly string ScriptGetViewNames = @"
SELECT name 
FROM sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id AND type = 'V' 
where name not like 'sp_%' order by name";

        /// <summary>
        /// SQL脚本，用于获取某个数据库的自定义函数清单
        /// </summary>
        public static readonly string ScriptGetFunctionNames = @"
SELECT name 
FROM sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id AND type in (N'TF', N'FN')
order by name";

        /// <summary>
        /// SQL脚本，用于获取某个数据库的存储过程清单
        /// </summary>
        public static readonly string ScriptGetStoreProcedureNames = @"
SELECT name 
FROM sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id AND type = 'P' 
where name not like 'sp_%' order by name";

        /// <summary>
        /// SQL脚本，用于获取某个数据表的列定义描述
        /// </summary>
        public static readonly string ScriptGetTableFields = @"
SELECT clmns.name AS [Name], baset.name AS [DataType], 
		CAST(CASE WHEN baset.name IN (N'nchar', N'nvarchar') AND clmns.max_length <> -1 
			THEN clmns.max_length/2 ELSE clmns.max_length END AS int) AS [Length], clmns.scale,
		CAST(clmns.precision AS int) AS [Precision], clmns.is_identity AS [Identity], 
		clmns.is_nullable AS [Nullable] ,clmns.is_computed as [Computed],cmc.is_persisted as [IsPersisted] ,
		defCst.definition as [DefaultValue], cmc.definition as [Formular],
		idc.seed_value as [SeedValue], idc.increment_value as [IncrementValue]
FROM sys.tables AS tbl 
INNER JOIN sys.all_columns AS clmns ON clmns.object_id=tbl.object_id 
LEFT OUTER JOIN sys.types AS baset ON baset.user_type_id = clmns.system_type_id and baset.user_type_id = baset.system_type_id 
LEFT OUTER JOIN sys.schemas AS sclmns ON sclmns.schema_id = baset.schema_id 
LEFT OUTER JOIN sys.identity_columns AS ic ON ic.object_id = clmns.object_id and ic.column_id = clmns.column_id 
left outer join sys.default_constraints defCst on defCst.parent_object_id = clmns.object_id and defCst.parent_column_id = clmns.column_id 
left outer join sys.computed_columns cmc on cmc.object_id = clmns.object_id and cmc.column_id = clmns.column_id 
left outer join sys.identity_columns idc on idc.object_id = clmns.object_id and idc.column_id = clmns.column_id 
WHERE (tbl.name= @TableName ) ORDER BY clmns.column_id ASC";

        /// <summary>
        /// SQL脚本，用于获取某个数据表的主键字段
        /// </summary>
        public static readonly string ScriptGetTablePrimaryKey = @"
select col.name as column_nName 
from sys.indexes ind 
left outer join (sys.index_columns ind_col inner join sys.columns col on col.object_id = ind_col.object_id and col.column_id = ind_col.column_id )  on ind_col.object_id = ind.object_id and ind_col.index_id = ind.index_id 
where ind.object_id = object_id( @TableName )  and ind.index_id >= 0 and ind.type = 1 
and ind.is_hypothetical = 0   order by ind.index_id, ind_col.key_ordinal
";

        /// <summary>
        /// SQL脚本，用于获取某个存储过程的定义脚本
        /// </summary>
        public static readonly string ScriptGetProcedureSQL = @"
select definition 
from sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id  AND type = 'P' 
where name = @ObjectName";

        /// <summary>
        /// SQL脚本，用于获取某个视图的定义脚本
        /// </summary>
        public static readonly string ScriptGetViewSQL = @"
select definition 
FROM sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id AND type = N'V' 
where name = @ObjectName";

        /// <summary>
        /// SQL脚本，用于获取某个自定义函数的定义脚本
        /// </summary>
        public static readonly string ScriptGetFunctionSQL = @"
select definition 
FROM sys.sql_modules JOIN sys.objects ON sys.sql_modules.object_id = sys.objects.object_id AND type in (N'TF', N'FN')
where name = @ObjectName";


        /// <summary>
        /// SQL脚本，获取数据库的所有数据表的统计信息
        /// </summary>
        public static readonly string ScriptGetTableStatisticalInformation = @"
SELECT Object_schema_name(p.object_id) AS [Schema],  
       Object_name(p.object_id)        AS [Table],  
       i.name                          AS [Index],  
       p.partition_number,  
       p.rows                          AS [RowCount],  
       i.type_desc                     AS [IndexType]  
FROM   sys.partitions p  
       INNER JOIN sys.indexes i  
               ON p.object_id = i.object_id  
                  AND p.index_id = i.index_id  
WHERE  Object_schema_name(p.object_id) <> 'sys'  
     --  AND Object_name(p.object_id) = 'table_1' --获取某个表  
ORDER  BY [Schema],  
          [Table],  
          [Index]  
";

		#endregion


		/// <summary>
		/// 测试连接字符串是否有效
		/// </summary>
		/// <param name="connectionString">数据库连接字符串。</param>
		/// <param name="connectTimeout">连接的超时时间，单位：秒</param>
		/// <returns></returns>
		public static string TestConnection(string connectionString, int connectTimeout = 0)
		{
			// 提示：
			// 为了快速检查连接是否有效，应该在连接字符串中指定【连接超时时间】，
			// 或者通过参数 connectTimeout 来指定，避免长时间等待


			// 如果通过参数指定了超时时间，就修改连接字符串
			if( connectTimeout > 0 ) {
				SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
				builder.ConnectTimeout = connectTimeout;

				connectionString = builder.ToString();
			}

			string sql = "select getdate() as time1";

			using( DbContext db = CreateContext(connectionString) ) {
				return db.CPQuery.Create(sql)
								.SetCommand(x => x.CommandTimeout = 3)
								.ExecuteScalar<string>();
			}
		}


		/// <summary>
		/// 创建DbContext实例
		/// </summary>
		/// <param name="connectionString">数据库连接字符串。</param>
		/// <param name="database">数据库名称。可以不指定，不指定时使用connectionString中的数据库。</param>
		/// <returns></returns>
		public static DbContext CreateContext(string connectionString, string database = null)
        {
			if( string.IsNullOrEmpty(connectionString) )
				throw new ArgumentNullException(nameof(connectionString));

            if( string.IsNullOrEmpty(database) )
                return DbContext.Create(connectionString, "System.Data.SqlClient");


            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = database;

            return DbContext.Create(builder.ToString(), "System.Data.SqlClient");
        }


		/// <summary>
		/// 获取SQLSERVER版本号
		/// </summary>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static int GetVersion(this DbContext dbContext)
        {
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));

			string query = "select (@@microsoftversion / 0x01000000);";
			return dbContext.CPQuery.Create(query).ExecuteScalar<int>();
		}




		/// <summary>
		/// 获取数据表的所有列定义
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="tablename"></param>
		/// <returns></returns>
		public static List<DbField> GetTableFields(this DbContext dbContext, string tablename)
		{
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));
			if( string.IsNullOrEmpty(tablename) )
				throw new ArgumentNullException(nameof(tablename));

			var parameter = new { TableName = tablename };
			return dbContext.CPQuery.Create(ScriptGetTableFields, parameter).ToList<DbField>();
		}

		/// <summary>
		/// 获取SQL查询结果的列定义
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static List<DbField> GetQueryFields(this DbContext dbContext, string query)
		{
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));
			if( string.IsNullOrEmpty(query) )
				throw new ArgumentNullException(nameof(query));

			List<DbField> list = new List<DbField>();

			CPQuery cpquery = dbContext.CPQuery.Create(query);

			using( SqlDataReader reader = (SqlDataReader)cpquery.ExecuteReader() ) {
				for( int i = 0; i < reader.FieldCount; i++ ) {
					list.Add(new DbField {
						Name = reader.GetName(i),
						DataType = reader.GetFieldType(i).ToString(),
						Nullable = false
					});
				}
			}

			foreach( DbField f in list )
				if( string.IsNullOrEmpty(f.Name) )
					f.Name = "NoneName";

			return list;
		}



		/// <summary>
		/// 获取SQLSERVER实例的数据库名称列表
		/// </summary>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static List<string> GetDataBaseNames(this DbContext dbContext)
		{
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));

			return dbContext.CPQuery.Create(ScriptGetDataBaseNames).ToScalarList<string>();
		}

		/// <summary>
		/// 获取某个数据库的数据表清单
		/// </summary>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static List<string> GetTableNames(this DbContext dbContext)
		{
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));

			return dbContext.CPQuery.Create(ScriptGetTableNames).ToScalarList<string>();
		}

		/// <summary>
		/// 获取某个数据库的视图清单
		/// </summary>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static List<string> GetViewNames(this DbContext dbContext)
		{
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));

			return dbContext.CPQuery.Create(ScriptGetViewNames).ToScalarList<string>();
		}

		/// <summary>
		/// 获取某个数据库的存储过程清单
		/// </summary>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static List<string> GetSpNames(this DbContext dbContext)
		{
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));

			return dbContext.CPQuery.Create(ScriptGetStoreProcedureNames).ToScalarList<string>();
		}



		/// <summary>
		/// 获取某个存储过程的定义脚本
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetProcedureCode(this DbContext dbContext, string name)
		{
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));
			if( string.IsNullOrEmpty(name) )
				throw new ArgumentNullException(nameof(name));

			var parameter = new { ObjectName = name };
			return dbContext.CPQuery.Create(ScriptGetProcedureSQL, parameter).ExecuteScalar<string>();
		}

		/// <summary>
		/// 获取某个视图的定义脚本
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetViewCode(this DbContext dbContext, string name)
		{
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));
			if( string.IsNullOrEmpty(name) )
				throw new ArgumentNullException(nameof(name));

			var parameter = new { ObjectName = name };
			return dbContext.CPQuery.Create(ScriptGetViewSQL, parameter).ExecuteScalar<string>();
		}



		/// <summary>
		/// 获取数据库的所有数据表的统计信息
		/// </summary>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static DataTable GetTablesStatisticalInformation(this DbContext dbContext)
        {
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));

			return dbContext.CPQuery.Create(ScriptGetTableStatisticalInformation).ToDataTable();
		}


		/// <summary>
		/// 检验SQL语句的语法正确性
		/// </summary>
		/// <param name="dbContext"></param>
		/// <param name="sql"></param>
		/// <returns></returns>
		public static string ValidSql(this DbContext dbContext, string sql)
        {
			if( dbContext == null )
				throw new ArgumentNullException(nameof(dbContext));
			if( string.IsNullOrEmpty(sql) )
				throw new ArgumentNullException(nameof(sql));

			dbContext.CPQuery.Create("SET PARSEONLY ON").ExecuteNonQuery();

			try {
				dbContext.CPQuery.Create(sql).ExecuteNonQuery();
				return null;
			}
			catch( Exception ex ) {
				return ex.Message;
			}
			finally {
				dbContext.CPQuery.Create("SET PARSEONLY OFF").ExecuteNonQuery();
			}
		}


        /// <summary>
        /// 判断一个数据库异常是否由于重复插入导致的，由唯一索引控制。
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsDuplicateInsert(this Exception ex)
        {
            if( ex == null )
                throw new ArgumentNullException(nameof(ex));


            Exception baseException = ex.GetBaseException();

            SqlException sqlException = baseException as SqlException;
            if( sqlException != null )
                return (sqlException.Number == 2601 || sqlException.Number == 2627);


            // select * from master.dbo.sysmessages where error = 2601
            // error	severity	dlevel	description	                                                        msglangid
            // 2601	    14	        0	    不能在具有唯一索引“%2!”的对象“%1!”中插入重复键的行。重复键值为 %3!。	2052
            // For example: 不能在具有唯一索引“IX_Table1_IntValue”的对象“dbo.Table1”中插入重复键的行。重复键值为 (31)。


            // select * from master.dbo.sysmessages where error = 2627
            // error	severity	dlevel	description	                                                        msglangid
            // 2601	    14	        0	    违反了 %1! 约束“%2!”。不能在对象“%3!”中插入重复键。重复键值为 %4!。 	2052
            // For example: 违反了 PRIMARY KEY 约束“PK_HtTypeGUID”。不能在对象“dbo.cb_HtType”中插入重复键。重复键值为 (c4e4facd-e4e3-4b04-9755-00cdbdc653e5)。


            return false;
        }


    }
}
