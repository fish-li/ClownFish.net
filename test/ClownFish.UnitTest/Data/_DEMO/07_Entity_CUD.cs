using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data._DEMO;

class DEMO_7_Entity_CUD
{
    // CUD, 属性赋值风格


    public void Test_属实体CUD_属性赋值风格()
    {
        using( DbContext db = DbContext.Create() ) {

            // 先删除之前测试可能遗留下来的数据
            ModelX xx = db.Entity.BeginEdit<ModelX>();
            xx.IntField = 1978;
            xx.Delete();


            // 插入一条记录，只给2个字段赋值
            ModelX obj = db.Entity.BeginEdit<ModelX>();
            obj.IntField = 1978;
            obj.StringField = "abc";
            obj.Insert();

            // 检验刚才插入的数据行
            ModelX m1 = (from x in db.Entity.Query<ModelX>() where x.IntField == 1978 select x).FirstOrDefault();
            Assert.IsNotNull(m1);
            Assert.AreEqual("abc", m1.StringField);

            // m1 进入编辑状态
            m1 = db.Entity.BeginEdit(m1);
            m1.StringField = "12345";
            int effect = m1.Update();        // 提交更新，WHERE过滤条件由主键字段决定
            Assert.AreEqual(1, effect);

            // 检验刚才更新的数据行
            ModelX m2 = (from x in db.Entity.Query<ModelX>() where x.IntField == 1978 select x).FirstOrDefault();
            Assert.IsNotNull(m2);
            Assert.AreEqual("12345", m2.StringField);

            // 删除数据行
            ModelX obj2 = db.Entity.BeginEdit<ModelX>();
            obj2.IntField = 1978;
            effect = obj2.Delete();
            Assert.AreEqual(1, effect);

            // 检验删除结果
            ModelX m3 = (from x in db.Entity.Query<ModelX>() where x.IntField == 1978 select x).FirstOrDefault();
            Assert.IsNull(m3);
        }
    }

}
