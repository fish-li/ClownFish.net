using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;

namespace ClownFish.Web.Aspnetcore.ActionResults;

public sealed class DataTableResult : ActionResult, IWebApiResult
{
    private readonly DataTable _table;
    private readonly string _format;

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="table">将要输出的数据表</param>
    /// <param name="format">输出格式，可选值：xml (默认), json</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DataTableResult(DataTable table, string format = "xml")
    {
        _table = table ?? throw new ArgumentNullException(nameof(table));
        _format = format;
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        NHttpContext httpContextNetCore = HttpPipelineContext.Get2().HttpContext;
        await OutResultAsync(httpContextNetCore);
    }

    public override void ExecuteResult(ActionContext context)
    {
        throw new NotImplementedException();
    }


    public async Task OutResultAsync(NHttpContext httpContext)
    {
        NHttpResponse response = httpContext.Response;
        response.StatusCode = 200;
        response.SetHeader("x-data-rows", _table.Rows.Count.ToString());
        response.SetHeader(HttpHeaders.Response.ContentEncoding, "gzip");

        TransparentOutStream stream = new TransparentOutStream(response.OutputStream);
        using( GZipStream gzip = new GZipStream(stream, CompressionLevel.SmallestSize, true) ) {

            if( _format.Is("xml") ) {
                OutResultXmlAsync(response, gzip);
            }
            else if( _format.Is("json") ) {
                OutResultJsonAsync(response, gzip);
            }
            else {
                OutResultXmlAsync(response, gzip);
            }
        }

        await stream.FlushAsync();

        OprLog oprlog = httpContext.OprLog;
        if( oprlog != null ) {
            oprlog.Addition = "rows=" + _table.Rows.Count.ToString();
            oprlog.OutSize = stream.GetOutSize();
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026: ds.WriteXml")]
    [UnconditionalSuppressMessage("AOT", "IL3050: ds.WriteXml")]
    private void OutResultXmlAsync(NHttpResponse response, Stream outStream)
    {
        DataSet ds = null;
        if( _table.DataSet == null ) {
            ds = new DataSet("ds1");
            ds.Tables.Add(_table);
        }
        else {
            ds = _table.DataSet;
        }

        if( _table.TableName.IsNullOrEmpty() )
            _table.TableName = "row";

        response.ContentType = ResponseContentType.XmlUtf8;
        ds.WriteXml(outStream, XmlWriteMode.WriteSchema);

    }

    private void OutResultJsonAsync(NHttpResponse response, Stream outStream)
    {
        response.ContentType = ResponseContentType.JsonUtf8;

        using( StreamWriter sw = new StreamWriter(outStream, Encoding.UTF8, 1024, true) ) {
            _table.ToJson(sw);
        }
    }


}
