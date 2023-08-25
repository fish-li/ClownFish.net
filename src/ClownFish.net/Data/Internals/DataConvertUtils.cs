using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.Internals;
internal static class DataConvertUtils
{
    internal static readonly DateTime Year2000 = new DateTime(2000, 1, 1);

    internal static TimeSpan ToTimeSpan(object value)
    {
        if( value == null || value == DBNull.Value )
            return TimeSpan.Zero;

        if( value is DateTime t1 ) {
            return new TimeSpan(t1.Hour, t1.Minute, t1.Second);
        }
        if( value is TimeSpan t2 ) {
            return t2;
        }
        if( value is DateTimeOffset t3 ) {
            return new TimeSpan(t3.Hour, t3.Minute, t3.Second);
        }
#if NET6_0_OR_GREATER
        if( value is TimeOnly t4 ) {
            return new TimeSpan(t4.Hour, t4.Minute, t4.Second);
        }
#endif
        if( value is long ticks) {
            return new TimeSpan(ticks);
        }
        if( value is string s1 ) {
            return TimeSpan.Parse(s1);
        }
        throw new InvalidCastException($"无法将类型 {value.GetType().FullName} 转换成 TimeSpan");
    }

//#if NET6_0_OR_GREATER

//    internal static TimeOnly ToTimeOnly(object value)
//    {
//        if( value == null || value == DBNull.Value )
//            return TimeOnly.MinValue;

//        if( value is DateTime t1 ) {
//            return TimeOnly.FromDateTime(t1);
//        }
//        if( value is TimeSpan t2 ) {
//            return TimeOnly.FromTimeSpan(t2);
//        }
//        if( value is DateTimeOffset t3 ) {
//            return TimeOnly.FromDateTime(t3.DateTime);
//        }

//        if( value is TimeOnly t4 ) {
//            return t4;
//        }

//        if( value is long ticks ) {
//            return new TimeOnly(ticks);
//        }
//        if( value is string s1 ) {
//            return TimeOnly.Parse(s1);
//        }
//        throw new InvalidCastException($"无法将类型 {value.GetType().FullName} 转换成 TimeOnly");
//    }

//#endif
}
