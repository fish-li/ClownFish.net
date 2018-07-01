using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
    /// <summary>
    /// 包含一些查找X509证书的工具方法
    /// </summary>
    public static class X509Finder
    {
        /// <summary>
        /// 根据证书主题查找X509证书，优先查找LocalMachine存储区域，如果失败则再查找CurrentUser
        /// </summary>
        /// <param name="subject">证书主题，例如： CN=name123，那么这个参数就是 name123</param>
        /// <param name="ifNotFoundThrowException">如果没有找到证书是否需要抛出异常</param>
        /// <returns></returns>
        public static X509Certificate2 FindBySubject(string subject, bool ifNotFoundThrowException)
        {
            // 先查找 LocalMachine
            X509Certificate2 cert = FindBySubject(subject, StoreLocation.LocalMachine, StoreName.My);
            if( cert == null ) {

                // 再查找  CurrentUser
                cert = FindBySubject(subject, StoreLocation.CurrentUser, StoreName.My);
            }

            if( cert == null && ifNotFoundThrowException )
                throw new ArgumentException($"不能根据指定的证书主题 {subject} 找到匹配的证书。");

            return cert;
        }




        /// <summary>
        /// 根据指定的证书主题和位置，查找证书。
        /// </summary>
        /// <param name="subject">证书主题，例如： CN=name123，那么这个参数就是 name123</param>
        /// <param name="storeLocation"></param>
        /// <param name="storeName"></param>
        /// <returns></returns>
        public static X509Certificate2 FindBySubject(string subject, 
                                                        StoreLocation storeLocation = StoreLocation.LocalMachine, 
                                                        StoreName storeName = StoreName.My)
        {
            X509Store x509Store = new X509Store(storeName, storeLocation);
            try {
                x509Store.Open(OpenFlags.ReadOnly);
                string subjectName = "CN=" + subject;

                foreach( X509Certificate2 current in x509Store.Certificates )
                    if( current.Subject == subjectName )
                        return current;
            }
            finally {
                x509Store.Close();
            }
            return null;
        }


        /// <summary>
        /// 根据证书指纹查找X509证书，优先查找LocalMachine存储区域，如果失败则再查找CurrentUser
        /// </summary>
        /// <param name="thumbprint">证书指纹</param>
        /// <param name="ifNotFoundThrowException">如果没有找到证书是否需要抛出异常</param>
        /// <returns></returns>
        public static X509Certificate2 FindByThumbprint(string thumbprint, bool ifNotFoundThrowException)
        {
            // 先查找 LocalMachine
            X509Certificate2 cert = FindByThumbprint(thumbprint, StoreLocation.LocalMachine, StoreName.My);
            if( cert == null ) {

                // 再查找  CurrentUser
                cert = FindByThumbprint(thumbprint, StoreLocation.CurrentUser, StoreName.My);
            }

            if( cert == null && ifNotFoundThrowException )
                throw new ArgumentException($"不能根据指定的证书证书指纹 {thumbprint} 找到匹配的证书。");

            return cert;
        }





        /// <summary>
        /// 根据指定的证书指纹和位置，查找证书。
        /// </summary>
        /// <param name="thumbprint">证书指纹</param>
        /// <param name="storeLocation"></param>
        /// <param name="storeName"></param>
        /// <returns></returns>
        public static X509Certificate2 FindByThumbprint(string thumbprint,
                                                        StoreLocation storeLocation = StoreLocation.LocalMachine,
                                                        StoreName storeName = StoreName.My)
        {
            X509Store x509Store = new X509Store(storeName, storeLocation);
            try {
                x509Store.Open(OpenFlags.ReadOnly);

                foreach( X509Certificate2 current in x509Store.Certificates )
                    if( current.Thumbprint == thumbprint )
                        return current;
            }
            finally {
                x509Store.Close();
            }
            return null;
        }


        /// <summary>
        /// 从一个公钥字符串中加载X509证书
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadFromPublicKey(string publicKey)
        {
            if( string.IsNullOrEmpty(publicKey) )
                throw new ArgumentNullException(nameof(publicKey));


            byte[] bb = Encoding.ASCII.GetBytes(publicKey.Trim());

            // 注意：下面这行代码在 .NET 3.5及以下版本中会有问题：每次会在临时目录下创建二个空文件，最后调用 Path.GetTempFilename() 时，当临时文件超过65536时会出现异常
            // 可参考以链接： https://blogs.msmvps.com/infinitec/2009/03/29/x509certificate2-constructor-creates-two-empty-files-in-the-temporary-files-directory/
            // 对应解决办法：
            // 1，用 byte[] 生成临时文件，文件临时文件中构造X509Certificate2实例
            // 2，创建临时文件时，计算publicKey的sha1值，用来做临时文件名，用完后不删除，就当是文件缓存了

            X509Certificate2 cert = new X509Certificate2(bb);

            if( cert.HasPrivateKey )        // 增加这个检查可以防止把私钥写到字符串中，从而泄露私钥
                throw new ArgumentException("公钥证书中不允许包含私钥！");

            return cert;
        }


    }
}
