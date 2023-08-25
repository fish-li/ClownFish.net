#if NET6_0_OR_GREATER

namespace ClownFish.Data.Sync;


internal interface IDataImport
{
    void Execute(DataImportArgs args);

    Task ExecuteAsync(DataImportArgs args);
}


/// <summary>
/// 数据导入工具类
/// </summary>
public static class DataImport
{
    /// <summary>
    /// 执行数据同步操作
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static void Execute(DataImportArgs args)
    {
        IDataImport dataImport = CreateImportImpl(args);
        dataImport.Execute(args);
    }


    /// <summary>
    /// 执行数据同步操作
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static async Task ExecuteAsync(DataImportArgs args)
    {
        IDataImport dataImport = CreateImportImpl(args);
        await dataImport.ExecuteAsync(args);
    }



    private static IDataImport CreateImportImpl(DataImportArgs args)
    {
        if( args == null )
            throw new ArgumentNullException(nameof(args));

        args.Validate();

        if( args.DestDbContext.DatabaseType != DatabaseType.MySQL )
            throw new NotSupportedException("目前仅支持MySQL数据库，不支持其它数据库类型！");

        // 目前只支持 MySQL
        return new DataImportMySql();
    }
}

#endif
