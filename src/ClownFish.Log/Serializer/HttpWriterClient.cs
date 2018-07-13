using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Http;
using ClownFish.Base.WebClient;
using ClownFish.Log.Configuration;

namespace ClownFish.Log.Serializer
{
    /// <summary>
    /// 发送日志数据的客户端
    /// </summary>
    public class HttpWriterClient
    {
        private string _url = null;
        private SerializeFormat _format = SerializeFormat.None;
        private List<NameValue> _header = null;

        private int _retryCount = 0;
        private int _retryWaitMillisecond = 0;

        private readonly int _waitMillisecond = ConfigurationManager.AppSettings["ClownFish.Log.Serializer.HttpWriterClient:WaitMillisecond"].TryToUInt(500);
        private readonly ObjectQueue _messageQueue = new ObjectQueue();


        private Thread _thread = null;
        private bool _threadInited = false;
        private readonly object _lock = new object();
        



        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Init(WriterSection config)
        {
            string url = config.GetOptionValue("url");
            if( string.IsNullOrEmpty(url) )
                throw new LogConfigException("日志配置文件中，没有为HttpWriter指定url参数。");

            string format = config.GetOptionValue("format");
            _format = (string.IsNullOrEmpty(format) || format.Is("json")) ? SerializeFormat.Json : SerializeFormat.Xml;


            _retryCount = config.GetOptionValue("retry-count").TryToUInt(10);
            _retryWaitMillisecond = config.GetOptionValue("retry-wait-millisecond").TryToUInt(1000);

            _header = ReadHttpArgs(config, "header:");

            List<NameValue> queryString = ReadHttpArgs(config, "querystring:");
            _url = url.ConcatQueryStringArgs(queryString);
        }


        /// <summary>
        /// 读取指定前缀的配置参数
        /// </summary>
        /// <param name="config"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected virtual List<NameValue> ReadHttpArgs(WriterSection config, string flag)
        {
            List<NameValue> list = new List<NameValue>();

            foreach( var item in config.Options ) {
                string key = item.Key;

                if( key.StartsWith(flag) ) {

                    key = key.Substring(flag.Length);
                    if( key.Trim().Length == 0 )
                        continue;

                    list.Add(new NameValue { Name = key, Value = item.Value });
                }
            }

            if( list.Count == 0 )
                return null;
            else
                return list;
        }

        

        /// <summary>
        /// 添加一条消息到待发送队列
        /// </summary>
        /// <param name="message"></param>
        public virtual void AddMessage(object message)
        {
            if( message == null )
                return;

            StartThread();

            _messageQueue.Enqueue(message);
        }


        /// <summary>
        /// 启动后台发送线程
        /// </summary>
        protected void StartThread()
        {
            if( _threadInited == false ) {
                lock( _lock ) {
                    if( _threadInited == false ) {

                        _thread = new Thread(ThreadMethod);
                        _thread.IsBackground = true;
                        _thread.Name = "HttpWriterClient";
                        _thread.Start();

                        _threadInited = true;
                    }
                }
            }
        }
        

        private void ThreadMethod()
        {
            while( true ) {
                SendMessageToServer();

                // 间隔 500 毫秒执行
                Thread.Sleep(_waitMillisecond);
            }
        }


        private void SendMessageToServer()
        {
            while( true ) {
                object message = _messageQueue.Dequeue();
                if( message == null )
                    break;

                SendData(message);
            }

        }

        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        /// <param name="data"></param>
        public virtual void SendData(object data)
        {
            if( data == null )
                return;

            HttpOption httpOption = CreateHttpOption(data);
            if( httpOption == null )
                return;

            Retry retry = Retry.Create(_retryCount, _retryWaitMillisecond);
            httpOption.GetResult(retry);
        }


        /// <summary>
        /// 创建HttpOption实例
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual HttpOption CreateHttpOption(object data)
        {
            string url = _url.ConcatQueryStringArgs("x-datatype", data.GetType().Name);
            
            HttpOption httpOption = new HttpOption();
            httpOption.Url = url;
            httpOption.Method = "POST";
            httpOption.Format = _format;
            httpOption.Data = data;

            if( _header != null && _header.Count > 0 ) {
                foreach( var item in _header )
                    httpOption.Headers.Add(item.Name.UrlEncode(), item.Value.UrlEncode());
            }

            //httpOption.Headers.Add("x-datatype", data.GetType().Name.UrlEncode());
            return httpOption;
        }


    }
}
