#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Data;
using ClownFish.Data.Sync;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Data.Sync;

[TestClass]
public class DataSyncTest
{
    [TestMethod]
    public void Test_MySql_数据发布_主外键映射()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {
            dbContext.EnableDelimiter = true;

            // 创建测试表及数据
            CreateTable_Sync1(dbContext);

            DataSyncArgs args1 = new DataSyncArgs {
                BizName = "DataSyncTest--Test_MySql_1",
                SrcDbContext = dbContext,
                DestDbContext = dbContext,
                SrcTableName = "products",
                DestTableName = "products_sync1",
                SrcKeyField = "ProductID",
                DestRelatedKey = "PID",
                SrcFilterSql = "CategoryID = -1",
            };

            DataSyncResult result1 = DataSync.Execute(args1);
            Assert.AreEqual(0, result1.InsertCount);
            Assert.AreEqual(0, result1.UpdateCount);




            DataSyncArgs args2 = new DataSyncArgs {
                BizName = "DataSyncTest--Test_MySql_1",
                SrcDbContext = dbContext,
                DestDbContext = dbContext,
                SrcTableName = "products",
                DestTableName = "products_sync1",
                SrcKeyField = "ProductID",
                DestRelatedKey = "PID",
                SrcFilterSql = "CategoryID = 1",
            };

            DataSyncResult result2 = DataSync.Execute(args2);

            Assert.AreEqual(68, result2.InsertCount);
            Assert.AreEqual(10, result2.UpdateCount);
        }
    }


    [TestMethod]
    public async Task Test_MySql_数据发布_主外键映射_Async()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {
            dbContext.EnableDelimiter = true;

            // 创建测试表及数据
            CreateTable_Sync1(dbContext);

            DataSyncArgs args1 = new DataSyncArgs {
                BizName = "DataSyncTest--Test_MySql_1",
                SrcDbContext = dbContext,
                DestDbContext = dbContext,
                SrcTableName = "products",
                DestTableName = "products_sync1",
                SrcKeyField = "ProductID",
                DestRelatedKey = "PID",
                SrcFilterSql = "CategoryID = -1",
            };

            DataSyncResult result1 = await DataSync.ExecuteAsync(args1);
            Assert.AreEqual(0, result1.InsertCount);
            Assert.AreEqual(0, result1.UpdateCount);




            DataSyncArgs args2 = new DataSyncArgs {
                BizName = "DataSyncTest--Test_MySql_1",
                SrcDbContext = dbContext,
                DestDbContext = dbContext,
                SrcTableName = "products",
                DestTableName = "products_sync1",
                SrcKeyField = "ProductID",
                DestRelatedKey = "PID",
                SrcFilterSql = "CategoryID = 1",
            };

            DataSyncResult result2 = await DataSync.ExecuteAsync(args2);

            Assert.AreEqual(68, result2.InsertCount);
            Assert.AreEqual(10, result2.UpdateCount);
        }
    }


    [TestMethod]
    public void Test_MySql_数据发布_主键同步映射_启用事务()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {
            dbContext.EnableDelimiter = true;

            // 创建测试表及数据
            CreateTable_Sync2(dbContext);

            DataSyncArgs args = new DataSyncArgs {
                BizName = "DataSyncTest--Test_MySql_2",
                SrcDbContext = dbContext,
                DestDbContext = dbContext,
                SrcTableName = "products",
                DestTableName = "products_sync2",
                SrcKeyField = "ProductID",
                DestRelatedKey = "ProductID",
                SrcFilterSql = "CategoryID = 1",
                WithTranscation = true
            };

            DataSyncResult result = DataSync.Execute(args);

            Assert.AreEqual(68, result.InsertCount);
            Assert.AreEqual(10, result.UpdateCount);
        }
    }


    [TestMethod]
    public async Task Test_MySql_数据发布_主键同步映射_启用事务_Async()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {
            dbContext.EnableDelimiter = true;

            // 创建测试表及数据
            CreateTable_Sync2(dbContext);

            DataSyncArgs args = new DataSyncArgs {
                BizName = "DataSyncTest--Test_MySql_2",
                SrcDbContext = dbContext,
                DestDbContext = dbContext,
                SrcTableName = "products",
                DestTableName = "products_sync2",
                SrcKeyField = "ProductID",
                DestRelatedKey = "ProductID",
                SrcFilterSql = "CategoryID = 1",
                WithTranscation = true
            };

            DataSyncResult result = await DataSync.ExecuteAsync(args);

            Assert.AreEqual(68, result.InsertCount);
            Assert.AreEqual(10, result.UpdateCount);
        }
    }


    private void CreateTable_Sync1(DbContext dbContext)
    {
        string sql1 = @"
DROP TABLE IF EXISTS `products_sync1`;
CREATE TABLE `products_sync1`  (
  `RId` int NOT NULL AUTO_INCREMENT,
  `PID` int NOT NULL DEFAULT 0,
  `ProductName` varchar(50)  NOT NULL,
  `CategoryID` int NOT NULL,
  `Unit` varchar(10)  NOT NULL DEFAULT '个',
  `UnitPrice` decimal(20, 4) NOT NULL DEFAULT 0.0000,
  `Remark` longtext  NOT NULL,
  `Quantity` int NOT NULL DEFAULT 0,
  `Text1` varchar(255)  NULL DEFAULT NULL,
  `Text2` varchar(255)  NULL DEFAULT NULL,
  PRIMARY KEY (`RId`) USING BTREE
) ENGINE = InnoDB;
";

        string sql2 = @"
SET FOREIGN_KEY_CHECKS = 0;
INSERT INTO `products_sync1` VALUES (1, 7, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (2, 6, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (3, 3, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (4, 5, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (5, 2, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (6, 9, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (7, 1, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (8, 8, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (9, 10, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync1` VALUES (10, 4, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
SET FOREIGN_KEY_CHECKS = 1;
";

        dbContext.CPQuery.Create(sql1).ExecuteNonQuery();
        dbContext.CPQuery.Create(sql2).ExecuteNonQuery();
    }



    private void CreateTable_Sync2(DbContext dbContext)
    {
        string sql1 = @"
DROP TABLE IF EXISTS `products_sync2`;
CREATE TABLE `products_sync2`  (
  `ProductID` int NOT NULL,
  `ProductName` varchar(50)  NOT NULL,
  `CategoryID` int NOT NULL,
  `Unit` varchar(10)  NOT NULL DEFAULT '个',
  `UnitPrice` decimal(20, 4) NOT NULL DEFAULT 0.0000,
  `Remark` longtext  NOT NULL,
  `Quantity` int NOT NULL DEFAULT 0,
  `Text1` varchar(255)  NULL DEFAULT NULL,
  `Text2` varchar(255)  NULL DEFAULT NULL,
  PRIMARY KEY (`ProductID`) USING BTREE
) ENGINE = InnoDB;
";

        string sql2 = @"
SET FOREIGN_KEY_CHECKS = 0;
INSERT INTO `products_sync2` VALUES (1, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (2, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (3, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (4, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (5, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (6, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (7, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (8, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (9, 'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
INSERT INTO `products_sync2` VALUES (10,'ppppppppp', 2, 'xx', 11111.2200, 'aaaaaaaaa', 9999999, 'aaaaa', 'bbbbbbb');
SET FOREIGN_KEY_CHECKS = 1;
";

        dbContext.CPQuery.Create(sql1).ExecuteNonQuery();
        dbContext.CPQuery.Create(sql2).ExecuteNonQuery();
    }


    private void Create_NotStand_Table(DbContext dbContext)
    {
        string sql = @"
DROP TABLE IF EXISTS `notstand_table`;
CREATE TABLE `notstand_table`  (
  `Rid` int NOT NULL,
  `Text1` varchar(255)  NULL DEFAULT NULL
) ENGINE = InnoDB;
";

        dbContext.CPQuery.Create(sql).ExecuteNonQuery();
    }


    private void Create_NotStand_Table2(DbContext dbContext)
    {
        string sql = @"
DROP TABLE IF EXISTS `notstand_table2`;
CREATE TABLE `notstand_table2`  (
  `xname` varchar(255)  NOT NULL,
  `xvalue1` int NULL DEFAULT NULL,
  PRIMARY KEY (`xname`) USING BTREE
) ENGINE = InnoDB;
";

        dbContext.CPQuery.Create(sql).ExecuteNonQuery();
    }


    [TestMethod]
    public void Test_ArgumentNullException()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = DataSync.Execute(null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = DataSync.Execute(new DataSyncArgs());  // BizName is null
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = DataSync.Execute(new DataSyncArgs { BizName = "111111111" });  // SrcDbContext is null
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                };
                _ = DataSync.Execute(args);  // DestDbContext is null
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                };
                _ = DataSync.Execute(args);  // SrcTableName is null
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "table1"
                };
                _ = DataSync.Execute(args);  // DestTableName is null
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "table1",
                    DestTableName = "table2"
                };
                _ = DataSync.Execute(args);  // SrcKeyField is null
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "table1",
                    DestTableName = "table2",
                    SrcKeyField = "id",
                };
                _ = DataSync.Execute(args);  // DestRelatedKey is null
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "table1",
                    DestTableName = "table2",
                    SrcKeyField = "id",
                    DestRelatedKey = "id2",
                };
                _ = DataSync.Execute(args);  // SrcFilterSql is null
            });
        }
                
    }


    [TestMethod]
    public void Test_NotSupportedException()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {
            using( DbContext dbContext2 = DbContext.Create("sqlserver") ) {

                MyAssert.IsError<NotSupportedException>(() => {
                    DataSyncArgs args = new DataSyncArgs {
                        BizName = "111111111",
                        SrcDbContext = dbContext,
                        DestDbContext = dbContext2,  // sqlserver
                        SrcTableName = "table1",
                        DestTableName = "table2",
                        SrcKeyField = "id",
                        DestRelatedKey = "id2",
                        SrcFilterSql = "categoryId = 2"
                    };
                    _ = DataSync.Execute(args);
                });
            }
        }



        using( DbContext dbContext = DbContext.Create("sqlserver") ) {

            MyAssert.IsError<NotSupportedException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "table1",
                    DestTableName = "table2",
                    SrcKeyField = "id",
                    DestRelatedKey = "id2",
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });
        }
    }



    [TestMethod]
    public async Task Test_NotSupportedExceptionAsync()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {
            using( DbContext dbContext2 = DbContext.Create("sqlserver") ) {

                await MyAssert.IsErrorAsync<NotSupportedException>(async () => {
                    DataSyncArgs args = new DataSyncArgs {
                        BizName = "111111111",
                        SrcDbContext = dbContext,
                        DestDbContext = dbContext2,  // sqlserver
                        SrcTableName = "table1",
                        DestTableName = "table2",
                        SrcKeyField = "id",
                        DestRelatedKey = "id2",
                        SrcFilterSql = "categoryId = 2"
                    };
                    _ = await DataSync.ExecuteAsync(args);
                });
            }
        }



        using( DbContext dbContext = DbContext.Create("sqlserver") ) {

            await MyAssert.IsErrorAsync<NotSupportedException>(async () => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "table1",
                    DestTableName = "table2",
                    SrcKeyField = "id",
                    DestRelatedKey = "id2",
                    SrcFilterSql = "categoryId = 2"
                };
                _ = await DataSync.ExecuteAsync(args);
            });
        }
    }


    [TestMethod]
    public void Test_InvalidCodeException_PrimaryKeyCount()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            Create_NotStand_Table(dbContext);

            MyAssert.IsError<InvalidCodeException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "NotStand_Table",    // 源表 0个主键字段
                    DestTableName = "products",
                    SrcKeyField = "CategoryID",
                    DestRelatedKey = "id2",
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });

            MyAssert.IsError<InvalidCodeException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "products",
                    DestTableName = "orderdetails",    // 目标表 2个主键字段
                    SrcKeyField = "CategoryID",
                    DestRelatedKey = "id2",
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });

        }

    }


    [TestMethod]
    public void Test_InvalidCodeException_Src_PrimaryKey()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            MyAssert.IsError<InvalidCodeException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "products",
                    DestTableName = "products",
                    SrcKeyField = "Xid",    // 源表中不存在此字段
                    DestRelatedKey = "id2",
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });


            MyAssert.IsError<InvalidCodeException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "products",
                    DestTableName = "products",
                    SrcKeyField = "CategoryID",    // 此字段不是主键字段
                    DestRelatedKey = "id2",
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });
        }
    }

    [TestMethod]
    public void Test_InvalidCodeException_Dest_PrimaryKey()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            MyAssert.IsError<InvalidCodeException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "products",
                    DestTableName = "products",
                    SrcKeyField = "ProductID",
                    DestRelatedKey = "id2",    // 目标表中不存在此字段
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });


            MyAssert.IsError<InvalidCodeException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "products",
                    DestTableName = "products",
                    SrcKeyField = "ProductID",
                    DestRelatedKey = "Remark",    // 2个字段类型不一致
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });


            MyAssert.IsError<InvalidCodeException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "products",
                    DestTableName = "products",
                    SrcKeyField = "ProductID",
                    DestRelatedKey = "ProductID",    // 目标表 {_args.DestTableName} 的主键字段不能是自增列
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });
        }
    }

    [TestMethod]
    public void Test_NotSupportedException_PrimaryKey_DataType()
    {
        using( DbContext dbContext = DbContext.Create("mysql") ) {

            Create_NotStand_Table2(dbContext);

            MyAssert.IsError<NotSupportedException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "NotStand_Table2",
                    DestTableName = "products",
                    SrcKeyField = "xname",    // 此字段必须是 int,long 类型，其它类型都不支持
                    DestRelatedKey = "id2",
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });


            MyAssert.IsError<NotSupportedException>(() => {
                DataSyncArgs args = new DataSyncArgs {
                    BizName = "111111111",
                    SrcDbContext = dbContext,
                    DestDbContext = dbContext,
                    SrcTableName = "products",
                    DestTableName = "NotStand_Table2",
                    SrcKeyField = "ProductID",
                    DestRelatedKey = "xvalue1",    // 目标表的主键字段必须是 int,long 类型，其它类型都不支持
                    SrcFilterSql = "categoryId = 2"
                };
                _ = DataSync.Execute(args);
            });
        }
    }




    [TestMethod]
    public void Test_DataSyncResult()
    {
        DataSyncResult result = new DataSyncResult();
        result.InsertCount = 11;
        result.UpdateCount = 22;

        Assert.AreEqual(11, result.InsertCount);
        Assert.AreEqual(22, result.UpdateCount);
    }

    [TestMethod]
    public void Test_FieldMap()
    {
        FieldMap map = new FieldMap();
        map.SrcField = "aa";
        map.DestField = "bb";

        Assert.AreEqual("aa", map.SrcField);
        Assert.AreEqual("bb", map.DestField);
    }
}
#endif
