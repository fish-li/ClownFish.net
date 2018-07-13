using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClownFish.Base;

namespace ClownFish.Log
{
    /// <summary>
    /// 一个简单的流式消息日志记录实现类，所有写入将会以同步方式写入文件。
    /// </summary>
    public class MessageLoger
    {
        private readonly string _filePath;
        private readonly object _lock;
        private readonly long _maxLength;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logFilePath">日志的保存文件路径</param>
        /// <param name="supportConcurrent">是否支持多线程的并发调用</param>
        /// <param name="maxLength">文件的最大长度</param>
        public MessageLoger(string logFilePath, bool supportConcurrent = false, long maxLength = 0)
        {
            if( string.IsNullOrEmpty(logFilePath) )
                throw new ArgumentNullException(logFilePath);

            _filePath = logFilePath;
            _maxLength = maxLength;

            if( supportConcurrent ) {
                _lock = new object();
            }
        }


 
        /// <summary>
        /// 写入一条消息到日志文件。
        /// 说明：为了防止程序突然崩溃，写入消息时，不做任何缓冲处理，且每次写入时打开文件
        /// </summary>
        /// <param name="category">消息类别，默认：INFO</param>
        /// <param name="message">消息文本</param>
        public virtual string Write(string message, string category = null)
        {
            // 扩展点：如果希望在写文件时，同时将消息输出到控制台，可以重写这个方法。

            string line = GetLine(message, category);
            if( line == null )
                return null;

            if( _lock != null ) {
                lock( _lock ) {
                    FileHelper.AppendAllText(_filePath, line, Encoding.UTF8, _maxLength);
                }
            }
            else {
                FileHelper.AppendAllText(_filePath, line, Encoding.UTF8, _maxLength);
            }
            return line;
        }

        /// <summary>
        /// 根据指定参数计算要写入文件的消息行文本
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual string GetLine(string message, string category = null)
        {
            if( string.IsNullOrEmpty(message) )
                return null;

            if( string.IsNullOrEmpty(category) )
                category = "INFO";

            string time = DateTime.Now.ToTimeString();
            Thread currentThread = Thread.CurrentThread;
            string threadId = currentThread.Name ?? "Thread " + currentThread.ManagedThreadId.ToString();

            return $"{time} [{threadId}] [{category}] {message}\r\n";
        }
    }
}
