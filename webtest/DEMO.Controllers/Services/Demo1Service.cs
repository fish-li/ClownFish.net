using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using ClownFish.Web;
using System.Collections.Specialized;
using System.Web;
using ClownFish.Base;


// ClownFish.Web的用法可参考：http://www.cnblogs.com/fish-li/archive/2012/02/12/2348395.html
// ClownFish.Web下载页面：http://www.cnblogs.com/fish-li/archive/2012/02/21/2361982.html


namespace DEMO.Controllers.Services
{
	public class Demo1Service : BaseController
	{
		[Action]
		public string GetMd5(string input)
		{
			if( input == null )
				input = string.Empty;


			this.WriteCookie(new HttpCookie("cookie1", DateTime.Now.Ticks.ToString()));

			return HashHelper.Md5(input).ToLower();
		}

		[Action]
		public int Add(int a, int b)
		{
			return a + b;
		}

		[Action]
		public string TestNameValueCollection(NameValueCollection queryString, NameValueCollection form, 
			NameValueCollection headers, NameValueCollection serverVariables)
		{
			StringBuilder sb = new StringBuilder();

			foreach( string key in queryString.AllKeys )
				sb.AppendFormat("queryString, {0} = {1}\r\n", key, queryString[key]);
			foreach( string key in form.AllKeys )
				sb.AppendFormat("form, {0} = {1}\r\n", key, form[key]);
			foreach( string key in headers.AllKeys )
				sb.AppendFormat("headers, {0} = {1}\r\n", key, headers[key]);
			foreach( string key in serverVariables.AllKeys )
				sb.AppendFormat("serverVariables, {0} = {1}\r\n", key, serverVariables[key]);
			return sb.ToString();
		}


		[OutputCache(Duration=3, VaryByParam="none")]
		[Action]
		public string TestOutputCache()
		{
			return DateTime.Now.ToString();
		}


		[SessionMode(SessionMode.Support)]
		[Action]
		public int TestSessionMode(int a)
		{
			// 一个累加的方法，检验是否可以访问Session
			// 警告：示例代码的这样做法会影响Action的单元测试。

			if( System.Web.HttpContext.Current.Session == null )
				throw new InvalidOperationException("Session没有开启。");

			object obj = System.Web.HttpContext.Current.Session["counter"];
			int counter = (obj == null ? 0 : (int)obj);
			counter += a;
			System.Web.HttpContext.Current.Session["counter"] = counter;
			return counter;
		}

		[Action]
		public string TestGuid(Guid guid)
		{
			return guid.ToString("N");
		}


		[Action]
		public string TestEnum(DayOfWeek week)
		{
			return week.ToString();
		}

		public string Test1()
		{
			return "aaa";
		}

		[Action(Verb="post")]
		public string Test2()
		{
			return "bb";
		}

		[Action(Verb = "PUT")]
		public string TestGuid2(Guid a)
		{
			return a.ToString();
		}

		[Action]
		public string TestContextDataAttribute([ContextData("Request.UserAgent")]string useragent)
		{
			return useragent;
		}


        public BinaryResult TestBinaryResult(string s)
        {
            if( string.IsNullOrEmpty(s) )
                s = "123";

            byte[] buffer = Encoding.UTF8.GetBytes(s);
            return new BinaryResult(buffer);
        }

		
	}


	



}
