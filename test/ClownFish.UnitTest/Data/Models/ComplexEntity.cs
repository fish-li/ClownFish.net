namespace ClownFish.UnitTest.Data.Models;


[DbEntity(Alias = "complex_entity")]
public partial class ComplexEntity : Entity
{
    [DbColumn(Alias = "id", PrimaryKey = true, Identity = true)]
    public virtual int Id { get; set; }

    [DbColumn(Alias = "location")]
    public virtual System.Drawing.Point Location { get; set; }

    [DbColumn(Alias = "securestring")]
    public virtual EncSaveString SecureString { get; set; }
}

public partial class ComplexEntity2 : Entity
{
    [DbColumn(Alias = "id", PrimaryKey = true, Identity = true)]
    public virtual int Id { get; set; }

    [DbColumn(Alias = "location")]
    public virtual System.Drawing.Point Location { get; set; }

    [DbColumn(Alias = "securestring")]
    public virtual EncSaveString SecureString { get; set; }

    [DbColumn(Alias = "wday")]
    public virtual DayOfWeek DayOfWeek { get; set; }

    [DbColumn(Alias = "textx")]
    public virtual List<string> Textx { get; set; }

    [DbColumn(Alias = "countx")]
    public virtual long? CountX { get; set; }
}


public sealed class EncSaveString
{
    private readonly string _value;

    public string Value => _value;

    public EncSaveString(string value)
    {
        _value = value;
    }

    public static implicit operator EncSaveString(string value)
    {
        return new EncSaveString(value);
    }

    public static implicit operator string(EncSaveString value)
    {
        return value._value;
    }
}

public sealed class EncSaveStringDataFieldTypeHandler : IDataFieldTypeHandler
{
    public object GetValue(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        return (EncSaveString)reader.GetString(index).FromBase64();
    }

    public object GetValue(DataRow row, int index, Type entityType, string propertyName)
    {
        return (EncSaveString)row[index].ToString().FromBase64();
    }

    public void SetValue(DbParameter parameter, object value)
    {
        EncSaveString value2 = (EncSaveString)value;     // 得到原始对象，去 object
        string text = value2 ?? string.Empty;
        parameter.Value = text.ToBase64();   // 将原文本做 base64 编码再写入数据库
    }
}



public sealed class PointDataFieldTypeHandler : IDataFieldTypeHandler
{
    public object GetValue(DbDataReader reader, int index, Type entityType, string propertyName)
    {
        int[] xy = reader.GetString(index).Split(',').Select(x=>int.Parse(x)).ToArray();
        return new System.Drawing.Point(xy[0], xy[1]);
    }

    public object GetValue(DataRow row, int index, Type entityType, string propertyName)
    {
        int[] xy = row[index].ToString().Split(',').Select(x => int.Parse(x)).ToArray();
        return new System.Drawing.Point(xy[0], xy[1]);
    }

    public void SetValue(DbParameter parameter, object value)
    {
        System.Drawing.Point p = (System.Drawing.Point)value;
        parameter.DbType = DbType.String;
        parameter.Value = $"{p.X},{p.Y}";
    }
}
