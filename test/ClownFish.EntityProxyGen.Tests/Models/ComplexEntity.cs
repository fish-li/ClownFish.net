using System;
using System.Collections.Generic;
using System.Text;

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

