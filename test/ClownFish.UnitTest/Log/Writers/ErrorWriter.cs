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
    public class ErrorWriter : ILogWriter
    {
        public void Init(LogConfiguration config, WriterConfig section)
        {
        }

        public void Write<T>(List<T> list) where T : class, IMsgObject
        {
            throw new NotImplementedException();
        }
    }

    public class ErrorMessage : IMsgObject
    {
        public string Message { get; set; }

        public string GetId()
        {
            return this.Message.GetHashCode().ToString();
        }

        public DateTime GetTime()
        {
            return DateTime.MinValue;
        }
    }
}
