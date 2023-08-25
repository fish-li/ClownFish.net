namespace ClownFish.Base;

/// <summary>
/// 存储“空数据”的类型
/// </summary>
public static class Empty
{
    /// <summary>
    /// 与 System.Array.Empty() 的结果一样，但是支持 .net 461 以下版本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[] Array<T>()
    {
        return Data<T>.Array;
    }


    /// <summary>
    /// 获取一个空的列表。
    /// 警告：此方法的返回值建议仅用于Action的返回结果，【不能再添加元素】，否则一定会出现 BUG ！！
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCodeException"></exception>
    public static List<T> List<T>()
    {
        List<T> list = Data<T>.List;

        if( list.Count > 0 )
            throw new InvalidCodeException("The empty list has been changed!");

        return list;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private static class Data<T>
    {
        public static readonly T[] Array = new T[0];

        public static readonly List<T> List = new List<T>(0);  // 这里不能保证 Immutable
    }
}
