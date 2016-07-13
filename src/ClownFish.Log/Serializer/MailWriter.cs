using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Xml;
using ClownFish.Log.Configuration;

namespace ClownFish.Log.Serializer
{
	/// <summary>
	/// 将日志以邮件形式发送的写入器
	/// </summary>
	public sealed class MailWriter : ILogWriter
	{
		private static string[] s_recipients = null;

		#region ILogWriter 成员

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="config"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Init(WriterSection config)
		{
			string value = config.GetOptionValue("Receivers");
			if( string.IsNullOrEmpty(value) )
				throw new LogConfigException("日志配置文件中，没有为MailWriter指定Receivers属性。");


			if( s_recipients != null )
				return;

			s_recipients = value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// 写入单条日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="info"></param>
		public void Write<T>(T info) where T : Model.BaseInfo
		{
			if( info == null )
				return;

			string xml = XmlHelper.XmlSerialize(info, Encoding.UTF8);
			string subject = info.Message ?? info.GetType().FullName;
			SendEmail(s_recipients, subject, xml);
		}


		private static void SendEmail(string[] to, string subject, string body)
		{
			if( to == null || to.Length == 0 )
				throw new ArgumentNullException("to");


			MailMessage mail = new MailMessage();
			foreach( string address in to )
				mail.To.Add(new MailAddress(address));

			mail.Subject = subject;
			mail.Body = body;
			mail.BodyEncoding = System.Text.Encoding.UTF8;
			mail.SubjectEncoding = System.Text.Encoding.UTF8;
			mail.IsBodyHtml = false;

			SmtpClient smtp = new SmtpClient();
			string from = (smtp.Credentials as System.Net.NetworkCredential).UserName;
			mail.From = new MailAddress(from);
			smtp.Send(mail);
		}

		/// <summary>
		/// 批量写入日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public void Write<T>(List<T> list) where T : Model.BaseInfo
		{
			if( list == null || list.Count == 0 )
				return;

			foreach( T info in list )
				Write(info);
		}

		/// <summary>
		/// 根据日志ID获取单条日志信息
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="guid"></param>
		/// <returns></returns>
		public T Get<T>(Guid guid) where T : Model.BaseInfo
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
		public List<T> GetList<T>(DateTime t1, DateTime t2) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
