using ClownFish.Log.Writers;

namespace ClownFish.Log.Configuration;

[UnconditionalSuppressMessage("Trimming", "IL2026: TypeHelper.GetType")]
[UnconditionalSuppressMessage("Trimming", "IL2072: Activator.CreateInstance")]
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
        List<DataTypeWriterMap> maps = CreateMapList();

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
        // 例如：可以先在配置文件中指定 Rabbit, MongoDb，当项目不启用时不必安装这些依赖服务。
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
            _ = (ILogWriter)Activator.CreateInstance(t);

            wconf.TypeObject = t;
        }
    }


    [UnconditionalSuppressMessage("Trimming", "IL2067: Activator.CreateInstance")]
    private List<DataTypeWriterMap> CreateMapList()
    {
        List<DataTypeWriterMap> resultList = new List<DataTypeWriterMap>(_config.Types.Length);

        foreach( var item in _config.Types ) {

            // 一种数据类型可以定义多个写入器，所以这里展开
            string[] writers = item.Writers.ToArray2().Distinct().ToArray();
            List<WriterConfig> list = new List<WriterConfig>(writers.Length);

            foreach( string writerName in writers ) {
                WriterConfig conf = _config.Writers.FirstOrDefault(x => x.Name.Is(writerName) && x.TypeObject != null);

                if( conf != null ) {
                    list.Add(conf);
                }
                else
                    throw new LogConfigException($"日志配置文件中Types/Type/Writers属性值 [{writerName}] 无效（不是有效的写入器名称）。");
            }


            DataTypeWriterMap map = new DataTypeWriterMap();
            map.DataType = item.TypeObject;
            map.WriterTypes = list.Select(x => x.TypeObject).ToArray();

            // 如果写入器被引用多次，那就创建多个实例，不能共用。
            // 例如：FileWriter 就是一个典型的需要多个实例的写入器，因为每个数据类型都要写入不同的文件。
            map.Writers = map.WriterTypes.Select(x => (ILogWriter)Activator.CreateInstance(x)).ToArray();
            resultList.Add(map);

            // 初始化写入器
            foreach( var x in map.Writers ) {
                x.Init(_config, item.TypeObject);
            }
        }

        return resultList;
    }


}
