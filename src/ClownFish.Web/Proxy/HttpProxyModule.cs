using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base;


namespace ClownFish.Web.Proxy
{
    /// <summary>
    /// 一个可配置转发规则的反向代理模块
    /// </summary>
    public sealed class HttpProxyModule : IHttpModule
    {
        #region init and config

        private static readonly object s_lock = new object();
        private static bool s_inited = false;

        private static NameValue[] s_rules;


        private static void ModuleInit()
        {
            if( s_inited == false ) {
                lock( s_lock ) {
                    if( s_inited == false ) {
                        LoadConfig();
                        s_inited = true;
                    }
                }
            }
        }


        private static void LoadConfig()
        {
            List<NameValue> list = new List<NameValue>();

            foreach(string name in ConfigurationManager.AppSettings.AllKeys ) {

                if( name.StartsWithIgnoreCase("proxy-rule") ) {

                    string value = ConfigurationManager.AppSettings[name];
                    if( string.IsNullOrEmpty(value) == false ) {

                        // value format: /v20/api/xxx-service/ = http://www.abc.com/
                        string[] items = value.SplitTrim('=');
                        if( items.Length == 2 ) {
                            list.Add(new NameValue(items[0], items[1]));
                        }
                    }
                }
            }

            if( list.Count > 0 )
                s_rules = list.ToArray();
        }

        #endregion


        /// <summary>
        /// 实现 IHttpModule.Init 方法
        /// </summary>
        /// <param name="app"></param>
        public void Init(HttpApplication app)
        {
            ModuleInit();

            app.BeginRequest += App_BeginRequest;
        }

   
        private void App_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
                        
            // 尝试从配置规则中获取目标站点地址            
            string destRoot = GetProxySite(app);

            // 如果没有匹配项，则表示从当前站点中处理
            if( string.IsNullOrEmpty(destRoot) )
                return;
            

            // 计算用转发的实际网址
            string destUrl = destRoot + app.Request.RawUrl;
            string srcUrl = app.Request.Url.AbsoluteUri;


            HttpProxyHandler hander = new HttpProxyHandler(srcUrl, destUrl);
            app.Context.RemapHandler(hander);
            app.Context.Response.Headers.Add("X-HttpProxyModule", destUrl);	// 用于调试诊断

            // 既然转发了请求，就不用当前站点的身份认证了
            app.Context.SkipAuthorization = true;
        }


        private string GetProxySite(HttpApplication app)
        {
            if( s_rules == null )
                return null;

            string path = app.Request.Path;

            for(int i = 0; i< s_rules.Length;i++ ) {
                NameValue nv = s_rules[i];

                // 转发所有请求
                if( nv.Name == "**" )  // 2 个星号
                    return nv.Value;

                // 只要包含指定的部分就转发
                // 说明：这里不用【正则表达式】，因为几乎遇到的所有场景中都不会有复杂到必须使用正则表达式才能搞定的
                //     反而应该在设计URL时保留一些特定的名称就可以识别不同的应用
                if( path.IndexOfIgnoreCase(nv.Name) >= 0 )
                    return nv.Value;
            }

            return null;
        }

        /// <summary>
        /// 实现 IHttpModule.Dispose 方法
        /// </summary>
        public void Dispose()
        {
        }

        
    }
}
