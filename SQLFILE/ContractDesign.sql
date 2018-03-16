USE `xyhdb`;

/*上传附件和创建修改合同 追加内容，都要写modify表，modify表自己跟踪审核和合同*/

-- ----------------------------
-- Table structure for XYH_DT_CONTRACTINFO
-- ----------------------------
DROP TABLE IF EXISTS `XYH_DT_CONTRACTINFO`;
CREATE TABLE `XYH_DT_CONTRACTINFO` (
  `ID` varchar(127) NOT NULL DEFAULT '', /*数据库和合同id*/
  `Type` varchar(64) NOT NULL DEFAULT '',/*合同类型*/
  `Settleaccounts` varchar(127) NOT NULL DEFAULT '',/*结算方式*/
  `IsSubmmitShop` BOOLEAN DEFAULT FALSE,/*是否提交铺号*/
  `IsSubmmitRelation` BOOLEAN DEFAULT FALSE,/*是否提交关系证明*/
  `Commission` varchar(127) DEFAULT '',/*佣金方案*/
  `Relation` int(11) NOT NULL,/*合同关系*/
  `Name` varchar(256) DEFAULT '',/*合同名称*/
  `ContractEstate` varchar(127) DEFAULT '',/*合同楼盘内容-连接其它表;多个楼盘分号*/
  `Modify` int(11) NOT NULL,/*合同修改内容是否有-包含创建*/
  `CurrentModify` varchar(127) NOT NULL DEFAULT '',
  `Annex` int(11) DEFAULT '0',/*合同附件是否有*/
  `Complement` int(11) DEFAULT '0',/*合同补充内容-是否有*/
  `Follow` varchar(127) DEFAULT '',/*合同续签id*/
  `Remark` varchar(4000) DEFAULT '',/*合同备注*/
  `ProjectName` varchar(64) NOT NULL DEFAULT '',/*项目名称*/
  `ProjectType` varchar(64) NOT NULL DEFAULT '',/*项目类型*/
  `CompanyA` varchar(64) NOT NULL DEFAULT '',/*甲方公司全称*/
  `CompanyAT` int(11) NOT NULL,/*甲方公司类型*/
  `PrincipalpepoleA` varchar(32) NOT NULL DEFAULT '',/*甲方负责人*/
  `PrincipalpepoleB` varchar(32) NOT NULL DEFAULT '',/*乙方负责人*/
  `ProprincipalPepole` varchar(32) NOT NULL DEFAULT '',/*项目负责人*/
  `CreateUser` varchar(127) NOT NULL DEFAULT '',/*申请人*/
  `CreateTime` datetime DEFAULT NULL,/*申请时间*/
  `CreateDepartment` varchar(32) DEFAULT '',/*申请部门*/
  `IsDelete` BOOLEAN DEFAULT FALSE,/*是否删除*/
  `DeleteUser` varchar(127) DEFAULT '',/*删除人*/
  `DeleteTime` datetime DEFAULT NULL,/*删除时间*/
  `CommisionType` varchar(64) DEFAULT '',/*佣金方式*/
  `StartTime` datetime NOT NULL,/*合同开始时间*/
  `EndTime` datetime NOT NULL,/*合同结束时间*/
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
  `Type` varchar(32) NOT NULL DEFAULT '',/*附件类型*/
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
  `ExamineStatus` int(11) NOT NULL,/*审核状态*/
  `ExamineTime` datetime DEFAULT NULL,/*审核更新时间*/
  `ModifyPepole` varchar(127) NOT NULL DEFAULT '',
  `ModifyStartTime` datetime DEFAULT NULL,
  `ModifyCheck` varchar(127) NOT NULL DEFAULT '',/*审核流程*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
