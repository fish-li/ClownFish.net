using ClownFish.Log.Writers;

namespace ClownFish.Log.Configuration;

internal class ConfigLoader
{
    private LogConfiguration _config;

    internal List<DataTypeWriterMap> Load(LogConfiguration config)
    {
        _config = config;

        if( config.Performance == null )
            config.Performance = new PerformanceConfig();

        if( config.File == null )
            config.File = new FileConfig();

        config.File.CheckOrSetDefault();
        config.Performance.CheckOrSetDefault();

        // 检查配置参数是否有效
        ChceckDataTypeConfig();
        CheckWritersConfig();

        // 生成【数据类型】和【写入器】的映射关系
        List<DataTypeWriterMap> maps = GetMapList();

        // 初始化写入器
        InitWrites();

        return maps;
    }


    private void ChceckDataTypeConfig()
    {
        if( _config.Types.IsNullOrEmpty() )
            throw new LogConfigException("日志配置文件中没有配置Types节点。");


        // 检查无效的数据类型定义
        foreach( var item in _config.Types ) {
            if( string.IsNullOrEmpty(item.DataType) )
                throw new LogConfigException("日志配置文件中Types/Type/DataType属性不允许为空。");

            if( item.Writers.IsNullOrEmpty() )
                throw new LogConfigException("日志配置文件中Types/Type/Writers属性不允许为空。");

            if( item.Writers.Trim(StringExtensions.ItemSeparators).Length == 0 )
                throw new LogConfigException("日志配置文件中Types/Type/Writers属性值无效（没有实际内容）。");


            // 如果指定的类型不正确，下面代码会抛出异常
            item.TypeObject = TypeHelper.GetType(item.DataType, true);
        }
    }


    private void CheckWritersConfig()
    {
        if( _config.Writers.IsNullOrEmpty() )
            throw new LogConfigException("日志配置文件中没有配置Writers节点。");

        // 增加内置的写入器
        List<WriterConfig> list = _config.Writers.ToList();
        list.Add(new WriterConfig { Name = "NULL", Type = "ClownFish.Log.Writers.NullWriter, ClownFish.net" });
        _config.Writers = list.ToArray();


        // 先统计使用了哪些写入器，对于没有使用的写入器，不执行初始化。
        // 好处是：预先配置各种写入器，项目使用时开箱即用。
        // 例如：可以先在配置文件中指定 Rabbit, MongoDb，连接参数指向配置服务，可以事先不安装这些依赖服务。
        string names = string.Join(";", _config.Types.Select(x => x.Writers).ToArray());
        string[] allwriters = names.ToArray2().Distinct().ToArray();


        foreach( var wconf in _config.Writers ) {
            if( string.IsNullOrEmpty(wconf.Name) )
                throw new LogConfigException("日志配置文件中Writers/Writer/Name属性不允许为空。");

            if( string.IsNullOrEmpty(wconf.Type) )
                throw new LogConfigException("日志配置文件中Writers/Writer/Type属性不允许为空。");

            // 忽略没有【启用】的写入器
            if( allwriters.FirstOrDefault(x => x.Is(wconf.Name)) == null )
                continue;


            // 如果指定的类型不正确，下面代码会抛出异常
            Type t = TypeHelper.GetType(wconf.Type, true);

            if( typeof(ILogWriter).IsAssignableFrom(t) == false )
                throw new LogConfigException($"日志配置文件中Writers/Writer/Type属性值 [{wconf.Type}] 没有实现接口ILogWriter。");


            // 确认可以实例化
            ILogWriter instance = (ILogWriter)Activator.CreateInstance(t);

            // 目前写入器都是单例的，所以用内部变量来实现
            // 如果以后要支持 “不同数据类型使用自己专属的写入器“，那么需要重构这里！
            wconf.TypeObject = t;
            wconf.WriterInstnace = instance;
        }
    }


    private void InitWrites()
    {
        foreach( var wconf in _config.Writers.Where(x => x.WriterInstnace != null) ) {
            wconf.WriterInstnace.Init(_config, wconf);
        }
    }

    private List<DataTypeWriterMap> GetMapList()
    {
        List<DataTypeWriterMap> resultList = new List<DataTypeWriterMap>(_config.Types.Length);

        foreach( var item in _config.Types ) {

            // 一种数据类型可以定义多个写入器，所以这里展开
            string[] writers = item.Writers.ToArray2().Distinct().ToArray();
            List<WriterConfig> list = new List<WriterConfig>(writers.Length);

            for( int i = 0; i < writers.Length; i++ ) {
                string writerName = writers[i];
                WriterConfig conf = _config.Writers.FirstOrDefault(x => x.Name.Is(writerName) && x.TypeObject != null);

                if( conf != null ) { 
                    list.Add(conf);
                }
                else
                    throw new LogConfigException($"日志配置文件中Types/Type/Writers属性值 [{writerName}] 无效（不是有效的写入器名称）。");
            }


            DataTypeWriterMap map = new DataTypeWriterMap();
            map.DataType = item.TypeObject;
            map.WriteTypes = list.Select(x => x.TypeObject).ToArray();

            // 注意：这里的写入器实例可能供多个数据类型共享使用
            map.Instances = list.Select(x => x.WriterInstnace).ToArray();
            resultList.Add(map);
        }

        return resultList;
    }


}
