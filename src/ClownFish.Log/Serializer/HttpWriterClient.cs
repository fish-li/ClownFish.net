using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClownFish.Base;

namespace ClownFish.Log.Serializer
{
    internal class HttpWriterClient
    {
        /// <summary>
        /// 一个简单的，线程安全的，消息队列，用于缓存将要发送到服务端的消息
        /// </summary>
        internal sealed class HttpMessageQueue
        {
            private readonly object _lock = new object();

            private Queue<object> _queue = new Queue<object>(1024);

            /// <summary>
            /// 队列中允许的最大长度，如果超过这个长度，队列就不接收消息。避免占用大量内存而导致OOM
            /// </summary>
            private static readonly int s_maxQueueLength = ConfigurationManager.AppSettings["ClownFish.Log.Serializer.HttpWriterClient:MaxQueueLength"].TryToUInt(10000);


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


        private static readonly int s_WaitMillisecond = ConfigurationManager.AppSettings["ClownFish.Log.Serializer.HttpWriterClient:WaitMillisecond"].TryToUInt(500);
        private readonly HttpMessageQueue _messageQueue = new HttpMessageQueue();
        private Thread _thread = null;



        /// <summary>
        /// 添加一条消息到待发送队列
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(object message)
        {
            if( message == null )
                return;

            Start();

            _messageQueue.Enqueue(message);
        }

        /// <summary>
        /// 启动后台发送线程
        /// </summary>
        private void Start()
        {
            if( _thread != null )
                return;

            _thread = new Thread(ThreadMethod);
            _thread.IsBackground = true;
            _thread.Name = "HttpWriterClient";
            _thread.Start();
        }



        private void ThreadMethod()
        {
            while( true ) {
                SendMessageToServer();

                // 间隔 500 毫秒执行
                Thread.Sleep(s_WaitMillisecond);
            }
        }


        private void SendMessageToServer()
        {
            while( true ) {
                object message = _messageQueue.Dequeue();
                if( message == null )
                    break;

                HttpWriter.SendData(message);
            }

        }

    }
}
