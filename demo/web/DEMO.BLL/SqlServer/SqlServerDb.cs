using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ClownFish;
using DEMO.Model;
using System.IO;
using System.Web;

namespace DEMO.BLL
{
	public class SqlServerDb : IDatabase
	{
		
		public void AppStart()
		{
			Profiler.ApplicationName = "ClownFish.Web DEMO - StoreProcedure";
			DbHelper.DefaultCommandKind = CommandKind.StoreProcedure;

			ConfigClownFish();
		}

		public void AppEnd()
		{
		}


		internal static void ConfigClownFish()
		{
			ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["MyNorthwind"];
			DbContext.RegisterDbConnectionInfo("default", setting.ProviderName, "@", setting.ConnectionString);

			DbContextDefaultSetting.AutoRetrieveOutputValues = true;

			Profiler.TryStartClownFishProfiler();


			// 记录所有编译异常。
			BuildManager.OnBuildException += new BuildExceptionHandler(BuildManager_OnBuildException);


			// 手动提交要编译加载器的数据实体类型。
			// 说明：这个步骤也可以不执行，那么所有使用过的数据实体类型将会在自动编译模式下得到处理，但不会那么及时。
			Type[] models = BuildManager.FindModelTypesFromCurrentApplication(t => t.Namespace =="DEMO.Model");
			BuildManager.CompileModelTypesSync(models, true);


			// 启动自动编译数据实体加载器的工作模式。
			//BuildManager.StartAutoCompile(() => true);	// 只要定时器被触发就执行编译任务。
			BuildManager.StartAutoCompile(() => BuildManager.RequestCount > 500);	// 当请求编译的数量超过500次时就执行编译任务。
		}

		private static void BuildManager_OnBuildException(Exception ex)
		{
			CompileException ce = ex as CompileException;
			if( ce != null )
				SafeLogException(ce.GetDetailMessages());
			else
				SafeLogException(ex.ToString());
		}
		private static void SafeLogException(string message)
		{
			try {
				string logfilePath = Path.Combine(HttpRuntime.AppDomainAppPath, @"App_Data\ErrorLog.txt");

				File.AppendAllText(logfilePath, "=========================================\r\n" + message, System.Text.Encoding.UTF8);
			}
			catch { }
		}

	}
}
