using System;
using System.Threading.Tasks;
using ClownFish.Data.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest
{
	[TestClass]
	public class EntityEditTest : BaseTestWithConnectionScope
	{
		[TestMethod]
		public void Test_属实体CUD_属性赋值风格()
		{
			// 先删除之前测试可能遗留下来的数据
			Entity.From<ModelX>().Where(m => m.IntField = 1978).Delete();

			// 插入一条记录，只给2个字段赋值
			ModelX obj = Entity.BeginEdit<ModelX>();
			obj.IntField = 1978;
			obj.StringField = "abc";
			obj.Insert();

			// 检验刚才插入的数据行
			ModelX m1 = Entity.From<ModelX>().Where(m => m.IntField = 1978).ToSingle();
			Assert.IsNotNull(m1);
			Assert.AreEqual("abc", m1.StringField);

			// m1 进入编辑状态
			m1 = Entity.BeginEdit(m1);
			m1.StringField = "12345";
			int effect = m1.Update();        // 提交更新，WHERE过滤条件由主键字段决定
			Assert.AreEqual(1, effect);

			// 检验刚才更新的数据行
			ModelX m2 = Entity.From<ModelX>().Where(m => m.IntField = 1978).ToSingle();
			Assert.IsNotNull(m2);
			Assert.AreEqual("12345", m2.StringField);

			// 删除数据行
			ModelX obj2 = Entity.BeginEdit<ModelX>();
			obj2.IntField = 1978;
			effect = obj2.Delete();
			Assert.AreEqual(1, effect);

			// 检验删除结果
			ModelX m3 = Entity.From<ModelX>().Where(m => m.IntField = 1978).ToSingle();
			Assert.IsNull(m3);
		}



		[TestMethod]
		public async Task Test_属实体CUD_属性赋值风格_Async()
		{
			ShowCurrentThread();

			// 注意：
			// 1、异步调用必须使用 DbContext，不能再使用 ConnectionScope 创建的作用域
			// 2、代码中也不能再使用快捷的静态方法。
			using( DbContext db = DbContext.Create() ) {

				// 先删除之前测试可能遗留下来的数据
				await db.Entity.From<ModelX>().Where(m => m.IntField = 1978).DeleteAsync();

				ShowCurrentThread();

				// 插入一条记录，只给2个字段赋值
				ModelX obj = db.Entity.BeginEdit<ModelX>();
				obj.IntField = 1978;
				obj.StringField = "abc";
				await obj.InsertAsync();
				ShowCurrentThread();

				// 检验刚才插入的数据行
				ModelX m1 = await db.Entity.From<ModelX>().Where(m => m.IntField = 1978).ToSingleAsync();
				Assert.IsNotNull(m1);
				Assert.AreEqual("abc", m1.StringField);
				ShowCurrentThread();

				// m1 进入编辑状态
				m1 = db.Entity.BeginEdit(m1);
				m1.StringField = "12345";
				int effect = await m1.UpdateAsync();        // 提交更新，WHERE过滤条件由主键字段决定
				Assert.AreEqual(1, effect);
				ShowCurrentThread();

				// 检验刚才更新的数据行
				ModelX m2 = await db.Entity.From<ModelX>().Where(m => m.IntField = 1978).ToSingleAsync();
				Assert.IsNotNull(m2);
				Assert.AreEqual("12345", m2.StringField);
				ShowCurrentThread();

				// 删除数据行
				ModelX obj2 = db.Entity.BeginEdit<ModelX>();
				obj2.IntField = 1978;
				effect = await obj2.DeleteAsync();
				Assert.AreEqual(1, effect);
				ShowCurrentThread();

				// 检验删除结果
				ModelX m3 = await db.Entity.From<ModelX>().Where(m => m.IntField = 1978).ToSingleAsync();
				Assert.IsNull(m3);
				ShowCurrentThread();
			}
		}


	}
}
