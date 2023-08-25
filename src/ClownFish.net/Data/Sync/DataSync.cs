#if NET6_0_OR_GREATER
namespace ClownFish.Data.Sync;

// 需求背景：https://www.processon.com/view/link/6368755d637689059c475846


internal interface IDataSync
{
    DataSyncResult Execute(DataSyncArgs args);
    Task<DataSyncResult> ExecuteAsync(DataSyncArgs args);
}

/// <summary>
/// 数据表同步工具类
/// </summary>
public static class DataSync
{
    /// <summary>
    /// 执行数据同步操作
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static DataSyncResult Execute(DataSyncArgs args)
    {
        IDataSync dataSync = CreateSyncImpl(args);
        return dataSync.Execute(args);
    }


    /// <summary>
    /// 执行数据同步操作
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static async Task<DataSyncResult> ExecuteAsync(DataSyncArgs args)
    {
        IDataSync dataSync = CreateSyncImpl(args);
        return await dataSync.ExecuteAsync(args);
    }


    private static IDataSync CreateSyncImpl(DataSyncArgs args)
    {
        if( args == null )
            throw new ArgumentNullException(nameof(args));

        args.Validate();

        if( args.SrcDbContext.DatabaseType != DatabaseType.MySQL || args.DestDbContext.DatabaseType != DatabaseType.MySQL )
            throw new NotSupportedException("目前仅支持MySQL数据库，不支持其它数据库类型！");

        // 目前只支持 MySQL
        return new DataSyncMySql();
    }
}


/// <summary>
/// 数据同步操作的执行结果
/// </summary>
/// <param name="InsertCount">插入行数</param>
/// <param name="UpdateCount">更新行数</param>
public record struct DataSyncResult(int InsertCount, int UpdateCount);


#endif