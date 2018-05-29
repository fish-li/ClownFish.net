using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
	/// <summary>
	/// RSA算法（签名/验证签名/加密/解密）的封装工具类
	/// </summary>
	public static class RsaHelper
	{
		/// <summary>
		/// 用指定的证书名称对数据做签名
		/// </summary>
		/// <param name="data"></param>
		/// <param name="certName"></param>
		/// <returns></returns>
		public static string Sign(byte[] data, string certName)
		{
			if( data == null )
				return null;

			// 查找私钥，优先在计算机的证书存储中查找
			X509Certificate2 cert = FindCertificate(certName);

			return Sign(data, cert);
		}


		private static string Sign(byte[] data, X509Certificate2 cert)
		{
			if( cert.HasPrivateKey == false )
				throw new ArgumentException("指定的证书没有包含私钥：" + cert.Subject);

			RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;


			//计算数据哈希值
			SHA1Managed sha1 = new SHA1Managed();
			byte[] hash = sha1.ComputeHash(data);

			// 签名数据
			byte[] bb = rsa.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
			return Convert.ToBase64String(bb);
		}


		/// <summary>
		/// 验证RSA签名
		/// </summary>
		/// <param name="data"></param>
		/// <param name="signature"></param>
		/// <param name="publicKey"></param>
		/// <returns></returns>
		public static bool Verify(byte[] data, string signature, string publicKey)
		{
			// 这里参数是 publicKey，而不是 certname 主要是为了方便，
			// publicKey 可以写死在代码中

			if( data == null )
				return signature == null;

			// 公钥放在程序集中
			byte[] bb = Encoding.ASCII.GetBytes(publicKey.Trim());

            // 注意：下面这行代码在 .NET 3.5及以下版本中会有问题：每次会在临时目录下创建二个空文件，最后调用 Path.GetTempFilename() 时，当临时文件超过65536时会出现异常
            // 可参考以链接： https://blogs.msmvps.com/infinitec/2009/03/29/x509certificate2-constructor-creates-two-empty-files-in-the-temporary-files-directory/
            // 对应解决办法：
            // 1，用 byte[] 生成临时文件，文件临时文件中构造X509Certificate2实例
            // 2，创建临时文件时，计算publicKey的sha1值，用来做临时文件名，用完后不删除，就当是文件缓存了

            X509Certificate2 cert = new X509Certificate2(bb);

			if( cert.HasPrivateKey )        // 增加这个检查可以防止把私钥写到字符串中，从而泄露私钥
				throw new ArgumentException("字符串证书中不允许包含私钥！");

			return Verify(data, signature, cert);
		}

		/// <summary>
		/// 验证RSA签名
		/// </summary>
		/// <param name="data"></param>
		/// <param name="signature"></param>
		/// <param name="cert"></param>
		/// <returns></returns>
		public static bool Verify(byte[] data, string signature, X509Certificate2 cert)
		{
			// 获得证书公钥
			RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;

			// 哈希数据
			SHA1Managed sha1 = new SHA1Managed();
			byte[] hash = sha1.ComputeHash(data);


			// 验证哈希签名
			byte[] bb = Convert.FromBase64String(signature);
			return rsa.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), bb);
		}

		/// <summary>
		/// 根据证书名查找X509证书，优先查找LocalMachine存储区域，如果失败则再查找CurrentUser
		/// </summary>
		/// <param name="certName"></param>
		/// <returns></returns>
		public static X509Certificate2 FindCertificate(string certName)
		{
			// 先查找 LocalMachine
			X509Certificate2 cert = FindCertificate(certName, StoreName.My, StoreLocation.LocalMachine);
			if( cert == null ) {

				// 再查找  CurrentUser
				cert = FindCertificate(certName, StoreName.My, StoreLocation.CurrentUser);

				if( cert == null )
					throw new ArgumentException($"加密证书{certName}不存在。");
			}

			return cert;
		}

		private static X509Certificate2 FindCertificate(string certName, StoreName storeName, StoreLocation storeLocation)
		{
			// 查找这个存储区域，是与生成证书所使用的命令行对应的： -ss my -sr localMachine
			X509Store x509Store = new X509Store(storeName, storeLocation);
			try {
				x509Store.Open(OpenFlags.ReadOnly);
				string subjectName = "CN=" + certName;

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
		/// RSA数据加密
		/// </summary>
		/// <param name="data">二进制数据</param>
		/// <param name="certName">证书名称</param>
		/// <returns>加密后的数据</returns>
		public static byte[] Encrypt(byte[] data, string certName)
		{
			if( data == null )
				return null;

			// 私钥存在在计算机的证书存储中
			X509Certificate2 cert = FindCertificate(certName);
			if( cert == null )
				throw new ArgumentException($"加密证书{certName}不存在。");

			// 获得证书公钥
			RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PublicKey.Key;

			// 注意：这个方法只能加密比较短的内容（一般是密钥）
			return rsa.Encrypt(data, true);
		}

		/// <summary>
		/// 用X509证书解密数据
		/// </summary>
		/// <param name="data"></param>
		/// <param name="certName"></param>
		/// <returns></returns>
		public static byte[] Decrypt(byte[] data, string certName)
		{
			if( data == null )
				return null;

			// 私钥存在在计算机的证书存储中
			X509Certificate2 cert = FindCertificate(certName);
			if( cert == null )
				throw new ArgumentException($"加密证书{certName}不存在。");

			// 读取证书私钥
			RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert.PrivateKey;
			if( rsa == null )
				throw new ArgumentException("证书没有私钥。");

			return rsa.Decrypt(data, true);
		}
	}
}
