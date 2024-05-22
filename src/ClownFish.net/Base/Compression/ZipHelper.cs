namespace ClownFish.Base;

/// <summary>
/// 表示一个ZIP包内结构
/// </summary>
public sealed class ZipItem
{
    /// <summary>
    /// ZIP包内的文件名，含相对路径。
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// 压缩时，表示要压缩的文件内容。
    /// 解压时，表示解压后的文件内容。
    /// </summary>
    public byte[] Body { get; set; }

    /// <summary>
    /// 仅用于压缩操作，表示要将一个本地文件，它即将用于压缩（只读取）。
    /// </summary>
    public string LocalFilePath { get; set; }
}



/// <summary>
/// 提供一些操作ZIP文件的工具方法
/// </summary>
public static class ZipHelper
{
    /// <summary>
    /// 将ZIP文件解压缩到指定的目录
    /// </summary>
    /// <param name="zipPath">需要解压缩的ZIP文件</param>
    /// <param name="extractPath">要释放的目录</param>
    public static void ExtractFiles(string zipPath, string extractPath)
    {
        //ZipFile.ExtractToDirectory(zipPath, extractPath);
        // .net framework 的实现版本中，如果要释放的文件已存在，会抛出异常，所以这里就不使用了。


        if( string.IsNullOrEmpty(zipPath) )
            throw new ArgumentNullException("zipPath");
        if( string.IsNullOrEmpty(extractPath) )
            throw new ArgumentNullException("extractPath");

        using( ZipArchive archive = ZipFile.OpenRead(zipPath) ) {
            foreach( ZipArchiveEntry entry in archive.Entries ) {

                // 处理目录节点
                if( entry.FullName.EndsWith('/') ) {
                    string path = Path.Combine(extractPath, entry.FullName.TrimEnd('/'));
                    if( Directory.Exists(path) == false )
                        Directory.CreateDirectory(path);

                    continue;
                }


                // 计算要释放到哪里
                string targetFile = Path.Combine(extractPath, entry.FullName);

                // 如果要释放的目标目录不存在，就创建目标
                string destPath = Path.GetDirectoryName(targetFile);
                Directory.CreateDirectory(destPath);

                // 如果目标文件已经存在，就删除
                RetryFile.Delete(targetFile);

                // 释放文件
                entry.ExtractToFile(targetFile, true);
            }
        }
    }

    /// <summary>
    /// 读取ZIP到内存中
    /// </summary>
    /// <param name="zipPath">需要读取的ZIP文件</param>
    /// <returns></returns>
    public static List<ZipItem> Read(string zipPath)
    {
        if( string.IsNullOrEmpty(zipPath) )
            throw new ArgumentNullException("zipPath");

        using( ZipArchive archive = ZipFile.OpenRead(zipPath) ) {
            return Read(archive);
        }
    }


    /// <summary>
    /// 读取ZIP到内存中
    /// </summary>
    /// <param name="archive"></param>
    /// <returns></returns>
    public static List<ZipItem> Read(ZipArchive archive)
    {
        List<ZipItem> result = new List<ZipItem>();

        foreach( ZipArchiveEntry entry in archive.Entries ) {

            // 忽略目录节点
            if( entry.FullName.EndsWith('/') )
                continue;


            // 读取某个文件内容
            // 由于读取过程涉及解压缩，所以不能一次到 byte[]，只好引入一个内存流
            using( MemoryStream ms = MemoryStreamPool.GetStream() ) {
                using( Stream steam = entry.Open() ) {
                    steam.CopyTo(ms);
                }
                ms.Position = 0;

                ZipItem item = new ZipItem { FullName = entry.FullName, Body = ms.ToArray() };
                result.Add(item);
            }
        }

        return result;
    }


    /// <summary>
    /// 将指定的目录打包成ZIP文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="zipPath"></param>
    public static void CompressDirectory(string path, string zipPath)
    {
        RetryFile.Delete(zipPath);

        // 直接调用 .NET framework
        ZipFile.CreateFromDirectory(path, zipPath);
    }



    /// <summary>
    /// 根据指定的包内文件名及对应的文件内容打包成ZIP流
    /// </summary>
    /// <param name="files"></param>
    /// <param name="zipStream"></param>
    [SuppressMessage("Microsoft.Usage", "CA2202")]
    public static void Compress(List<ZipItem> files, Stream zipStream)
    {
        if( files == null )
            throw new ArgumentNullException(nameof(files));
        if( zipStream == null )
            throw new ArgumentNullException(nameof(zipStream));


        using( ZipArchive zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true, Encoding.UTF8) ) {

            foreach( ZipItem item in files ) {

                var entry = zip.CreateEntry(item.FullName, CompressionLevel.Optimal);

                if( item.LocalFilePath.IsNullOrEmpty() == false ) {
                    using( var stream = entry.Open() ) {
                        using( FileStream fs = RetryFile.OpenRead(item.LocalFilePath) ) {
                            fs.CopyTo(stream);
                        }
                    }
                }
                else if( item.Body != null ) {
                    using( BinaryWriter writer = new BinaryWriter(entry.Open()) ) {
                        writer.Write(item.Body);
                    }
                }
                else {
                    // 暂且忽略错误的参数吧。
                }
            }
        }
    }



    /// <summary>
    /// 根据指定的包内文件名及对应的文件内容打包成ZIP文件
    /// </summary>
    /// <param name="files"></param>
    /// <param name="zipPath"></param>
    [SuppressMessage("Microsoft.Usage", "CA2202")]
    public static void Compress(List<ZipItem> files, string zipPath)
    {
        if( files == null )
            throw new ArgumentNullException(nameof(files));
        if( string.IsNullOrEmpty(zipPath) )
            throw new ArgumentNullException(nameof(zipPath));


        RetryFile.Delete(zipPath);


        using( FileStream fileStream = RetryFile.Create(zipPath) ) {

            Compress(files, fileStream);
        }
    }


    /// <summary>
    /// 创建一个ZIP文件，并将文本做为唯一的文件添加到压缩包
    /// </summary>
    /// <param name="text">将要被压缩的文本</param>
    /// <param name="innerFileName"></param>
    /// <returns></returns>
    public static MemoryStream CreateZipFromText(string text, string innerFileName = null)
    {
        if( text.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(text));

        if( innerFileName.IsNullOrEmpty() )
            innerFileName = "file1";

        ZipItem file1 = new ZipItem { FullName = innerFileName, Body = Encoding.UTF8.GetBytes(text) };
        List<ZipItem> files = new List<ZipItem> { file1 };

        MemoryStream zipStream = new MemoryStream();
        ZipHelper.Compress(files, zipStream);
        return zipStream;
    }

}
