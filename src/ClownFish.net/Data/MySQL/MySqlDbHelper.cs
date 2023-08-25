namespace ClownFish.Data.MySQL;

/// <summary>
/// MySQL操作的工具类
/// </summary>
public static class MySqlDbHelper
{

    /// <summary>
    /// 获取数据库列表
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="includeSysDb">是否包含MySQL系统内部库</param>
    /// <returns></returns>
    public static List<string> GetDatabases(DbContext dbContext, bool includeSysDb = false)
    {
        string sql = "SHOW DATABASES";
        //return dbContext.CPQuery.Create(sql).ToScalarList<string>();

        List<string> databases = dbContext.CPQuery.Create(sql).ToScalarList<string>();

        if( includeSysDb == false ) {
            databases.Remove("information_schema");
            databases.Remove("performance_schema");
            databases.Remove("mysql");
            databases.Remove("sys");
        }

        return databases;
    }

    private static readonly string s_sqlGetTables = "select TABLE_NAME from information_schema.tables where table_schema=SCHEMA() and  table_type='BASE TABLE' ORDER BY TABLE_NAME";

    /// <summary>
    /// 获取当前连接所在数据库的所有表名
    /// </summary>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public static List<string> GetTables(DbContext dbContext)
    {
        //string sql = "show TABLES";
        string sql = s_sqlGetTables;
        return dbContext.CPQuery.Create(sql).ToScalarList<string>();
    }


    /// <summary>
    /// 获取当前连接所在数据库的所有表名
    /// </summary>
    /// <param name="dbContext"></param>
    /// <returns></returns>
    public static async Task<List<string>>  GetTablesAsync(DbContext dbContext)
    {
        //string sql = "show TABLES";
        string sql = s_sqlGetTables;
        return await dbContext.CPQuery.Create(sql).ToScalarListAsync<string>();
    }


    private static readonly string s_sqlGetFields = $@"
select COLUMN_NAME as `Name`, DATA_TYPE as DataType, COLUMN_TYPE as `ColType`, IS_NULLABLE as `Nullable`, 
       COLUMN_KEY as `Key`, COLUMN_DEFAULT as `Default`, EXTRA as Extra, COLUMN_COMMENT as `Comment`
from information_schema.`COLUMNS` 
where TABLE_NAME = @table  and TABLE_SCHEMA = DATABASE() 
order by ordinal_position";

    /// <summary>
    /// 获取某个表的所有字段信息
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    public static List<MySqlDbField> GetFields(DbContext dbContext, string table)
    {
        // 参考链接：https://dev.mysql.com/doc/refman/8.0/en/information-schema-columns-table.html

        string sql = s_sqlGetFields;
        var args = new { table };

        return dbContext.CPQuery.Create(sql, args).ToList<MySqlDbField>();
    }


    /// <summary>
    /// 获取某个表的所有字段信息
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="table"></param>
    /// <returns></returns>
    public static async Task<List<MySqlDbField>> GetFieldsAsync(DbContext dbContext, string table)
    {
        // 参考链接：https://dev.mysql.com/doc/refman/8.0/en/information-schema-columns-table.html

        string sql = s_sqlGetFields;
        var args = new { table };

        return await dbContext.CPQuery.Create(sql, args).ToListAsync<MySqlDbField>();
    }

}
