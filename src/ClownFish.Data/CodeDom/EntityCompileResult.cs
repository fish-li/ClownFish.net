using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.CodeDom
{
	internal class EntityCompileResult
	{
		public Type EntityType { get; set; }

		public string ProxyName { get; set; }
		public Type ProxyType { get; set; }

		public string LoaderName { get; set; }
		public Type LoaderType { get; set; }

		public Object LoaderInstnace { get; set; }
	}
}
