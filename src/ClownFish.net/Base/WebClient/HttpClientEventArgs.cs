namespace ClownFish.Base.WebClient;

/// <summary>
/// 创建请求对象前事件对应的事件参数
/// </summary>
public sealed class BeforeCreateRequestEventArgs : EventArgs
{
    /// <summary>
    /// 操作ID
    /// </summary>
    public string OperationId { get; internal set; }

    /// <summary>
    /// HttpOption实例引用。
    /// </summary>
    public HttpOption HttpOption { get; internal set; }
}


/// <summary>
/// 创建请求对象前事件对应的事件参数
/// </summary>
public sealed class BeforeSendEventArgs : EventArgs
{
    /// <summary>
    /// 操作ID
    /// </summary>
    public string OperationId { get; internal set; }

    /// <summary>
    /// HttpOption实例引用。
    /// </summary>
    public HttpOption HttpOption { get; internal set; }

#if NETFRAMEWORK
    /// <summary>
    /// HttpWebRequest 实例
    /// </summary>
    public System.Net.HttpWebRequest Request { get; internal set; }
#else
    /// <summary>
    /// HttpRequestMessage 实例
    /// </summary>
    public System.Net.Http.HttpRequestMessage Request { get; internal set; }
#endif
}



/// <summary>
/// 请求结束事件对应的事件参数
/// </summary>
public sealed class RequestFinishedEventArgs : EventArgs, ILoggingObject
{
    /// <summary>
    /// 操作ID
    /// </summary>
    public string OperationId { get; internal set; }

    /// <summary>
    /// HttpOption实例引用。
    /// </summary>
    public HttpOption HttpOption { get; internal set; }


#if NETFRAMEWORK
    /// <summary>
    /// HttpWebRequest 实例
    /// </summary>
    public System.Net.HttpWebRequest Request { get; internal set; }
#else
    /// <summary>
    /// HttpRequestMessage 实例
    /// </summary>
    public System.Net.Http.HttpRequestMessage Request { get; internal set; }
#endif


    /// <summary>
    /// HttpWebResponse实例
    /// </summary>
    public System.Net.HttpWebResponse Response { get; internal set; }

    /// <summary>
    /// 与执行相关的异常对象
    /// </summary>
    public Exception Exception { get; internal set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime { get; internal set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime { get; internal set; }



    /// <summary>
    /// 获取当前对象的日志展示文本
    /// </summary>
    /// <returns></returns>
    public string ToLoggingText()
    {
        //if( this.Request == null )

#if NETCOREAPP
        return this.Request != null
                ? this.Request.ToLoggingText()
                : this.HttpOption.ToLoggingText();
#else
        return this.HttpOption.ToLoggingText();
#endif
        //#if NETCOREAPP
        //            if( this.Request is HttpWebRequest )
        //                return this.ToLoggingText1();
        //            else
        //                return this.ToLoggingText2();
        //#else
        //            if( this.Request is HttpWebRequest )
        //                return this.ToLoggingText1();
        //            else
        //                throw new NotImplementedException();
        //#endif

    }


    //        private string ToLoggingText1()
    //        {
    //            HttpWebRequest request = (HttpWebRequest)this.Request;

    //            StringBuilder sb = new StringBuilder();
    //            sb.Append(request.Method).Append(" ")
    //                .Append(request.RequestUri.AbsoluteUri)
    //                .Append(" HTTP/").AppendLineRN(request.ProtocolVersion.ToString());

    //            foreach( var name in request.Headers.AllKeys ) {

    //                string value = request.Headers[name] ?? string.Empty;
    //                //sb.AppendLineRN($"{name}: {value.SubstringN(128)}");
    //                sb.AppendLineRN($"{name}: {value}");
    //            }

    //            //string body = this.HttpOption.GetPostBodyStringForLogging();
    //            //if( body != null )
    //            //    sb.AppendLineRN().Append(body);

    //            return sb.ToString();
    //        }

    //#if NETCOREAPP
    //        private string ToLoggingText2()
    //        {
    //            System.Net.Http.HttpRequestMessage request = (System.Net.Http.HttpRequestMessage)this.Request;

    //            StringBuilder sb = new StringBuilder();
    //            sb.Append(request.Method).Append(" ")
    //                .Append(request.RequestUri.AbsoluteUri)
    //                .Append(" HTTP/").AppendLineRN(request.Version.ToString());

    //            foreach( var x in request.Headers ) {
    //                foreach( var v in x.Value ) {
    //                    sb.AppendLineRN($"{x.Key}: {v}");
    //                }
    //            }

    //            //string body = this.HttpOption.GetPostBodyStringForLogging();
    //            //if( body != null )
    //            //    sb.AppendLineRN().Append(body);

    //            return sb.ToString();
    //        }
    //#endif

}
