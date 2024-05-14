namespace ClownFish.Base;

/// <summary>
/// 用于处理临时文件的工具类
/// </summary>
public static class TempFilesUtils
{
    /// <summary>
    /// 每一次的删除操作的执行结果
    /// </summary>
    public sealed class DeleteResult
    {
        /// <summary>
        /// 文件或目录的全路径
        /// </summary>
        public string FullPath { get; set; }
        /// <summary>
        /// FileInfo对象，仅当删除文件时会指定
        /// </summary>
        public FileInfo FileInfo { get; set; }
        /// <summary>
        /// 删除过程中出现的异常，如果没有异常则为null
        /// </summary>
        public Exception Exception { get; set; }
    }


    /// <summary>
    /// 删除临时文件
    /// </summary>
    /// <param name="path">要执行删除的根目录</param>
    /// <param name="timeAgo">一个时间间隔，表示需要删除多久前的文件</param>
    /// <param name="topDirectoryOnly">是否只扫描指定的根目录（不包含子目录），如果需要扫描子目录，请指定为 false</param>
    /// <returns></returns>
    public static List<DeleteResult> DeleteOldFiles(string path, TimeSpan timeAgo, bool topDirectoryOnly)
    {
        if( string.IsNullOrEmpty(path) || Directory.Exists(path) == false )
            return new List<DeleteResult>();


        DateTime now = DateTime.Now;
        SearchOption searchOption = topDirectoryOnly
                                        ? SearchOption.TopDirectoryOnly
                                        : SearchOption.AllDirectories;

        List<DeleteResult> resultList = new List<DeleteResult>(128);

        IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", searchOption);

        foreach( string file in files ) {

            // 清除过程中，也有可能其它进程正在删除文件，所有文件不存在就忽略
            if( RetryFile.Exists(file) == false )
                continue;

            // 以文件的最后修改时间做为对比标准
            DateTime time = RetryFile.GetLastWriteTime(file);
            TimeSpan span = now - time;

            // 删除 指定时间 前的文件
            if( span >= timeAgo ) {

                // 清除过程中，也有可能其它进程正在删除文件，所有文件不存在就忽略
                if( RetryFile.Exists(file) == false )
                    continue;

                DeleteResult deleteResult = new DeleteResult();
                try {
                    deleteResult.FileInfo = new FileInfo(file);
                    RetryFile.Delete(file);
                    deleteResult.FullPath = file;
                }
                catch( Exception ex ) {
                    deleteResult.Exception = ex;
                }
                resultList.Add(deleteResult);
            }
        }

        return resultList;
    }

    /// <summary>
    /// 删除空子目录
    /// </summary>
    /// <param name="path">要执行删除的根目录</param>
    /// <param name="timeAgo">一个时间间隔，表示需要删除多久前的空子目录</param>
    public static List<DeleteResult> DeleteEmptyDirectories(string path, TimeSpan timeAgo)
    {
        if( string.IsNullOrEmpty(path) || Directory.Exists(path) == false )
            return new List<DeleteResult>();


        DateTime now = DateTime.Now;

        List<DeleteResult> resultList = new List<DeleteResult>(128);

        // 删除空子目录时，固定扫描所有子目录
        SearchOption searchOption = SearchOption.AllDirectories;

        IEnumerable<string> dirs = Directory.EnumerateDirectories(path, "*.*", searchOption);

        foreach( string dir in dirs ) {

            // 清除过程中，也有可能其它进程正在删除，所有不存在就忽略
            if( RetryDirectory.Exists(dir) == false )
                continue;


            // 以目录的最后修改时间做为对比标准
            DateTime time = RetryDirectory.GetLastWriteTime(dir);
            TimeSpan span = now - time;

            // 删除 指定时间 前的目录
            if( span >= timeAgo ) {

                // 判断目录是否为空（只要不包含文件，就认为是空目录，即使包含空的子目录）
                if( DirectoryIsEmpty(dir) == false )
                    continue;

                DeleteResult deleteResult = new DeleteResult();
                try {
                    // 删除目录及其空子目录
                    RetryDirectory.Delete(dir, true);
                    deleteResult.FullPath = dir;
                }
                catch( Exception ex ) {
                    deleteResult.Exception = ex;
                }
                resultList.Add(deleteResult);
            }
        }

        return resultList;
    }

    private static bool DirectoryIsEmpty(string path)
    {
        IEnumerable<string> items = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
        return items.Any() == false;
    }

}
