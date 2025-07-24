namespace ClownFish.Base;

/// <summary>
/// 文件相关操作的工具类
/// </summary>
public static class FileHelper
{
    private static readonly int s_bufferSize = 4 * 1024;

    /// <summary>
    /// 将一个流写入到文件
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="savePath"></param>
    public static void SaveToFile(this Stream stream, string savePath)
    {
        if( stream == null )
            throw new ArgumentNullException(nameof(stream));
        if( savePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(savePath));


        if( stream.CanSeek )
            stream.Position = 0;

        using( FileStream file = RetryFile.Create(savePath) ) {

            using( ByteBuffer byteBuffer = new ByteBuffer(s_bufferSize) ) {
                byte[] buffer = byteBuffer.Buffer;
                int len = 0;
                while( (len = stream.Read(buffer, 0, buffer.Length)) > 0 ) {
                    file.Write(buffer, 0, len);
                }

                file.Flush(true);
            }
        }
    }




    /// <summary>
    /// 加密文件
    /// </summary>
    /// <param name="srcFilePath"></param>
    /// <param name="destFilePath"></param>
    /// <param name="password"></param>
    [SuppressMessage("Microsoft.Usage", "CA2202:不要多次释放对象")]
    public static void EncryptFile(string srcFilePath, string destFilePath, string password)
    {
        if( RetryFile.Exists(srcFilePath) == false )
            throw new FileNotFoundException("文件不存在：" + srcFilePath);
        if( string.IsNullOrEmpty(destFilePath) )
            throw new ArgumentNullException(nameof(destFilePath));
        if( string.IsNullOrEmpty(password) )
            throw new ArgumentNullException(nameof(password));


        RetryFile.Delete(destFilePath);

        using( Aes aes = Aes.Create() ) {

            CryptoHelper.SetKeyIV(aes, password);
            var encryptor = aes.CreateEncryptor();

            using( FileStream fileIn = RetryFile.OpenRead(srcFilePath) ) {

                using( FileStream fileOut = RetryFile.Create(destFilePath) ) {
                    using( CryptoStream cryptSteam = new CryptoStream(fileOut, encryptor, CryptoStreamMode.Write) ) {

                        int num = 0;
                        using( ByteBuffer byteBuffer = new ByteBuffer(s_bufferSize) ) {
                            byte[] buffer = byteBuffer.Buffer;

                            do {
                                num = fileIn.Read(buffer, 0, buffer.Length);
                                if( num > 0 ) {
                                    cryptSteam.Write(buffer, 0, num);
                                }
                            }
                            while( num > 0 );

                            cryptSteam.Flush();
                            fileOut.Flush(true);
                        }
                    }
                }
            }
        }
    }



    /// <summary>
    /// 解密文件
    /// </summary>
    /// <param name="srcFilePath"></param>
    /// <param name="destFilePath"></param>
    /// <param name="password"></param>
    [SuppressMessage("Microsoft.Usage", "CA2202:不要多次释放对象")]
    public static void DecryptFile(string srcFilePath, string destFilePath, string password)
    {
        if( RetryFile.Exists(srcFilePath) == false )
            throw new FileNotFoundException("文件不存在：" + srcFilePath);
        if( string.IsNullOrEmpty(destFilePath) )
            throw new ArgumentNullException(nameof(destFilePath));
        if( string.IsNullOrEmpty(password) )
            throw new ArgumentNullException(nameof(password));

        RetryFile.Delete(destFilePath);

        using( Aes aes = Aes.Create() ) {

            CryptoHelper.SetKeyIV(aes, password);

            var decryptor = aes.CreateDecryptor();

            using( FileStream fileIn = RetryFile.OpenRead(srcFilePath) ) {
                using( CryptoStream cryptSteam = new CryptoStream(fileIn, decryptor, CryptoStreamMode.Read) ) {

                    using( FileStream fileOut = RetryFile.Create(destFilePath) ) {

                        int num = 0;
                        using( ByteBuffer byteBuffer = new ByteBuffer(s_bufferSize) ) {
                            byte[] buffer = byteBuffer.Buffer;

                            do {
                                num = cryptSteam.Read(buffer, 0, buffer.Length);
                                if( num > 0 ) {
                                    fileOut.Write(buffer, 0, num);
                                }
                            }
                            while( num > 0 );

                            fileOut.Flush(true);
                        }
                    }
                }
            }
        }
    }



    /// <summary>
    /// 给文件追加一段内容。
    /// 注意：写文件时固定采用 带 BOM 头的 UTF-8 编码。
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="text"></param>
    /// <param name="addNewLine"></param>
    /// <param name="maxLength"></param>
    /// <returns>如果成功写入返回true，如果失败(文件长度超过最大长度)返回false</returns>
    public static bool AppendAllText(string filePath, string text, bool addNewLine, long maxLength)
    {
        Encoding encoding = Encoding.UTF8;

        using( FileStream file = RetryFile.OpenAppend(filePath) ) {

            // 如果文件已超过指定长度，就不再写入
            // 说明：text.Length 这个取值只是粗咯的估计，真正的长度是下面的 bb.Length
            if( maxLength > 0  // 启用长度检查
                && file.Position > 0 // 文件不空，允许追加。 排除一种特殊场景：text.Length >= maxLength
                && file.Position + text.Length >= maxLength )
                return false;


            if( file.Position == 0 ) {
                // 写入 BOM
                byte[] preamble = encoding.GetPreamble();
                if( preamble.Length != 0 ) {
                    file.Write(preamble, 0, preamble.Length);
                }
            }

            byte[] bb = encoding.GetBytes(text);
            file.Write(bb, 0, bb.Length);

            if( addNewLine ) {
                byte[] cc = encoding.GetBytes(System.Environment.NewLine);
                file.Write(cc, 0, cc.Length);
            }

            file.Flush(true);
        }
        return true;
    }


    ///// <summary>
    ///// 读取文本文件的尾部
    ///// </summary>
    ///// <param name="filePath">文本文件的路径</param>
    ///// <param name="maxRows">需要读取的最大行数</param>
    ///// <param name="encoding">文本编码方式</param>
    ///// <returns></returns>
    //public static string ReadFileTails(string filePath, int maxRows, Encoding encoding = null)
    //{
    //    encoding = encoding ?? Encoding.UTF8;

    //    using( FileStream file = RetryFile.OpenRead(filePath) ) {

    //        if( maxRows > 0 ) {
    //            file.Seek(0, SeekOrigin.End);

    //            while( file.Position > 0 && maxRows > 0 ) {

    //                file.Seek(-1, SeekOrigin.Current);

    //                // 读取一个字节，判断是不是“换行符”，注意：此时 file.Position 会 +1，所以要在读完后回退一位
    //                char c = (char)file.ReadByte();
    //                file.Seek(-1, SeekOrigin.Current);

    //                if( c == '\n' ) {
    //                    maxRows--;

    //                    if( maxRows == 0 ) {
    //                        // 跳过当前 “换行符”
    //                        file.Seek(1, SeekOrigin.Current);
    //                    }
    //                }
    //            }               
    //        }

    //        using( StreamReader reader = new StreamReader(file, encoding) ) {
    //            return reader.ReadToEnd();
    //        }
    //    }
    //}


    /// <summary>
    /// 读取文本文件的尾部
    /// </summary>
    /// <param name="filePath">文本文件的路径</param>
    /// <param name="maxRows">需要读取的最大行数</param>
    /// <param name="encoding">文本编码方式</param>
    /// <returns></returns>
    public static string ReadFileTails(string filePath, int maxRows, Encoding encoding = null)
    {
        encoding = encoding ?? Encoding.UTF8;

        if( maxRows <= 0 )
            return RetryFile.ReadAllText(filePath, encoding);
        

        using( FileStream fileStream = RetryFile.OpenRead(filePath) ) {

            const int bufferSize = 4096;
            byte[] buffer = new byte[bufferSize];
            long position = fileStream.Length;

            using( StreamReader reader = new StreamReader(fileStream, encoding) ) {
                while( position > 0 && maxRows > 0 ) {

                    long readSize = Math.Min(bufferSize, position);
                    position -= readSize;
                    fileStream.Seek(position, SeekOrigin.Begin);
                    int bytesRead = fileStream.Read(buffer, 0, (int)readSize);

                    // 反向读取缓冲区中的内容
                    for( int i = bytesRead - 1; i >= 0; i-- ) {
                        if( buffer[i] == '\n' ) {
                            maxRows--;
                            if( maxRows <= 0 ) {
                                // 跳过当前行的换行符
                                fileStream.Seek(position + i + 1, SeekOrigin.Begin);
                                break;
                            }
                        }
                    }

                    // 如果已经找到所需的行数，则开始读取数据
                    if( maxRows <= 0 ) {
                        return reader.ReadToEnd();
                    }
                }

                // 如果文件中的行数不足，则读取整个文件
                fileStream.Seek(0, SeekOrigin.Begin);
                return reader.ReadToEnd();
            }
        }
    }


    /// <summary>
    /// 获取文件的版本号
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileVersion(string filePath)
    {
        try {
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(filePath);
            return fileVersion.FileVersion;
        }
        catch {
            return "0.0.0.0";
        }
    }

}
