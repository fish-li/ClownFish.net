﻿namespace ClownFish.Base;

/// <summary>
/// 提供一些与 System.IO.Directory 相同签名且功能相同的工具方法，
/// 差别在于：当出现IOException时，这个类中的方法支持重试功能。
/// </summary>
public static class RetryDirectory
{
    /// <summary>
    /// 等同于：System.IO.Directory.Delete()
    /// </summary>
    /// <param name="path"></param>
    /// <param name="recursive"></param>
    public static void Delete(string path, bool recursive = true)
    {
        if( Directory.Exists(path) == false )
            return;

        RetryFile.CreateRetry().Run(() => {
            Directory.Delete(path, recursive);
            return 1;
        });
    }

    /// <summary>
    /// 等同于：System.IO.Directory.CreateDirectory()
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static DirectoryInfo CreateDirectory(string path)
    {
        return RetryFile.CreateRetry().Run(() => {
            return Directory.CreateDirectory(path);
        });
    }

    /// <summary>
    /// 等同于：System.IO.Directory.GetFiles()
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern"></param>
    /// <param name="searchOption"></param>
    /// <returns></returns>
    public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
    {
        return RetryFile.CreateRetry().Run(() => {
            return Directory.GetFiles(path, searchPattern, searchOption);
        });
    }


    /// <summary>
    /// 等同于：System.IO.Directory.GetDirectories()
    /// </summary>
    /// <param name="path"></param>
    /// <param name="searchPattern"></param>
    /// <param name="searchOption"></param>
    /// <returns></returns>
    public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        return RetryFile.CreateRetry().Run(() => {
            return Directory.GetDirectories(path, searchPattern, searchOption);
        });
    }


    /// <summary>
    /// 等同于：System.IO.Directory.Exists()
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Exists(string path)
    {
        // 这个方法内部会吃掉所有异常，所以不做重试处理
        return Directory.Exists(path);
    }



    /// <summary>
    /// 等同于：System.IO.Directory.GetLastWriteTime()
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static DateTime GetLastWriteTime(string path)
    {
        return RetryFile.CreateRetry().Run(() => {
            return Directory.GetLastWriteTime(path);
        });
    }

}
