using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DEMO.BLL
{
	public class XmlDb :IDatabase
	{
		private static readonly string XmlDbFilePath =
			System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\MyNorthwindDataBase.xml");

		public void AppStart()
		{
			WebSiteDB.LoadDbFromXml(XmlDbFilePath);
		}

		public void AppEnd()
		{
			WebSiteDB.SaveDbToXml(XmlDbFilePath);
		}


	}
}
