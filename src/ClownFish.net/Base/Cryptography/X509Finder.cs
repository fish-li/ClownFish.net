namespace ClownFish.Base;

/// <summary>
/// 包含一些查找X509证书的工具方法
/// </summary>
public static class X509Finder
{
    /// <summary>
    /// 根据证书主题查找X509证书，优先查找LocalMachine存储区域，如果失败则再查找CurrentUser
    /// </summary>
    /// <param name="certSubject">证书主题，例如： CN=name123，那么这个参数就是 name123</param>
    /// <param name="ifNotFoundThrowException">如果没有找到证书是否需要抛出异常</param>
    /// <returns></returns>
    public static X509Certificate2 FindBySubject(string certSubject, bool ifNotFoundThrowException)
    {
        if( certSubject.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(certSubject));

        // 先查找 LocalMachine
        X509Certificate2 cert = FindBySubject(certSubject, StoreLocation.LocalMachine, StoreName.My);
        if( cert == null ) {

            // 再查找  CurrentUser
            cert = FindBySubject(certSubject, StoreLocation.CurrentUser, StoreName.My);
        }

        if( cert == null && ifNotFoundThrowException )
            throw new ArgumentException($"不能根据指定的证书主题 {certSubject} 找到匹配的证书。");

        return cert;
    }




    /// <summary>
    /// 根据指定的证书主题和位置，查找证书。
    /// </summary>
    /// <param name="certSubject">证书主题，例如： CN=name123，那么这个参数就是 name123</param>
    /// <param name="storeLocation"></param>
    /// <param name="storeName"></param>
    /// <returns></returns>
    public static X509Certificate2 FindBySubject(string certSubject,
                                                    StoreLocation storeLocation = StoreLocation.LocalMachine,
                                                    StoreName storeName = StoreName.My)
    {
        if( certSubject.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(certSubject));

        X509Store x509Store = new X509Store(storeName, storeLocation);
        try {
            x509Store.Open(OpenFlags.ReadOnly);
            string subjectName = "CN=" + certSubject;

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
        if( thumbprint.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(thumbprint));

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
        if( thumbprint.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(thumbprint));

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
    public static X509Certificate2 LoadFromPublicKey(byte[] publicKey)
    {
        if( publicKey.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(publicKey));


        // 注意：下面这行代码在 .NET 3.5及以下版本中会有问题：每次会在临时目录下创建二个空文件，最后调用 Path.GetTempFilename() 时，当临时文件超过65536时会出现异常
        // 可参考以链接： https://blogs.msmvps.com/infinitec/2009/03/29/x509certificate2-constructor-creates-two-empty-files-in-the-temporary-files-directory/
        // 对应解决办法：
        // 1，用 byte[] 生成临时文件，文件临时文件中构造X509Certificate2实例
        // 2，创建临时文件时，计算publicKey的sha1值，用来做临时文件名，用完后不删除，就当是文件缓存了

        X509Certificate2 cert = new X509Certificate2(publicKey);

        if( cert.HasPrivateKey )        // 防止开发保员把私钥写到程序中，泄露私钥
            throw new ArgumentException("公钥证书中不允许包含私钥！");

        return cert;
    }


    /// <summary>
    /// 从一个公钥字符串中加载X509证书
    /// </summary>
    /// <param name="publicKeyText"></param>
    /// <returns></returns>
    public static X509Certificate2 LoadFromPublicKey(string publicKeyText)
    {
        if( string.IsNullOrEmpty(publicKeyText) )
            throw new ArgumentNullException(nameof(publicKeyText));

        byte[] bb = Encoding.ASCII.GetBytes(publicKeyText.Trim());

        X509Certificate2 cert = new X509Certificate2(bb);

        if( cert.HasPrivateKey )        // 增加这个检查可以防止把私钥写到字符串中，从而泄露私钥
            throw new ArgumentException("公钥证书中不允许包含私钥！");

        return cert;
    }


    /// <summary>
    /// 从一个公钥文件中加载X509Certificate2
    /// </summary>
    /// <param name="publicKeyFilePath"></param>
    /// <returns></returns>
    public static X509Certificate2 LoadPublicKeyFile(string publicKeyFilePath)
    {
        if( publicKeyFilePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(publicKeyFilePath));

        X509Certificate2 cert = new X509Certificate2(publicKeyFilePath);

        if( cert.HasPrivateKey )
            throw new ArgumentException("公钥证书中不允许包含私钥！");

        return cert;
    }


    /// <summary>
    /// 从pfx文件内容中加载一个X509Certificate2对象
    /// </summary>
    /// <param name="pfxBody"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static X509Certificate2 LoadPfx(byte[] pfxBody, string password)
    {
        if( pfxBody == null )
            throw new ArgumentNullException(nameof(pfxBody));

        return new X509Certificate2(pfxBody, password, X509KeyStorageFlags.DefaultKeySet);
    }


    /// <summary>
    /// 从pfx文件内容中加载一个X509Certificate2对象
    /// </summary>
    /// <param name="pfxFilePath"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static X509Certificate2 LoadPfx(string pfxFilePath, string password)
    {
        if( pfxFilePath.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(pfxFilePath));

        return new X509Certificate2(pfxFilePath, password, X509KeyStorageFlags.DefaultKeySet);
    }


    /// <summary>
    /// 从配置文件中加载一个包含私钥的证书文件，
    /// 配置文件格式：base64(证书加载密码)\nbase64(证书文件二进制内容)
    /// </summary>
    /// <param name="fileBdoy">配置文件内容</param>
    /// <returns></returns>
    public static X509Certificate2 LoadFromConfigFile(string fileBdoy)
    {
        // 包含私钥的证书文件通常在加载和导入时会需要密码，所以就需要2样东西：证书文件 和 导入密码
        // 为了让这2个“强关联”，方便配置和复制，
        // 所以就要求把这2个都放在一个项目文件中，配置文件格式：base64(证书加载密码)\nbase64(证书文件二进制内容)

        if( fileBdoy.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(fileBdoy));

        string[] lines = fileBdoy.ToLines();
        if( lines.Length != 2 )
            throw new ArgumentException("配置文件的内容格式不正确！");

        string pwd = lines[0].FromBase64(true);
        byte[] body = Convert.FromBase64String(lines[1]);
        return new X509Certificate2(body, pwd, X509KeyStorageFlags.DefaultKeySet);
    }
}
