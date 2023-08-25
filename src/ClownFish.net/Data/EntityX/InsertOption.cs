namespace ClownFish.Data;

/// <summary>
/// Insert操作控制参数
/// </summary>
/// <remarks>
/// insert 会有以下几类需求：
/// 1，根据指定过的属性动态生成INSERT语句，未指定的字段使用数据库默认值。
/// 2，为全部字段生成INSERT语句，此方法性能会更好。
/// 3，执行INSERT之后，需要获取新产生的 自增ID
/// 4，实现幂等操作，忽略重插入复异常（由唯一索引触发）
/// 
/// insert 返回值规则定义：
/// 对于需求1，如果不能生成INSERT语句(没有给任何属性赋值)，则返回 0
/// IgnoreDuplicateError=true 且出现 重复插入异常，则返回 -1
/// GetNewId=false，正常情况返回 ExecuteNonQuery() 结果
/// GetNewId=true，正常情况返回 新产生的自增ID
/// </remarks>
[Flags]
public enum InsertOption
{
    /// <summary>
    /// 默认行为。
    /// </summary>
    NoSet = 0,

    /// <summary>
    /// 为全部字段生成INSERT语句，
    /// 如果不指定此项，则（根据指定过的属性动态生成INSERT语句）
    /// </summary>
    AllFields = 1,

    /// <summary>
    /// 执行INSERT之后，需要获取新产生的 自增ID，
    /// 目前仅支持 SQLSERVER/MySQL/PostgreSQL
    /// </summary>
    GetNewId = 2,

    /// <summary>
    /// 实现幂等操作，忽略重插入复异常，
    /// 如果出现异常，Inert方法返回 -1
    /// </summary>
    IgnoreDuplicateError = 4
}
