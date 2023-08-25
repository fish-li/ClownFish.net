using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data.Models
{
    // 用于测试 MySQL 数据类型的实体

    [Serializable]
    [DbEntity(Alias = "TestType")]
    public partial class MySqlDataType : Entity
    {
        [DbColumn(Alias = "Rid", PrimaryKey = true, Identity = true)]
        public virtual int Rid { get; set; }

        [DbColumn(Alias = "StrValue")]
        public virtual string StrValue { get; set; }

        [DbColumn(Alias = "Str2Value")]
        public virtual string Str2Value { get; set; }

        [DbColumn(Alias = "TextValue")]
        public virtual string TextValue { get; set; }

        [DbColumn(Alias = "Text2Value")]
        public virtual string Text2Value { get; set; }

        [DbColumn(Alias = "IntValue")]
        public virtual int IntValue { get; set; }

        [DbColumn(Alias = "Int2Value")]
        public virtual int? Int2Value { get; set; }

        [DbColumn(Alias = "ShortValue")]
        public virtual short ShortValue { get; set; }

        [DbColumn(Alias = "Short2Value")]
        public virtual short? Short2Value { get; set; }

        [DbColumn(Alias = "LongValue")]
        public virtual long LongValue { get; set; }

        [DbColumn(Alias = "Long2Value")]
        public virtual long? Long2Value { get; set; }

        [DbColumn(Alias = "CharValue")]
        public virtual char CharValue { get; set; }

        [DbColumn(Alias = "Char2Value")]
        public virtual char? Char2Value { get; set; }

        [DbColumn(Alias = "BoolValue")]
        public virtual bool BoolValue { get; set; }

        [DbColumn(Alias = "Bool2Value")]
        public virtual bool? Bool2Value { get; set; }

        [DbColumn(Alias = "TimeValue")]
        public virtual DateTime TimeValue { get; set; }

        [DbColumn(Alias = "Time2Value")]
        public virtual DateTime? Time2Value { get; set; }

        [DbColumn(Alias = "DecimalValue")]
        public virtual decimal DecimalValue { get; set; }

        [DbColumn(Alias = "Decimal2Value")]
        public virtual decimal? Decimal2Value { get; set; }

        [DbColumn(Alias = "FloatValue")]
        public virtual float FloatValue { get; set; }

        [DbColumn(Alias = "Float2Value")]
        public virtual float? Float2Value { get; set; }

        [DbColumn(Alias = "DoubleValue")]
        public virtual double DoubleValue { get; set; }

        [DbColumn(Alias = "Double2Value")]
        public virtual double? Double2Value { get; set; }

        [DbColumn(Alias = "GuidValue")]
        public virtual Guid GuidValue { get; set; }

        [DbColumn(Alias = "Guid2Value")]
        public virtual Guid? Guid2Value { get; set; }

        [DbColumn(Alias = "ByteValue")]
        public virtual byte ByteValue { get; set; }

        [DbColumn(Alias = "Byte2Value")]
        public virtual byte? Byte2Value { get; set; }

        [DbColumn(Alias = "SByteValue")]
        public virtual sbyte SByteValue { get; set; }

        [DbColumn(Alias = "SByte2Value")]
        public virtual sbyte? SByte2Value { get; set; }

        [DbColumn(Alias = "TimeSpanValue")]
        public virtual TimeSpan TimeSpanValue { get; set; }

        [DbColumn(Alias = "TimeSpan2Value")]
        public virtual TimeSpan? TimeSpan2Value { get; set; }

        [DbColumn(Alias = "WeekValue")]
        public virtual DayOfWeek WeekValue { get; set; }

        [DbColumn(Alias = "Week2Value")]
        public virtual DayOfWeek? Week2Value { get; set; }

        [DbColumn(Alias = "BinValue")]
        public virtual byte[] BinValue { get; set; }

        [DbColumn(Alias = "Bin2Value")]
        public virtual byte[] Bin2Value { get; set; }

        [DbColumn(Alias = "UShortValue")]
        public virtual ushort UShortValue { get; set; }

        [DbColumn(Alias = "UShort2Value")]
        public virtual ushort? UShort2Value { get; set; }

        [DbColumn(Alias = "UIntValue")]
        public virtual uint UIntValue { get; set; }

        [DbColumn(Alias = "UInt2Value")]
        public virtual uint? UInt2Value { get; set; }

        [DbColumn(Alias = "ULongValue")]
        public virtual ulong ULongValue { get; set; }

        [DbColumn(Alias = "ULong2Value")]
        public virtual ulong? ULong2Value { get; set; }

    }
}
