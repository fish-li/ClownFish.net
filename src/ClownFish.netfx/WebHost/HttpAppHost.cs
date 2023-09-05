using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Http.Pipleline;
using ClownFish.WebApi.Controllers;
using ClownFish.WebApi.Routing;
using ClownFish.WebHost.Config;

namespace ClownFish.WebHost
{
    internal class HttpAppHost
    {
        private static NHttpApplication s_httpApplication;

        private static readonly object s_lock = new object();
		private static bool s_inited = false;


		public static void Init()
		{
			if( s_inited == false ) {
				lock( s_lock ) {
					if( s_inited == false ) {

						// 开启静态文件下载
						StaticFileHandlerFactory.Instance.Init();
						DirectoryBrowseHandlerFactory.Instance.Init();

						// 初始化路由表
						RoutingManager.Init();

						// 加载HTTP模块
						LoadModules();

						// 启动 HTTP管线
						s_httpApplication = NHttpApplication.Start();

						s_inited = true;
					}
				}
			}
		}


		private static void LoadModules()
		{
			ServerOption option = ServerOption.Get();
			if( option.Modules.IsNullOrEmpty() )
				return;

			foreach( string s in option.Modules ) {
				Type t = Type.GetType(s, true, false);
				NHttpModuleFactory.RegisterModule(t);
			}
		}


		public static NHttpApplication Application {
			get {
				//Init();

				if( s_inited == false )
					throw new InvalidOperationException("HTTP管道还没有初始化。");

				return s_httpApplication;
			}
		}
	}
}
