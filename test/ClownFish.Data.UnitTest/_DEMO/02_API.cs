using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.UnitTest._DEMO
{
    class _2_DEMO
    {
        private void 一般用法介绍()
        {
            string connectionName = "default";
            IsolationLevel isolation = IsolationLevel.ReadCommitted;
            string name = "command1";
            string sql = "select * from table1 where .....";
            object args = new { a = 1, b = 2 };




            // 基本数据库操作
            using( DbContext db = DbContext.Create(connectionName) ) {
                db.BeginTransaction(isolation);   // 开启事务，并允许设置事务隔离级别（可选）
                db.XmlCommand.Create(name, args).ExecuteNonQuery();
                db.CPQuery.Create(sql, args).ExecuteNonQuery();
                db.StoreProcedure.Create(name, args).ExecuteNonQuery();
                db.Commit();
            }




            // 封装 DbContext ，允许跨方法调用
            using( ConnectionScope scope = ConnectionScope.Create(connectionName) ) {
                scope.BeginTransaction(isolation);   // 开启事务，并允许设置事务隔离级别（可选）

                // 下面三个调用，调用了三个静态工厂方法，
                // 因此可以放在不同的方法中，不必传递 scope 变量

                XmlCommand.Create(name, args).ExecuteNonQuery();
                CPQuery.Create(sql, args).ExecuteNonQuery();
                StoreProcedure.Create(name, args).ExecuteNonQuery();

                scope.Commit();
            }




            // DbContext/ConnectionScope 其它创建方法
            DbContext c1 = DbContext.Create("connectionString", "providerName");
            ConnectionScope c2 = ConnectionScope.Create("connectionString", "providerName");

            // 甚至可以由隐式转换来实现：
            DbContext c3 = "connectionString";





            // 嵌套使用（一段代码访问不同数据源）
            using( ConnectionScope scope = ConnectionScope.Create("connectionName_1") ) {
                XmlCommand.Create(name, args).ExecuteNonQuery();

                using( DbContext db = DbContext.Create("connectionName_2") ) {
                    db.XmlCommand.Create(name, args).ExecuteNonQuery();
                }
            }


            /*
             Execute 包含的操作

            abstract class BaseCommand {
                ExecuteNonQuery()
                ExecuteScalar<T>()
                ToScalarList<T>()
                ToSingle<T>()
                ToList<T>()
                ToDataTable()
                ToDataSet()

                ExecuteNonQueryAsync()
                ExecuteScalarAsync<T>()
                ToScalarListAsync<T>()
                ToSingleAsync<T>()
                ToListAsync<T>()
                ToDataTableAsync()
                ToDataSetAsync()
            }
            */




            /*
            // 从DataTable中加载数据
            class TableExtensions{
                ToList<T>(DataTable);
                ToSingle<T>(DataRow);
            }
             */


            // 嵌套使用，允许：XmlCommnad 包含 CPQuery， CPQuery 包含 CPQuery 
            using( DbContext db = DbContext.Create(connectionName) ) {
                CPQuery subQuery1 = db.CPQuery.Create(name, args);
                CPQuery subQuery2 = db.CPQuery.Create(name, args);

                db.XmlCommand.Create(
                    "select * from t1 where id=@id and {filter1} and {filter2}",
                    new {
                        id = 2,
                        filter1 = subQuery1,
                        filter2 = subQuery2
                    }
                ).ExecuteNonQuery();


                db.XmlCommand.Create(
                    "select * from t1 where id=@id and {filter1} and {filter2}",
                    new {
                        id = 2,
                        filter1 = CPQuery.Create(name, args),
                        filter2 = CPQuery.Create(name, args)
                    }
                ).ExecuteNonQuery();
            }




        }
    }
}
