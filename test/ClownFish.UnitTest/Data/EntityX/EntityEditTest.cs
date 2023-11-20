using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.EntityX;

[TestClass]
public class EntityEditTest : BaseTest
{
    private static readonly string s_name = "temp_b55acaacb1234367a29d5b92ac7edc22";

    [TestMethod]
    public void Test_属实体CUD_属性赋值风格()
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                // 先删除之前测试可能遗留下来的数据
                db.Entity.From<Product>().Where(x => x.ProductName = s_name).Delete();

                // 插入一条记录
                Product p = db.Entity.CreateProxy<Product>();
                p.CategoryID = 1;
                p.ProductName = s_name;
                p.Quantity = 100;
                p.Unit = "x";
                p.UnitPrice = 112;
                p.Remark = "abcd";
                p.Insert();

                // 检验刚才插入的数据行
                Product p2 = db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingle();
                Assert.IsNotNull(p2);
                Assert.AreEqual(s_name, p2.ProductName);

                // 进入编辑状态，然后更新数据
                Product p3 = db.Entity.CreateProxy(p2);
                p3.Unit = "x2";
                p3.UnitPrice = 222;
                int effect = p3.Update();        // 提交更新，WHERE过滤条件由主键字段决定
                Assert.AreEqual(1, effect);

                // 检验刚才更新的数据行
                Product p4 = db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingle();
                Assert.IsNotNull(p4);
                Assert.AreEqual("x2", p4.Unit);
                Assert.AreEqual(222, p4.UnitPrice);

                // 删除数据行
                Product p5 = db.Entity.CreateProxy<Product>();
                p5.ProductName = s_name;
                effect = p5.Delete();
                Assert.AreEqual(1, effect);

                // 检验删除结果
                Product p6 = db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingle();
                Assert.IsNull(p6);
            }
        }
    }



    [TestMethod]
    public async Task Test_属实体CUD_属性赋值风格_Async()
    {
        foreach( var conn in BaseTest.ConnNames ) {
            using( DbContext db = DbContext.Create(conn) ) {

                ShowCurrentThread();

                // 先删除之前测试可能遗留下来的数据
                await db.Entity.From<Product>().Where(x => x.ProductName = s_name).DeleteAsync();
                ShowCurrentThread();

                // 插入一条记录，只给2个字段赋值
                Product p = db.Entity.CreateProxy<Product>();
                p.CategoryID = 1;
                p.ProductName = s_name;
                p.Quantity = 100;
                p.Unit = "x";
                p.UnitPrice = 112;
                p.Remark = "abcd";
                await p.InsertAsync();
                ShowCurrentThread();

                // 检验刚才插入的数据行
                Product p2 = await db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingleAsync();
                Assert.IsNotNull(p2);
                Assert.AreEqual(s_name, p2.ProductName);
                ShowCurrentThread();

                // 进入编辑状态，然后更新数据
                Product p3 = db.Entity.CreateProxy(p2);
                p3.Unit = "x2";
                p3.UnitPrice = 222;
                int effect = await p3.UpdateAsync();        // 提交更新，WHERE过滤条件由主键字段决定
                Assert.AreEqual(1, effect);
                ShowCurrentThread();

                // 检验刚才更新的数据行
                Product p4 = await db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingleAsync();
                Assert.IsNotNull(p4);
                Assert.AreEqual("x2", p4.Unit);
                Assert.AreEqual(222, p4.UnitPrice);
                ShowCurrentThread();

                // 删除数据行
                Product p5 = db.Entity.CreateProxy<Product>();
                p5.ProductName = s_name;
                effect = await p5.DeleteAsync();
                Assert.AreEqual(1, effect);
                ShowCurrentThread();

                // 检验删除结果
                Product p6 = await db.Entity.From<Product>().Where(x => x.ProductName = s_name).ToSingleAsync();
                Assert.IsNull(p6);
                ShowCurrentThread();
            }
        }
    }


    [TestMethod]
    public void Test_属实体CUD_指定表名()
    {
        using( DbContext db = DbContext.Create("sqlserver") ) {
            db.EnableDelimiter = true;

            Product p = db.Entity.CreateProxy<Product>();
            p.SetDbTableName("product_bak1");
            p.CategoryID = 1;
            p.ProductName = s_name;
            p.Quantity = 100;
            p.Unit = "x";
            p.UnitPrice = 112;
            p.Remark = "abcd";
            CPQuery query1 = p.GetInsertQueryCommand();
            Assert.IsTrue(query1.Command.CommandText.Contains("[product_bak1]"));


            Product p2 = db.Entity.Query<Product>().Where(x => x.Quantity > 1).ToSingle();
            Assert.IsNotNull(p2);


            Product p3 = db.Entity.CreateProxy(p2);
            p3.SetDbTableName("product_bak2");
            p3.Unit = "x2";
            p3.UnitPrice = 222;
            CPQuery query2 = p3.GetUpdateQueryCommand();
            Assert.IsTrue(query2.Command.CommandText.Contains("[product_bak2]"));


            Product p5 = db.Entity.CreateProxy<Product>();
            p5.SetDbTableName("product_bak3");
            p5.ProductName = s_name;
            CPQuery query3 = p5.GetDeleteQueryCommand();
            Assert.IsTrue(query3.Command.CommandText.Contains("[product_bak3]"));
        }
    }

}
