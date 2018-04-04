using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ClownFish.Log.Model;

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
            app.BeginRequest += App_BeginRequest;
        }

        private void App_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            // 记录HTTP请求的所有数据
            HttpRequestData data = new HttpRequestData();
            data.FillBaseInfo();
            data.HttpInfo = HttpInfo.Create(app.Context);

            ClownFish.Log.LogHelper.Write(data);
        }


    }
}
