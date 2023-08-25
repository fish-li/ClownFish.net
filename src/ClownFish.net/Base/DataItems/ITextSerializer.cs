namespace ClownFish.Base;

/// <summary>
/// 定义一个简单的文本序列化接口
/// </summary>
public interface ITextSerializer
{
    /// <summary>
    /// 将对象序列化成文本字符串
    /// </summary>
    /// <returns></returns>
    string ToText();

   
    /// <summary>
    /// 从文本中加载数据
    /// </summary>
    /// <param name="text"></param>
    void LoadData(string text);
}
