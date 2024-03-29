
-- database: mynorthwind

-- ----------------------------
-- 数据填充说明
-- ----------------------------
-- 数据表结构创建完成后，运行 TestConsoleApp1
-- 执行 "从SQLSERVER/MyNorthwind导出数据到PostgreSQL"


DROP TABLE IF EXISTS testguid;
CREATE TABLE testguid (
  "rowindex" int NOT NULL GENERATED ALWAYS AS IDENTITY,
  "intvalue" int NOT NULL,
  "rowguid" uuid NOT NULL,
  PRIMARY KEY ("rowindex")
)
;

-- ----------------------------
-- Table structure for Categories
-- ----------------------------
DROP TABLE IF EXISTS Categories;
CREATE TABLE Categories (
  CategoryID int4 NOT NULL GENERATED ALWAYS AS IDENTITY ,
  CategoryName varchar(20)  NOT NULL
)
;

-- ----------------------------
-- Indexes structure for table Categories
-- ----------------------------
CREATE UNIQUE INDEX IX_categoryname ON Categories USING btree (
  CategoryName  ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table Categories
-- ----------------------------
ALTER TABLE Categories ADD CONSTRAINT categories_pkey PRIMARY KEY (CategoryID);


-- ----------------------------
-- Table structure for Customers
-- ----------------------------
DROP TABLE IF EXISTS Customers;
CREATE TABLE Customers (
  CustomerID int4 NOT NULL GENERATED ALWAYS AS IDENTITY ,
  CustomerName varchar(50) NOT NULL,
  ContactName varchar(50) ,
  Address varchar(50) ,
  PostalCode varchar(10) ,
  Tel varchar(50) 
)
;

-- ----------------------------
-- Primary Key structure for table Customers
-- ----------------------------
ALTER TABLE Customers ADD CONSTRAINT customers_pkey PRIMARY KEY (CustomerID);




-- ----------------------------
-- Table structure for Products
-- ----------------------------
DROP TABLE IF EXISTS Products;
CREATE TABLE Products (
  ProductID int4 NOT NULL GENERATED ALWAYS AS IDENTITY ,
  ProductName varchar(50) NOT NULL,
  CategoryID int4 NOT NULL,
  Unit varchar(10) NOT NULL,
  UnitPrice numeric(20,4) NOT NULL,
  Remark text NOT NULL,
  Quantity int4 NOT NULL
)
;

-- ----------------------------
-- Indexes structure for table Products
-- ----------------------------
CREATE INDEX ix_categoryid ON Products USING btree (
  CategoryID  ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table Products
-- ----------------------------
ALTER TABLE Products ADD CONSTRAINT products_pkey PRIMARY KEY (ProductID);






-- ----------------------------
-- Table structure for Products2
-- ----------------------------
DROP TABLE IF EXISTS Products2;
CREATE TABLE Products2 (
  ProductID int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
  ProductName varchar(50) NOT NULL,
  CategoryID int4 NOT NULL,
  Unit varchar(10) NOT NULL,
  UnitPrice numeric(20,4) NOT NULL,
  Remark text NOT NULL,
  Quantity int4 NOT NULL
)
;






-- ----------------------------
-- Table structure for Orders
-- ----------------------------
DROP TABLE IF EXISTS Orders;
CREATE TABLE Orders (
  OrderID int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
  CustomerID int4,
  OrderDate timestamp(6) NOT NULL,
  SumMoney numeric(20,4) NOT NULL,
  Comment varchar(300) ,
  Finished bool NOT NULL
)
;


-- ----------------------------
-- Indexes structure for table Orders
-- ----------------------------
CREATE INDEX fk_orders_customers ON Orders USING btree (
  CustomerID  ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table Orders
-- ----------------------------
ALTER TABLE Orders ADD CONSTRAINT orders_pkey PRIMARY KEY (OrderID);






-- ----------------------------
-- Table structure for OrderDetails
-- ----------------------------
DROP TABLE IF EXISTS OrderDetails;
CREATE TABLE OrderDetails (
  OrderID int4 NOT NULL,
  ProductID int4 NOT NULL,
  UnitPrice numeric(20,4) NOT NULL,
  Quantity int4 NOT NULL
)
;

-- ----------------------------
-- Indexes structure for table OrderDetails
-- ----------------------------
CREATE INDEX fk_orderdetails_products ON OrderDetails USING btree (
  ProductID ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table OrderDetails
-- ----------------------------
ALTER TABLE OrderDetails ADD CONSTRAINT orderdetails_pkey PRIMARY KEY (OrderID, ProductID);





-- ----------------------------
-- Table structure for testtype
-- ----------------------------
DROP TABLE IF EXISTS testtype;
CREATE TABLE testtype (
  rid int4 NOT NULL GENERATED ALWAYS AS IDENTITY,
  strvalue varchar(255) NOT NULL,
  str2value varchar(255) ,
  textvalue text NOT NULL,
  text2value text ,
  intvalue int4 NOT NULL,
  int2value int4,
  shortvalue int2 NOT NULL,
  short2value int2,
  longvalue int8 NOT NULL,
  long2value int8,
  charvalue char(1) NOT NULL,
  char2value char(1) ,
  boolvalue bool NOT NULL,
  bool2value bool,
  timevalue timestamp(6) NOT NULL,
  time2value timestamp(6),
  decimalvalue numeric(20,4) NOT NULL,
  decimal2value numeric(20,4),
  floatvalue float4 NOT NULL,
  float2value float4,
  doublevalue float8 NOT NULL,
  double2value float8,
  guidvalue uuid NOT NULL,
  guid2value uuid,
  bytevalue int2 NOT NULL,
  byte2value int2,
  timespanvalue time NOT NULL,
  timespan2value time,
  weekvalue int4 NOT NULL,
  week2value int4,
  binvalue bytea NOT NULL,
  bin2value bytea  
)
;

-- ----------------------------
-- Primary Key structure for table testtype
-- ----------------------------
ALTER TABLE testtype ADD CONSTRAINT testtype_pkey PRIMARY KEY (rid);







