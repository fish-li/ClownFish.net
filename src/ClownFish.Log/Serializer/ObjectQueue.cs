using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;

namespace ClownFish.Log.Serializer
{
    /// <summary>
    /// 一个简单的线程安全的消息队列，主要特点是可以限制队列长度。
    /// </summary>
    internal sealed class ObjectQueue
    {
        private readonly object _lock = new object();

        private Queue<object> _queue = new Queue<object>(1024);

        /// <summary>
        /// 队列中允许的最大长度，如果超过这个长度，队列就不接收消息。避免占用大量内存而导致OOM
        /// </summary>
        private static readonly int s_maxQueueLength = ConfigurationManager.AppSettings["ClownFish.Log.Serializer.ObjectQueue:MaxQueueLength"].TryToUInt(10000);


        /// <summary>
        /// 将消息压到消息队列
        /// </summary>
        /// <param name="message"></param>
        public void Enqueue(object message)
        {
            lock( _lock ) {
                if( _queue.Count >= s_maxQueueLength )
                    return;

                _queue.Enqueue(message);
            }
        }

        /// <summary>
        /// 从消息队列取出一条消息
        /// </summary>
        /// <returns></returns>
        public object Dequeue()
        {
            lock( _lock ) {
                if( _queue.Count == 0 )
                    return null;

                return _queue.Dequeue();
            }
        }
    }
}
