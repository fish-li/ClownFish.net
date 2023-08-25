using System;
using System.Data;
using System.IO;
using System.Linq;
using ExcelDataReader;

namespace ClownFish.Office;

/// <summary>
/// Excel导入工具类
/// </summary>
public static class ExcelImport
{
    /// <summary>
    /// 从Excel文件中加载数据，每个Excel表要求第一行是列名
    /// </summary>
    /// <param name="fileBytes">Excel文件内容</param>
    /// <param name="templateTable">包含列的类型定义的空表，加载数据时以这个为模板创建数据列</param>
    /// <returns></returns>
    public static DataTable Import(byte[] fileBytes, DataTable templateTable)
    {
        if( fileBytes == null )
            throw new ArgumentNullException(nameof(fileBytes));


        using( var stream = new MemoryStream(fileBytes, false) ) {

            return Import(stream, templateTable);
        }
    }

    /// <summary>
    /// 从Excel文件中加载数据，每个Excel表要求第一行是列名
    /// </summary>
    /// <param name="xlsFilePath">Excel文件名</param>
    /// <param name="templateTable">包含列的类型定义的空表，加载数据时以这个为模板创建数据列</param>
    /// <returns></returns>
    public static DataTable Import(string xlsFilePath, DataTable templateTable)
    {
        if( string.IsNullOrEmpty(xlsFilePath) )
            throw new ArgumentNullException(nameof(xlsFilePath));


        using( var stream = File.OpenRead(xlsFilePath) ) {

            return Import(stream, templateTable);
        }
    }


    /// <summary>
    /// 从Excel文件中加载数据，每个Excel表要求第一行是列名
    /// </summary>
    /// <param name="fileStream">Excel文件内容输入流</param>
    /// <param name="templateTable">包含列的类型定义的空表，加载数据时以这个为模板创建数据列</param>
    /// <returns></returns>
    public static DataTable Import(Stream fileStream, DataTable templateTable)
    {
        if( fileStream == null )
            throw new ArgumentNullException(nameof(fileStream));

        if( templateTable == null )
            throw new ArgumentNullException(nameof(templateTable));


        using( var reader = ExcelReaderFactory.CreateReader(fileStream) ) {

            reader.Reset();

            // 从 reader 中获取一张表的数据
            DataTable table = LoadDataTable(reader, templateTable);
            return table;
        }
    }


    /// <summary>
    /// 从Excel文件中加载数据，每个Excel表要求第一行是列名
    /// </summary>
    /// <param name="fileBytes">Excel文件名</param>
    /// <param name="dataTypeEmtpySet">包含列的类型定义的空DataSet，加载数据时以这个为模板创建数据列</param>
    /// <returns></returns>
    public static DataSet Import(byte[] fileBytes, DataSet dataTypeEmtpySet)
    {
        if( fileBytes == null )
            throw new ArgumentNullException(nameof(fileBytes));


        using( var stream = new MemoryStream(fileBytes, false) ) {

            return Import(stream, dataTypeEmtpySet);
        }
    }


    /// <summary>
    /// 从Excel文件中加载数据，每个Excel表要求第一行是列名
    /// </summary>
    /// <param name="filePath">Excel文件名</param>
    /// <param name="dataTypeEmtpySet">包含列的类型定义的空DataSet，加载数据时以这个为模板创建数据列</param>
    /// <returns></returns>
    public static DataSet Import(string filePath, DataSet dataTypeEmtpySet)
    {
        if( string.IsNullOrEmpty(filePath) )
            throw new ArgumentNullException(nameof(filePath));


        using( var stream = File.OpenRead(filePath) ) {

            return Import(stream, dataTypeEmtpySet);
        }
    }



    /// <summary>
    /// 从Excel文件中加载数据，每个Excel表要求第一行是列名
    /// </summary>
    /// <param name="fileStream">Excel文件名</param>
    /// <param name="dataTypeEmtpySet">包含列的类型定义的空DataSet，加载数据时以这个为模板创建数据列</param>
    /// <returns></returns>
    public static DataSet Import(Stream fileStream, DataSet dataTypeEmtpySet)
    {
        if( fileStream == null )
            throw new ArgumentNullException(nameof(fileStream));

        if( dataTypeEmtpySet == null )
            throw new ArgumentNullException(nameof(dataTypeEmtpySet));


        using( var reader = ExcelReaderFactory.CreateReader(fileStream) ) {

            reader.Reset();

            DataSet result = new DataSet();
            do {
                // 获取模板表，用于获取列的数据类型
                DataTable templateTable = dataTypeEmtpySet.Tables[reader.Name];
                if( templateTable == null )
                    throw new ArgumentException("dataTypeEmtpySet中不包含要加载的sheet名称：" + reader.Name);

                // 从 reader 中获取一张表的数据
                DataTable table = LoadDataTable(reader, templateTable);

                // 将数据添加到 DataSet
                result.Tables.Add(table);
            }
            // 转到下一个 Sheet 页
            while( reader.NextResult() );

            result.AcceptChanges();
            return result;
        }
    }




    private static DataTable LoadDataTable(IExcelDataReader reader, DataTable templateTable)
    {
        DataTable result = new DataTable { TableName = reader.Name };
        bool first = true;
        int rowIndex = 0;

        while( reader.Read() ) {
            rowIndex++;

            if( first ) {
                ProcessFirstRow(reader, templateTable, result);
                first = false;
                continue;
            }

            if( LoadOneRow(reader, result, rowIndex) == false )
                break;
        }

        result.EndLoadData();
        return result;
    }


    private static void ProcessFirstRow(IExcelDataReader reader, DataTable templateTable, DataTable result)
    {
        for( var i = 0; i < reader.FieldCount; i++ ) {
            // 读取列名
            string name = Convert.ToString(reader.GetValue(i));

            // 根据列名从模板表中读取匹配的列
            DataColumn templateColumn = (from x in templateTable.Columns.Cast<DataColumn>()
                                         where x.ColumnName.Equals(name, StringComparison.OrdinalIgnoreCase)
                                         select x).FirstOrDefault();

            // 根据模板表的数据列来创建新的数据列
            DataColumn column = templateColumn == null
                        ? new DataColumn(name, typeof(object)) { Caption = name }
                        : new DataColumn(name, templateColumn.DataType) { Caption = name };

            result.Columns.Add(column);
        }

        result.BeginLoadData();
    }


    private static bool LoadOneRow(IExcelDataReader reader, DataTable result, int rowIndex)
    {
        DataRow row = result.NewRow();
        bool isEmptyRow = true;

        for( var i = 0; i < reader.FieldCount; i++ ) {
            object value = reader.GetValue(i);
            if( value != null ) {
                Type dataType = value.GetType();

                if( dataType == typeof(string) ) {
                    if( value.ToString() == "NULL" )    // 导出时将所有 null 以 "NULL" 形式的字符串导出，这里又再转回来                                
                        value = DBNull.Value;
                    else {
                        DataColumn column = result.Columns[i];
                        if( column.DataType != dataType ) {
                            try {
                                value = ChangeType((string)value, column.DataType);
                            }
                            catch( Exception ex ) {
                                throw new InvalidCastException($"数据类型转换失败，{rowIndex}行 {i + 1}列，值：\"{value}\" 不能转换成 {column.DataType.Name} 类型。", ex);
                            }
                        }
                    }
                }
                else {
                    // 非字符串类型暂不处理
                }
            }
            else {
                // 当文本字段为空字符串时，在导出到EXCEL时，会没有任何内容输出
                // 但是在导入时，会被认为是 null
                // 所以，这里再把 null 转换成 string.Empty
                value = string.Empty;
            }
            row[i] = value;

            // 检查一下，是不是读取到的数据
            // 有时候可能是由于做过格式，一些空行也被读到了，但是这样的空行是没有意义的
            if( value is string text && text.Length > 0 ) {
                isEmptyRow = false;
            }
        }

        if( isEmptyRow ) {
            return false;
        }
        else {
            result.Rows.Add(row);
            return true;
        }
    }

    private static object ChangeType(string value, Type destType)
    {
        if( destType == typeof(Guid) )
            return new Guid(value);

        return Convert.ChangeType(value, destType);
    }

}
