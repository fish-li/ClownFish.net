using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DEMO.Model.PageData;
using ClownFish.Web;
using ClownFish.Web.Client;
using ClownFish.Base;

namespace DEMO.Controllers.TestTask
{
	public class TaskDemoController
	{
		[Action]
		[PageUrl(Url = "/Pages/Demo/TestTaskPage.aspx")]
		public async Task<PageResult> GetWebsiteTitles(HttpContext context, string urls)
		{
			context.WriteHeader("action-before-await");

			context.WriteHeader("---------------------------------------------------------");


			TestTaskPageViewData viewData = new TestTaskPageViewData();
			viewData.Input = urls;

			if( string.IsNullOrEmpty(urls) ) {
				viewData.Input = @"
http://cn.bing.com/
http://email.163.com/
http://www.aliyun.com/
http://baike.baidu.com/
http://www.dingtalk.com/
http://www.taobao.com/
http://www.tmall.com/";
			}
			else {
				string[] urlArray = urls.SplitTrim('\r', '\n');
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>(urlArray.Length);

				foreach( string url in urlArray ) {
					try {
						HttpOption option = new HttpOption {
							Url = url,
							Timeout = 2000
						};
						string text = await option.SendAsync<string>();
						context.WriteHeader("HttpClient.SendAsync<string>(url)--after-await");

						string title = GetHtmlTitle(text) ?? "-----------------";

						list.Add(new KeyValuePair<string, string>(url, title));
					}
					catch( Exception ex ) {
						list.Add(new KeyValuePair<string, string>(url, ex.GetBaseException().Message));
					}

					viewData.Result = list;
				}
			}

			context.WriteHeader("action-after-await-return");

			return new PageResult(null, viewData);
		}


		public static string GetHtmlTitle(string text)
		{
			if( string.IsNullOrEmpty(text) )
				return null;

			int p1 = text.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
			int p2 = text.IndexOf("</title>", StringComparison.OrdinalIgnoreCase);

			if( p2 > p1 && p1 > 0 ) {
				p1 += "<title>".Length;
				return  text.Substring(p1, p2 - p1);
			}

			return null;
		}
	}
}
