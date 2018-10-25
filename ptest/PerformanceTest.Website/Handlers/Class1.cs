using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web;

namespace PerformanceTest.Website.Handlers
{
    public sealed class Class1Controller : BaseController
    {
        [PageUrl(Url = "/hello/add.aspx")]
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
