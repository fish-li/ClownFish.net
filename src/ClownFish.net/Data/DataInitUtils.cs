namespace ClownFish.Data;

internal static class DataInitUtils
{
    private static bool s_dalInited = false;


    /// <summary>
    /// 初始化 ClownFish.Data
    /// </summary>
#if NETCOREAPP     // 下面几个类型不参与裁剪，保留无参构造函数，确保可序列化
    //[UnconditionalSuppressMessage("Trimming", "IL2026: xml")]

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Entity))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IDataLoader<>))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IDataFieldTypeHandler))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IDbConfig))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TypeList))]

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DataFieldMapKV))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DataReaderUtils))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DataTableUtils))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(IEntityProxy))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(FieldNvObject))]
#endif
    public static void InitDAL()
    {
        if( s_dalInited == false ) {
            AutoRegisterDbProviders();

            if( EnvArgs0.IsAot == false ) {
                Initializer.Instance.LoadXmlCommandFromDirectory();

                // 【单文件部署】场景下，不允许在运行时生成代理程序集
                if( AsmHelper.IsSingleFileDeploy == false ) {
                    string newName = AsmHelper.GetExeName() + ".EntityProxy.dll";
                    string dllOutPath = Path.Combine(EnvUtils.GetTempPath(), newName);
                    Initializer.Instance.CompileAllEntityProxy(dllOutPath);
                }
            }

            s_dalInited = true;
        }
    }


    private static void AutoRegisterDbProviders()
    {
#if NETFRAMEWORK
        DbClientFactory.Init0();
#endif
        Initializer.Instance.RegisterSqlServerProvider();

        Initializer.Instance.RegisterMySqlProvider();

        AutoRegisterOthersSqlClient();
    }

    private static void AutoRegisterOthersSqlClient()
    {
        string[] asmList = AsmHelper.GetCurrentDomainAssemblies().Select(x => x.GetName().Name).OrderBy(x => x).ToArray();

        if( asmList.Contains("Npgsql") ) {
            Initializer.Instance.RegisterPostgreSqlProvider();
        }

        // 达梦早期的程序集名称叫：DmProvider ，最新版本已改名：DM.DmProvider
        if( asmList.Contains("DM.DmProvider") ) {
            Initializer.Instance.RegisterDamengProvider();
        }

        if( asmList.Contains("System.Data.SQLite") ) {
            Initializer.Instance.RegisterSQLiteProvider();
        }
    }
}
