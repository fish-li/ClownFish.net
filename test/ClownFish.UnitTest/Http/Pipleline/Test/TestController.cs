using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Http.Pipleline.Test
{
    [TestMvc(X1 = "root")]
    public class TestController
    {
        [TestMvc(X1 = "xx1")]
        public void Action1()
        {

        }

        public void Action2()
        {

        }
    }


    public class TestController2
    {
        public void Action1()
        {

        }
    }



}
