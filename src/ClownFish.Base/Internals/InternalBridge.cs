using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.Internals
{
    /// <summary>
    /// 为实现框架内部解耦的辅助类
    /// </summary>
    internal static class InternalBridge
    {
        public static Func<bool> RegisterEventManagerEventSubscriber;


        public static Action<DbCommand, TimeSpan> PerformanceModuleCheckDbExecuteTime;
    }
}
