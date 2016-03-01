using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ClownFish.Base.Xml;

namespace ClownFish.Log.Model
{
	/// <summary>
	/// Name / Value 值对
	/// </summary>
	[Serializable]
	public sealed class NameValue
	{
		/// <summary>
		/// Name 值
		/// </summary>
		[XmlAttribute]
		public string Name { get; set; }

		/// <summary>
		/// Value 值
		/// </summary>
		//[XmlAttribute]
		public XmlCdata Value { get; set; }
	}
}
