using Microsoft.AspNetCore.Http;

namespace ClownFish.Web.Utils;
public static class HttpResponseExtensions
{
    /// <summary>
    /// 一次性写入一段文本，并且后续不再写入
    /// </summary>
    /// <param name="response"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static async Task WriteAllAsync(this HttpResponse response, string text)
    {
        if( text.IsNullOrEmpty() )
            return;

        byte[] buffer = Encoding.UTF8.GetBytes(text);

        if( buffer != null && buffer.Length > 0 ) {
            response.Headers.ContentLength = buffer.Length;
            response.ContentType = ResponseContentType.TextUtf8;

            await response.Body.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
