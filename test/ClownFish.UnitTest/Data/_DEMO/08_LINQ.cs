using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data._DEMO;

class DEMO_8_LINQ
{
    // LINQ 风格查询


    // 更多示例请参考：
    // https://github.com/fish-li/ClownFish.net/blob/master/test/ClownFish.UnitTest.Data/LinqTest.cs
    // https://github.com/fish-li/ClownFish.net/blob/master/test/ClownFish.UnitTest.Data/LinqSelectTest.cs
    // https://github.com/fish-li/ClownFish.net/blob/master/test/ClownFish.UnitTest.Data/LinqAsyncTest.cs


    private void DEMO()
    {
        using( DbContext db = DbContext.Create() ) {
            int a = 5, b = 3;
            var query = from t in db.Entity.Query<Product>()
                        where t.ProductID == a && t.CategoryID < b
                        select t;

            Product p = query.FirstOrDefault();
        }
    }



}
