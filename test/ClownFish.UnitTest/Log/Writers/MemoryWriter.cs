using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Configuration;
using ClownFish.Log.Models;
using ClownFish.Log.Writers;

namespace ClownFish.UnitTest.Log.Writers
{
    public sealed class MemoryWriter : ILogWriter
    {
        private readonly List<object> _list = new List<object>(128);

        public List<object> PullALL()
        {
            List<object> newList = (from x in _list select x).ToList();
            _list.Clear();
            return newList;
        }

        public void Init(LogConfiguration config, WriterConfig section)
        {

        }


        public void Write<T>(List<T> list) where T : class, IMsgObject
        {
            _list.AddRange(list);
        }
    }
}
