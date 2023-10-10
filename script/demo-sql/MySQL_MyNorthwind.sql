

-- ----------------------------
-- 数据填充说明
-- ----------------------------
-- 数据表结构创建完成后，运行 TestConsoleApp1
-- 执行 "从SQLSERVER/MyNorthwind导出数据到MySQL"


CREATE TABLE `TestGuid`  (
  `RowIndex` int NOT NULL AUTO_INCREMENT,  
  `IntValue` int NOT NULL,
  `RowGuid` char(36) NOT NULL,
  PRIMARY KEY (`RowIndex`)
);



SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for Categories
-- ----------------------------
DROP TABLE IF EXISTS `categories`;
CREATE TABLE `Categories`  (
  `CategoryID` int NOT NULL AUTO_INCREMENT,
  `CategoryName` varchar(20) NOT NULL,
  PRIMARY KEY (`CategoryID`) USING BTREE,
  UNIQUE INDEX `IX_CategoryName`(`CategoryName`) USING BTREE
) ENGINE = InnoDB;


-- ----------------------------
-- Table structure for customers
-- ----------------------------
DROP TABLE IF EXISTS `Customers`;
CREATE TABLE `Customers` (
  `CustomerID` int NOT NULL AUTO_INCREMENT,
  `CustomerName` varchar(50) NOT NULL,
  `ContactName` varchar(50) DEFAULT NULL,
  `Address` varchar(50) DEFAULT NULL,
  `PostalCode` varchar(10) DEFAULT NULL,
  `Tel` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`CustomerID`)
) ENGINE=InnoDB;


-- ----------------------------
-- Table structure for orderdetails
-- ----------------------------
DROP TABLE IF EXISTS `OrderDetails`;
CREATE TABLE `OrderDetails` (
  `OrderID` int NOT NULL,
  `ProductID` int NOT NULL,
  `UnitPrice` decimal(20,4) NOT NULL,
  `Quantity` int NOT NULL,
  PRIMARY KEY (`OrderID`,`ProductID`),
  KEY `FK_Order Details_Products` (`ProductID`) USING BTREE
) ENGINE=InnoDB ;



-- ----------------------------
-- Table structure for orders
-- ----------------------------
DROP TABLE IF EXISTS `Orders`;
CREATE TABLE `Orders` (
  `OrderID` int NOT NULL AUTO_INCREMENT,
  `CustomerID` int DEFAULT NULL,
  `OrderDate` datetime NOT NULL,
  `SumMoney` decimal(20,4) NOT NULL,
  `Comment` varchar(300) DEFAULT NULL,
  `Finished` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`OrderID`),
  KEY `FK_Orders_Customers` (`CustomerID`) USING BTREE
) ENGINE=InnoDB ;


-- ----------------------------
-- Table structure for products
-- ----------------------------
DROP TABLE IF EXISTS `Products`;
CREATE TABLE `Products` (
  `ProductID` int NOT NULL AUTO_INCREMENT COMMENT '产品ID',
  `ProductName` varchar(50) NOT NULL COMMENT '产品名称',
  `CategoryID` int NOT NULL COMMENT ' 目录ID',
  `Unit` varchar(10) NOT NULL DEFAULT '个' COMMENT ' 单位',
  `UnitPrice` decimal(20,4) NOT NULL DEFAULT '0.0000' COMMENT ' 单价',
  `Remark` longtext NOT NULL COMMENT ' 备注',
  `Quantity` int NOT NULL DEFAULT 0 COMMENT ' 数量',
  PRIMARY KEY (`ProductID`),
  KEY `IX_CategoryID` (`CategoryID`) USING BTREE
) ENGINE=InnoDB;



-- ----------------------------
-- Table structure for p_tenant
-- ----------------------------
DROP TABLE IF EXISTS `p_tenant`;
CREATE TABLE `p_tenant` (
  `tenant_id` varchar(50) NOT NULL ,
  `dbserver` varchar(50) NOT NULL ,
  `dbname` varchar(50) NOT NULL ,
  `dbuser` varchar(50) NOT NULL ,
  `dbpwd` varchar(50) NOT NULL ,
  PRIMARY KEY (`tenant_id`)
) ENGINE=InnoDB ;

-- ----------------------------
-- Records of p_tenant
-- ----------------------------
INSERT INTO `p_tenant` VALUES ('561f78c61816f', '125.106.222.66', 'xiang_my561f78c61816f', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('56a9c966df069', '125.106.222.66', 'xiang_my56a9c966df069', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('57a849b725b3e', '125.106.222.66', 'xiang_my57a849b725b3e', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('5909a38c9c5bc', '125.106.222.66', 'xiang_my5909a38c9c5bc', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('595cb5e7687de', '125.106.222.66', 'xiang_my595cb5e7687de', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('596837c6430d6', '125.106.222.66', 'xiang_my596837c6430d6', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('596c8548cc560', '125.106.222.66', 'xiang_my596c8548cc560', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('596c871f21722', '125.106.222.66', 'xiang_my596c871f21722', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('596c897825049', '125.106.222.66', 'xiang_my596c897825049', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('596d704133581', '125.106.222.66', 'xiang_my596d704133581', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('596d80e51314b', '125.106.222.66', 'xiang_my596d80e51314b', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('596dac84ba824', '125.106.222.66', 'xiang_my596dac84ba824', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('596db41be0726', '125.106.222.66', 'xiang_my596db41be0726', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('598d0f008882d', '125.106.222.66', 'xiang_my598d0f008882d', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('598d126fadf85', '125.106.222.66', 'xiang_my598d126fadf85', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('5a4498d93e264', '125.106.222.66', 'xiang_my5a4498d93e264', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('5b1e17d3a0d54', '125.106.222.66', 'xiang_my5b1e17d3a0d54', 'dev', 'xxxxxxxxxx');
INSERT INTO `p_tenant` VALUES ('mssqltest', 'FishDbServer\\SQLEXPRESS2016', 'MyNorthwind', 'demo', '123456');
INSERT INTO `p_tenant` VALUES ('mysqltest', 'FishDbServer', 'mynorthwind', 'user1', 'qaz1@wsx');


-- ----------------------------
-- Table structure for testtype
-- ----------------------------
DROP TABLE IF EXISTS `TestType`;
CREATE TABLE `TestType`  (
  `Rid` int NOT NULL AUTO_INCREMENT,
  `StrValue` varchar(255) NOT NULL,
  `Str2Value` varchar(255) NULL DEFAULT NULL,
  `TextValue` text NOT NULL,
  `Text2Value` text NULL,
  `IntValue` int NOT NULL,
  `Int2Value` int NULL DEFAULT NULL,
  `ShortValue` smallint NOT NULL,
  `Short2Value` smallint NULL DEFAULT NULL,
  `LongValue` bigint NOT NULL,
  `Long2Value` bigint NULL DEFAULT NULL,
  `CharValue` char(1) NOT NULL,
  `Char2Value` char(1) NULL DEFAULT NULL,
  `BoolValue` tinyint(1) NOT NULL,
  `Bool2Value` tinyint(1) NULL DEFAULT NULL,
  `TimeValue` datetime(0) NOT NULL,
  `Time2Value` datetime(0) NULL DEFAULT NULL,
  `DecimalValue` decimal(20, 4) NOT NULL,
  `Decimal2Value` decimal(20, 4) NULL DEFAULT NULL,
  `FloatValue` float NOT NULL,
  `Float2Value` float NULL DEFAULT NULL,
  `DoubleValue` double NOT NULL,
  `Double2Value` double NULL DEFAULT NULL,
  `GuidValue` char(36) NOT NULL,
  `Guid2Value` char(36) NULL DEFAULT NULL,
  `ByteValue` tinyint(4) UNSIGNED NOT NULL,
  `Byte2Value` tinyint(4) UNSIGNED NULL DEFAULT NULL,
  `SByteValue` tinyint(4) NOT NULL,
  `SByte2Value` tinyint(4) NULL DEFAULT NULL,
  `TimeSpanValue` time NOT NULL,
  `TimeSpan2Value` time NULL DEFAULT NULL,
  `WeekValue` int NOT NULL,
  `Week2Value` int NULL DEFAULT NULL,
  `BinValue` varbinary(255) NOT NULL,
  `Bin2Value` longblob NULL,
  `UShortValue` smallint UNSIGNED NOT NULL,
  `UShort2Value` smallint UNSIGNED NULL DEFAULT NULL,
  `UIntValue` int UNSIGNED NOT NULL,
  `UInt2Value` int UNSIGNED NULL DEFAULT NULL,
  `ULongValue` bigint UNSIGNED NOT NULL,
  `ULong2Value` bigint UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`Rid`) USING BTREE
) ENGINE = InnoDB;



-- ----------------------------
-- Table structure for Trans1
-- ----------------------------
DROP TABLE IF EXISTS `Trans1`;
CREATE TABLE `Trans1`  (
  `RId` int NOT NULL AUTO_INCREMENT,
  `StrValue` varchar(255)  NOT NULL,
  PRIMARY KEY (`RId`) USING BTREE
) ENGINE = InnoDB ;


-- ----------------------------
-- Table structure for Trans2
-- ----------------------------
DROP TABLE IF EXISTS `Trans2`;
CREATE TABLE `Trans2`  (
  `Rid` int NOT NULL AUTO_INCREMENT,
  `IntValue` int NOT NULL,
  PRIMARY KEY (`Rid`) USING BTREE
) ENGINE = InnoDB ;


-- ----------------------------
-- Table structure for TestDateTimeTable
-- ----------------------------
DROP TABLE IF EXISTS `TestDateTimeTable`;
CREATE TABLE `TestDateTimeTable`  (
  `Rid` int NOT NULL AUTO_INCREMENT,
  `Time1` datetime NULL DEFAULT NULL,
  `Time2` datetime NOT NULL,
  `Time3` datetime NOT NULL,
  PRIMARY KEY (`Rid`) USING BTREE
) ENGINE = InnoDB ;



-- ----------------------------
-- Table structure for complex_entity
-- ----------------------------
DROP TABLE IF EXISTS `complex_entity`;
CREATE TABLE `complex_entity`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `location` varchar(255)  NOT NULL,
  `securestring` text  NOT NULL,
  PRIMARY KEY (`id`)  USING BTREE
) ENGINE = InnoDB;




-- ----------------------------
-- Table structure for xlog
-- ----------------------------
DROP TABLE IF EXISTS `xlog`;
CREATE TABLE `xlog`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `xtext` varchar(255)  NOT NULL,
  `createtime` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB;



