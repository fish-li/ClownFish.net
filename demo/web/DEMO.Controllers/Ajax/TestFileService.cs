using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClownFish.Web;

namespace DEMO.Controllers.Ajax
{
	public class TestFileService
	{
		[Action]
		public string Test1(HttpFile file1, string abc)
		{
			return string.Format("Name: {0}; Length: {1}\r\nabc: {2}", 
						file1.FileName, file1.ContentLength, abc);
		}


		[Action]
		public JsonResult Test2(HttpFile[] files)
		{
			var list = (from f in files
			 select new { name = f.FileName, length = f.ContentLength }).ToList();

			return new JsonResult(list);
		}

		[Action]
		public int Sum(int[] numbers)
		{
			int count = 0;

			foreach( int n in numbers )
				count += n;

			return count;
		}


		[Action]
		public StreamResult Download1(string filename)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(Guid.Empty.ToString());
			return new StreamResult(buffer, null, filename);
		}



		[Action]
		public StreamResult Download2(string filename)
		{
			// 下载文件，使用URL中的文件名，避开响应头在不同浏览器的乱码问题。
			// 这个示例要结合URL Routing来实现（写Handler也可以）

			// 注意：
			// 如果要支持所有字符的文件名下载，需要修改web.config中的二个配置：
			//<requestFiltering allowDoubleEscaping="true" />
			//<httpRuntime requestPathInvalidCharacters=""/>

			byte[] buffer = Encoding.UTF8.GetBytes(Guid.Empty.ToString());
			return new StreamResult(buffer);
		}
	}
}
