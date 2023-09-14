namespace ClownFish.Base;

/// <summary>
/// 包含一些字典相关的扩展方法
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// 往字典中插入数据项，如果有异常，则报告KEY是什么
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        try {
            dict.Add(key, value);

            // .NET 异常提示：已添加了具有相同键的项。
            // 这种提示很不利于排查问题，所以才封装了这个方法
        }
        catch( ArgumentException ex ) {
            throw new ArgumentException(string.Format("往集合中插入元素时发生了异常，当前Key={0}", key), ex);
        }
    }



    /// <summary>
    /// 往Hashtable中插入数据项，如果有异常，则报告KEY是什么
    /// </summary>
    /// <param name="table"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddValue(this IDictionary table, object key, object value)
    {
        try {
            table.Add(key, value);

            // 往HashTable中插入相同的KEY时，
            // .NET的异常提示：已添加项。字典中的关键字:“abc”所添加的关键字:“abc”

            // 虽然已在异常消息中指出了KEY值，
            // 但是为了和Dictionary有着一到的API，所以仍然封装了这个方法。
        }
        catch( ArgumentException ex ) {
            throw new ArgumentException(string.Format("往集合中插入元素时发生了异常，当前Key={0}", key), ex);
        }
    }


    /// <summary>
    /// 从字典中获取指定KEY关联的数据，如果KEY不存在，则返回类型的默认值。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    {
        TValue value = default(TValue);
        dict.TryGetValue(key, out value);
        return value;
    }


    // 说明： 
    // IDictionary 可以直接访问索引器，当KEY不存在时，不会抛出异常，所以就不再提供 TryGet方法
    // IDictionary<TKey, TValue> 的索引器在调用时，如果KEY不存在，会抛出KeyNotFoundException异常



    /// <summary>
    /// 创建一个新的字典集合，包含其中的所有元素。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(dict.Count, dict.Comparer);

        foreach( var kv in dict )
            result[kv.Key] = kv.Value;

        return result;
    }


    /// <summary>
    /// 将一个对象转成字典表
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IDictionary<string, object> ToDictionary(this object obj)
    {
        if( obj == null )
            throw new ArgumentNullException(nameof(obj));

        Dictionary<string, object> dict = new Dictionary<string, object>(64);

        PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach( PropertyInfo property in properties ) {
            object value = property.FastGetValue(obj);
            dict[property.Name] = value;
        }

        return dict;
    }


    /// <summary>
    /// 将一个对象转成字典表
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IDictionary<string, string> ToStringDictionary(this object obj)
    {
        if( obj == null )
            throw new ArgumentNullException(nameof(obj));

        Dictionary<string, string> dict = new Dictionary<string, string>(64);

        PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach( PropertyInfo property in properties ) {
            object value = property.FastGetValue(obj);
            dict[property.Name] = value?.ToString2();
        }

        return dict;
    }


    /// <summary>
    /// 将字典表转成指定的类型实例
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="destType">目标类型实例，必须要支持公开无参的构造方法。</param>
    /// <returns></returns>
    public static object ToObject(this IDictionary<string, object> dict, Type destType)
    {
        if( dict == null )
            throw new ArgumentNullException(nameof(dict));

        object result = Activator.CreateInstance(destType);

        PropertyInfo[] properties = result.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach( PropertyInfo property in properties ) {

            object value;
            if( dict.TryGetValue(property.Name, out value) )
                property.FastSetValue(result, value);
        }

        return result;
    }


    /// <summary>
    /// 获取一个字典中所有的KEY
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dict"></param>
    /// <returns></returns>
    public static K[] ToKeys<K, V>(this IDictionary<K, V> dict)
    {
        if( dict == null || dict.Count == 0 )
            return Empty.Array<K>();

        return (from x in dict select x.Key).ToArray();
    }


    /// <summary>
    /// 与 System.Linq.Enumerable.ToDictionary 方法的功能相同，差别在于可以指定返回值集合的初始容量，避免在填充过程中反复扩容。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="capacity"></param>
    /// <param name="keySelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TSource> ToDictionary2<TSource, TKey>(this IEnumerable<TSource> source, int capacity, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer = null) where TKey : notnull
    {
        if( source == null ) {
            throw new ArgumentNullException(nameof(source));
        }
        if( keySelector == null ) {
            throw new ArgumentNullException(nameof(keySelector));
        }

        Dictionary<TKey, TSource> dictionary = new Dictionary<TKey, TSource>(capacity, comparer);
        foreach( TSource item in source ) {
            dictionary.AddValue(keySelector(item), item);
        }
        return dictionary;
    }


    /// <summary>
    /// 与 System.Linq.Enumerable.ToDictionary 方法的功能相同，差别在于可以指定返回值集合的初始容量，避免在填充过程中反复扩容。
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="source"></param>
    /// <param name="capacity"></param>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TElement> ToDictionary2<TSource, TKey, TElement>(this IEnumerable<TSource> source, int capacity, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer = null) where TKey : notnull
    {
        if( source == null ) {
            throw new ArgumentNullException(nameof(source));
        }
        if( keySelector == null ) {
            throw new ArgumentNullException(nameof(keySelector));
        }
        if( elementSelector == null ) {
            throw new ArgumentNullException(nameof(elementSelector));
        }

        Dictionary<TKey, TElement> dictionary = new Dictionary<TKey, TElement>(capacity, comparer);
        foreach( TSource item in source ) {
            dictionary.AddValue(keySelector(item), elementSelector(item));
        }
        return dictionary;
    }
}
