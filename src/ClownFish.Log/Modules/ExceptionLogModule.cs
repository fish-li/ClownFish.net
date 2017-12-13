using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Log.Model;

namespace ClownFish.Log.Modules
{
    /// <summary>
    /// 一个简单的仅仅记录异常日志的HttpModule
    /// </summary>
    public class ExceptionLogModule : IHttpModule
    {
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="app"></param>
        public void Init(HttpApplication app)
        {
            app.Error += App_Error;
        }

        /// <summary>
        /// Error事件处理方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_Error(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            Exception ex = app.Server.GetLastError();
            if( ex != null ) {
                ExceptionInfo exceptionInfo = ExceptionInfo.Create(ex);
                ClownFish.Log.LogHelper.Write(exceptionInfo);
            }
        }
    }
}
