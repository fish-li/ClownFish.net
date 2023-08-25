using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.Pipleline.Test
{
    [AttributeUsage(AttributeTargets.All)]
    public class TestMvcAttribute : Attribute
    {
        public string X1 { get; set; }
    }
}
