using System.Xml;

namespace ClownFish.Base;

/// <summary>
/// DataTable相关扩展工具类
/// </summary>
public static class DataTableExtensions
{
    /// <summary>
    /// 将一个DataTable转成XML
    /// </summary>
    /// <param name="table"></param>
    /// <param name="mode"></param>
    /// <param name="datasetName"></param>
    /// <returns></returns>
    public static string TableToXml(this DataTable table, XmlWriteMode mode = XmlWriteMode.WriteSchema, string datasetName = "ds")
    {
        if( table == null )
            throw new ArgumentNullException(nameof(table));

        DataSet ds = table.DataSet;
        if( ds == null ) {
            ds = new DataSet(datasetName);
            ds.Tables.Add(table);
        }

        if( datasetName.HasValue() )
            ds.DataSetName = datasetName;

        if( table.TableName.IsNullOrEmpty() )
            table.TableName = "row";

        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {
            ds.WriteXml(ms, mode);

            ms.Position = 0;
            using( StreamReader reader = new StreamReader(ms) ) {
                return reader.ReadToEnd();
            }
        }
    }


    /// <summary>
    /// 将XML转换成DataTable
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static DataTable XmlToTable(string xml)
    {
        if( xml.IsNullOrEmpty() )
            throw new ArgumentNullException(nameof(xml));

        DataSet ds = new DataSet();

        using( StringReader reader = new StringReader(xml) ) {
            using( XmlTextReader reader2 = new XmlTextReader(reader) ) {
                ds.ReadXml(reader2, XmlReadMode.ReadSchema);
            }
        }

        return ds.Tables.Count > 0 ? ds.Tables[0] : null;
    }
}
