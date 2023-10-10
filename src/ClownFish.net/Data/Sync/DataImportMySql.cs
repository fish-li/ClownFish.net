#if NET6_0_OR_GREATER
using ClownFish.Data.MySQL;

namespace ClownFish.Data.Sync;
internal class DataImportMySql : IDataImport
{
    private DataImportArgs _args;
    private List<MySqlDbField> _destFields;
    private List<string> _insertFields;
    private string _insertSql;

    public void Execute(DataImportArgs args)
    {
        _args = args;

        _destFields = MySqlDbHelper.GetFields(_args.DestDbContext, _args.DestTableName);

        GenerateDestInsertSql();

        WriteTable();
    }


    public async Task ExecuteAsync(DataImportArgs args)
    {
        _args = args;

        _destFields = await MySqlDbHelper.GetFieldsAsync(_args.DestDbContext, _args.DestTableName);

        GenerateDestInsertSql();

        await WriteTableAsync();
    }


    private void GenerateDestInsertSql()
    {
        MySqlDbField destIdField = _destFields.FirstOrDefault(x => x.IsPrimaryKey && x.IsAutoIncrement);

        // 如果存在【自增列主键】，但参数不允许，就忽略这个字段
        if( destIdField != null && _args.AllowAutoIncrement == false ) {
            _destFields.Remove(destIdField);  // 不操作这个字段
        }


        HashSet<string> inputColNames = _args.Data.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        // INSERT插入哪些列，不仅由表结构决定，还要参考输入数据的列
        // 如果输入数据表没有包含某个列，但是目标表的那个列不允许NULL，那么INSERT将会出现异常
        _insertFields = _destFields.Where(x => inputColNames.Contains(x.Name))
                                                    .Select(x => x.Name).ToList();

        StringBuilder sb = StringBuilderPool.Get();
        try {
            // insert into tableXXXX( xId
            sb.Append("/*DataImport*/ insert into ").Append(_args.DestDbContext.GetObjectFullName(_args.DestTableName)).Append('(');

            // , f1, f2, f3
            int index = 0;
            foreach( var f in _insertFields ) {
                if( index++ > 0 )
                    sb.Append(", ");
                sb.Append(_args.DestDbContext.GetObjectFullName(f));
            }

            sb.Append(") \nvalues(");

            // , @f1, @f2, @f3
            index = 0;
            foreach( var f in _insertFields ) {
                if( index++ > 0 )
                    sb.Append(", ");
                sb.Append("@").Append(f);
            }

            sb.Append(')');

            _insertSql = sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }

    }


    private void WriteTable()
    {
        List<BaseCommand> list = GetInsertCommands();

        _args.DestDbContext.CPQuery.Create("SET FOREIGN_KEY_CHECKS = 0;").ExecuteNonQuery();

        if( _args.WithTranscation ) {
            _args.DestDbContext.BeginTransaction();
            _args.DestDbContext.Batch.Execute(list);
            _args.DestDbContext.Commit();
        }
        else {
            _args.DestDbContext.Batch.Execute(list);
        }

        _args.DestDbContext.CPQuery.Create("SET FOREIGN_KEY_CHECKS = 1;").ExecuteNonQuery();
    }


    private async Task WriteTableAsync()
    {
        List<BaseCommand> list = GetInsertCommands();

        await _args.DestDbContext.CPQuery.Create("SET FOREIGN_KEY_CHECKS = 0;").ExecuteNonQueryAsync();

        if( _args.WithTranscation ) {
            _args.DestDbContext.BeginTransaction();
            await _args.DestDbContext.Batch.ExecuteAsync(list);
            _args.DestDbContext.Commit();
        }
        else {
            await _args.DestDbContext.Batch.ExecuteAsync(list);
        }

        await _args.DestDbContext.CPQuery.Create("SET FOREIGN_KEY_CHECKS = 1;").ExecuteNonQueryAsync();
    }


    private List<BaseCommand> GetInsertCommands()
    {
        List<BaseCommand> list = new List<BaseCommand>(_args.Data.Rows.Count);

        foreach(DataRow dataRow in _args.Data.Rows ) {

            Dictionary<string, object> dict = new Dictionary<string, object>(_args.Data.Columns.Count);
            
            foreach(string name in _insertFields ) {
                dict[name] = dataRow[name];
            }

            CPQuery query = _args.DestDbContext.CPQuery.Create(_insertSql, dict);
            list.Add(query);
        }

        return list;
    }
}

#endif
