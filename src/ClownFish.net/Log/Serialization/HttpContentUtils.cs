#if NETCOREAPP
using System.Net.Http;

namespace ClownFish.Log;

internal static class HttpContentUtils
{
    internal static string ReadBodyAsText(this HttpContent content)
    {
        if( content == null )
            return null;

        // 写日志时，HttpRequestMessage/HttpResponseMessage 早已 dispose 了，所以按常规方式去读取肯定是得不到结果的

        // 下面这种方法，绝大多数情况下是读不到内容的
        //return content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        // 压缩的响应会在读取时修改：例如，System.Net.Http.DecompressionHandler.GZipDecompressedContent
        // 所以这里只处理几个已知的类型

        try {
            if( content is StreamContent content2 )
                return TryReadBodyFromStreamContent(content2);

            if( content is ByteArrayContent content3 )
                return TryReadBodyFromByteArrayContent(content3);
        }
        catch( Exception ex ) {
            return ex.ToString();
        }

        return null;
    }

    private static string TryReadBodyFromStreamContent(StreamContent content)
    {
        MemoryStream ms = TryGetMemoryStreamFromStreamContent(content);
        if( ms != null ) {
            byte[] bytes = ms.ToArray();
            return Encoding.UTF8.GetString(bytes);     // 这里使用固定编码
        }
        return null;
    }

    private static string TryReadBodyFromByteArrayContent(ByteArrayContent content)
    {
        // TODO: 注意，这里使用了一个内部方法，以后升级.NET时可能无法运行~~
        MethodInfo method = typeof(ByteArrayContent).GetMethod("CreateMemoryStreamForByteArray", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        using MemoryStream ms = (MemoryStream)method.Invoke(content, null);
        byte[] bytes = ms.ToArray();
        return Encoding.UTF8.GetString(bytes);     // 这里使用固定编码
    }

    internal static MemoryStream TryGetMemoryStreamFromStreamContent(this StreamContent content)
    {
        if( content != null ) {
            FieldInfo filed1 = typeof(StreamContent).GetField("_content", BindingFlags.Instance | BindingFlags.NonPublic);
            if( filed1 != null ) {
                MemoryStream ms = filed1.GetValue(content) as MemoryStream;
                return ms;
            }
        }

        return null;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool BodyIsMemoryStream(this HttpContent content)
    {
        if( content != null ) {
            if( content is StreamContent content2 ) {
                MemoryStream ms = content2.TryGetMemoryStreamFromStreamContent();
                return ms != null;
            }
        }
        return false;
    }


}
#endif
