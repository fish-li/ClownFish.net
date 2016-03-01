using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web;
using ClownFish;
using DEMO.Model;


namespace DEMO.BLL
{
	public class SqlServerDb2 : IDatabase
	{
		public void AppStart()
		{
			Profiler.ApplicationName = "ClownFish.Web DEMO - XmlCommand";
			DbHelper.DefaultCommandKind = CommandKind.XmlCommand;

			SqlServerDb.ConfigClownFish();

			string path = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\XmlCommand");
			XmlCommandManager.LoadCommnads(path);
		}

		public void AppEnd()
		{
		}

		


	}
}
