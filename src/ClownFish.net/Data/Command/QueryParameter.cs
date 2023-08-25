namespace ClownFish.Data;

/// <summary>
/// 表示一个SQL参数对象
/// </summary>
public sealed class QueryParameter
{
    private readonly object _val;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="val">要包装的参数值</param>
    public QueryParameter(object val)
    {
        this._val = val;
    }

    /// <summary>
    /// 参数值
    /// </summary>
    public object Value {
        get { return this._val; }
    }


    // 注意：string 不能以隐式方式转QueryParameter，
    // 因为：CPQuery重载了与string的 + 运算符，行为是拼接SQL语句，而非做为参数处理。

    /// <summary>
    /// 将string【显式】转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static explicit operator QueryParameter(string value)
    {
        return new QueryParameter(value);
    }


    #region 各种数据类型到QueryParameter的隐式转换

    /// <summary>
    /// 将DBNull隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(DBNull value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将bool隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(bool value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将byte隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(byte value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将sbyte隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(sbyte value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将byte隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(byte[] value)
    {
        return new QueryParameter(value);
    }


    /// <summary>
    /// 将char隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(char value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将short隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(short value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将ushort隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(ushort value)
    {
        return new QueryParameter(value);
    }



    /// <summary>
    /// 将int隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(int value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将uint隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(uint value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将long隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(long value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将ulong隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(ulong value)
    {
        return new QueryParameter(value);
    }



    /// <summary>
    /// 将float隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(float value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将double隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(double value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将decimal隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(decimal value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将DateTime隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(DateTime value)
    {
        return new QueryParameter(value);
    }


    /// <summary>
    /// 将TimeSpan隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(TimeSpan value)
    {
        return new QueryParameter(value);
    }


    /// <summary>
    /// 将Guid隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(Guid value)
    {
        return new QueryParameter(value);
    }


    /// <summary>
    /// 将int[]隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(int[] value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将int[]隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(List<int> value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将long[]隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(long[] value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将long[]隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(List<long> value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将Guid[]隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(Guid[] value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将Guid[]隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(List<Guid> value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将string[]隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(string[] value)
    {
        return new QueryParameter(value);
    }

    /// <summary>
    /// 将string[]隐式转换为QueryParameter
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>QueryParameter实例</returns>
    public static implicit operator QueryParameter(List<string> value)
    {
        return new QueryParameter(value);
    }



    #endregion

}
