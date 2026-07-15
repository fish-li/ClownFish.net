namespace ClownFish.Log.Writers;

/// <summary>
/// 将日志记录到文件的写入器
/// </summary>
internal abstract class FileWriter : ILogWriter
{
    private string _currentFile;

    public virtual void Init(LogConfiguration config, Type dataType)
    {
        // 初始化目录
        FileUtils.InitDirectory(config);

        // 清理老旧的文件，开发测试阶段，程序反复启动，每个日志文件都比较小，所以也要应及时清理过多的文件
        string xx = GetFilePath(dataType, DateTime.Now);        
        DeleteOldFile(Path.GetDirectoryName(xx), this.FileExtName);
    }


    /// <summary>
    /// 文件扩展名
    /// </summary>
    protected virtual string FileExtName => ".log";

    /// <summary>
    /// 日志之间是否需要添加分隔行，默认为 false，如果需要添加分隔行，子类重写为 true 即可。
    /// </summary>
    protected virtual bool NeedFlagLine => false;

    /// <summary>
    /// 将对象转成要保存的文本
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public abstract string ObjectToText(object obj);

    /// <summary>
    /// 累计写入次数的计数器
    /// </summary>
    protected abstract ValueCounter WriteCounter { get; }


    /// <summary>
    /// 获取一个用于写入日志内容的文件名
    /// </summary>
    /// <param name="type">数据类型</param>
    /// <param name="time"></param>
    /// <returns></returns>
    internal string GetFilePath(Type type, DateTime time)
    {
        string datatype = type.Name;
        string timeString = time.ToString("yyyyMMdd_HHmmss");

        // 文件名示例：     /logs/OprLog/OprLog_20210126_171122.json.log				
        return string.Concat(FileUtils.RootPath, datatype, "/", datatype, "_", timeString, this.FileExtName);
    }

    private static readonly string s_flagLine = "----------------9af955fc890b403b9be4f58a88b022f1--";

    /// <summary>
    /// 写入单条日志信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="msg"></param>
    public virtual void WriteOne<T>(T msg) where T : class, IMsgObject
    {
        // 数据对象序列化
        string text = ObjectToText(msg);

        if( this.NeedFlagLine ) {  // 添加分隔行          
            text = text + Environment.NewLine + s_flagLine + Environment.NewLine;
        }

        // 数据日志内容写入到文件
        WriteToFile<T>(text, true);

        // 更新计数器
        this.WriteCounter?.Increment();
    }

    /// <summary>
    /// 批量写入日志信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public virtual void WriteList<T>(List<T> list) where T : class, IMsgObject
    {
        string block = null;
        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( T msg in list ) {
                string line = ObjectToText(msg);
                sb.AppendLine(line);

                if( this.NeedFlagLine ) {
                    sb.AppendLine(s_flagLine);
                }
            }
            block = sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }

        // 数据日志内容写入到文件
        WriteToFile<T>(block, false);
        this.WriteCounter?.Add(list.Count);
    }



    /// <summary>
    /// 将日志内容写入文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="text"></param>
    /// <param name="addNewLine"></param>
    protected int WriteToFile<T>(string text, bool addNewLine)
    {
        if( text.IsNullOrEmpty() )
            return 0;

        // 一个 writer 实例在运行时只负责一种数据类型，所以持续使用文件名是没有问题的
        if( _currentFile == null ) {
            // 注意：取类型名称时，不采用 msg.GetType().Name ，因为可能有继承情况
            _currentFile = GetFilePath(typeof(T), DateTime.Now);
        }


        // 追加到文件，如果失败则表示超过文件最大长度
        if( FileHelper.AppendAllText(_currentFile, text, addNewLine, FileUtils.MaxLength) == false ) {

            // 清理老旧的文件
            DeleteOldFile(Path.GetDirectoryName(_currentFile), this.FileExtName);

            // _currentFile 文件已经超过最大长度了，不能再继续追加了，所以必须生成一个新的文件名
            _currentFile = GetFilePath(typeof(T), DateTime.Now);

            // 再次写入文件，此时【当前文件】已不存在，会自动创建
            bool flag = FileHelper.AppendAllText(_currentFile, text, addNewLine, FileUtils.MaxLength);
            return flag ? 2 : 3;
        }
        else {
            return 1;
        }
    }

    public static int DeleteOldFile(string path, string extName)
    {
        // 每次写入至少存在一个文件，如果只保留一个文件就没有意义了
        if( FileUtils.MaxCount < 2 )
            return 0;

        // 如果最大保留5个文件，这里就修改为4个，因为后面马上要执行写入动作，会创建一个新文件，最终会是5个文件
        // 也就是说要 多删除 1件文件
        int maxCount = FileUtils.MaxCount - 1;

        // 先获取目录中的文件
        //string path = Path.GetDirectoryName(_currentFile);
        var files = (from f in Directory.GetFiles(path, "*" + extName, SearchOption.TopDirectoryOnly)
                     let f2 = new FileInfo(f)
                     orderby f2.LastWriteTime descending
                     select f2).ToList();

        // 删除当前目录中过旧的文件
        if( files.Count > maxCount ) {
            files = files.Skip(maxCount).ToList();

            foreach( var file in files ) {
                RetryFile.Delete(file.FullName);
            }

            return files.Count;
        }

        return 0;
    }


}
