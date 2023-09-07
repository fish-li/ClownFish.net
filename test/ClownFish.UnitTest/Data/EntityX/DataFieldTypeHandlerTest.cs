using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.EntityX;
[TestClass]
public class DataFieldTypeHandlerTest
{
    [TestMethod]
    public void Test1()
    {
        ComplexEntity x = new ComplexEntity {
            Location = new System.Drawing.Point(1, 2),
            SecureString = "中文汉字"
        };

        using( DbContext db = DbContext.Create("mysql") ) {
            int newId = (int)db.Entity.Insert(x, InsertOption.GetNewId);

            DataTable table = db.CPQuery.Create($"select * from complex_entity where id = {newId}").ToDataTable();
            Console.WriteLine(table.ToJson(JsonStyle.Indented));

            Assert.AreEqual(1, table.Rows.Count);
            Assert.AreEqual("1,2", table.Rows[0]["location"].ToString());

            // base64("中文汉字") => 5Lit5paH5rGJ5a2X
            Assert.AreEqual("5Lit5paH5rGJ5a2X", table.Rows[0]["securestring"].ToString());

            ComplexEntity x2 = (from e in db.Entity.Query<ComplexEntity>()
                                where e.Id == newId
                                select e
                                ).FirstOrDefault();

            Assert.IsNotNull(x2);
            Console.WriteLine(x2.ToJson(JsonStyle.Indented));

            Assert.AreEqual(1, x2.Location.X);
            Assert.AreEqual(2, x2.Location.Y);
            Assert.AreEqual("中文汉字", (string)x2.SecureString);


            ComplexEntity x3 = table.ToList<ComplexEntity>()[0];
            Assert.AreEqual(1, x3.Location.X);
            Assert.AreEqual(2, x3.Location.Y);
            Assert.AreEqual("中文汉字", (string)x3.SecureString);

            db.Entity.Delete<ComplexEntity>(x => x.Id == newId);
        }
    }

    [TestMethod]
    public void Test2()
    {
        ComplexEntity x = new ComplexEntity {
            Location = new System.Drawing.Point(1, 2),
            SecureString = "中文汉字"
        };

        using( DbContext db = DbContext.Create("mysql") ) {
            int newId = (int)db.Entity.Insert(x, InsertOption.GetNewId);

            string querySql = $"select * from complex_entity where id = {newId}";
            DataTable table = db.CPQuery.Create(querySql).ToDataTable();

            DefaultDataFieldTypeHandler typeHandler = new DefaultDataFieldTypeHandler();
            Assert.AreEqual(newId, (int)typeHandler.GetValue(table.Rows[0], 0, null, null));

            using(DbDataReader reader = db.CPQuery.Create(querySql).ExecuteReader()) {
                reader.Read();
                Assert.AreEqual(newId, (int)typeHandler.GetValue(reader, 0, null, null));
            }

            db.Entity.Delete<ComplexEntity>(x => x.Id == newId);
        }
    }


    [TestMethod]
    public void Test3()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            DataFieldTypeHandlerFactory.Add(null, new PointDataFieldTypeHandler());
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            DataFieldTypeHandlerFactory.Add(typeof(System.Drawing.Point), null);
        });
    }


    
}
