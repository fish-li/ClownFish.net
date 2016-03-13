using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Web.UnitTest.Attributes
{
	[TestClass]
	public class PageRegexUrlAttributeTest
	{
		[TestMethod]
		public void Test1() 
		{
			PageRegexUrlAttribute a = new PageRegexUrlAttribute {
				Url = @"/page/{id}/{year}-{month}-{day}.aspx"
			};

			Assert.IsTrue(a.GetRegex().IsMatch("/Page/23/2016-3-13.aspx"));
			Assert.IsTrue(a.GetRegex().IsMatch("/Page/abc/2016-3-13.aspx"));
			Assert.IsTrue(a.GetRegex().IsMatch("/Page/abc/16-3-13.aspx"));
			Assert.IsTrue(a.GetRegex().IsMatch("/Page/abc/cc-3-13.aspx"));

			// 没有 day 参数
			Assert.IsFalse(a.GetRegex().IsMatch("/Page/abc/cc-13.aspx"));
		}

		[TestMethod]
		public void Test2()
		{
			PageRegexUrlAttribute a = new PageRegexUrlAttribute {
				Url = @"/page/{id:int}/{day:date}.aspx"
			};

			Assert.IsTrue(a.GetRegex().IsMatch("/Page/23/2016-3-13.aspx"));

			// id 不是数字，所以不匹配
			Assert.IsFalse(a.GetRegex().IsMatch("/Page/abc/2016-3-13.aspx"));

			// 年份只有二位数字，所以不匹配
			Assert.IsFalse(a.GetRegex().IsMatch("/Page/23/16-3-13.aspx"));
		}

		[TestMethod]
		public void Test3()
		{
			PageRegexUrlAttribute a = new PageRegexUrlAttribute {
				Url = @"/page/{id:int}/{day:date}/{g:guid}.aspx"
			};

			Assert.IsTrue(a.GetRegex().IsMatch("/Page/23/2016-3-13/43e42101-79a3-4894-aa65-c086447badef.aspx"));

			// 日期格式不正确，所以不匹配
			Assert.IsFalse(a.GetRegex().IsMatch("/Page/23/2016-13/43e42101-79a3-4894-aa65-c086447badef.aspx"));

			// 月份有三位数字，所以不匹配
			Assert.IsFalse(a.GetRegex().IsMatch("/Page/23/2016-333-13/43e42101-79a3-4894-aa65-c086447badef.aspx"));

			// GUID格式不正确（最后一个字符：Z），所以不匹配
			Assert.IsFalse(a.GetRegex().IsMatch("/Page/23/2016-333-13/43e42101-79a3-4894-aa65-c086447badeZ.aspx"));

			// GUID格式不正确（少了一段），所以不匹配
			Assert.IsFalse(a.GetRegex().IsMatch("/Page/23/2016-333-13/43e42101-79a3-4894-c086447badez.aspx"));
		}
	}
}
