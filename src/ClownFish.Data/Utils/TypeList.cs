using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
    /// <summary>
    /// 免得老是写typeof(xxx)，定义一个静态类型方便写代码
    /// </summary>
    internal static class TypeList
    {
        // 说明：这个类型中的字段命名并不规范，之所以不规范是因为：
        // 1、大小写：主要是希望保持与C#代码中看到的类型名称一致！
        // 2、下划线：如果没有下划线，字段名与C#的类型关键字同名了。

        public static readonly Type _string = typeof(string);
        public static readonly Type _int = typeof(int);
        public static readonly Type _int_null = typeof(int?);
        public static readonly Type _long = typeof(long);
        public static readonly Type _long_null = typeof(long?);
        public static readonly Type _short = typeof(short);
		public static readonly Type _short_null = typeof(short?);
		public static readonly Type _DateTime = typeof(DateTime);
        public static readonly Type _DateTime_null = typeof(DateTime?);
        public static readonly Type _bool = typeof(bool);
        public static readonly Type _bool_null = typeof(bool?);
        public static readonly Type _double = typeof(double);
        public static readonly Type _double_null = typeof(double?);
        public static readonly Type _decimal = typeof(decimal);
        public static readonly Type _decimal_null = typeof(decimal?);
        public static readonly Type _float = typeof(float);
		public static readonly Type _float_null = typeof(float?);
		public static readonly Type _Guid = typeof(Guid);
        public static readonly Type _Guid_null = typeof(Guid?);
        public static readonly Type _ulong = typeof(ulong);
        public static readonly Type _uint = typeof(uint);
        public static readonly Type _ushort = typeof(ushort);
        public static readonly Type _char = typeof(char);
        public static readonly Type _char_null = typeof(char?);
        public static readonly Type _byte = typeof(byte);
        public static readonly Type _sbyte = typeof(sbyte);
        public static readonly Type _byteArray = typeof(byte[]);

        public static readonly Type _object = typeof(object);

        public static readonly Type _void = typeof(void);
        public static readonly Type _Nullable = typeof(Nullable<>);

        public static readonly Type _Entity = typeof(Entity);
        public static readonly Type _IEntityProxy = typeof(IEntityProxy);


    }
}
