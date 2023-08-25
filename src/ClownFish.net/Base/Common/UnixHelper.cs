#if NETCOREAPP
using System.Net.Http;
#endif

using System.Net.Sockets;

namespace ClownFish.Base;

/// <summary>
/// Unix相关的工具类
/// </summary>
public static class UnixHelper
{

#if NET6_0_OR_GREATER

    /// <summary>
    /// Create a UnixSocket HttpMessageHandler
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static HttpMessageHandler CreateSocketHandler(string path)
    {
        // 参考：https://stackoverflow.com/questions/53547152/how-do-i-nicely-send-http-over-a-unix-domain-socket-in-net-core

        return new SocketsHttpHandler {
            ConnectCallback = async (context, token) => {
                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
                var endpoint = new UnixDomainSocketEndPoint(path);
                await socket.ConnectAsync(endpoint);
                return new NetworkStream(socket, ownsSocket: true);
            }
        };
    }

#endif


    //internal static readonly Encoding Utf8Linux = new UTF8Encoding(false);

    /// <summary>
    /// 清除Linux控制台日志中的控制字符
    /// </summary>
    /// <param name="logBytes"></param>
    /// <returns></returns>
    public static string TrimTerminalCtrolChar(byte[] logBytes)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            using( MemoryStream ms = new MemoryStream(logBytes) ) {
                using( BinaryReader reader = new BinaryReader(ms) ) {

                    while( true ) {
                        if( ms.Position >= ms.Length )
                            break;

                        // 每行的格式：\u0001\0\0\0   {len,4}   text \n
                        // 注意：有些 linux 的第一个字符是 SOH =1, 有些使用的是 STX =2

                        // 具体格式可参考：https://note.youdao.com/web/#/file/FA1ED079FC6F49F5A33600009990282C/note/WEBfb79958cc6862228ebaf451a984620d5/
                        // 参考：https://www.cnblogs.com/zhaoxd07/p/5012433.html

                        // 每行的开始标记
                        int x = reader.ReadInt32();
                        if( x > 2 )
                            break;

                        // 指示当前行的长度
                        byte[] b2 = reader.ReadBytes(4);
                        int len = (int)b2[3] + (int)b2[2] * 256;  // 最大值 0,0,64,0
                        if( len == 0 )
                            break;

                        byte[] bb = reader.ReadBytes(len - 1);
                        if( bb.HasValue() ) {
                            sb.AppendLineRN(Encoding.Default.GetString(bb).TrimEnd('\r', '\n'));
                        }
                        reader.ReadByte(); // 丢弃 \n
                    }
                }
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


    /// <summary>
    /// 清除Linux控制台日志中的控制字符
    /// </summary>
    /// <param name="logBytes"></param>
    /// <returns></returns>
    public static string TrimTerminalCtrolChar2(byte[] logBytes)
    {
        string text = Encoding.UTF8.GetString(logBytes);

        StringBuilder sb = StringBuilderPool.Get();
        try {
            string line = null;
            using( StringReader reader = new StringReader(text) ) {
                while( (line = reader.ReadLine()) != null ) {
                    sb.AppendLineRN(line.Substring(8));
                }
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }


}
