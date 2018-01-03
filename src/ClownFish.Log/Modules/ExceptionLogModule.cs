using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Log.Model;

namespace ClownFish.Log
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

            // 注意：这里只记录异常，不解决异常
            // 所以异常会继续抛出，会被其它地方处理。
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

                DbCommand dbCommand = null;
                Type t = ex.GetType();
                if( t.Namespace == "ClownFish.Data" && t.Name == "DbExceuteException" ) {
                    PropertyInfo p = t.GetProperty("Command");
                    dbCommand = (DbCommand)p.GetValue(ex);
                }

                ExceptionInfo exceptionInfo = ExceptionInfo.Create(ex, app.Context, dbCommand);
                ClownFish.Log.LogHelper.Write(exceptionInfo);
            }
        }
    }
}
