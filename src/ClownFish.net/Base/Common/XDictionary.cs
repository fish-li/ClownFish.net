using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ClownFish.Base;

/// <summary>
/// 统一封装 HttpContext.Items 的 “包壳” 类型
/// </summary>
public sealed class XDictionary : IDictionary<object, object>
{
    private readonly IDictionary _dict1;
    private readonly IDictionary<object, object> _dict2;

    /// <summary>
    /// 构造方法（通常情况不建议使用此版本）
    /// </summary>
    public XDictionary()
    {
        _dict1 = null;
        _dict2 = new Dictionary<object, object>();
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="dict"></param>
    public XDictionary(IDictionary dict)
    {
        _dict1 = dict;
        _dict2 = null;
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="dict"></param>
    public XDictionary(IDictionary<object, object> dict)
    {
        _dict1 = null;
        _dict2 = dict;
    }

    /// <summary>
    /// 索引器
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object this[object key] {
        get {
            if( _dict1 != null )
                return _dict1[key];
            else
                return _dict2[key];
        }
        set {
            if( _dict1 != null )
                _dict1[key] = value;
            else
                _dict2[key] = value;
        }
    }

    /// <summary>
    /// Keys
    /// </summary>
    public ICollection<object> Keys {
        get {
            if( _dict1 != null )
                return _dict1.Keys.Cast<object>().ToList();
            else
                return _dict2.Keys;
        }
    }

    /// <summary>
    /// 此方法没有实现！
    /// </summary>
    public ICollection<object> Values => throw new NotImplementedException();

    /// <summary>
    /// Count
    /// </summary>
    public int Count => _dict1 != null ? _dict1.Count : _dict2.Count;

    /// <summary>
    /// IsReadOnly
    /// </summary>
    public bool IsReadOnly => _dict1 != null ? _dict1.IsReadOnly : _dict2.IsReadOnly;

    /// <summary>
    /// Add
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(object key, object value)
    {
        if( _dict1 != null )
            _dict1.Add(key, value);
        else
            _dict2.Add(key, value);
    }

    /// <summary>
    /// Add
    /// </summary>
    /// <param name="item"></param>
    public void Add(KeyValuePair<object, object> item)
    {
        if( _dict1 != null )
            _dict1.Add(item.Key, item.Value);
        else
            _dict2.Add(item);
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear()
    {
        if( _dict1 != null )
            _dict1.Clear();
        else
            _dict2.Clear();
    }

    /// <summary>
    /// Contains
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(KeyValuePair<object, object> item)
    {
        if( _dict1 != null )
            return _dict1.Contains(item.Key);
        else
            return _dict2.Contains(item);
    }

    /// <summary>
    /// ContainsKey
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(object key)
    {
        if( _dict1 != null )
            return _dict1.Contains(key);
        else
            return _dict2.ContainsKey(key);
    }

    /// <summary>
    /// 此方法没有实现！
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// GetEnumerator
    /// </summary>
    /// <returns></returns>
    public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
    {
        if( _dict1 != null ) {
            foreach( DictionaryEntry kv in _dict1 )
                yield return new KeyValuePair<object, object>(kv.Key, kv.Value);
        }
        else {
            foreach( var kv in _dict2 )
                yield return new KeyValuePair<object, object>(kv.Key, kv.Value);
        }
    }

    /// <summary>
    /// GetEnumerator
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _dict1 != null ? _dict1.GetEnumerator() : _dict2.GetEnumerator();
    }

    /// <summary>
    /// Remove
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Remove(object key)
    {
        if( _dict1 != null ) {
            int count1 = _dict1.Count;
            _dict1.Remove(key);
            return _dict1.Count != count1;
        }
        else {
            return _dict2.Remove(key);
        }
    }

    /// <summary>
    /// Remove
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(KeyValuePair<object, object> item)
    {
        return this.Remove(item.Key);
    }

    /// <summary>
    /// TryGetValue
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(object key, out object value)
    {
        if( _dict1 != null ) {
            if( _dict1.Contains(key) ) {
                value = _dict1[key];
                return true;
            }
            else {
                value = null;
                return false;
            }
        }
        else {
            return _dict2.TryGetValue(key, out value);
        }
    }

    /// <summary>
    /// TryGet
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object TryGet(object key)
    {
        if( this.TryGetValue(key, out object value) )
            return value;
        else
            return null;
    }
}
