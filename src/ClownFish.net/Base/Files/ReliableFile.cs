namespace ClownFish.Base;

/// <summary>
/// 实现文件的可靠读取工具类。
/// 可靠性的要求：写-读，过程中，允许何意时刻断电，最终都可以提供一个可用的文件（最坏情况就是上一个版本）。
/// </summary>
public static class ReliableFile
{
    /// <summary>
    /// 文件内容的结尾标记，读取文件时要检查有没有这个标记。
    /// </summary>
    private static readonly string s_fileBodyEndFlag = "a725982063d746788cb5f599e4a4c52f";

    /// <summary>
    /// 临时文件的扩展名
    /// </summary>
    private static readonly string s_tempFileExtensionName = ".ce200fd3b8ec4860b17ffe1dd5b5c7a2";


    /// <summary>
    /// 写文件
    /// </summary>
    /// <param name="text"></param>
    /// <param name="filePath"></param>
    public static void Write(string text, string filePath)
    {
        if( text == null )
            throw new ArgumentNullException(nameof(text));

        if( string.IsNullOrEmpty(filePath) )
            throw new ArgumentNullException(nameof(filePath));


        // 写入文件时，要包含结束标记
        text = text + s_fileBodyEndFlag;

        if( RetryFile.Exists(filePath) ) {

            // 先将内容写入临时文件
            string tempPath = filePath + s_tempFileExtensionName;
            RetryFile.WriteAllText(tempPath, text, Encoding.UTF8);

            // 删除目标文件
            RetryFile.Delete(filePath);

            // 临时文件改名成目标文件
            RetryFile.Move(tempPath, filePath);
        }
        else {
            RetryFile.WriteAllText(filePath, text, Encoding.UTF8);
        }
    }

    /// <summary>
    /// 读文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string Read(string filePath)
    {
        if( string.IsNullOrEmpty(filePath) )
            throw new ArgumentNullException(nameof(filePath));



        // 先判断临时文件是否存在，
        // 如果存在这个文件，表示上次操作过程没有成功执行（例如突然断电）
        string tempPath = filePath + s_tempFileExtensionName;

        if( RetryFile.Exists(tempPath) ) {
            string text = RetryFile.ReadAllText(tempPath, Encoding.UTF8);
            if( string.IsNullOrEmpty(text) == false && text.EndsWith0(s_fileBodyEndFlag) ) {
                // 临时文件是完整的，先继续执行上次的操作
                // 注意：必须在返回前，先将内容写入目标文件，因为有可能刚刚返回时，又断电了！
                RetryFile.WriteAllText(filePath, text, Encoding.UTF8);
                RetryFile.Delete(tempPath);

                // 返回时将结束标记去掉
                return text.Substring(0, text.Length - s_fileBodyEndFlag.Length);
            }
            else {
                // 不完整的临时文件，删除它
                RetryFile.Delete(tempPath);
            }
        }


        // 读取目标文件
        string text2 = RetryFile.ReadAllText(filePath, Encoding.UTF8);
        if( text2.EndsWith0(s_fileBodyEndFlag) == false )
            //throw new InvalidOperationException($"读取文件 {filePath} 失败，没有发现预期的结束标记。");
            return null;

        // 返回时将结束标记去掉
        return text2.Substring(0, text2.Length - s_fileBodyEndFlag.Length);
    }

    /// <summary>
    /// 将一个对象做XML序列化后写入到文件
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="filePath"></param>
    public static void WriteObject(object obj, string filePath)
    {
        if( obj == null )
            throw new ArgumentNullException(nameof(obj));
        if( string.IsNullOrEmpty(filePath) )
            throw new ArgumentNullException(nameof(filePath));


        string xml = obj.ToXml();
        Write(xml, filePath);
    }


    /// <summary>
    /// 从文件中读取一个对象，使用XML反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static T ReadObject<T>(string filePath) where T : class
    {
        if( string.IsNullOrEmpty(filePath) )
            throw new ArgumentNullException(nameof(filePath));

        string xml = Read(filePath);
        if( string.IsNullOrEmpty(xml) )
            return null;


        return xml.FromXml<T>();
    }


}
