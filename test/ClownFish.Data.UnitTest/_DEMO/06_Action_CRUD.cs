using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest._DEMO
{
    class _6_Action_CRUD
    {
        // CRUD，匿名委托风格

        /*
            Entity.From<T>()
                .Set/Where(Action<T> action)
                .Select(Action<T> action)
                .Insert/Delete/Update/ToList/ToSingle()

            Entity.From<Model1>().Set(m => {
                m.IntValue = 2;
                m.StrValue = "abc";
            }).Insert();
            Entity.From<Model1>().Where(m => {
                m.IntValue = 2;
                m.StrValue = "abc";
            }).Delete();
            Entity.From<Model1>().Set(m => {
                m.StrValue = "abc";
            })
            .Where(m => {
                m.IntValue = 2;
            }).Update();
         */



        public void Test_实体CUD_匿名委托风格()
        {
            // 先删除之前测试可能遗留下来的数据
            Entity.From<ModelX>().Where(m => m.IntField = 1978).Delete();

            // 插入一条记录，只给2个字段赋值
            Entity.From<ModelX>()
                .Set(m => { m.IntField = 1978; m.StringField = "abc"; })
                .Insert();

            // 检验刚才插入的数据行
            ModelX m1 = Entity.From<ModelX>().Where(m => m.IntField = 1978).ToSingle();
            Assert.IsNotNull(m1);
            Assert.AreEqual("abc", m1.StringField);

            // 更新数据行，WHERE条件是IntField，要更新的字符是StringField
            Entity.From<ModelX>()
                .Set(m => m.StringField = "12345")
                .Where(m => m.IntField = 1978)
                .Update();

            // 检验刚才更新的数据行，本次查询只加载一个字段StringField
            ModelX m2 = Entity.From<ModelX>()
                .Where(m => m.IntField = 1978)
                .Select(m => m.StringField = null)  // 只加载StringField字段
                .ToSingle();
            Assert.IsNotNull(m2);
            Assert.AreEqual("12345", m2.StringField);
            Assert.AreEqual(0, m2.IntField);    // 没有加载这个字段


            // 再次插入一条记录，只给2个字段赋值
            Entity.From<ModelX>()
                .Set(m => { m.IntField = 1978; m.StringField = "www"; })
                .Insert();

            // 查询列表，应该包含本次测试插入的二条记录
            List<ModelX> list = Entity.From<ModelX>()
                .Where(m => m.IntField = 1978)
                // 下行代码指出结果需要包含哪些字段，只需要在语句中列出相应的属性即可，赋了什么值并不重要
                .Select(m => { m.IntField = 0; m.StringField = null; })
                .ToList();

            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("12345", list[0].StringField);
            Assert.AreEqual("www", list[1].StringField);

            // 根据“IntField = 1978”来删除，应该删除2条记录
            int rows = Entity.From<ModelX>().Where(m => m.IntField = 1978).Delete();
            Assert.AreEqual(2, rows);

            // 再查询一次，应该是没有记录了
            ModelX m3 = Entity.From<ModelX>().Where(m => m.IntField = 1978).ToSingle();
            Assert.IsNull(m3);

        }

    }
}
