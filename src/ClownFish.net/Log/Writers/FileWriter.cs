namespace ClownFish.Log.Writers;

/// <summary>
/// 将日志记录到文件的写入器
/// </summary>
internal abstract class FileWriter : ILogWriter
{
    private string _currentFile;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="config"></param>
    /// <param name="section"></param>
    public virtual void Init(LogConfiguration config, WriterConfig section)
    {
        // 初始化目录
        FileUtils.InitDirectory(config);
    }


    /// <summary>
    /// 文件扩展名
    /// </summary>
    protected virtual string FileExtName => ".log";

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
    private string GetFilePath(Type type, DateTime time)
    {
        string datatype = type.Name;
        string timeString = time.ToString("yyyyMMdd_HHmmss");

        // 文件名示例：     /logs/OprLog/OprLog_20210126_171122.json.log				
        return string.Concat(FileUtils.RootPath, datatype, "/", datatype, "_", timeString, this.FileExtName);
    }


    /// <summary>
    /// 写入单条日志信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="info"></param>
    public virtual void Write<T>(T info) where T : class, IMsgObject
    {
        // 数据对象序列化
        string text = ObjectToText(info);

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
    public virtual void Write<T>(List<T> list) where T : class, IMsgObject
    {
        string block = null;
        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( T info in list ) {
                string line = ObjectToText(info);
                sb.AppendLine(line);
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
            // 注意：取类型名称时，不采用 info.GetType().Name ，因为可能有继承情况
            _currentFile = GetFilePath(typeof(T), DateTime.Now);
        }


        // 追加到文件，如果失败则表示超过文件最大长度
        if( FileHelper.AppendAllText(_currentFile, text, addNewLine, FileUtils.MaxLength) == false ) {

            // 清理老旧的文件
            DeleteOldFile();

            // 再次写入文件，此时【当前文件】已不存在，会自动创建
            _currentFile = GetFilePath(typeof(T), DateTime.Now);

            bool flag = FileHelper.AppendAllText(_currentFile, text, addNewLine, FileUtils.MaxLength);
            return flag ? 2 : 3;
        }
        else {
            return 1;
        }
    }

    private int DeleteOldFile()
    {
        // 每次写入至少存在一个文件，如果只保留一个文件就没有意义了
        if( FileUtils.MaxCount < 2 )
            return 0;

        // 如果最大保留5个文件，这里就修改为4个，因为后面马上要执行写入动作，会创建一个新文件，最终会是5个文件
        // 也就是说要 多删除 1件文件
        int maxCount = FileUtils.MaxCount - 1;

        // 先获取目录中的文件
        string path = Path.GetDirectoryName(_currentFile);
        var files = (from f in Directory.GetFiles(path, "*" + this.FileExtName, SearchOption.TopDirectoryOnly)
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
