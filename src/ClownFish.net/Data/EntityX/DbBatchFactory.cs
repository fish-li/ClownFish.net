#if NET6_0_OR_GREATER

namespace ClownFish.Data;

/// <summary>
/// 提供批量操作的入口类
/// </summary>
public sealed class DbBatchFactory
{
    private readonly DbContext _dbContext;

    internal DbBatchFactory(DbContext dbContext)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        _dbContext = dbContext;
    }


    internal DbBatch CreateDbBatch<T>(List<T> list, CurdKind kind) where T : Entity, new()
    {
        DbConnection connection = _dbContext.Connection;
        DbBatch batch = connection.CreateBatch();
        batch.Transaction = _dbContext.Transaction;

        foreach( var x in list ) {

            CPQuery query = kind switch {
                CurdKind.Insert => EntityCudUtils.GetInsertQuery(x, _dbContext),
                CurdKind.Update => EntityCudUtils.GetUpdateQuery(x, _dbContext),   // 这里可能返回 null
                _ => throw new NotSupportedException()
            };

            if( query == null )
                continue;

            DbCommand command = query.Command;

            DbParameter[] parameters = command.Parameters.Cast<DbParameter>().ToArray();
            command.Parameters.Clear();

            DbBatchCommand batchCommand = batch.CreateBatchCommand();
            batchCommand.CommandText = command.CommandText;
            batchCommand.Parameters.AddRange(parameters);

            batch.BatchCommands.Add(batchCommand);
        }

        return batch;
    }


    internal DbBatch CreateDbBatch(List<BaseCommand> list)
    {
        DbConnection connection = _dbContext.Connection;
        DbBatch batch = connection.CreateBatch();
        batch.Transaction = _dbContext.Transaction;

        foreach( var x in list ) {

            if( x == null )
                continue;

            DbCommand command = x.Command;

            DbParameter[] parameters = command.Parameters.Cast<DbParameter>().ToArray();
            command.Parameters.Clear();

            DbBatchCommand batchCommand = batch.CreateBatchCommand();
            batchCommand.CommandText = command.CommandText;
            batchCommand.Parameters.AddRange(parameters);

            batch.BatchCommands.Add(batchCommand);
        }

        return batch;
    }

    private int Execute0(DbBatch batch, string operationName)
    {
        // 触发执行 前 事件
        string operationId = Guid.NewGuid().ToString("N");
        //DbContextEvent.BeforeExecuteBatch(_dbContext, batch, operationId, operationName, false);
        DateTime start = DateTime.Now;

        try {
            // 执行批量插入操作
            int result = batch.ExecuteNonQuery();

            // 触发执行 后 事件
            DbContextEvent.AfterExecuteBatch(_dbContext, batch, operationId, operationName, start, false, null);

            return result;
        }
        catch( Exception ex ) {
            // 触发 异常 事件
            DbContextEvent.AfterExecuteBatch(_dbContext, batch, operationId, operationName, start, false, ex);

            // 重新抛出异常
            throw;
        }
    }

    private async Task<int> ExecuteAsync0(DbBatch batch, string operationName)
    {
        // 触发执行 前 事件
        string operationId = Guid.NewGuid().ToString("N");
        //DbContextEvent.BeforeExecuteBatch(_dbContext, batch, operationId, operationName, false);
        DateTime start = DateTime.Now;

        try {
            // 执行批量插入操作
            int result = await batch.ExecuteNonQueryAsync();

            // 触发执行 后 事件
            DbContextEvent.AfterExecuteBatch(_dbContext, batch, operationId, operationName, start, true, null);

            return result;
        }
        catch( Exception ex ) {
            // 触发 异常 事件
            DbContextEvent.AfterExecuteBatch(_dbContext, batch, operationId, operationName, start, true, ex);

            // 重新抛出异常
            throw;
        }
    }

    private int Execute0<T>(List<T> list, CurdKind kind, string operationName) where T : Entity, new()
    {
        if( list.IsNullOrEmpty() )
            return 0;

        _dbContext.OpenConnection();

        DbBatch batch = CreateDbBatch(list, kind);

        return Execute0(batch, operationName);
    }

    private async Task<int> ExecuteAsync0<T>(List<T> list, CurdKind kind, string operationName) where T : Entity, new()
    {
        if( list.IsNullOrEmpty() )
            return 0;

        await _dbContext.OpenConnectionAsync();

        DbBatch batch = CreateDbBatch(list, kind);

        return await ExecuteAsync0(batch, operationName);
    }

    /// <summary>
    /// 将实体列表以批量形式 插入 到数据表中
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="list">实体列表</param>
    /// <param name="batchSize">每个批次大小，小于等于零表示不做拆分</param>
    public int Insert<T>(List<T> list, int batchSize = 100) where T : Entity, new()
    {
        if( _dbContext.Factory.CanCreateBatch ) {
            if( batchSize <= 0 || list.Count <= batchSize ) {
                return Execute0(list, CurdKind.Insert, "SQL_BatchInsert");
            }
            else {
                int sum = 0;
                List<List<T>> list2 = list.SplitList(int.MaxValue, batchSize);
                foreach( var subList in list2 ) {
                    sum += Execute0(subList, CurdKind.Insert, "SQL_BatchInsert");
                }
                return sum;
            }
        }
        else {
            foreach( var x in list.Where(t => t != null) ) {
                _dbContext.Entity.Insert(x);
            }
            return list.Count;
        }
    }


    /// <summary>
    /// 将实体列表以批量形式 插入 到数据表中
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="list">实体列表</param>
    /// <param name="batchSize">每个批次大小，小于等于零表示不做拆分</param>
    public async Task<int> InsertAsync<T>(List<T> list, int batchSize = 100) where T : Entity, new()
    {
        if( _dbContext.Factory.CanCreateBatch ) {
            if( batchSize <= 0 || list.Count <= batchSize ) {
                return await ExecuteAsync0(list, CurdKind.Insert, "SQL_BatchInsert_Async");
            }
            else {
                int sum = 0;
                List<List<T>> list2 = list.SplitList(int.MaxValue, batchSize);
                foreach( var subList in list2 ) {
                    sum += await ExecuteAsync0(subList, CurdKind.Insert, "SQL_BatchInsert_Async");
                }
                return sum;
            }
        }
        else {
            foreach( var x in list.Where(t => t != null) ) {
                await _dbContext.Entity.InsertAsync(x);
            }
            return list.Count;
        }
    }


    /// <summary>
    /// 将实体列表以批量形式 更新 到数据表中
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="list">实体列表</param>
    /// <param name="batchSize">每个批次大小，小于等于零表示不做拆分</param>
    public int Update<T>(List<T> list, int batchSize = 100) where T : Entity, new()
    {
        if( _dbContext.Factory.CanCreateBatch ) {
            if( batchSize <= 0 || list.Count <= batchSize ) {
                return Execute0(list, CurdKind.Update, "SQL_BatchUpdate");
            }
            else {
                int sum = 0;
                List<List<T>> list2 = list.SplitList(int.MaxValue, batchSize);
                foreach( var subList in list2 ) {
                    sum += Execute0(subList, CurdKind.Update, "SQL_BatchUpdate");
                }
                return sum;
            }
        }
        else {
            int count = 0;
            foreach( var x in list.Where(t => t != null) ) {
                count += _dbContext.Entity.Update(x);
            }
            return count;
        }
    }


    /// <summary>
    /// 将实体列表以批量形式 更新 到数据表中
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <param name="list">实体列表</param>
    /// <param name="batchSize">每个批次大小，小于等于零表示不做拆分</param>
    public async Task<int> UpdateAsync<T>(List<T> list, int batchSize = 100) where T : Entity, new()
    {
        if( _dbContext.Factory.CanCreateBatch ) {
            if( batchSize <= 0 || list.Count <= batchSize ) {
                return await ExecuteAsync0(list, CurdKind.Update, "SQL_BatchUpdate_Async");
            }
            else {
                int sum = 0;
                List<List<T>> list2 = list.SplitList(int.MaxValue, batchSize);
                foreach( var subList in list2 ) {
                    sum += await ExecuteAsync0(subList, CurdKind.Update, "SQL_BatchUpdate_Async");
                }
                return sum;
            }
        }
        else {
            int count = 0;
            foreach( var x in list.Where(t => t != null) ) {
                count += await _dbContext.Entity.UpdateAsync(x);
            }
            return count;
        }
    }


    /// <summary>
    /// 批量执行一些CRD操作
    /// </summary>
    /// <param name="list">命令列表</param>
    /// <param name="batchSize">每个批次大小，小于等于零表示不做拆分</param>
    /// <returns></returns>
    public int Execute(List<BaseCommand> list, int batchSize = 100)
    {
        if( list.IsNullOrEmpty() )
            return 0;

        _dbContext.OpenConnection();

        if( _dbContext.Factory.CanCreateBatch ) {
            if( batchSize <= 0 || list.Count <= batchSize ) {
                DbBatch batch = CreateDbBatch(list);
                return Execute0(batch, "SQL_BatchExecute");
            }
            else {
                int sum = 0;
                List<List<BaseCommand>> list2 = list.SplitList(int.MaxValue, batchSize);
                foreach( var subList in list2 ) {
                    DbBatch batch = CreateDbBatch(subList);
                    sum += Execute0(batch, "SQL_BatchExecute");
                }
                return sum;
            }
        }
        else {
            int count = 0;
            foreach( var x in list.Where(t => t != null) ) {
                count += x.ExecuteNonQuery();
            }
            return count;
        }
    }


    /// <summary>
    /// 批量执行一些CRD操作
    /// </summary>
    /// <param name="list">命令列表</param>
    /// <param name="batchSize">每个批次大小，小于等于零表示不做拆分</param>
    /// <returns></returns>
    public async Task<int> ExecuteAsync(List<BaseCommand> list, int batchSize = 100)
    {
        if( list.IsNullOrEmpty() )
            return 0;

        _dbContext.OpenConnection();

        if( _dbContext.Factory.CanCreateBatch ) {
            if( batchSize <= 0 || list.Count <= batchSize ) {
                DbBatch batch = CreateDbBatch(list);
                return await ExecuteAsync0(batch, "SQL_BatchExecute_Async");
            }
            else {
                int sum = 0;
                List<List<BaseCommand>> list2 = list.SplitList(int.MaxValue, batchSize);
                foreach( var subList in list2 ) {
                    DbBatch batch = CreateDbBatch(subList);
                    sum += await ExecuteAsync0(batch, "SQL_BatchExecute_Async");
                }
                return sum;
            }
        }
        else {
            int count = 0;
            foreach( var x in list.Where(t => t != null) ) {
                count += await x.ExecuteNonQueryAsync();
            }
            return count;
        }
    }
}


#endif