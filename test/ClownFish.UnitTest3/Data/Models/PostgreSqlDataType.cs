namespace ClownFish.UnitTest.Data.Models;

[Serializable]
[DbEntity(Alias = "testtype")]
public partial class PostgreSqlDataType : Entity
{
    [DbColumn(Alias = "rid", PrimaryKey = true, Identity = true)]
    public virtual int Rid { get; set; }

    [DbColumn(Alias = "strvalue")]
    public virtual string StrValue { get; set; }

    [DbColumn(Alias = "str2value")]
    public virtual string Str2Value { get; set; }

    [DbColumn(Alias = "textvalue")]
    public virtual string TextValue { get; set; }

    [DbColumn(Alias = "text2value")]
    public virtual string Text2Value { get; set; }

    [DbColumn(Alias = "intvalue")]
    public virtual int IntValue { get; set; }

    [DbColumn(Alias = "int2value")]
    public virtual int? Int2Value { get; set; }

    [DbColumn(Alias = "shortvalue")]
    public virtual short ShortValue { get; set; }

    [DbColumn(Alias = "short2value")]
    public virtual short? Short2Value { get; set; }

    [DbColumn(Alias = "longvalue")]
    public virtual long LongValue { get; set; }

    [DbColumn(Alias = "long2value")]
    public virtual long? Long2Value { get; set; }

    [DbColumn(Alias = "charvalue")]
    public virtual char CharValue { get; set; }

    [DbColumn(Alias = "char2value")]
    public virtual char? Char2Value { get; set; }

    [DbColumn(Alias = "boolvalue")]
    public virtual bool BoolValue { get; set; }

    [DbColumn(Alias = "bool2value")]
    public virtual bool? Bool2Value { get; set; }

    [DbColumn(Alias = "timevalue")]
    public virtual DateTime TimeValue { get; set; }

    [DbColumn(Alias = "time2value")]
    public virtual DateTime? Time2Value { get; set; }

    [DbColumn(Alias = "decimalvalue")]
    public virtual decimal DecimalValue { get; set; }

    [DbColumn(Alias = "decimal2value")]
    public virtual decimal? Decimal2Value { get; set; }

    [DbColumn(Alias = "floatvalue")]
    public virtual float FloatValue { get; set; }

    [DbColumn(Alias = "float2value")]
    public virtual float? Float2Value { get; set; }

    [DbColumn(Alias = "doublevalue")]
    public virtual double DoubleValue { get; set; }

    [DbColumn(Alias = "double2value")]
    public virtual double? Double2Value { get; set; }

    [DbColumn(Alias = "guidvalue")]
    public virtual Guid GuidValue { get; set; }

    [DbColumn(Alias = "guid2value")]
    public virtual Guid? Guid2Value { get; set; }

    [DbColumn(Alias = "bytevalue")]
    public virtual byte ByteValue { get; set; }

    [DbColumn(Alias = "byte2value")]
    public virtual byte? Byte2Value { get; set; }

    //[DbColumn(Alias = "SByteValue")]
    //public virtual sbyte SByteValue { get; set; }

    //[DbColumn(Alias = "SByte2Value")]
    //public virtual sbyte? SByte2Value { get; set; }

    [DbColumn(Alias = "timespanvalue")]
    public virtual TimeSpan TimeSpanValue { get; set; }

    [DbColumn(Alias = "timespan2value")]
    public virtual TimeSpan? TimeSpan2Value { get; set; }

    [DbColumn(Alias = "weekvalue")]
    public virtual DayOfWeek WeekValue { get; set; }

    [DbColumn(Alias = "week2value")]
    public virtual DayOfWeek? Week2Value { get; set; }

    [DbColumn(Alias = "binvalue")]
    public virtual byte[] BinValue { get; set; }

    [DbColumn(Alias = "bin2value")]
    public virtual byte[] Bin2Value { get; set; }

    //[DbColumn(Alias = "UShortValue")]
    //public virtual ushort UShortValue { get; set; }

    //[DbColumn(Alias = "UShort2Value")]
    //public virtual ushort? UShort2Value { get; set; }

    //[DbColumn(Alias = "UIntValue")]
    //public virtual uint UIntValue { get; set; }

    //[DbColumn(Alias = "UInt2Value")]
    //public virtual uint? UInt2Value { get; set; }

    //[DbColumn(Alias = "ULongValue")]
    //public virtual ulong ULongValue { get; set; }

    //[DbColumn(Alias = "ULong2Value")]
    //public virtual ulong? ULong2Value { get; set; }

}
