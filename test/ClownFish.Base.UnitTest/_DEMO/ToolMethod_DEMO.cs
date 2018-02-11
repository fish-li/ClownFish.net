using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Base.UnitTest._DEMO
{
    class ToolMethod_DEMO
    {
        // 这里演示一些杂类的工具方法

        public void BASE64()
        {
            string str1 = "aaaaaaaaaaaaaaaaaa";

            // 字符串 => BASE64
            string base64 = str1.ToBase64();

            // BASE64 => 字符串
            string str2 = base64.FromBase64();

            Assert.AreEqual(str1, str2);
        }


        public void Aes加密()
        {
            string str1 = "aaaaaaaaaaaaaaaaaa";
            string password = "@#$%@#$%$#^%";

            // 加密
            string base64 = AesHelper.Encrypt(str1, password);

            // 解密
            string str2 = AesHelper.Decrypt(base64, password);

            Assert.AreEqual(str1, str2);
        }



        public void Gzip压缩字符串()
        {
            string str1 = "aaaaaaaaaaaaaaaaaa";

            // 压缩字符串
            string base64 = GzipHelper.Compress(str1);

            // 解压缩
            string str2 = GzipHelper.Decompress(base64);

            Assert.AreEqual(str1, str2);
        }



        public void 常用HASH算法()
        {
            string str1 = "aaaaaaaaaaaaaaaaaa";

            // 计算字符串的 sha1
            string sha1 = str1.Sha1();

            // 计算【小文件】的 sha1
            sha1 = HashHelper.FileSha1(@"c:\aa.txt");



            // 计算字符串的 md5
            string md5 = str1.Md5();

            // 计算【小文件】的 md5
            md5 = HashHelper.FileMD5(@"c:\aa.txt");
        }



    }
}
