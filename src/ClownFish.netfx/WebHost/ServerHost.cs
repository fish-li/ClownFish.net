using ClownFish.WebHost.Config;

namespace ClownFish.WebHost;

/// <summary>
/// 用于监听HTTP请求的服务器类型
/// </summary>
public sealed class ServerHost : IDisposable
{
    private Thread _workThread;
    private volatile bool _exit = false;
    private volatile HttpListener _listener;
    private readonly WebHostSpacer _webApiSpacer = new WebHostSpacer();


    /// <summary>
    /// 启动HTTP监听
    /// </summary>
    /// <param name="configPath"></param>
    public void Run(string configPath)
    {
        if( File.Exists(configPath) == false )
            throw new FileNotFoundException("指定的配置文件不存在：" + configPath);

        ServerOption option = ServerOption.LoadFile(configPath);
        Run(option);
    }


    /// <summary>
    /// 启动HTTP监听
    /// </summary>
    /// <param name="option"></param>
    public void Run(ServerOption option)
    {
        if( option == null )
            throw new ArgumentNullException(nameof(option));

        ServerOption.SetDefault(option);

        string[] urls = option.GetUrlPrefixes();
        Run(urls);
    }

    /// <summary>
    /// 启动HTTP监听
    /// </summary>
    /// <param name="urlPrefixes"></param>
    public void Run(string[] urlPrefixes)
    {
        if( _listener != null )  // 已启动监听
            return;


        if( urlPrefixes.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(urlPrefixes));


        // 创建HttpListener实例并初始化
        if( HttpListener.IsSupported == false )
            throw new NotSupportedException("当前操作系统不支持HttpListener");


        // 初始化HTTP管道。
        // 提前初始化的好处在于：如果有异常可以提前暴露，否则在HTTP请求进入后再初始化会导致产生一大堆异常
        HttpAppHost.Init();

        HttpListener listener = new HttpListener();
        listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;  // 允许匿名访问

        // 监听端口
        foreach( var url in urlPrefixes )
            listener.Prefixes.Add(url);


        // 用后台线程接收请求，防止阻塞当前线程
        _workThread = new Thread(ServerProc);
        _workThread.IsBackground = true;
        _workThread.Start();

        listener.Start();
        _listener = listener;
    }



    /// <summary>
    /// 关闭 HttpListener
    /// </summary>
    public void Dispose()
    {
        _exit = true;

        if( _listener != null ) {
            _listener.Close();
            _listener = null;
        }
    }


    private void ServerProc(object runOptions)
    {
        while( true ) {

            if( _exit )
                break;

            if( _listener == null || _listener.IsListening == false ) {
                // 如果没有监听，就休眠一下
                System.Threading.Thread.Sleep(1000);
                continue;
            }

            if( _listener == null || _listener.IsListening == false ) {
                continue;
            }

            try {
                HttpListenerContext context = _listener.GetContext();
                ProcessRequest(context);
            }
            catch( ObjectDisposedException ) { // 此对象已关闭。 
            }
            catch( InvalidOperationException ) { // 此对象尚未启动或当前已停止。
            }
            catch( HttpListenerException ) {
                // 暂时先不处理这个异常，以后遇到再说
            }
        }
    }


    private void ProcessRequest(HttpListenerContext context)
    {
        using( ExecutionContext.SuppressFlow() ) {
            Task.Run(() => _webApiSpacer.ProcessRequest(context));
        }


        // 模拟后台线程崩溃
        //throw new NotImplementedException("xxxxxxxxxxx模拟后台线程崩溃xxxxxxxxxx");
    }


}
