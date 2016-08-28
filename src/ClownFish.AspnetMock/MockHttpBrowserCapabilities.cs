using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections;

namespace ClownFish.AspnetMock
{
    /// <summary>
	/// 模拟HttpBrowserCapabilities，
	/// 为解决Request.Browser["key"]
    /// </summary>
    public sealed class MockHttpBrowserCapabilities : HttpBrowserCapabilities
    {
        internal MockHttpBrowserCapabilities()
        {
			Dictionary<string, string> dict = new Dictionary<string, string>();
			//dict.Add("requiresPostRedirectionHandling", "false");
            this.Items = dict;
        }

		public Dictionary<string,string> Items { get; private set; }

        public override string this[string key]
        {
            get
            {
                return this.Items[key];
            }

        }

		new public string Browser
		{
			set { this.Set("browser", value); }
			get { return this.Items["browser"]; }
		}

		//new public string Version
		//{
		//	set { this.Set("version", value); }
		//}

		new public int MajorVersion
		{
			set { this.Set("majorversion", value.ToString()); }
			get
			{
				int value = 0;
				int.TryParse(this.Items["browser"], out value);
				return value;
			}
		}


		public void Set(string key, string value)
		{
			this.Items[key] = value;
		}
    }
}
