using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Log.UnitTest
{
	[TestClass]
	public class Initializer
	{
		[AssemblyInitialize]
		public static void InitRuntime(TestContext context)
		{


			CopyConfigFiles();
		}


		private static void CopyConfigFiles()
		{
			string bakPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\bin_bak") + "\\";
			string binPath = AppDomain.CurrentDomain.BaseDirectory + "\\";

			CopyFile(bakPath, binPath, "ClownFish.Log.config");
		}

		private static void CopyFile(string bakPath, string binPath, string filename)
		{
			if( File.Exists(binPath + filename) == false )
				File.Copy(bakPath + filename, binPath + filename);
		}
	}
}
