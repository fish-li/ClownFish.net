using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.TypeExtend;
using ClownFish.MockAspnetRuntime;
using ClownFish.Web.UnitTest.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest
{
	[TestClass]
	public class Initializer
	{
		private static readonly HttpRuntime HttpRuntimeInstance = new HttpRuntime();


		[AssemblyInitialize]
		public static void InitRuntime(TestContext context)
		{
			ExtenderManager.RegisterExtendType(typeof(MvcRuntimeExt));

			MockHttpRuntime.AppDomainAppPath = AppDomain.CurrentDomain.BaseDirectory;
			MockHttpRuntime.AppDomainAppVirtualPath = "/";

			CopyConfigFiles();
		}


		private static void CopyConfigFiles()
		{
			string bakPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\bin_bak") + "\\";
			string binPath = AppDomain.CurrentDomain.BaseDirectory + "\\";

			
			if( Directory.Exists(binPath + "Page") == false )
				Directory.CreateDirectory(binPath + "Page");

			CopyFile(bakPath, binPath, "ClownFish.Web.config");
			CopyFile(bakPath, binPath, "ClownFish.Web.OutputCache.config");
			CopyFile(bakPath, binPath, "ClownFish.Web.RouteTable.config");
			CopyFile(bakPath, binPath, "Page\\404DiagnoseResult.aspx");
		}


		private static void CopyFile(string bakPath, string binPath, string filename)
		{
			if( File.Exists(binPath + filename) == false )
				File.Copy(bakPath + filename, binPath + filename);
		}
	}
}
