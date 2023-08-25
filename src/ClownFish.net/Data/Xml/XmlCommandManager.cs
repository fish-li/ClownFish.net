namespace ClownFish.Data.Xml;

/// <summary>
/// 用于维护配置文件中数据库访问命令的管理类
/// </summary>
public sealed class XmlCommandManager
{
    /// <summary>
    /// static readonly Instance
    /// </summary>
    public static readonly XmlCommandManager Instance = new XmlCommandManager();

    private readonly Dictionary<string, XmlCommandItem> _dict = new Dictionary<string, XmlCommandItem>(1024 * 2);
    private readonly Dictionary<string, XmlCommandItem> _dictXml = new Dictionary<string, XmlCommandItem>();


    /// <summary>
    /// 从指定的Xml字符串加载XmlCommand（例如将XmlCommand文件做为嵌入程序集资源）
    /// </summary>
    /// <param name="xml">xml字符串</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void LoadFromText(string xml)
    {
        if( string.IsNullOrEmpty(xml) )
            return;

        List<XmlCommandItem> list = XmlHelper.XmlDeserialize<List<XmlCommandItem>>(xml);
        list.ForEach(x => _dictXml.AddValue(x.CommandName, x));
    }


    /// <summary>
    /// <para>从指定的目录中加载全部的用于数据访问命令。</para>
    /// </summary>
    /// <param name="directoryPath">包含数据访问命令的目录。不加载子目录，仅加载扩展名为 .config 的文件。</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void LoadFromDirectory(string directoryPath)
    {
        //if( s_dict.Count > 0 )
        //    throw new InvalidOperationException("不允许重复调用这个方法。");

        if( string.IsNullOrEmpty(directoryPath) )
            throw new ArgumentNullException(nameof(directoryPath));
        if( Directory.Exists(directoryPath) == false )
            throw new DirectoryNotFoundException(string.Format("目录 {0} 不存在。", directoryPath));


        string[] files = Directory.GetFiles(directoryPath, "*.config", SearchOption.AllDirectories);
        if( files.Length > 0 ) {
            foreach( string file in files ) {
                List<XmlCommandItem> list = XmlHelper.XmlDeserializeFromFile<List<XmlCommandItem>>(file);
                list.ForEach(x => _dict.AddValue(x.CommandName, x));
            }
        }
    }

    //private static Dictionary<string, XmlCommandItem> LoadFromDirectoryInternal(string directoryPath, out Exception exception)
    //{
    //    exception = null;
    //    Dictionary<string, XmlCommandItem> dict = null;

    //    try {
    //        string[] files = Directory.GetFiles(directoryPath, "*.config", SearchOption.AllDirectories);
    //        if( files.Length > 0 ) {
    //            dict = new Dictionary<string, XmlCommandItem>(1024 * 2);

    //            foreach( string file in files ) {
    //                List<XmlCommandItem> list = XmlHelper.XmlDeserializeFromFile<List<XmlCommandItem>>(file);
    //                list.ForEach(x => dict.AddValue(x.CommandName, x));
    //            }
    //        }
    //    }
    //    catch( Exception ex ) {
    //        exception = ex;
    //        dict = null;
    //    }


    //    // 注册缓存移除通知，以便在用户修改了配置文件后自动重新加载。
    //    // 参考：细说 ASP.NET Cache 及其高级用法
    //    //	      http://www.cnblogs.com/fish-li/archive/2011/12/27/2304063.html
    //    //CacheDependency dep = new CacheDependency(directoryPath);
    //    //HttpRuntime.Cache.Insert(s_CacheKey, directoryPath, dep,
    //    //	System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,
    //    //	CacheItemPriority.NotRemovable, CacheRemovedCallback);

    //    return dict;
    //}


    //private static void CacheRemovedCallback(string key, object value, CacheItemRemovedReason reason)
    //{
    //	Exception exception = null;
    //	string directoryPath = (string)value;

    //	for( int i = 0; i < 5; i++ ) {
    //		// 由于事件发生时，文件可能还没有完全关闭，所以只好让程序稍等。
    //		System.Threading.Thread.Sleep(3000);


    //		// 重新加载配置文件
    //		Dictionary<string, XmlCommandItem> dict = LoadFromDirectoryInternal(directoryPath, out exception);

    //		if( exception == null ) {
    //			try {
    //			}
    //			finally {
    //				s_dict = dict;
    //				s_ExceptionOnLoad = null;
    //			}
    //			return;
    //		}
    //		//else: 有可能是文件还在更新，此时加载了不完整的文件内容，最终会导致反序列化失败。
    //	}

    //	if( exception != null )
    //		s_ExceptionOnLoad = exception;
    //}



    /// <summary>
    /// 根据配置文件中的命令名获取对应的命令对象。
    /// </summary>
    /// <param name="name">命令名称，它应该能对应一个XmlCommand</param>
    /// <returns>如果找到符合名称的XmlCommand，则返回它，否则返回null</returns>
    public XmlCommandItem GetCommand(string name)
    {
        //if( s_exceptionOnLoad != null )
        //	throw s_exceptionOnLoad;


        // 优先加载文件中指定的XmlCommand，这样可以实现文件对程序集资源的覆盖

        return _dict.TryGet(name)
               ?? _dictXml.TryGet(name)
               ;

        //throw new ArgumentOutOfRangeException("name", "不能根据指定的名称找到匹配的XmlCommand，name: " + name);
    }


}
