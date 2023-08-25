namespace ClownFish.Base;

/// <summary>
/// 提供一些与 System.IO.File 相同签名且功能相同的工具方法，
/// 差别在于：当出现IOException时，这个类中的方法支持重试功能。
/// </summary>
public static class RetryFile
{
    /// <summary>
    /// 等同于：System.IO.File.Exists()
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool Exists(string filePath)
    {
        // 由于 File.Exists 内部已经吃掉了很多异常，包含IOException，所以这里就不再对它重试。
        // 只是为了在代码中消灭 File.xxxxxxx 就提供了这个方法。
        return File.Exists(filePath);
    }


    // 本地磁盘 I/O 的重试参数
    // 这里不能调用 AppConfig.GetSetting，否则会产生循环调用: 
    /// AppConfig.GetSetting() => AppConfig.Init() => XmlHelper.XmlDeserializeFromFile() => RetryFile.OpenRead()
    //private static readonly int s_tryCount = AppConfig.GetSetting("ClownFish.Base.RetryFile.RetryCount").TryToUInt(5);
    //private static readonly int s_WaitMillisecond =  AppConfig.GetSetting("ClownFish.Base.RetryFile.WaitMillisecond").TryToUInt(500);

    internal static Retry CreateRetry()
    {
        // 重试策略：当发生 IOException 时，最大重试 5 次，每次间隔 500 毫秒
        return Retry.Create(Retry.Default.Count, Retry.Default.WaitMillisecond).Filter<IOException>();
    }

    /// <summary>
    /// 等同于：System.IO.File.Delete()，
    /// 但是会在删除文件前检查文件是否存在。
    /// </summary>
    /// <param name="filePath"></param>
    public static void Delete(string filePath)
    {
        if( File.Exists(filePath) == false )
            return;

        CreateRetry()
            .Filter<UnauthorizedAccessException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is UnauthorizedAccessException )
                    try {
                        File.SetAttributes(filePath, FileAttributes.Normal);
                    }
                    catch { // 这里就是一个尝试机制，所以如果出错就忽略这个异常
                    }
            })
            .Run(() => {
                File.Delete(filePath);
                return 1;
            });
    }

    /// <summary>
    /// 先判断文件是否存在，再读取文件所有内容
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string TryReadAllText(string filePath, Encoding encoding = null)
    {
        if( File.Exists(filePath) == false )
            return null;

        return ReadAllText(filePath, encoding);
    }


    /// <summary>
    /// 等同于：System.IO.File.ReadAllText()
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string ReadAllText(string filePath, Encoding encoding = null)
    {
        return CreateRetry().Run(() => {
            return File.ReadAllText(filePath, encoding.GetOrDefault());
        });
    }


    /// <summary>
    /// 等同于：System.IO.File.ReadAllLines()
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string[] ReadAllLines(string filePath, Encoding encoding = null)
    {
        return CreateRetry().Run(() => {
            return File.ReadAllLines(filePath, encoding.GetOrDefault());
        });
    }


    /// <summary>
    /// 等同于：System.IO.File.ReadAllBytes()
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static byte[] ReadAllBytes(string filePath)
    {
        return CreateRetry().Run(() => {
            return File.ReadAllBytes(filePath);
        });
    }


    private static void SafeCreateDirectory(string path)
    {
        try {
            Directory.CreateDirectory(path);
        }
        catch {// 这里就是一个尝试机制，所以如果出错就忽略这个异常
        }
    }

    /// <summary>
    /// 等同于：System.IO.File.WriteAllText()，且当目录不存在时自动创建。
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    public static void WriteAllText(string filePath, string text, Encoding encoding = null)
    {
        CreateRetry()
            .Filter<DirectoryNotFoundException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is DirectoryNotFoundException )
                    SafeCreateDirectory(Path.GetDirectoryName(filePath));
            })
            .Run(() => {
                File.WriteAllText(filePath, text, encoding.GetOrDefault());
                return 1;
            });
    }


    /// <summary>
    /// 等同于：System.IO.File.WriteAllBytes()，且当目录不存在时自动创建。
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="buffer"></param>
    public static void WriteAllBytes(string filePath, byte[] buffer)
    {
        CreateRetry()
            .Filter<DirectoryNotFoundException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is DirectoryNotFoundException )
                    SafeCreateDirectory(Path.GetDirectoryName(filePath));
            })
            .Run(() => {
                File.WriteAllBytes(filePath, buffer);
                return 1;
            });
    }


    /// <summary>
    /// 等同于：System.IO.File.AppendAllText()，且当目录不存在时自动创建。
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="text"></param>
    /// <param name="encoding"></param>
    public static void AppendAllText(string filePath, string text, Encoding encoding = null)
    {
        CreateRetry()
            .Filter<DirectoryNotFoundException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is DirectoryNotFoundException )
                    SafeCreateDirectory(Path.GetDirectoryName(filePath));
            })
            .Run(() => {
                File.AppendAllText(filePath, text, encoding.GetOrDefault());
                return 1;
            });
    }


    /// <summary>
    /// 等同于：System.IO.File.GetLastWriteTime()
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static DateTime GetLastWriteTime(string filePath)
    {
        return CreateRetry().Run(() => {

            // 下面这个调用即使文件不存在也不会引发异常，因为内部是这样实现的：
            // int num = FillAttributeInfo(fullPath, ref data, returnErrorOnNotFound: false);
            // 此时返回结果：1601-01-01 08:00:00

            return File.GetLastWriteTime(filePath);
        });
    }


    /// <summary>
    /// 等同于：System.IO.File.GetLastWriteTimeUtc()
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static DateTime GetLastWriteTimeUtc(string filePath)
    {
        return CreateRetry().Run(() => {

            // 下面这个调用即使文件不存在也不会引发异常，因为内部是这样实现的：
            // int num = FillAttributeInfo(fullPath, ref data, returnErrorOnNotFound: false);
            // 此时返回结果：1601-01-01 00:00:00

            return File.GetLastWriteTimeUtc(filePath);
        });
    }


    /// <summary>
    /// 等同于：System.IO.File.GetAttributes()
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static FileAttributes GetAttributes(string filePath)
    {
        return CreateRetry().Run(() => {
            return File.GetAttributes(filePath);
        });
    }

    /// <summary>
    /// 判断文件是否为隐藏文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static bool IsHidden(string filePath)
    {
        return RetryFile.GetAttributes(filePath).HasFlag(FileAttributes.Hidden);
    }


    /// <summary>
    /// 取消文件的只读设置
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static void ClearReadonly(string filePath)
    {
        FileAttributes attributes = RetryFile.GetAttributes(filePath);

        if( attributes.HasFlag(FileAttributes.ReadOnly) == false )
            return;

        // 清除只读属性
        attributes &= ~FileAttributes.ReadOnly;

        CreateRetry().Run(() => {
            File.SetAttributes(filePath, attributes);
            return 1;
        });
    }



    /// <summary>
    /// 等同于：System.IO.File.OpenRead()
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static FileStream OpenRead(string filePath)
    {
        return CreateRetry().Run(() => {
            return File.OpenRead(filePath);
        });
    }



    /// <summary>
    /// 等同于：System.IO.File.OpenWrite()，且当目录不存在时自动创建。
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static FileStream OpenWrite(string filePath)
    {
        return CreateRetry()
            .Filter<DirectoryNotFoundException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is DirectoryNotFoundException )
                    SafeCreateDirectory(Path.GetDirectoryName(filePath));
            })
            .Run(() => {
                return File.OpenWrite(filePath);
            });
    }


    /// <summary>
    /// 打开或者创建文件，后面以追加方式操作
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static FileStream OpenAppend(string filePath)
    {
        return CreateRetry()
            .Filter<DirectoryNotFoundException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is DirectoryNotFoundException )
                    SafeCreateDirectory(Path.GetDirectoryName(filePath));
            })
            .Run(() => {
                return new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.SequentialScan);
            });
    }


    /// <summary>
    /// 等同于：File.Create() ，且当目录不存在时自动创建。
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static FileStream Create(string filePath)
    {
        return CreateRetry()
            .Filter<DirectoryNotFoundException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is DirectoryNotFoundException )
                    SafeCreateDirectory(Path.GetDirectoryName(filePath));
            })
            .Run(() => {
                return File.Create(filePath);
            });
    }


    /// <summary>
    /// 等同于：System.IO.File.Copy()，且当目录不存在时自动创建。
    /// </summary>
    /// <param name="sourceFileName"></param>
    /// <param name="destFileName"></param>
    /// <param name="overwrite"></param>
    public static void Copy(string sourceFileName, string destFileName, bool overwrite = true)
    {
        //if( File.Exists(sourceFileName) == false )
        //    throw new FileNotFoundException("File not found: " + sourceFileName);

        CreateRetry()
            .Filter<DirectoryNotFoundException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is DirectoryNotFoundException )
                    SafeCreateDirectory(Path.GetDirectoryName(destFileName));
            })
            .Run(() => {
                File.Copy(sourceFileName, destFileName, overwrite);
                return 1;
            });
    }


    /// <summary>
    /// 等同于：System.IO.File.Move()，且当目录不存在时自动创建。
    /// </summary>
    /// <param name="sourceFileName"></param>
    /// <param name="destFileName"></param>
    public static void Move(string sourceFileName, string destFileName)
    {
        //if( File.Exists(sourceFileName) == false )
        //    throw new FileNotFoundException("File not found: " + sourceFileName);

        CreateRetry()
            .Filter<DirectoryNotFoundException>()
            .OnException((ex, n) => {
                if( n == 1 && ex is DirectoryNotFoundException )
                    SafeCreateDirectory(Path.GetDirectoryName(destFileName));
            })
            .Run(() => {
                File.Move(sourceFileName, destFileName);
                return 1;
            });
    }




}
