using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClownFish.Base.TypeExtend;
using ClownFish.TestApplication1.Common;
using ClownFish.TestApplication1.Test;
using ClownFish.Web.Client;

namespace ClownFish.TestApplication1
{
	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ExtenderManager.RegisterSubscriber(typeof(HttpClientEventSubscriber));

			Application.Run(new Form1());
		}

	}
}
