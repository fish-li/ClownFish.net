using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	/// <summary>
	/// XmlCommand工厂，供框架内部使用
	/// </summary>
	public sealed class XmlCommandFactory
	{
		internal XmlCommandFactory() { }

		internal DbContext Context { get; set; }

		/// <summary>
		/// 根据XmlCommand名称创建XmlCommand实例
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public XmlCommand Create(string name)
		{
			XmlCommand command = new XmlCommand(this.Context);
			command.Init(name, null);
			return command;
		}


		/// <summary>
		/// 根据XmlCommand名称和参数对象创建XmlCommand实例
		/// </summary>
		/// <param name="name"></param>
		/// <param name="argsObject"></param>
		/// <returns></returns>
		public XmlCommand Create(string name, object argsObject)
		{
			XmlCommand command = new XmlCommand(this.Context);
			command.Init(name, argsObject);
			return command;
		}

		/// <summary>
		/// 根据XmlCommand名称和参数对象创建XmlCommand实例
		/// </summary>
		/// <param name="name"></param>
		/// <param name="dictionary"></param>
		/// <returns></returns>
		public XmlCommand Create(string name, Dictionary<string, string> dictionary)
		{
			XmlCommand command = new XmlCommand(this.Context);
			command.Init(name, dictionary);
			return command;
		}
	}
}
