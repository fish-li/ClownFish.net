using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Http;
using ClownFish.Base.WebClient;
using ClownFish.Log.Configuration;
using ClownFish.Log.Model;

namespace ClownFish.Log.Serializer
{
    /// <summary>
    /// 将日志记录以HTTP形式发送到Web服务器
    /// </summary>
    public sealed class HttpWriter : ILogWriter
    {
        private static string s_url = null;
        private static SerializeFormat s_format = SerializeFormat.None;
        private static bool s_datatypeInHeader = true;
        private static List<NameValue> s_header = null;

        private static int s_retryCount = 0;
        private static int s_retryWaitMillisecond = 0;

        private static HttpWriterClient s_client = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        public void Init(WriterSection config)
        {
            // 避免重复调用
            if( s_url != null )
                return;


            string url = config.GetOptionValue("url");
            if( string.IsNullOrEmpty(url) )
                throw new LogConfigException("日志配置文件中，没有为HttpWriter指定url属性。");

            string format = config.GetOptionValue("format");
            if( string.IsNullOrEmpty(format) ) {
                s_format = SerializeFormat.Xml;     // 默认值
            }
            else {
                if( Enum.TryParse<SerializeFormat>(format, out s_format) == false
                    || s_format == SerializeFormat.None
                    || s_format == SerializeFormat.Auto
                    )
                    throw new LogConfigException("日志配置文件中，为HttpWriter指定的format属性无效，建议选择：Json or Xml");
            }

            s_retryCount = config.GetOptionValue("retry-count").TryToUInt(10);
            s_retryWaitMillisecond = config.GetOptionValue("retry-wait-millisecond").TryToUInt(1000);
            s_datatypeInHeader = config.GetOptionValue("datatype-in-header").TryToBool(true);

            List<NameValue> queryString = ReadHttpArgs(config, "querystring:");
            s_header = ReadHttpArgs(config, "header:");

            s_url = url.ConcatQueryStringArgs(queryString);
            s_client = new HttpWriterClient();
        }

        private static List<NameValue> ReadHttpArgs(WriterSection config, string flag)
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


        private static HttpOption CreateHttpOption(object data)
        {
            string url = s_url;
            if( s_datatypeInHeader == false ) {
                url = url.ConcatQueryStringArgs("x-datatype", data.GetType().Name.UrlEncode());
            }

            HttpOption httpOption = new HttpOption();
            httpOption.Url = s_url;
            httpOption.Method = "POST";
            httpOption.Format = s_format;
            httpOption.Data = data;

            if( s_header != null && s_header.Count > 0 ) {
                foreach( var item in s_header )
                    httpOption.Headers.Add(item.Name.UrlEncode(), item.Value.UrlEncode());
            }

            if( s_datatypeInHeader ) {
                httpOption.Headers.Add("x-datatype", data.GetType().Name.UrlEncode());
            }

            return httpOption;
        }


        internal static void SendData(object data)
        {
            if( data == null )
                throw new ArgumentNullException(nameof(data));

            HttpOption httpOption = CreateHttpOption(data);

            Retry retry = Retry.Create(s_retryCount, s_retryWaitMillisecond);
            httpOption.GetResult(retry);
        }



        /// <summary>
        /// 写入单条日志信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        public void Write<T>(T info) where T : BaseInfo
        {
            if( info == null )
                return;

            s_client.AddMessage(info);
        }

        /// <summary>
        /// 批量写入日志信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void Write<T>(List<T> list) where T : BaseInfo
        {
            if( list == null || list.Count == 0 )
                return;

            foreach(var info in list)
                s_client.AddMessage(info);
        }


        
        /// <summary>
        /// 不支持的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guid"></param>
        /// <returns></returns>
        public T Get<T>(Guid guid) where T : BaseInfo
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 不支持的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public List<T> GetList<T>(DateTime t1, DateTime t2) where T : BaseInfo
        {
            throw new NotImplementedException();
        }


    }
}
