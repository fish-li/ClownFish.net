using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Internals;
using ClownFish.Base.TypeExtend;
using ClownFish.Data;

namespace ClownFish.Data
{
    internal class EventManagerEventSubscriber : EventSubscriber<EventManager>
    {
        private static readonly object s_lock = new object();
        private static bool s_inited = false;

        private DateTime _startTime;

        internal static bool Register()
        {
            if( s_inited == false ) {
                lock( s_lock ) {
                    if( s_inited == false ) {
                        ExtenderManager.RegisterSubscriber(typeof(EventManagerEventSubscriber));

                        s_inited = true;

                        // 第一次注册返回 true，后面调用全部返回 false
                        return true;
                    }
                }
            }

            return false;
        }

        public override void SubscribeEvent(EventManager instance)
        {
            instance.BeforeExecute += Instance_BeforeExecute;
            instance.AfterExecute += Instance_AfterExecute;
        }

        
        private void Instance_BeforeExecute(object sender, CommandEventArgs e)
        {
            _startTime = DateTime.Now;
        }

        private void Instance_AfterExecute(object sender, CommandEventArgs e)
        {
            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime - _startTime;

            //PerformanceModule.CheckDbExecuteTime(e.DbCommand, ts);
            InternalBridge.PerformanceModuleCheckDbExecuteTime?.Invoke(e.DbCommand, ts);
        }
    }
}
