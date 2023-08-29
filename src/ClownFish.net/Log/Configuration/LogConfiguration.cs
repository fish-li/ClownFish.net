namespace ClownFish.Log.Configuration;

/// <summary>
/// 日志的配置数据结构
/// </summary>
[XmlRoot("LogConfig")]
public sealed class LogConfiguration
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enable { get; set; } = true;

    /// <summary>
    /// 定时刷新间隔
    /// </summary>
    public int TimerPeriod { get; set; }

    /// <summary>
    /// 性能日志的配置信息
    /// </summary>
    public PerformanceConfig Performance { get; set; }


    /// <summary>
    /// 日志文件的配置信息
    /// </summary>
    public FileConfig File { get; set; }



    /// <summary>
    /// 所有Writer的配置集合
    /// </summary>
    [XmlArrayItem("Writer")]
    public WriterConfig[] Writers { get; set; }

    /// <summary>
    /// 所有要写入的数据类型集合
    /// </summary>
    [XmlArrayItem("Type")]
    public TypeItemConfig[] Types { get; set; }




    /// <summary>
    /// 从文件中加载LogConfiguration
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="checkExist"></param>
    /// <returns></returns>
    public static LogConfiguration LoadFromFile(string filePath, bool checkExist = true)
    {
        if( filePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(filePath));


        if( System.IO.File.Exists(filePath) == false ) {
            if( checkExist )
                throw new FileNotFoundException("配置文件没有找到，filePath: " + filePath);
            else
                return null;
        }

        return XmlHelper.XmlDeserializeFromFile<LogConfiguration>(filePath);
    }


    /// <summary>
    /// 从XML文本中加载LogConfiguration
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static LogConfiguration LoadFromXml(string xml)
    {
        if( xml.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(xml));

        return XmlHelper.XmlDeserialize<LogConfiguration>(xml);
    }


    internal DebugReportBlock GetDebugReportBlock()
    {
        DebugReportBlock block = new DebugReportBlock { Category = LogConfig.ConfigFileName, Order = 100 };

        // 合并后的日志配置
        block.AppendLine(this.ToXml());
        return block;
    }



    /// <summary>
    /// 合并二个配置对象，以config1为基础，用config2覆盖config1
    /// </summary>
    /// <param name="config1"></param>
    /// <param name="config2"></param>
    /// <returns></returns>
    public static LogConfiguration MegerConfig(LogConfiguration config1, LogConfiguration config2)
    {
        if( config2 == null )
            return config1;

        if( config1.File == null )
            config1.File = new FileConfig();
        if( config1.Performance == null )
            config1.Performance = new PerformanceConfig();
        if( config1.Writers == null )
            config1.Writers = new WriterConfig[0];
        if( config1.Types == null )
            config1.Types = new TypeItemConfig[0];


        config1.Enable = config2.Enable;

        if( config2.TimerPeriod > 0 )
            config1.TimerPeriod = config2.TimerPeriod;

        if( config2.Performance != null ) {
            if( config2.Performance.HttpExecute != int.MinValue )
                config1.Performance.HttpExecute = config2.Performance.HttpExecute;

            if( config2.Performance.HandleMessage != int.MinValue )
                config1.Performance.HandleMessage = config2.Performance.HandleMessage;
        }

        if( config2.File != null ) {
            if( config2.File.RootPath.IsNullOrEmpty() == false )
                config1.File.RootPath = config2.File.RootPath;

            if( config2.File.MaxLength.IsNullOrEmpty() == false )
                config1.File.MaxLength = config2.File.MaxLength;

            if( config2.File.MaxCount > 0 )
                config1.File.MaxCount = config2.File.MaxCount;
        }


        // 写入器在合并时，只允许新增，不做配置节点属性修改
        if( config2.Writers.IsNullOrEmpty() == false ) {

            List<WriterConfig> list = config1.Writers.ToList();

            foreach( var w2 in config2.Writers ) {
                if( list.FirstOrDefault(x => x.Name == w2.Name) == null )
                    list.Add(w2);
            }

            config1.Writers = list.ToArray();
        }

        // 数据类型在合并时，除了新增类型，还要修改“写入器”属性
        if( config2.Types.IsNullOrEmpty() == false ) {

            List<TypeItemConfig> list = config1.Types.ToList();

            foreach( var t2 in config2.Types ) {
                TypeItemConfig t1 = list.FirstOrDefault(x => x.DataType == t2.DataType);
                if( t1 == null )
                    list.Add(t2);
                else
                    t1.Writers = t2.Writers;
            }

            config1.Types = list.ToArray();
        }

        return config1;
    }


    public void OverrideWriters(string logWriterNames)
    {
        // 配置参数示例：InvokeLog=Rabbit,ES;*=ES;xx=NULL
        // 表示将 InvokeLog 的数据类型用 Rabbit和ES 写入器，其它全部为 ES

        if( logWriterNames.IsNullOrEmpty() )
            return;

        if( logWriterNames.IndexOf('=') < 0 )
            //logWriterNames = "*=" + logWriterNames;
            return;

        var items = logWriterNames.ToKVList(';', '=');

        NameValue defaultValue = items.FirstOrDefault(z => z.Name == "*");
        if( defaultValue != null ) {
            foreach( var t in this.Types )
                t.Writers = defaultValue.Value;
        }

        foreach( var t in this.Types ) {
            string typename = TypeHelper.GetShortName(t.DataType);

            NameValue nv = items.FirstOrDefault(x => x.Name == typename);
            if( nv != null )
                t.Writers = nv.Value;
        }
    }


    public void TryUpdateFromLocalSetting()
    {
        // 下面这几个参数的调整概念比较大
        // 为了调整这几个参数而单独准备一个文件，又显然有点麻烦了，所以这里允许使用环境变量的方式来调整它们

        int x1 = LocalSettings.GetInt("ClownFish_Log_Performance_HttpExecute");
        if( x1 > 0 )
            this.Performance.HttpExecute = x1;


        int x2 = LocalSettings.GetInt("ClownFish_Log_Performance_HandleMessage");
        if( x2 > 0 )
            this.Performance.HandleMessage = x2;


        int x3 = LocalSettings.GetInt("ClownFish_Log_TimerPeriod");
        if( x3 > 0 )
            this.TimerPeriod = x3;

    }
}
















