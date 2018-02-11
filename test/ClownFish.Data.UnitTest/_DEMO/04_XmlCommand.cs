using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Data.UnitTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Data.UnitTest._DEMO
{
    class _4_XmlCommand
    {
        // XmlCommand 是什么？ 有什么作用？
        // =================================================

        // XmlCommand 是一种将SQL及参数保存到XML配置文件中的数据访问命令。
        // 目的是希望在代码中不出现SQL，避免 SQL 和 C# 混合在一起，
        // 而且将SQL保存在配置还有好处是可以在不改变代码的前提下优化SQL，甚至在运行时实际配置节点的替换（实际个性化需求）。


        // XmlCommand 以XML文件的形式存放，例如：
        // https://github.com/fish-li/ClownFish.Tucao/blob/master/src/web-async/ClownFish.Tucao.WebApplication/App_Data/XmlCommand/Tucao.config
        // https://github.com/fish-li/ClownFish.Tucao/blob/master/src/web-async/ClownFish.Tucao.WebApplication/App_Data/XmlCommand/TucaoList.config
        // 可以在 ClownFish.Data 初始化时一次性读入内存，然后用节点的名称来引用。



        // 使用方法
        // var parameters = new { IsResolved = isResolved };
        // return XmlCommand.Create("ItemListByResolve", parameters).ToPageList<CaoItem>(pageInfo);

        // 以上代码出自：https://github.com/fish-li/ClownFish.Tucao/blob/master/src/web-sync/ClownFish.Tucao.BLL/TucaoQueryBLL.cs



        // =================================================
        // 更多示例调用可参考：https://github.com/fish-li/ClownFish.net/blob/master/test/ClownFish.Data.UnitTest/XmlCommnadTest.cs
        // 演示涉及：
        // 1、同步，CRUD
        // 2、异步，CRUD
        // 3、查询获取列表 ToList
        // 4、WHERE 中的 IN 查询
        // 5、XmlCommand 中嵌套 CPQuery
        // 6、ConnectionScope




        

        /// <summary>
        /// XmlCommand 的简单CURD演示
        /// </summary>
        public void Test_XmlCommand_CRUD()
        {
            string newName = Guid.NewGuid().ToString();


            using( DbContext db = DbContext.Create() ) {

                // 形如事务，并设置事务隔离级别
                // 如果没有事务，可以不调用下面这行，和最后的 db.Commit();
                db.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                var newCustomer = new {
                    CustomerName = newName,
                    ContactName = Guid.NewGuid().ToString(),
                    Address = "111111 Address",
                    PostalCode = "111111",
                    Tel = "123456789"
                };

                // 执行INSERT操作
                db.XmlCommand.Create("InsertCustomer", newCustomer).ExecuteNonQuery();

                // ToSingle 方法用于获取单个对象
                var queryArgument = new { CustomerName = newName };
                Customer customer = db.XmlCommand.Create("GetCustomerByName", queryArgument).ToSingle<Customer>();

                // 验证插入与读取
                Assert.IsNotNull(customer);
                Assert.AreEqual(newCustomer.ContactName, customer.ContactName);





                // 准备更新数据
                Customer updateArgument = new Customer {
                    CustomerID = customer.CustomerID,
                    CustomerName = newCustomer.CustomerName,
                    ContactName = newCustomer.ContactName,
                    Address = Guid.NewGuid().ToString(),
                    PostalCode = newCustomer.PostalCode,
                    Tel = newCustomer.Tel
                };

                // 执行UPDATE操作
                db.XmlCommand.Create("UpdateCustomer", updateArgument).ExecuteNonQuery();

                // 读取刚更新的记录
                var queryArgument2 = new { CustomerID = customer.CustomerID };
                Customer customer2 = db.XmlCommand.Create("GetCustomerById", queryArgument2).ToSingle<Customer>();

                // 验证更新与读取
                Assert.IsNotNull(customer2);
                Assert.AreEqual(updateArgument.Address, customer2.Address);


                // 执行DELETE操作
                var deleteArgument = new { CustomerID = customer.CustomerID };
                db.XmlCommand.Create("DeleteCustomer", deleteArgument).ExecuteNonQuery();

                // 验证删除			
                Customer customer3 = db.XmlCommand.Create("GetCustomerById", queryArgument2).ToSingle<Customer>();
                Assert.IsNull(customer3);

                // 提交事务
                db.Commit();
            }
        }



        


    }
}
