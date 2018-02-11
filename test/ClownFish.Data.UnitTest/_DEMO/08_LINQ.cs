using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.UnitTest.Models;

namespace ClownFish.Data.UnitTest._DEMO
{
    class _8_LINQ
    {
        // LINQ 风格查询


        // 更多示例请参考：
        // https://github.com/fish-li/ClownFish.net/blob/master/test/ClownFish.Data.UnitTest/LinqTest.cs
        // https://github.com/fish-li/ClownFish.net/blob/master/test/ClownFish.Data.UnitTest/LinqSelectTest.cs
        // https://github.com/fish-li/ClownFish.net/blob/master/test/ClownFish.Data.UnitTest/LinqAsyncTest.cs


        private void DEMO()
        {
            int a = 5, b = 3;
            var query = from t in Entity.Query<Product>()
                        where t.ProductID == a && t.CategoryID < b
                        select t;

            Product p = query.FirstOrDefault();
        }



    }
}
