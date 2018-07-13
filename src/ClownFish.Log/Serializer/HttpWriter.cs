using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ClownFish.Base.TypeExtend;
using ClownFish.Log.Configuration;
using ClownFish.Log.Model;

namespace ClownFish.Log.Serializer
{
    /// <summary>
    /// 将日志记录以HTTP形式发送到Web服务器
    /// </summary>
    public class HttpWriter : ILogWriter
    {
        /// <summary>
        /// HttpWriterClient实例
        /// </summary>
        protected static HttpWriterClient s_client = null;

        #region ILogWriter 接口方法

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Init(WriterSection config)
        {
            // 避免重复调用
            if( s_client != null )
                return;

            HttpWriterClient client = ObjectFactory.New<HttpWriterClient>();
            client.Init(config);

            s_client = client;
        }

       



        /// <summary>
        /// 写入单条日志信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        public virtual void Write<T>(T info) where T : BaseInfo
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
        public virtual void Write<T>(List<T> list) where T : BaseInfo
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
        public virtual T Get<T>(Guid guid) where T : BaseInfo
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
        public virtual List<T> GetList<T>(DateTime t1, DateTime t2) where T : BaseInfo
        {
            throw new NotImplementedException();
        }

        #endregion




    }
}
