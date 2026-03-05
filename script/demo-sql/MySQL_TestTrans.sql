


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

