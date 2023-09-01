using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.WebHost.Utils;
using ClownFish.Http.Pipleline;
using ClownFish.Base.Exceptions;

namespace ClownFish.WebHost.Config
{
	internal class ServerOptionValidator
	{
		public void Validate(ServerOption option)
		{
			if( option == null )
				throw new ArgumentNullException(nameof(option));


			// 检查监听参数
			if( option.HttpListenerOptions == null || option.HttpListenerOptions.Length == 0 )
				throw new ConfigurationErrorsException("没有配置监听参数。");

			foreach(var a in option.HttpListenerOptions ) {
				if( string.IsNullOrEmpty(a.Protocol))
					throw new ConfigurationErrorsException("配置项 httpListener/protocol 不能为空。");


				if( a.Port < 0)
					throw new ConfigurationErrorsException("配置项 httpListener/port 的取值无效。");
			}



			// 检查 HttpModule
			if( option.Modules != null ) {
				foreach(var b in option.Modules ) {
					Type t = Type.GetType(b, false, false);
					if( t == null )
						throw new ConfigurationErrorsException($"配置项 modules/type/{b} 的取值无效，找不到指定的类型。");

					if( t.IsSubclassOf(typeof(NHttpModule)) == false )
						throw new ArgumentException($"配置项 modules/type/{b} 的取值无效，类型必须继承于 NHttpModule");
				}
			}



			// 检查站点设置
			if( option.Website != null ) {
				if( string.IsNullOrEmpty(option.Website.LocalPath))
					throw new ConfigurationErrorsException("配置项 website/localPath 不能为空。");

				if( System.IO.Directory.Exists(option.Website.LocalPath) == false)
					throw new ConfigurationErrorsException("配置项 website/localPath 的取值无效，找不到指定的目录。");

				if( option.Website.StaticFiles != null ) {
					foreach(var f in option.Website.StaticFiles ) {
						if( string.IsNullOrEmpty(f.Ext))
							throw new ConfigurationErrorsException("配置项 website/staticFile/ext 不能为空。");
					}
				}

			}
		}


		public void SetDefaultValues(ServerOption option)
		{
			if( option == null )
				throw new ArgumentNullException(nameof(option));

			if(option.HttpListenerOptions.IsNullOrEmpty() == false ) {
				foreach( var a in option.HttpListenerOptions ) {
					if( string.IsNullOrEmpty(a.Ip) )
						a.Ip = "*";

					// 端口号为 0 表示使用动态端口
					if( a.Port == 0 )
						// 这里不检查返回值，因为不太可能把端口全部用完
						a.Port = NetHelper.GetFreeTcpPort(11000, 50000);
				}
			}			


			if( option.Website != null ) {
                option.Website.LocalPath = Path.GetFullPath(option.Website.LocalPath);

                if( option.Website.CacheDuration <= 0 )
                    option.Website.CacheDuration = 3600 * 24 * 365;      // 默认缓存1年


                if( option.Website.StaticFiles != null ) {
                    foreach( var f in option.Website.StaticFiles ) {
                        if( f.Cache == 0 )
                            f.Cache = option.Website.CacheDuration;      // 取默认缓存值

						if( string.IsNullOrEmpty(f.Mine) )
							f.Mine = ResponseContentType.OctetStream;
					}
                }
            }

		}



	}
}
