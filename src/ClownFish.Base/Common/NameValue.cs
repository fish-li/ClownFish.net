using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClownFish.Base
{
    /// <summary>
    /// 表示一组用于HTTP传输的 【名称/值】 对。
    /// </summary>
    [Serializable]
    public sealed class NameValue
	{
        /// <summary>
        /// 键名
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// 键值
        /// </summary>
        [XmlAttribute]
        public string Value { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        public NameValue() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public NameValue(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

		/// <summary>
		/// ToString
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}={1}", this.Name, this.Value);
		}
	}
}
