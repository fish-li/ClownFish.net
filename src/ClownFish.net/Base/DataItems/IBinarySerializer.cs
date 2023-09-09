namespace ClownFish.Base;

#if NETCOREAPP

/// <summary>
/// 定义一个简单的二进制序列化接口
/// </summary>
public interface IBinarySerializer
{
    /// <summary>
    /// 将对象序列化成二进制数组
    /// </summary>
    /// <returns></returns>
    byte[] ToBytes();


    /// <summary>
    /// 从二进制数组中加载数据
    /// </summary>
    /// <param name="body"></param>
    void LoadData(ReadOnlyMemory<byte> body);
}

#endif
