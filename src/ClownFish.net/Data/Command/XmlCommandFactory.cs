namespace ClownFish.Data;

/// <summary>
/// XmlCommand工厂
/// </summary>
public sealed class XmlCommandFactory
{
    private readonly DbContext _dbContext;

    internal XmlCommandFactory(DbContext dbContext)
    {
        if( dbContext == null )
            throw new ArgumentNullException(nameof(dbContext));

        _dbContext = dbContext;
    }


    /// <summary>
    /// 根据XmlCommand名称和参数对象创建XmlCommand实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="argsObject"></param>
    /// <returns></returns>
    public XmlCommand Create(string name, object argsObject = null)
    {
        XmlCommand command = new XmlCommand(_dbContext);
        command.Init(name, argsObject);
        return command;
    }

    /// <summary>
    /// 根据XmlCommand名称和参数对象创建XmlCommand实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public XmlCommand Create(string name, Hashtable dictionary)
    {
        XmlCommand command = new XmlCommand(_dbContext);
        command.Init(name, dictionary);
        return command;
    }

    /// <summary>
    /// 根据XmlCommand名称和参数对象创建XmlCommand实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public XmlCommand Create(string name, IDictionary<string, object> dictionary)
    {
        XmlCommand command = new XmlCommand(_dbContext);
        command.Init(name, dictionary);
        return command;
    }
}
