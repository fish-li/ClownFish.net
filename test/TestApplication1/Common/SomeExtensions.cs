using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ClownFish.Base.WebClient;

namespace ClownFish.TestApplication1.Common
{
	public static class SomeExtensions
	{
		public static int GetStatusCode(this WebException ex)
		{
			return (int)(ex.Response as HttpWebResponse).StatusCode;
		}

		public static int GetStatusCode(this RemoteWebException ex)
		{
			return (ex.InnerException as WebException).GetStatusCode();
		}


		public async static Task<int> GetStatusCode(this HttpOption option)
		{
			try {
				await option.SendAsync<string>();
				return 200;
			}
			catch( RemoteWebException remoteWebException ) {
				return remoteWebException.GetStatusCode();
			}
			catch( WebException ex ) {
				return ex.GetStatusCode();
			}
			catch( Exception ) {
				return 500;
			}
		}
	}
}
