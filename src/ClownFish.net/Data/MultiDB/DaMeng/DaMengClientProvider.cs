namespace ClownFish.Data.MultiDB.DaMeng;

internal class DaMengClientProvider : BaseClientProvider
{
    public static readonly BaseClientProvider Instance = new DaMengClientProvider();

    private readonly DbProviderFactory _dbProviderFactory;
    private readonly Type _exceptionType;
    private readonly IGetValue _getter;

    internal DaMengClientProvider()
    {
        Type factoryType = Type.GetType("Dm.DmClientFactory, DmProvider", true, false);

        _dbProviderFactory = (DbProviderFactory)factoryType.InvokeMember("Instance",
                                BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null);


        _exceptionType = Type.GetType("Dm.DmException, DmProvider", true, false);
        PropertyInfo p = _exceptionType.GetProperty("Number");
        if( p == null )
            throw new InvalidOperationException("没有找到属性：Dm.DmException.Number");

        _getter = GetterSetterFactory.GetPropertyGetterWrapper(p);
    }

    public override DatabaseType DatabaseType => DatabaseType.DaMeng;

    public override DbProviderFactory ProviderFactory => _dbProviderFactory;

    public override string GetObjectFullName(string symbol)
    {
        return "\"" + symbol + "\"";
    }

    public override string GetParamterName(string name, DbContext dbContext)
    {
        return ":" + name;
    }

    public override string GetParamterPlaceholder(string name, DbContext dbContext)
    {
        if( dbContext.EnableDelimiter )
            return ":\"" + name + "\"";
        else
            return ":" + name;
    }

    public override void ChangeDatabase(DbContext dbContext, string databaseName)
    {
        dbContext.CPQuery.Create("set schema \"" + databaseName + "\"").ExecuteNonQuery();
    }

    public override void PrepareCommand(DbCommand command, DbContext dbContext)
    {
        if( command.Parameters.Count > 0 ) {
            command.CommandText = command.CommandText.Replace('@', ':');

            foreach( DbParameter p in command.Parameters ) {
                if( p.ParameterName[0] == '@' ) {
                    p.ParameterName = p.ParameterName.Replace('@', ':');
                }
            }
        }
    }

    public override CPQuery GetNewIdQuery(CPQuery query, object entity)
    {
        if( entity == null )
            return query;

        if( entity.GetType().IsSubclassOf(typeof(Entity)) == false )
            throw new ArgumentOutOfRangeException(nameof(entity), "entity参数值不是实体对象!");

        EntityDescription description = EntityDescriptionCache.Get(entity.GetType());
        string tableName = query.Context.GetObjectFullName(description.TableName);

        //string schema = (query.Context.Connection as DmConnection).Schema;
        //object dmconn = query.Context.Connection;
        //string schema = (string)dmconn.GetType().InvokeMember("Schema", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public, null, dmconn, null);

        return query + $"; SELECT IDENT_CURRENT('{tableName}');";
    }

    public override bool IsDuplicateInsertException(Exception ex)
    {
        //if( ex is Dm.DmException ex2 ) {
        //    return (ex2.Number == -6602);
        //}
        // https://www.cndba.cn/dave/article/3738

        if( ex.GetType().IsCompatible(_exceptionType) ) {
            return (int)_getter.Get(ex) == -6602;
        }

        return false;
    }


    public override CPQuery SetPagedQuery(CPQuery query, int skip, int take)
    {
        return StdClientProvider.SetPagedQuery(query, skip, take);
    }

    public override Page2Query GetPagedCommand(BaseCommand query, PagingInfo pagingInfo)
    {
        return StdClientProvider.GetPagedCommand(query, pagingInfo);
    }
}
