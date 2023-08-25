﻿namespace ClownFish.Data;

/// <summary>
/// 动态生成的DataLoader的基类，
/// 框架内部使用的类型，请不要直接使用
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseDataLoader<T> where T : class, new()
{
    /// <summary>
    /// 根据数据源生成字段的索引数组
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public abstract int[] CreateIndex(object dataSource);

    /// <summary>
    /// 从DbDataReader中填充一个数据实体的属性
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cols"></param>
    /// <param name="m"></param>
    public abstract void LoadFromDataReader(DbDataReader reader, int[] cols, T m);

    /// <summary>
    /// 从DataRow中填充一个数据实体的属性
    /// </summary>
    /// <param name="row"></param>
    /// <param name="cols"></param>
    /// <param name="m"></param>
    public abstract void LoadFromDataRow(DataRow row, int[] cols, T m);

    /// <summary>
    /// 从DataRow中加载一个实体对象
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public T ToSingle(DataRow row)
    {
        int[] cols = CreateIndex(row.Table);
        T m = new T();
        LoadFromDataRow(row, cols, m);
        return m;
    }

    /// <summary>
    /// 从DbDataReader中加载一个实体对象
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public T ToSingle(DbDataReader reader)
    {
        int[] cols = CreateIndex(reader);
        if( reader.Read() ) {
            T m = new T();
            LoadFromDataReader(reader, cols, m);
            return m;
        }
        return null;
    }

    /// <summary>
    /// 从DataTable中加载一个实体列表
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public List<T> ToList(DataTable table)
    {
        if( table.Rows.Count == 0 )
            return new List<T>(0);

        int[] cols = CreateIndex(table);
        List<T> list = new List<T>(Initializer.Instance.ListInitLength);
        foreach( DataRow row in table.Rows ) {
            T m = new T();
            LoadFromDataRow(row, cols, m);
            list.Add(m);
        }
        return list;
    }

    /// <summary>
    /// 从DbDataReader中加载一个实体列表
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public List<T> ToList(DbDataReader reader)
    {
        int[] cols = CreateIndex(reader);
        List<T> list = new List<T>(Initializer.Instance.ListInitLength);
        while( reader.Read() ) {
            T m = new T();
            LoadFromDataReader(reader, cols, m);
            list.Add(m);
        }
        return list;
    }

}
