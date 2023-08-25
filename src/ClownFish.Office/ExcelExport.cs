using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using ClownFish.Base;
using ClownFish.Data.SqlClient;

namespace ClownFish.Office;

/// <summary>
/// 描述一个数据表信息的结构
/// </summary>
public sealed class DbTableInfo
{
    /// <summary>
    /// 表名
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 字段列表描述
    /// </summary>
    public List<MsSqlDbField> Fields { get; set; }
}


/// <summary>
/// Excel导出工具类
/// </summary>
public static class ExcelExport
{
    /// <summary>
    /// 将DataTable导出到Excel文件
    /// </summary>
    /// <param name="table">包含数据的DataTable实例</param>
    /// <param name="tableInfo">数据表的字段描述，可选项。</param>
    public static byte[] Export(DataTable table, DbTableInfo tableInfo = null)
    {
        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {
            Export(table, ms, tableInfo);

            return ms.ToArray();
        }
    }


    /// <summary>
    /// 将DataTable导出到Excel文件
    /// </summary>
    /// <param name="table">包含数据的DataTable实例</param>
    /// <param name="xlsFilePath">要导出到Excel的文件路径</param>
    /// <param name="tableInfo">数据表的字段描述，可选项。</param>
    public static void Export(DataTable table, string xlsFilePath, DbTableInfo tableInfo = null)
    {
        if( string.IsNullOrEmpty(xlsFilePath) )
            throw new ArgumentNullException(nameof(xlsFilePath));

        using( FileStream fileStream = File.OpenWrite(xlsFilePath) ) {
            Export(table, fileStream, tableInfo);
        }
    }



    /// <summary>
    /// 将DataTable导出到Excel文件
    /// </summary>
    /// <param name="table">包含数据的DataTable实例</param>
    /// <param name="outStream">要导出到Excel的输出流</param>
    /// <param name="tableInfo">数据表的字段描述，可选项。</param>
    public static void Export(DataTable table, Stream outStream, DbTableInfo tableInfo = null)
    {
        if( table == null )
            throw new ArgumentNullException(nameof(table));
        if( outStream == null )
            throw new ArgumentNullException(nameof(outStream));


        using( var wb = new XLWorkbook() ) {
            // 增加一个页签
            string name = tableInfo?.TableName ?? table.TableName;

            if( name.IsNullOrEmpty() )
                name = "Table1";

            var ws = wb.Worksheets.Add(name);

            // 将数据写入新页签
            TableToSheet(table, ws, tableInfo);

            // 保存到文件
            wb.SaveAs(outStream);
        }
    }




    /// <summary>
    /// 将DataSet导出到Excel文件
    /// </summary>
    /// <param name="ds">包含多个数据表的DataSet实例</param>
    /// <param name="tableInfoList">数据表的字段描述，可选项。</param>
    public static byte[] Export(DataSet ds, IEnumerable<DbTableInfo> tableInfoList)
    {
        using( MemoryStream ms = MemoryStreamPool.GetStream() ) {
            Export(ds, ms, tableInfoList);

            return ms.ToArray();
        }
    }


    /// <summary>
    /// 将DataSet导出到Excel文件
    /// </summary>
    /// <param name="ds">包含多个数据表的DataSet实例</param>
    /// <param name="xlsFilePath">要导出到Excel的文件路径</param>
    /// <param name="tableInfoList">数据表的字段描述，可选项。</param>
    public static void Export(DataSet ds, string xlsFilePath, IEnumerable<DbTableInfo> tableInfoList)
    {
        if( string.IsNullOrEmpty(xlsFilePath) )
            throw new ArgumentNullException(nameof(xlsFilePath));


        using( FileStream fileStream = File.OpenWrite(xlsFilePath) ) {
            Export(ds, fileStream, tableInfoList);
        }
    }



    /// <summary>
    /// 将DataSet导出到Excel文件
    /// </summary>
    /// <param name="ds">包含多个数据表的DataSet实例</param>
    /// <param name="outStream">要导出到Excel的输出流</param>
    /// <param name="tableInfoList">数据表的字段描述，可选项。</param>
    public static void Export(DataSet ds, Stream outStream, IEnumerable<DbTableInfo> tableInfoList)
    {
        if( ds == null || ds.Tables.Count == 0 )
            throw new ArgumentNullException(nameof(ds));
        if( outStream == null )
            throw new ArgumentNullException(nameof(outStream));


        using( var wb = new XLWorkbook() ) {

            int index = 0;
            foreach( DataTable table in ds.Tables ) {
                index++;


                DbTableInfo tableInfo = tableInfoList?.FirstOrDefault(x => x.TableName == table.TableName);
                string name = tableInfo?.TableName ??  table.TableName;

                if( name.IsNullOrEmpty() )
                    name = "Table" + index.ToString();

                // 增加一个页签
                var ws = wb.Worksheets.Add(name);

                // 将数据写入新页签
                TableToSheet(table, ws, tableInfo);
            }

            // 保存到文件
            wb.SaveAs(outStream);
        }
    }


    private static void TableToSheet(DataTable table, IXLWorksheet ws, DbTableInfo tableInfo = null)
    {
        // 先创建一个字典表，用于根据列名查找字段定义
        Dictionary<string, MsSqlDbField> dict = CreateTableColumnInfoDictionary(table, tableInfo);

        // 先输出第一行列名
        for( int i = 0; i < table.Columns.Count; i++ ) {
            ws.Cell(1, i + 1).Value = table.Columns[i].ColumnName;
        }

        // 设置首行背景色
        var headerRange = ws.Range(1, 1, 1, table.Columns.Count);
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // 首行冻结
        ws.SheetView.FreezeRows(1);

        // 输出数据
        for( int x = 0; x < table.Rows.Count; x++ ) {
            for( int y = 0; y < table.Columns.Count; y++ ) {

                IXLCell cell = ws.Cell(x + 2, y + 1);
                object value = table.Rows[x][y];

                if( value == null || value == DBNull.Value ) {
                    // null 值不区分数据类型，在导入时也会单独转成 null
                    cell.Value = "NULL";
                    // 模拟 SQL Server Management Studio 的显示效果
                    cell.Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 225);
                }
                else {
                    DataColumn col = table.Columns[y];
                    SetCellValue(cell, value, col, dict);
                }
            }

            // 设置行高
            ws.Row(x + 2).Height = 22;
        }

        // 列宽自适应
        try {
            ws.Columns().AdjustToContents();
        }
        catch {
            // maybe: System.Runtime.InteropServices.ExternalException: A generic error occurred in GDI+.
        }

        // 纠正备注字段这类长文本可能导致的特宽列
        for( int i = 0; i < table.Columns.Count; i++ ) {
            if( ws.Column(i + 1).Width > 50 )
                ws.Column(i + 1).Width = 50;
        }
        ////设置单元格边框
        //IXLBorder border = ws.CellsUsed().Style.Border;
        //border.OutsideBorder = XLBorderStyleValues.Thin;
        //border.OutsideBorderColor = XLColor.FromArgb(192, 192, 192);
    }

    private static void SetCellValue(IXLCell cell, object value, DataColumn col, Dictionary<string, MsSqlDbField> dict)
    {
        if( col.DataType == typeof(string) || col.DataType == typeof(Guid) ) {
            string text = value.ToString();

            // 单元格有长度限制，如果不截断会出现异常：System.ArgumentOutOfRangeException: Cells can hold a maximum of 32,767 characters.
            if( text.Length > 32767 )
                text = text.Substring(0, 32767);

            cell.Value = text;
            cell.DataType = XLDataType.Text;
        }
        else if( col.DataType == typeof(DateTime) ) {
            cell.Value = value;
            cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm:ss";
            cell.DataType = XLDataType.DateTime;
        }
        else if( col.DataType == typeof(double) || col.DataType == typeof(decimal) || col.DataType == typeof(float) ) {
            cell.Value = value;

            // 设置小数位数
            string format = GetNumberCellFormat(col, dict);
            if( format != null )
                cell.Style.NumberFormat.Format = format;

            cell.DataType = XLDataType.Number;
        }
        else if( col.DataType == typeof(int) || col.DataType == typeof(long) || col.DataType == typeof(short) ) {
            cell.Value = value;
            cell.DataType = XLDataType.Number;
        }
        else {
            cell.Value = value;
        }
    }

    private static Dictionary<string, MsSqlDbField> CreateTableColumnInfoDictionary(DataTable table, DbTableInfo tableInfo = null)
    {
        if( tableInfo == null || tableInfo.Fields == null || tableInfo.Fields.Count == 0 )
            return null;

        Dictionary<string, MsSqlDbField> dict = new Dictionary<string, MsSqlDbField>();

        foreach( DataColumn col in table.Columns ) {
            MsSqlDbField f = tableInfo.Fields.FirstOrDefault(x => col.ColumnName.Is(x.Name));
            dict[col.ColumnName] = f;
        }

        return dict;
    }


    private static string GetNumberCellFormat(DataColumn col, Dictionary<string, MsSqlDbField> dict)
    {
        if( dict == null )
            return null;

        MsSqlDbField field = dict[col.ColumnName];
        if( field == null )
            return null;

        if( field.Scale == 0 )
            // 如果是数字类型，应该小数位数不会为零！
            return null;

        return "0." + new string('0', field.Scale);
    }
}
