USE `xyhdb`;

-- ----------------------------
-- Table structure for XYH_HU_HUMANMANAGE
-- ----------------------------
DROP TABLE IF EXISTS `XYH_CH_CHARGEMANAGE`;
CREATE TABLE `XYH_CH_CHARGEMANAGE` (
  `ID` varchar(127) NOT NULL DEFAULT '', /*数据库id*/
  `Department` varchar(127) NOT NULL DEFAULT '',/*部门(报销门店)*/
  `Note` varchar(255) DEFAULT '',/*备注*/
  `CreateTime` datetime DEFAULT NULL,/*创建时间*/
  `PostTime` datetime DEFAULT NULL,/*打款时间*/
  `CreateUser` varchar(127) DEFAULT NULL,
  `CreateUserName` varchar(127) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_CH_COST`;
CREATE TABLE `XYH_CH_COST` (
  `ID` varchar(127) NOT NULL DEFAULT '', /*数据库id*/
  `ChargeID` varchar(127) DEFAULT '',/*mange的id*/
  `Type` int(11) NOT NULL DEFAULT 0,/*费用类型*/
  `Cost` int(11) NOT NULL DEFAULT 0, /*金额*/
  `Comments` varchar(255) DEFAULT '',/*摘要*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_CH_RECEIPT`;/**/
CREATE TABLE `XYH_CH_RECEIPT` (
  `ID` varchar(127) NOT NULL DEFAULT '', /*数据库id*/
  `CostID` varchar(127) DEFAULT '',/*cost的id*/
  `ReceiptNumber` varchar(127) DEFAULT '',/*发票号*/
  `ReceiptMoney` int(11) DEFAULT 0,/*发票金额*/
  `Comments` varchar(127) DEFAULT '',/*备注信息*/
  `CreateUser` varchar(127) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/******************************图片文件具体信息********************************/
DROP TABLE IF EXISTS `XYH_CH_FILESCOPE`;/*关系表*/
CREATE TABLE `XYH_CH_FILESCOPE` (
  `ReceiptID` varchar(127) NOT NULL DEFAULT '', /*数据库id*/
  `FileGuid` varchar(127) DEFAULT '',/*cost的id*/
  PRIMARY KEY (`ReceiptID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_FILEINFOS`;
CREATE TABLE `XYH_HU_FILEINFOS` (/*关联表和文件表放一起  先写关联信息，回调写文件信息*/
  `FileGuid` varchar(127) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Size` double NOT NULL,
  `Type` varchar(64) NOT NULL,
  `FileExt` varchar(32) NOT NULL,
  `Height` int(11) DEFAULT NULL,
  `Uri` varchar(1000) DEFAULT NULL,
  `Width` int(11) DEFAULT NULL,
  `Ext1` varchar(1000) DEFAULT NULL,
  `Ext2` varchar(1000) DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL,
  `Summary` varchar(512) DEFAULT NULL,
  `UpdateTime` datetime(6) DEFAULT NULL,
  `UpdateUser` varchar(127) DEFAULT NULL,
  `CreateTime` datetime(6) DEFAULT NULL,
  `CreateUser` varchar(127) DEFAULT NULL,
  `DeleteTime` datetime(6) DEFAULT NULL,
  `DeleteUser` varchar(127) DEFAULT NULL,
  `Driver` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`FileGuid`,`FileExt`,`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

DROP TABLE IF EXISTS `XYH_CH_LIMIT`;
CREATE TABLE `XYH_CH_LIMIT` (
  `ID` varchar(127) DEFAULT '',/*工号*/
  `UserID` varchar(127) DEFAULT '',/*工号*/
  `LimitType` int(11) NOT NULL DEFAULT 0, /*限制类型*/
  `CostLimit` int(11) NOT NULL DEFAULT 0, /*金额限制*/
  `ContentLimit` varchar(127) DEFAULT '',/*限制内容*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_CH_MODIFY`;
CREATE TABLE `XYH_CH_MODIFY` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `ChargeID` varchar(127) NOT NULL DEFAULT '',
  `Type` int(11) NOT NULL,/*修改-创建-附件*/
  `ExamineStatus` int(11) NOT NULL,/*审核状态*/
  `ExamineTime` datetime DEFAULT NULL,/*审核更新时间*/
  `ModifyPepole` varchar(127) NOT NULL DEFAULT '',
  `ModifyStartTime` datetime DEFAULT NULL,
  `ModifyCheck` varchar(127) NOT NULL DEFAULT '',/*审核流程*/
  `Ext1` varchar(4000) DEFAULT '',/**/
  `Ext2` varchar(15000) DEFAULT '',/**/
  `Ext3` varchar(600) DEFAULT '',/**/
  `Ext4` varchar(100) DEFAULT '',/**/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;