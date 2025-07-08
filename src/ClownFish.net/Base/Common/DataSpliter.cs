namespace ClownFish.Base;

/// <summary>
/// 数据分割工具
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class DataSpliter<T> where T : class
{
    private readonly List<T> _list;
    private readonly Func<T, string> _serializer;
    private readonly int _partSize;
    private readonly string _separator;
    private StringBuilder _sb;

    private int _index = 0;
    private string _lastLine = null;


    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="list">数据列表，它的元素将用于序列化，并切割成期望大小的数据块</param>
    /// <param name="partSize">希望得到的每个序列化后的数据块长度</param>
    public DataSpliter(List<T> list, int partSize) : this(list, DefaultSerializer, partSize, "\n")
    {
    }

    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="list">数据列表，它的元素将用于序列化，并切割成期望大小的数据块</param>
    /// <param name="serializer">列表中单个元素的序列化方法</param>
    /// <param name="partSize">希望得到的每个序列化后的数据块长度</param>
    /// <param name="separator">每个元素序列化后的拼接分隔符</param>
    public DataSpliter(List<T> list, Func<T, string> serializer, int partSize, string separator)
    {
        if( list == null )
            throw new ArgumentNullException(nameof(list));
        if( serializer == null )
            throw new ArgumentNullException(nameof(serializer));
        if( partSize <= 0 )
            throw new ArgumentOutOfRangeException(nameof(partSize));

        _list = list;
        _partSize = partSize;
        _serializer = serializer;
        _separator = separator ?? "\n";
    }

    private static string DefaultSerializer(T data)
    {
        if( data == null )
            return null;

        return data.ToJson();
    }

    /// <summary>
    /// 按照构造方法的参数来获取数据块
    /// </summary>
    /// <returns></returns>
    public string GetNextPart()
    {
        if( _sb == null )
            _sb = new StringBuilder(_partSize);

        int count = 0;
        using( StringWriter writer = new StringWriter(_sb) ) {
            count = GetNextPart(writer);
        }

        if( count == 0 ) {
            return null;
        }
        else {
            string result = _sb.ToString();
            _sb.Clear();

            if( result.Length != count )
                throw new ApplicationException("DataSpliter.GetNextPart inner error!");

            return result;
        }
    }


    /// <summary>
    /// 按照构造方法的参数来获取数据块
    /// </summary>
    /// <param name="writer"></param>
    /// <returns></returns>
    public int GetNextPart(TextWriter writer)
    {
        int count = 0;

        // 上一次已读出，但由于超过分块长度而没有“合并”的部分
        if( string.IsNullOrEmpty(_lastLine) == false ) {
            writer.Write(_lastLine);
            count += _lastLine.Length;
            _lastLine = null;
        }

        // 上一次读了一个“大”块头，只能单独返回
        if( count >= _partSize )
            return count;

        if( _index >= _list.Count )
            return count;



        while( true ) {

            _lastLine = _serializer(_list[_index]);
            _index++;

            if( string.IsNullOrEmpty(_lastLine) == false ) {
                if( count + _separator.Length + _lastLine.Length > _partSize ) {

                    if( count == 0 ) {
                        writer.Write(_lastLine);
                        count += _lastLine.Length;
                        _lastLine = null;
                    }
                    else {
                        break;
                    }
                }
                else {
                    if( count > 0 ) {
                        writer.Write(_separator);
                        count += _separator.Length;
                    }

                    writer.Write(_lastLine);
                    count += _lastLine.Length;
                    _lastLine = null;
                }
            }

            if( _index >= _list.Count )
                break;

        }

        return count;
    }
}
