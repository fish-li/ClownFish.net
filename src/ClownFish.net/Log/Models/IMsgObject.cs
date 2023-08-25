using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Log;

/// <summary>
/// 基本的 "消息/日志" 对象接口
/// </summary>
public interface IMsgObject
{
    /// <summary>
    /// 获取对象的 ID
    /// </summary>
    string GetId();

    /// <summary>
    /// 对象的创建时间
    /// </summary>
    DateTime GetTime();
}
