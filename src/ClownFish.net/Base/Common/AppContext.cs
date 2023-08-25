//#if NET45

//using System;
//using System.Collections.Generic;

//namespace ClownFish.Base
//{
//    /// <summary>
//    /// AppContext 的简化版本专供.NET45使用
//    /// </summary>
//    public static class AppContext
//    {
//        private static readonly Dictionary<string, bool> s_dict = new Dictionary<string, bool>(32);

//        /// <summary>
//        /// 设置开关的值。请在程序初始化时调用。
//        /// </summary>
//        /// <param name="switchName"></param>
//        /// <param name="isEnabled"></param>
//        public static void SetSwitch(string switchName, bool isEnabled)
//        {
//            if( string.IsNullOrEmpty(switchName) )
//                throw new ArgumentNullException(nameof(switchName));

//            lock( s_dict ) {
//                s_dict[switchName] = isEnabled;
//            }
//        }

//        /// <summary>
//        /// 尝试获取开关的值。
//        /// </summary>
//        /// <param name="switchName"></param>
//        /// <param name="isEnabled"></param>
//        /// <returns></returns>
//        public static bool TryGetSwitch(string switchName, out bool isEnabled)
//        {
//            lock( s_dict ) {
//                if( s_dict.TryGetValue(switchName, out isEnabled) ) {
//                    return true;
//                }
//            }

//            isEnabled = false;
//            return false;
//        }
//    }
//}

//#endif
