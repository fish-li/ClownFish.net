namespace ClownFish.Data.Cleaning;

/// <summary>
/// 数据表清理执行工具
/// </summary>
public sealed class DataCleaner
{
    private readonly CleaningOption _option;
    private readonly DateTime _endTime;
    private readonly StringBuilder _logs = new StringBuilder();

    private readonly string _tableName;
    private readonly string _timeField;
    private string _batchId;

    /// <summary>
    /// 获取执行日志
    /// </summary>
    public string GetLogs() => _logs.ToString();

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="option"></param>
    public DataCleaner(CleaningOption option)
    {
        if( option == null )
            throw new ArgumentNullException(nameof(option));

        option.Validate();
        _option = option;
        _endTime = DateTime.Now.AddHours(0 - option.HoursAgo);

        using( DbContext dbContext = DbContext.Create(_option.DbConfig) ) {
            dbContext.EnableDelimiter = true;
            _tableName = dbContext.GetObjectFullName(_option.TableName);
            _timeField = dbContext.GetObjectFullName(_option.TimeFieldName);
        }
    }

    private void WriteLog(string subject, string message)
    {
        _logs.Append(DateTime.Now.ToTime23String()).Append(' ').Append(subject).Append(": ").AppendLine(message);
    }

    /// <summary>
    /// 执行清理动作
    /// </summary>
    /// <returns></returns>
    public long Execute()
    {
        int index = 0;
        long sum = 0;
        try {
            while( true ) {
                _batchId = (++index).ToString();

                DataTable dataTable = LoadData();

                if( dataTable.Rows.Count > 0 ) {
                    sum += DeleteData(dataTable);
                }

                if( dataTable.Rows.Count < _option.BatchRows )
                    break;
            }
        }
        catch( Exception ex ) {
            WriteLog("ERR1", ex.ToString());
            throw;
        }

        return sum;
    }

    private DataTable LoadData()
    {
        using( DbContext dbContext = DbContext.Create(_option.DbConfig) ) {
            dbContext.BeginTransaction(IsolationLevel.ReadUncommitted);

            string selectSQL = dbContext.DatabaseType == DatabaseType.SQLSERVER
                               ? $"select top {_option.BatchRows} {_timeField} from {_tableName}  where {_timeField} <= @endTime order by {_timeField}"
                               : $"select {_timeField} from {_tableName}  where {_timeField} <= @endTime order by {_timeField} limit {_option.BatchRows}";

            var queryArgs = new { endTime = _endTime };
            DataTable dataTable = dbContext.CPQuery.Create(selectSQL, queryArgs).SetTimeout(_option.DbTimeout).ToDataTable();

            dataTable.TableName = _option.TableName;

            WriteLog("LOAD", $"batch={_batchId}, load {dataTable.Rows.Count} rows");
            return dataTable;
        }
    }


    internal int DeleteData(DataTable dataTable)
    {
        // 注意：这里的 “删除操作” 影响的记录范围 可能会大于 dataTable.Rows.Count
        // 因为在同一时刻，程序可能产生了多条数据，LoadData方法中由于加了 limit，所以同一时刻的数据可能会被切分到不同批次
        // 所以按时间去删除，范围就会不一样。
        // 也有办法做到删除范围和 dataTable.Rows.Count 保持一致，那就是使用 IN 条件，但是那样做就很 2B 了！

        DateTime maxTime = (DateTime)Convert.ChangeType(dataTable.Rows[dataTable.Rows.Count - 1][_option.TimeFieldName], TypeCode.DateTime);

        // guid, string 类型的主键不方便用 “大于” 来做比较，所以只使用时间字段来过滤
        string query = $"delete from {_tableName} where {_timeField} <= @endTime";

        object queryArgs = new { endTime = maxTime };

        int count = ExecuteDelete(query, queryArgs);

        WriteLog("DELE", $"batch={_batchId}, delete {count} rows");
        return count;
    }



    private int ExecuteDelete(string query, object queryArgs)
    {
        Exception lastException = null;

        // 这里采用【重试】的方式执行删除，就是要尽量确保执行成功！
        for( int i = 0; i < _option.RetryCount; i++ ) {
            try {
                using( DbContext dbContext = DbContext.Create(_option.DbConfig) ) {
                    return dbContext.CPQuery.Create(query, queryArgs).SetTimeout(_option.DbTimeout).ExecuteNonQuery();
                }
            }
            catch( Exception ex ) {
                lastException = ex;
                WriteLog("ERR2", ex.ToString());

                System.Threading.Thread.Sleep(3_000);
            }
        }

        if( lastException != null )
            throw lastException;

        return 0;   // 无用的代码，不加不能通过编译！
    }


}
