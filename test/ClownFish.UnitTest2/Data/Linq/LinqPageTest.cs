using System.Linq;
using System.Threading.Tasks;
using ClownFish.Data;
using ClownFish.UnitTest.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Linq;

[TestClass]
public class LinqPageTest : BaseTest
{



    [TestMethod]
    public async Task Test_MySql_Async()
    {
        using( DbContext db = DbContext.Create("mysql") ) {
            var query = from t in db.Entity.Query<Product>()
                        orderby t.ProductID ascending
                        select new Product {ProductID = 0, ProductName = "" };

            query = query.Where(x => x.ProductID > 3).Skip(222).Take(666);

            var list = await query.ToListAsync();
        }
        AssertLastExecuteSQL(@"
SELECT ProductID,ProductName
FROM Products
WHERE (ProductID > @p1)
ORDER BY ProductID
LIMIT 666 OFFSET 222
@p1: (Int32), 3
");
    }






}
