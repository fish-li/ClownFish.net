using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;

namespace ClownFish.Log
{
    /// <summary>
    /// 一个简单的流式消息日志记录实现类，所有写入将会以同步方式写入文件。
    /// </summary>
    public sealed class MessageLoger
    {
        private readonly string _filePath;
        private readonly object _lock;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logFilePath">日志的保存文件路径</param>
        /// <param name="supportConcurrent">是否支持多线程的并发调用</param>
        public MessageLoger(string logFilePath, bool supportConcurrent)
        {
            if( string.IsNullOrEmpty(logFilePath) )
                throw new ArgumentNullException(logFilePath);

            _filePath = logFilePath;

            if( supportConcurrent )
                _lock = new object();
        }


        /// <summary>
        /// 写入一条消息到日志文件
        /// </summary>
        /// <param name="message">消息文本</param>
        public void Write(string message)
        {
            Write(null, message);
        }


        /// <summary>
        /// 写入一条消息到日志文件。
        /// 说明：为了防止程序突然崩溃，写入消息时，不做任何缓冲处理，且每次写入时打开文件
        /// </summary>
        /// <param name="category">消息类别，默认：INFO</param>
        /// <param name="message">消息文本</param>
        public void Write(string category, string message)
        {
            if( string.IsNullOrEmpty(message) )
                return;

            if( string.IsNullOrEmpty(category) )
                category = "INFO";

            string time = DateTime.Now.ToTimeString();
            string threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            string line = $"{time}, [{category}] [Thread: {threadId}], {message}\r\n";

            if( _lock != null ) {
                lock( _lock ) {
                    System.IO.File.AppendAllText(_filePath, line, Encoding.UTF8);
                }
            }
            else {
                System.IO.File.AppendAllText(_filePath, line, Encoding.UTF8);
            }

        }
    }
}
