using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace ClownFish.Web
{
	internal class HttpValueCollection : NameValueCollection
	{
		// 创建这类因为二个原因：
		// 1、确保集合是忽略大小写的。
		// 2、用于修改基类的 IsReadOnly 属性

		public HttpValueCollection()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}

		internal void MakeReadOnly()
		{
			base.IsReadOnly = true;
		}

		
		internal void MakeReadWrite()
		{
			base.IsReadOnly = false;
		}


	}
}
