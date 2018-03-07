USE `xyhdb`;

/*上传附件和创建修改合同 追加内容，都要写modify表，modify表自己跟踪审核和合同*/

-- ----------------------------
-- Table structure for XYH_DT_CONTRACTINFO
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_CONTRACTINFO`;
CREATE TABLE `XYH_DT_CONTRACTINFO` (
  `ID` varchar(127) NOT NULL DEFAULT '', /*数据库id*/
  `Number` varchar(256) DEFAULT '',/*合同id*/
  `Type` int(11) NOT NULL,/*合同类型*/
  `Relation` int(11) NOT NULL,/*合同关系*/
  `ContractEstate` varchar(127) NOT NULL DEFAULT '',/*合同楼盘内容-连接其它表;多个楼盘分号*/
  `Modify` int(11) NOT NULL,/*合同修改内容是否有-包含创建*/
  `Annex` int(11) NOT NULL,/*合同附件是否有*/
  `Complement` int(11) NOT NULL,/*合同补充内容-是否有*/
  `Follow` varchar(127) NOT NULL DEFAULT '',/*合同续签id*/
  `Remark` varchar(4000) DEFAULT '',/*合同备注*/
  `ProjectName` varchar(64) DEFAULT '',/*项目名称*/
  `ProjectType` int(11) NOT NULL,/*项目类型*/
  `CompanyA` varchar(64) DEFAULT '',/*甲方公司全称*/
  `PrincipalpepoleA` varchar(32) DEFAULT '',/*甲方负责人*/
  `PrincipalpepoleB` varchar(32) DEFAULT '',/*乙方负责人*/
  `ProprincipalPepole` varchar(32) DEFAULT '',/*项目负责人*/
  `CreateUser` varchar(127) DEFAULT '',/*申请人*/
  `CreateTime` datetime DEFAULT NULL,/*申请时间*/
  `CreateDepartment` varchar(32) DEFAULT '',/*申请部门*/
  `IsDelete` BOOLEAN DEFAULT FALSE,/*是否删除*/
  `DeleteUser` varchar(127) DEFAULT '',/*删除人*/
  `DeleteTime` datetime DEFAULT NULL,/*删除时间*/
  `CommisionType` int(11) DEFAULT '0',/*佣金方式*/
  `StartTime` datetime DEFAULT NULL,/*合同开始时间*/
  `EndTime` datetime DEFAULT NULL,/*合同结束时间*/
  `Count` int(11) NOT NULL,/*份数*/
  `ReturnOrigin` int(11) NOT NULL DEFAULT '0',/*返回原件*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_DT_CONTRACTCOMPLEMENT
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_CONTRACTCOMPLEMENT`;/*不明，未继续*/
CREATE TABLE `XYH_DT_CONTRACTCOMPLEMENT` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `ContractID` varchar(127) NOT NULL DEFAULT '',
  `ContentID` int(11) NOT NULL,
  `ContentInfo` varchar(255) DEFAULT '',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_DT_CONTRACTANNEX
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_CONTRACTANNEX`;
CREATE TABLE `XYH_DT_CONTRACTANNEX` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `ContractID` varchar(127) NOT NULL DEFAULT '',
  `Type` int(11) NOT NULL,/*附件类型*/
  `Path` varchar(255) NOT NULL DEFAULT '',/*附件路径*/
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
  `ModifyPepole` varchar(32) NOT NULL DEFAULT '',
  `ModifyStartTime` datetime DEFAULT NULL,
  `ModifyCheck` varchar(127) NOT NULL DEFAULT '',/*审核流程*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
