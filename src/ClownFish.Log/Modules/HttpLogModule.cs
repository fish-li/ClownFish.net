using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Base;
using ClownFish.Log.Model;
using ClownFish.Log.Serializer;

namespace ClownFish.Log
{
    /// <summary>
    /// 一个简单的记录HTTP请求数据的IHttpModule
    /// </summary>
    public class HttpLogModule : IHttpModule
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


            // 检查配置文件，是否启用 <Type DataType="ClownFish.Log.Model.HttpRequestData, ClownFish.Log"
            if( WriterFactory.Config.Types.FirstOrDefault(x => x.Type == typeof(HttpRequestData)) == null ) {
                throw new System.Configuration.ConfigurationErrorsException(
                        "启用 HttpLogModule 时，必需在 ClownFish.Log.config 的<Types>节点中注册 HttpRequestData 数据类型。");
            }

            app.BeginRequest += App_BeginRequest;
        }

        private void App_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            // 设置 RequestID
            app.Context.SetRequestId();

            // 记录HTTP请求的所有数据
            HttpRequestData data = new HttpRequestData();
            data.FillBaseInfo();
            data.HttpInfo = HttpInfo.Create(app.Context);

            ClownFish.Log.LogHelper.Write(data);
        }


    }
}
