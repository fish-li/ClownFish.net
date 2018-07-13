using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Xml;
using ClownFish.Log;
using ClownFish.Log.Configuration;

namespace ClownFish.Log.Serializer
{
	/// <summary>
	/// 将日志记录到MSMQ的写入器
	/// </summary>
	public class MsmqWriter : ILogWriter
	{
		private static string s_rootPath = null;

		#region ILogWriter 成员

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="config"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Init(WriterSection config)
		{
			string value = config.GetOptionValue("RootPath");
			if( string.IsNullOrEmpty(value) )
				throw new LogConfigException("日志配置文件中，没有为MsmqWriter指定RootPath属性。");


			if( s_rootPath != null )
				return;


			// 检查需要记录的各个数据类型的队列是否存在。
			foreach( var item in WriterFactory.Config.Types ) {
				string path = value + "-" + item.Type.Name;

				if( MessageQueue.Exists(path) == false )
					using( MessageQueue messageQueue = MessageQueue.Create(path) ) {
						messageQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
					}
			}

			s_rootPath = value + "-";
		}

        /// <summary>
        /// 写入单条日志信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="info"></param>
        public virtual void Write<T>(T info) where T : Model.BaseInfo
		{
			if( info == null )
				return;

			// 注意：取类型名称时，不采用 info.GetType().Name ，因为可能有继承情况
			string path = s_rootPath + typeof(T).Name;

			string xml = XmlHelper.XmlSerialize(info, Encoding.UTF8);

			using( MessageQueue messageQueue = new MessageQueue(path, QueueAccessMode.Send) ) {
				messageQueue.Send(xml, info.Message.ToString().SubstringN(128));
			}
		}

        /// <summary>
        /// 批量写入日志信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public virtual void Write<T>(List<T> list) where T : Model.BaseInfo
		{
			if( list == null || list.Count == 0 )
				return;

			// 注意：取类型名称时，不采用 info.GetType().Name ，因为可能有继承情况
			string path = s_rootPath + typeof(T).Name;

			using( MessageQueue messageQueue = new MessageQueue(path, QueueAccessMode.Send) ) {
				foreach( T info in list ) {
					string xml = XmlHelper.XmlSerialize(info, Encoding.UTF8);
					messageQueue.Send(xml, info.Message.ToString().SubstringN(128));
				}
			}
		}

        /// <summary>
        /// 根据日志ID获取单条日志信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="guid"></param>
        /// <returns></returns>
        public virtual T Get<T>(Guid guid) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

        /// <summary>
        /// 根据指定的一段时间获取对应的日志记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public virtual List<T> GetList<T>(DateTime t1, DateTime t2) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
