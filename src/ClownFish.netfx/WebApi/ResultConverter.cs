using ClownFish.WebApi.Result;

namespace ClownFish.WebApi;

/// <summary>
/// 用于将一个非IActionResult类型对象转换成IActionResult类型的转换器实现，
/// 可以继承此类型来实现个性化的定制转换过程。
/// </summary>
internal static class ResultConverter
{
    /// <summary>
    /// 将一个对象转换成IActionResult实例
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IActionResult Convert(object value)
    {
        if( value == null )
            return null;

        if( value is IActionResult )
            return (IActionResult)value;


        Type t = value.GetType();

        if( t.IsPrimitive || t.IsSimpleValueType() )
            return ConvertString(value);

        if( t == typeof(byte[]) )
            return ConvertByteArray(value);

        return ConvertObject(value);
    }


    /// <summary>
    /// 将一个字符串转换成IActionResult实例（默认采用TextResult类型）
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static IActionResult ConvertString(object value)
    {
        return new TextResult(value);
    }

    /// <summary>
    /// 将一个对象转换成IActionResult实例（默认采用BinaryResult类型）
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static IActionResult ConvertByteArray(object value)
    {
        return new BinaryResult((byte[])value);
    }



    /// <summary>
    /// 将一个对象转换成IActionResult实例（默认采用JsonResult类型）
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static IActionResult ConvertObject(object value)
    {
        return new JsonResult(value);
    }
}
