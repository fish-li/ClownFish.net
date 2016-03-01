using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Xml;

namespace ClownFish.Web.UnitTest.Models
{
	public class CDATAModel
	{
		public int A { get; set; }

		public XmlCdata Text { get; set; }
	}
}
