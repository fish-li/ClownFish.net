namespace ClownFish.Base.DEMO;

class ZipHelper_DEMO
{
    public void 将ZIP文件释放到指定目录()
    {
        string zipPath = @"D:\my-github\Snakehead.Cbdmt\src\Snakehead.Cbdmt\bin\bin.zip";
        string destPath = @"D:\test";

        ZipHelper.ExtractFiles(zipPath, destPath);
    }



    public void 在内存中读取ZIP文件()
    {
        string zipPath = @"D:\my-github\Snakehead.Cbdmt\src\Snakehead.Cbdmt\bin\bin.zip";

        var data = ZipHelper.Read(zipPath);

        // 结果是一个元组LIST
        // 每个元组中，Item1 就是压缩包内的文件名，Item2 就是文件内容。
        // 如果需要读取某个文件，可以从LIST中查找
    }

    public void 将指定目录打包成ZIP文件_含子目录()
    {
        string srcPath = @"D:\test";
        string zipPath = @"D:\my-github\Snakehead.Cbdmt\src\Snakehead.Cbdmt\bin\bin.zip";

        ZipHelper.CompressDirectory(srcPath, zipPath);
    }


    public void 将内存数据打包成ZIP文件()
    {
        List<ZipItem> data = null;    // 需要准备好这个结构

        string zipPath = @"D:\my-github\Snakehead.Cbdmt\src\Snakehead.Cbdmt\bin\bin.zip";

        ZipHelper.Compress(data, zipPath);
    }

}
