namespace ClownFish.Base.Exceptions;

/// <summary>
/// 
/// </summary>
//[Serializable]
public sealed class HttpException : Exception, IErrorCode
{
    /// <summary>
    /// StatusCode
    /// </summary>
    public int StatusCode { get; private set; }

    int IErrorCode.GetErrorCode() => this.StatusCode;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="statusCode"></param>
    /// <param name="message"></param>
    public HttpException(int statusCode, string message) : base(message)
    {
        this.StatusCode = statusCode;
    }

    

    //private HttpException(SerializationInfo info, StreamingContext context)
    //    : base(info, context)
    //{
    //    this.StatusCode = info.GetInt32("StatusCode");
    //}

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="info"></param>
    ///// <param name="context"></param>
    //public override void GetObjectData(SerializationInfo info, StreamingContext context)
    //{
    //    base.GetObjectData(info, context);
    //    info.AddValue("StatusCode", this.StatusCode, typeof(int));
    //}


}
