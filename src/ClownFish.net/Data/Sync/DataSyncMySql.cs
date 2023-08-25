#if NET6_0_OR_GREATER
using ClownFish.Data.MySQL;

namespace ClownFish.Data.Sync;

internal sealed class DataSyncMySql : IDataSync
{
    private class DestId2
    {
        public long RowId { get; set; }
        public long RelId { get; set; }
    }

    private DataSyncArgs _args;
    private List<MySqlDbField> _srcFields;
    private List<MySqlDbField> _destFields;

    private MySqlDbField _srcKeyField;
    private MySqlDbField _destKeyField;
    private MySqlDbField _destRelatedField;

    private string _insertSql;
    private string _updateSql;

    public DataSyncResult Execute(DataSyncArgs args)
    {
        _args = args;

        // 获取表结构
        ObtainTableStructure();

        // 检查主外键字段
        CheckField();

        // 填充字段映射关系
        FillFieldsMap();

        // 生成针对目标表的INSERT,UPDATE语句
        GenerateDestInsertSql();
        GenerateDestUpdateSql();


        // 加载源表数据
        DataTable srcTable = LoadSrcTableData();
        if( srcTable.Rows.Count == 0 )
            return new DataSyncResult();


        // 查询目标表的主外键记录，用于后面步骤中区分是INSERT还是UPDATE
        List<DestId2> destId2List = LoadDestTableData(srcTable);
        

        // 将数据写入到目标表
        return WriteToDestTable(srcTable, destId2List);
    }


    public async Task<DataSyncResult> ExecuteAsync(DataSyncArgs args)
    {
        _args = args;

        // 获取表结构
        await ObtainTableStructureAsync();

        // 检查主外键字段
        CheckField();

        // 填充字段映射关系
        FillFieldsMap();

        // 生成针对目标表的INSERT,UPDATE语句
        GenerateDestInsertSql();
        GenerateDestUpdateSql();


        // 加载源表数据
        DataTable srcTable = await LoadSrcTableDataAsync();
        if( srcTable.Rows.Count == 0 )
            return new DataSyncResult();


        // 查询目标表的主外键记录，用于后面步骤中区分是INSERT还是UPDATE
        List<DestId2> destId2List = await LoadDestTableDataAsync(srcTable);


        // 将数据写入到目标表
        return await WriteToDestTableAsync(srcTable, destId2List);
    }


    private void ObtainTableStructure()
    {
        _srcFields = MySqlDbHelper.GetFields(_args.SrcDbContext, _args.SrcTableName);
        _destFields = MySqlDbHelper.GetFields(_args.DestDbContext, _args.DestTableName);
    }

    private async Task ObtainTableStructureAsync()
    {
        _srcFields = await MySqlDbHelper.GetFieldsAsync(_args.SrcDbContext, _args.SrcTableName);
        _destFields = await MySqlDbHelper.GetFieldsAsync(_args.DestDbContext, _args.DestTableName);
    }


    private void CheckField()
    {
        if( _srcFields.Count(x => x.IsPrimaryKey) != 1 )
            throw new InvalidCodeException($"SrcTableName={_args.SrcTableName}，源表没有主键或者主键不是一个单一的字段！");

        if( _destFields.Count(x => x.IsPrimaryKey) != 1 )
            throw new InvalidCodeException($"DestTableName={_args.DestTableName}，目标表没有主键或者主键不是一个单一的字段！");


        _destKeyField = _destFields.FirstOrDefault(x => x.IsPrimaryKey);
        //if( _destKeyField == null )
        //    throw new InvalidCodeException($"目标表 {_args.DestTableName} 没有主键字段！");

        if( IsLongType(_destKeyField.DataType) == false )
            throw new NotSupportedException($"目标表的主键字段必须是 int,long 类型，其它类型都不支持！");



        _srcKeyField = _srcFields.FirstOrDefault(x => x.Name.Is(_args.SrcKeyField));
        if( _srcKeyField == null )
            throw new InvalidCodeException($"SrcKeyField={_args.SrcKeyField}，源表中不存在此字段！");

        if( _srcKeyField.IsPrimaryKey == false )
            throw new InvalidCodeException($"SrcKeyField={_args.SrcKeyField}，此字段不是主键字段！");


        // 这里为了后面的 IN查询 方便，所以就强制主键只允许 int, long
        if( IsLongType(_srcKeyField.DataType) == false )
            throw new NotSupportedException($"SrcKeyField={_args.SrcKeyField}，此字段必须是 int,long 类型，其它类型都不支持！");


        _destRelatedField = _destFields.FirstOrDefault(x => x.Name.Is(_args.DestRelatedKey));
        if( _destRelatedField == null )
            throw new InvalidCodeException($"DestRelatedKey={_args.DestRelatedKey}，目标表中不存在此字段！");

        if( _srcKeyField.DataType != _destRelatedField.DataType )
            throw new InvalidCodeException($"SrcKeyField={_args.SrcKeyField}，DestRelatedKey={_args.DestRelatedKey}，2个字段类型不一致！");



        // 对于【主外键】的映射场景，如果目标表的主键不是自增列，会导致INSERT时没有机会赋值
        if( _destRelatedField.Name != _destKeyField.Name && _destKeyField.IsAutoIncrement == false )
            throw new InvalidCodeException($"主外键映射场景下，目标表 {_args.DestTableName} 的主键字段必须是自增列！");

        // 在主键相同的映射场景下（复制模式），目标表的数据应该是只读的，由源表来维护，所以目标表的主键没有必要设置自增列
        if( _destRelatedField.Name == _destKeyField.Name && _destKeyField.IsAutoIncrement )
            throw new InvalidCodeException($"主键同步映射场景下，目标表 {_args.DestTableName} 的主键字段不能是自增列！");
    }

    private static bool IsLongType(string dataType)
    {
        return dataType switch {
            "int" => true,
            "integer" => true,
            "mediumint" => true,
            "bigint" => true,
            _ => false
        };
    }

    private void FillFieldsMap()
    {
        if( _args.DataFieldsMap.HasValue() )
            return;

        // 如果不指定映射关系，就按默认规则来生成映射关系，
        // 默认规则：先把主外键排除，再找同名的字段，再检查数据类型是否匹配
        List<MySqlDbField> srcfs = _srcFields.Where(x => x.Name.Is(_args.SrcKeyField) == false).ToList();
        List<MySqlDbField> destfs = _destFields.Where(x => x.Name.Is(_args.DestRelatedKey) == false).ToList();

        List<FieldMap> list = new List<FieldMap>(srcfs.Count);

        foreach( var x1 in srcfs ) {

            MySqlDbField d = _destFields.FirstOrDefault(x2 => x2.Name.Is(x1.Name));

            // 默认匹配行为：字段同名，数据类型一致
            if( d != null && x1.DataType == d.DataType ) {
                list.Add(new FieldMap(x1.Name, d.Name));
            }
        }

        _args.DataFieldsMap = list;
    }

    private DataTable LoadSrcTableData()
    {
        CPQuery query = LoadSrcTableDataQuery();

        // 以“脏读”方式加载源表数据
        _args.SrcDbContext.BeginTransaction(IsolationLevel.ReadUncommitted);
        DataTable table = query.ToDataTable();
        _args.SrcDbContext.Commit();

        return table;
    }

    private async Task<DataTable> LoadSrcTableDataAsync()
    {
        CPQuery query = LoadSrcTableDataQuery();

        // 以“脏读”方式加载源表数据
        _args.SrcDbContext.BeginTransaction(IsolationLevel.ReadUncommitted);
        DataTable table = await query.ToDataTableAsync();
        _args.SrcDbContext.Commit();

        return table;
    }


    private CPQuery LoadSrcTableDataQuery()
    {
        // 加载字段包括：主键 和所有需要映射的字段
        List<string> names = new List<string>();
        names.Add(_args.SrcKeyField);

        foreach( var x in _args.DataFieldsMap )
            names.Add(x.SrcField);


        string selectFieds = string.Join(",", names.Select(x => _args.SrcDbContext.GetObjectFullName(x)).ToArray());
        string tableName = _args.SrcDbContext.GetObjectFullName(_args.SrcTableName);

        string selectSql = $"/*DataSync*/ select {selectFieds} from {tableName} where {_args.SrcFilterSql}";

        return _args.SrcDbContext.CPQuery.Create(selectSql);
    }

    private List<DestId2> LoadDestTableData(DataTable srcTable)
    {
        CPQuery query = LoadDestTableDataQuery(srcTable);
        return query.ToList<DestId2>();
    }

    private async Task<List<DestId2>> LoadDestTableDataAsync(DataTable srcTable)
    {
        CPQuery query = LoadDestTableDataQuery(srcTable);
        return await query.ToListAsync<DestId2>();
    }

    private CPQuery LoadDestTableDataQuery(DataTable srcTable)
    {
        List<long> keyValues = (from row in srcTable.Rows.Cast<DataRow>()
                                let key = Convert.ToInt64(row[_args.SrcKeyField])
                                select key).ToList();

        // 下面2个字段有可能是一样的！ 在查询时必须指定别名：RowId, RelId
        string keyFieldName = _args.DestDbContext.GetObjectFullName(_destKeyField.Name);
        string relFieldName = _args.DestDbContext.GetObjectFullName(_args.DestRelatedKey);
        string talbeName = _args.DestDbContext.GetObjectFullName(_args.DestTableName);

        // 只查询2字段：主键（用于 update ... where）， 外键（用于判断是做 insert 还是 update）
        string selectSql = $"/*DataSync*/ select {keyFieldName} as RowId, {relFieldName} as RelId from {talbeName} where {_args.DestRelatedKey} in ( {{relatedValues}} )";
        object args = new {
            relatedValues = keyValues
        };

        return _args.DestDbContext.CPQuery.Create(selectSql, args);
    }


    private void GenerateDestInsertSql()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            // insert into tableXXXX( xId
            sb.Append("/*DataSync*/ insert into ").Append(_args.DestDbContext.GetObjectFullName(_args.DestTableName)).Append('(')
                .Append(_args.DestDbContext.GetObjectFullName(_args.DestRelatedKey));


            // , f1, f2, f3
            foreach( var f in _args.DataFieldsMap ) {
                sb.Append(", ").Append(_args.DestDbContext.GetObjectFullName(f.DestField));
            }

            sb.Append(") \nvalues(@").Append(_args.DestRelatedKey);

            // , @f1, @f2, @f3
            foreach( var f in _args.DataFieldsMap ) {
                sb.Append(", @").Append(f.DestField);
            }

            sb.Append(')');

            _insertSql = sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    private void GenerateDestUpdateSql()
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            // insert tableXXXX set 
            sb.Append("/*DataSync*/ update ").Append(_args.DestDbContext.GetObjectFullName(_args.DestTableName))
                    .Append(" \nset ");


            int index = 0;
            foreach( var f in _args.DataFieldsMap ) {
                if( index > 0 )
                    sb.Append(", ");
                index++;

                sb.Append(_args.DestDbContext.GetObjectFullName(f.DestField)).Append(" = @").Append(f.DestField);
            }

            sb.Append(" \nwhere ").Append(_args.DestDbContext.GetObjectFullName(_destKeyField.Name)).Append(" = @").Append(_destKeyField.Name);

            _updateSql = sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

    private DataSyncResult WriteToDestTable(DataTable srcTable, List<DestId2> destId2List)
    {
        // 拆分数据，分成：插入数据 和 更新数据
        SplitDataRow(srcTable, destId2List, out List<DataRow> insertRows, out List<DataRow> updateRows);

        List<BaseCommand> list1 = UpdateDestTable(updateRows, destId2List);
        List<BaseCommand> list2 = InsertDestTable(insertRows);

        list1.AddRange(list2);

        if( _args.WithTranscation ) {
            _args.DestDbContext.BeginTransaction();
            _args.DestDbContext.Batch.Execute(list1);
            _args.DestDbContext.Commit();
        }
        else {
            _args.DestDbContext.Batch.Execute(list1);
        }

        return new DataSyncResult(insertRows.Count, updateRows.Count);
    }

    private async Task<DataSyncResult> WriteToDestTableAsync(DataTable srcTable, List<DestId2> destId2List)
    {
        // 拆分数据，分成：插入数据 和 更新数据
        SplitDataRow(srcTable, destId2List, out List<DataRow> insertRows, out List<DataRow> updateRows);

        List<BaseCommand> list1 = UpdateDestTable(updateRows, destId2List);
        List<BaseCommand> list2 = InsertDestTable(insertRows);

        list1.AddRange(list2);

        if( _args.WithTranscation ) {
            _args.DestDbContext.BeginTransaction();
            await _args.DestDbContext.Batch.ExecuteAsync(list1);
            _args.DestDbContext.Commit();
        }
        else {
            await _args.DestDbContext.Batch.ExecuteAsync(list1);
        }

        return new DataSyncResult(insertRows.Count, updateRows.Count);
    }

    private void SplitDataRow(DataTable srcTable, List<DestId2> destId2List, out List<DataRow> insertRows, out List<DataRow> updateRows)
    {
        // 这里按照【外键】来判断记录是不是要做更新（存在即更新）

        // 先获取目标中所有已存在的 ID 清单
        HashSet<long> idList = destId2List.Select(x => x.RelId).ToHashSet();

        // 过滤 insert, update 数据行
        insertRows = srcTable.Rows.Cast<DataRow>().Where(x => idList.Contains(Convert.ToInt64(x[_args.SrcKeyField])) == false).ToList();
        updateRows = srcTable.Rows.Cast<DataRow>().Where(x => idList.Contains(Convert.ToInt64(x[_args.SrcKeyField]))).ToList();
    }

    private List<BaseCommand> InsertDestTable(List<DataRow> insertRows)
    {
        List<BaseCommand> list = new List<BaseCommand>(insertRows.Count);

        foreach(var row in insertRows ) {
            Dictionary<string, object> dict = new Dictionary<string, object>(row.Table.Columns.Count);
            dict[_args.DestRelatedKey] = Convert.ToInt64(row[_args.SrcKeyField]);

            foreach(var f in _args.DataFieldsMap ) {
                dict[f.DestField] = row[f.SrcField];
            }

            CPQuery query = _args.DestDbContext.CPQuery.Create(_insertSql, dict);
            list.Add(query);

            //query.ExecuteNonQuery();
        }

        return list;
    }

    private List<BaseCommand> UpdateDestTable(List<DataRow> updateRows, List<DestId2> destId2List)
    {
        List<BaseCommand> list = new List<BaseCommand>(updateRows.Count);

        foreach(var row in updateRows ) {
            Dictionary<string, object> dict = new Dictionary<string, object>(row.Table.Columns.Count);

            foreach( var f in _args.DataFieldsMap ) {
                dict[f.DestField] = row[f.SrcField];
            }

            // 取得源表的主键值
            long srcId = Convert.ToInt64(row[_args.SrcKeyField]);

            // 根据destId2List映射关系，获取目标表对应的主键值
            dict[_destKeyField.Name] = destId2List.First(x => x.RelId == srcId).RowId;

            CPQuery query = _args.DestDbContext.CPQuery.Create(_updateSql, dict);
            list.Add(query);

            //query.ExecuteNonQuery();
        }

        return list;
    }

}
#endif