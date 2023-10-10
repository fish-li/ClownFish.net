

-- ----------------------------
-- 数据填充说明
-- ----------------------------
-- 数据表结构创建完成后，运行 TestConsoleApp1
-- 执行 "从SQLSERVER/MyNorthwind导出数据到-达梦"

CREATE SCHEMA "MyNorthwind" AUTHORIZATION "SYSDBA";

set schema MyNorthwind;


-- ----------------------------
-- Table structure for TestGuid
-- ----------------------------
create table "TestGuid"
(
	"RowIndex" INT identity(1, 1) not null ,
	"IntValue" INT not null ,
	"RowGuid" CHAR(36) not null ,
	primary key("RowIndex")
)
;


-- ----------------------------
-- Table structure for Categories
-- ----------------------------
create table "Categories"
(
	"CategoryID" INT identity(1, 1) not null ,
	"CategoryName" VARCHAR(20) not null ,
	primary key("CategoryID")
)
storage(initial 1, next 1, minextents 1, fillfactor 0)
;

create unique  index "IX_CategoryName" on "Categories"("CategoryName");


-- ----------------------------
-- Table structure for customers
-- ----------------------------
create table "Customers"
(
	"CustomerID" INT identity(1, 1) not null ,
	"CustomerName" VARCHAR(50) not null ,
	"ContactName" VARCHAR(50) default ('') not null ,
	"Address" VARCHAR(50) default ('') not null ,
	"PostalCode" VARCHAR(10) default ('') not null ,
	"Tel" VARCHAR(50) default ('') not null ,
	primary key("CustomerID")
)
;


-- ----------------------------
-- Table structure for orderdetails
-- ----------------------------
create table "OrderDetails"
(
	"OrderID" INT not null ,
	"ProductID" INT not null ,
	"UnitPrice" DECIMAL(22, 6) not null ,
	"Quantity" INT not null ,
	primary key("OrderID", "ProductID")
)
;

create index "IX_ProductID" on "OrderDetails"("ProductID");

-- ----------------------------
-- Table structure for orders
-- ----------------------------
create table "Orders"
(
	"OrderID" INT identity(1, 1) not null ,
	"CustomerID" INT not null ,
	"OrderDate" DATETIME(6) not null ,
	"SumMoney" DECIMAL(22, 6) not null ,
	"Comment" VARCHAR(300) not null ,
	"Finished" BIT not null ,
	primary key("OrderID")
)
;

create index "IX_CustomerID" on "Orders"("CustomerID");


-- ----------------------------
-- Table structure for products
-- ----------------------------
create table "Products"
(
	"ProductID" INT identity(1, 1) not null ,
	"ProductName" VARCHAR(50) not null ,
	"CategoryID" INT not null ,
	"Unit" VARCHAR(10) not null ,
	"UnitPrice" DECIMAL(22, 6) not null ,
	"Remark" TEXT not null ,
	"Quantity" INT not null ,
	primary key("ProductID")
)
;

create index "IX_CategoryID" on "Products"("CategoryID");



