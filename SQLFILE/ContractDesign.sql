USE `xyhdb`;

/*上传附件和创建修改合同 追加内容，都要写modify表，modify表自己跟踪审核和合同*/

-- ----------------------------
-- Table structure for XYH_DT_CONTRACTINFO
-- ----------------------------
-- DROP TABLE IF EXISTS `XYH_DT_CONTRACTINFO`;
-- CREATE TABLE `XYH_DT_CONTRACTINFO` (
--   `ID` varchar(127) NOT NULL DEFAULT '', /*数据库和合同id*/
--   `Type` varchar(64) NOT NULL DEFAULT '',/*合同类型*/
--   `Settleaccounts` varchar(127) NOT NULL DEFAULT '',/*结算方式*/
--   `Commission` varchar(127) DEFAULT '',/*佣金方案*/
--   `Relation` int(11) NOT NULL,/*合同关系*/
--   `Name` varchar(256) DEFAULT '',/*合同名称*/
--   `ContractEstate` varchar(127) DEFAULT '',/*合同楼盘内容-连接其它表;多个楼盘分号*/
--   `Modify` int(11) NOT NULL,/*合同修改内容是否有-包含创建*/
--   `CurrentModify` varchar(127) NOT NULL DEFAULT '',
--   `Annex` int(11) DEFAULT '0',/*合同附件是否有*/
--   `Complement` int(11) DEFAULT '0',/*合同补充内容-是否有*/
--   `Follow` varchar(127) DEFAULT '',/*合同续签id*/
--   `Remark` varchar(4000) DEFAULT '',/*合同备注*/
--   `ProjectName` varchar(64) NOT NULL DEFAULT '',/*项目名称*/
--   `ProjectType` varchar(64) NOT NULL DEFAULT '',/*项目类型*/
--   `CompanyA` varchar(64) NOT NULL DEFAULT '',/*甲方公司全称*/
--   `CompanyAT` varchar(64) NOT NULL DEFAULT '',/*甲方公司类型*/
--   `PrincipalpepoleA` varchar(127) NOT NULL DEFAULT '',/*甲方负责人*/
--   `PrincipalpepoleB` varchar(127) NOT NULL DEFAULT '',/*乙方负责人*/
--   `ProprincipalPepole` varchar(127) NOT NULL DEFAULT '',/*项目负责人*/
--   `CreateUser` varchar(127) NOT NULL DEFAULT '',/*申请人*/
--   `CreateTime` datetime DEFAULT NULL,/*申请时间*/
--   `CreateDepartment` varchar(127) DEFAULT '',/*申请部门*/
--   `CreateDepartmentID` varchar(127) DEFAULT '',/*申请部门id*/
--   `Organizate` varchar(127) DEFAULT '',/*归属组织*/
--   `OrganizateID` varchar(127) DEFAULT '',/*归属组织id*/
--   `IsDelete` BOOLEAN DEFAULT FALSE,/*是否删除*/
--   `DeleteUser` varchar(127) DEFAULT '',/*删除人*/
--   `DeleteTime` datetime DEFAULT NULL,/*删除时间*/
--   `CommisionType` varchar(64) DEFAULT '',/*佣金方式*/
--   `StartTime` datetime NOT NULL,/*合同开始时间*/
--   `EndTime` datetime NOT NULL,/*合同结束时间*/
--   `Count` int(11) NOT NULL,/*份数*/
--   `ReturnOrigin` int(11) NOT NULL DEFAULT '0',/*返回原件*/
--   `Ext1` varchar(256) DEFAULT '',/*额外数据*/
--   `Ext2` varchar(256) DEFAULT '',/*额外数据*/
--   PRIMARY KEY (`ID`)
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8;
DROP TABLE IF EXISTS `xyh_dt_contractinfo`;
CREATE TABLE `xyh_dt_contractinfo` (
  `ID` varchar(127) CHARACTER SET utf8mb4 NOT NULL,
  `Type` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
  `Settleaccounts` varchar(127) CHARACTER SET utf8mb4 NOT NULL,
  `Commission` varchar(127) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Relation` int(11) NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 DEFAULT NULL,
  `ContractEstate` varchar(127) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Modify` int(11) NOT NULL,
  `CurrentModify` varchar(127) CHARACTER SET utf8mb4 NOT NULL,
  `Annex` int(11) DEFAULT '0',
  `Complement` int(11) DEFAULT '0',
  `Follow` varchar(127) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Remark` varchar(4000) CHARACTER SET utf8mb4 DEFAULT NULL,
  `ProjectName` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
  `ProjectType` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
  `CompanyA` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
  `CompanyAT` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
  `PrincipalpepoleA` varchar(127) CHARACTER SET utf8mb4 NOT NULL,
  `PrincipalpepoleB` varchar(127) CHARACTER SET utf8mb4 NOT NULL,
  `ProprincipalPepole` varchar(127) CHARACTER SET utf8mb4 NOT NULL,
  `CreateUser` varchar(127) CHARACTER SET utf8mb4 NOT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `CreateDepartment` varchar(127) CHARACTER SET utf8mb4 DEFAULT NULL,
  `CreateDepartmentID` varchar(127) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Organizate` varchar(127) CHARACTER SET utf8mb4 DEFAULT NULL,
  `OrganizateID` varchar(127) CHARACTER SET utf8mb4 DEFAULT NULL,
  `IsDelete` tinyint(1) DEFAULT '0',
  `DeleteUser` varchar(127) CHARACTER SET utf8mb4 DEFAULT NULL,
  `DeleteTime` datetime DEFAULT NULL,
  `CommisionType` varchar(64) CHARACTER SET utf8mb4 DEFAULT NULL,
  `StartTime` datetime NOT NULL,
  `EndTime` datetime NOT NULL,
  `Count` int(11) NOT NULL,
  `ReturnOrigin` bit(1) NOT NULL DEFAULT b'0',
  `Ext1` varchar(256) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Ext2` varchar(256) CHARACTER SET utf8mb4 DEFAULT NULL,
  `IsFollow` bit(1) DEFAULT b'1',
  `ProjectAddress` varchar(256) CHARACTER SET utf8mb4 DEFAULT NULL,
  `CompanyAId` varchar(127) CHARACTER SET utf8mb4 NOT NULL,
  `OrganizateFullId` varchar(512) CHARACTER SET utf8mb4 DEFAULT NULL,
  `Num` varchar(64) CHARACTER SET utf8mb4 NOT NULL,
  `FollowTime` datetime DEFAULT NULL,
  `FollowId` varchar(127) DEFAULT NULL,
  `ExamineStatus` int(11) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
-- ----------------------------
-- Table structure for XYH_DT_CONTRACTANNEX
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_CONTRACTANNEX`;
CREATE TABLE `XYH_DT_CONTRACTANNEX` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `ContractID` varchar(127) NOT NULL DEFAULT '',
  `FileGuid` varchar(127) NOT NULL,
  `CurrentModify` varchar(127) NOT NULL DEFAULT '',
  `From` varchar(255) DEFAULT NULL,
  `Group` varchar(255) DEFAULT NULL,
  `CreateUser` varchar(127) DEFAULT NULL,
  `CreateTime` datetime(6) DEFAULT NULL,
  `UpdateUser` varchar(127) DEFAULT NULL,
  `UpdateTime` datetime(6) DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL,
  `DeleteUser` varchar(127) DEFAULT NULL,
  `DeleteTime` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/******************************图片文件具体信息********************************/
DROP TABLE IF EXISTS `XYH_DT_CONTRACTFILEINFOS`;
CREATE TABLE `XYH_DT_CONTRACTFILEINFOS` (
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

-- ----------------------------
-- Table structure for xyh_dt_companya
-- ----------------------------
DROP TABLE IF EXISTS `xyh_dt_companya`;
CREATE TABLE `xyh_dt_companya` (
  `ID` varchar(127) NOT NULL,
  `Type` varchar(64) NOT NULL,
  `Address` varchar(256) DEFAULT NULL,
  `Name` varchar(256) NOT NULL,
  `PhoneNum` varchar(32) NULL,
  `CreateUser` varchar(127) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `IsDelete` tinyint(1) DEFAULT '0',
  `DeleteUser` varchar(127)  DEFAULT NULL,
  `DeleteTime` datetime DEFAULT NULL,
  `Ext1` varchar(256)  DEFAULT NULL,
  `Ext2` varchar(256)  DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
-- ----------------------------
-- Table structure for XYH_DT_CONTRACTCOMPLEMENT
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_CONTRACTCOMPLEMENT`;/*不明，未继续*/
CREATE TABLE `XYH_DT_CONTRACTCOMPLEMENT` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `ContractID` varchar(127) NOT NULL DEFAULT '',
  `CurrentModify` varchar(127) NOT NULL DEFAULT '',
  `ContentID` int(11) NOT NULL,
  `ContentInfo` varchar(255) DEFAULT '',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_DT_CHECK
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_CHECK`;/*审核流程 类型不明，暂定*/
CREATE TABLE `XYH_DT_CHECK` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `OriginID` varchar(127) NOT NULL DEFAULT '',/*发起源id*/
  `CheckID` varchar(127) NOT NULL DEFAULT '',
  `From` varchar(32) NOT NULL DEFAULT '',
  `To` varchar(32) NOT NULL DEFAULT '',
  `Current` varchar(32) NOT NULL DEFAULT '',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_DT_CONTRACTESTATE
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_CONTRACTESTATE`; /*楼盘；不明，未继续*/
CREATE TABLE `XYH_DT_CONTRACTESTATE` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `Organizate` varchar(127) NOT NULL DEFAULT '',/*组织*/
  `EstateName` varchar(255) NOT NULL DEFAULT '',
  `Developer` varchar(255) NOT NULL DEFAULT '',
  `Address` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_DT_MODIFY
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_MODIFY`; /*楼盘；不明，未继续*/
CREATE TABLE `XYH_DT_MODIFY` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `ContractID` varchar(127) NOT NULL DEFAULT '',
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
  `Ext5` varchar(100) DEFAULT '',/**/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
