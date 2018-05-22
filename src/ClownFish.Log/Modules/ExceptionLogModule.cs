using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base.Internals;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

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
            // 确保配置文件已读取
            WriterFactory.Init();


            // 检查配置文件，是否启用 <Type DataType="ClownFish.Log.Model.ExceptionInfo, ClownFish.Log"
            if( WriterFactory.Config.Types.FirstOrDefault(x => x.Type == typeof(ExceptionInfo)) == null ) {
                throw new System.Configuration.ConfigurationErrorsException(
                        "启用 ExceptionLogModule 时，必需在 ClownFish.Log.config 的<Types>节点中注册 ExceptionInfo 数据类型。");
            }


            app.Error += App_Error;

            // 注意：这里只记录异常，不解决异常
            // 所以异常会继续抛出，会被其它地方处理。
        }


        /// <summary>
        /// 决定某个异常是否需要写入异常日志，
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected virtual bool IsNeedLog(Exception ex)
        {
            // 扩展点：如果不希望记录某些类型的异常，可以重写这个方法。

            return true;
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

            if( ex == null )
                return;

            if( IsNeedLog(ex) == false )
                return;

            DbCommand dbCommand = null;

            IIncludeDbCommand dbExceuteException = ex as IIncludeDbCommand;
            if( dbExceuteException != null ) {
                dbCommand = dbExceuteException.Command;
            }

            ExceptionInfo exceptionInfo = ExceptionInfo.Create(ex, app.Context, dbCommand);
            LogHelper.Write(exceptionInfo);
        }
    }
}
