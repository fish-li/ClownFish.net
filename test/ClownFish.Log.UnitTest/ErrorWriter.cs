using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Configuration;
using ClownFish.Log.Serializer;

namespace ClownFish.Log.UnitTest
{
	public class ErrorWriter : ILogWriter
	{
		#region ILogWriter 成员

		public void Init(WriterSection config)
		{
		}

		public void Write<T>(T info) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		public void Write<T>(List<T> list) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		public T Get<T>(Guid guid) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		public List<T> GetList<T>(DateTime t1, DateTime t2) where T : Model.BaseInfo
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
