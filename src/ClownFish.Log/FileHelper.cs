using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;

namespace ClownFish.Log
{
    internal static class FileHelper
    {
        public static void AppendAllText(string filePath, string text, Encoding encoding, long maxLength)
        {
            using( FileStream file = RetryFile.OpenAppend(filePath) ) {

                // 如果文件已超过指定长度，就不再写入
                if( maxLength > 0 && file.Position >= maxLength )
                    return;

                // 注意：这样写出来的文件没有BOM头
                byte[] bb = encoding.GetBytes(text);
                file.Write(bb, 0, bb.Length);
            }
        }

    }
}
