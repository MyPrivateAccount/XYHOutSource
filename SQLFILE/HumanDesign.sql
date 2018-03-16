USE `xyhdb`;

-- ----------------------------
-- Table structure for XYH_HU_HUMANMANAGE
-- ----------------------------
DROP TABLE IF EXISTS `XYH_HU_HUMANMANAGE`;
CREATE TABLE `XYH_HU_HUMANMANAGE` (
  `ID` varchar(127) NOT NULL DEFAULT '', /*数据库和员工id*/
  `UserID` varchar(127) DEFAULT '',/*用户id(工号可能就用这个,暂时先不加工号字段)*/
  `Name` varchar(127) NOT NULL DEFAULT '',/*名字*/
  `IDCard` varchar(127) NOT NULL DEFAULT '',/*身份证号*/
  `Position` varchar(127) DEFAULT '',/*职位--外链*/
  `Payment` int(11) DEFAULT '0',/*薪酬*/
  `Modify` int(11) DEFAULT '0',/*修改个数*/
  `Picture` varchar(127) DEFAULT '',/*员工照片*/
  `RecentModify` varchar(127) DEFAULT '',/*最近修改:创建 入职 离职--外链表*/
  `Contract` varchar(127) DEFAULT '',/*合同上次内容个数--外链*/
  `Blacklist` BOOLEAN DEFAULT FALSE,/*黑名单*/
  `EntryTime` datetime DEFAULT NULL,/*入职时间*/
  `BecomeTime` datetime DEFAULT NULL,/*转正时间*/
  `IsSocialInsurance` BOOLEAN DEFAULT FALSE,/*是否参加社保*/
  `SocialInsuranceInfo` varchar(127) DEFAULT '',/*社保具体信息*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_HU_CONTRACT
-- ----------------------------
DROP TABLE IF EXISTS `XYH_HU_CONTRACT`;/*人事合同管理*/
CREATE TABLE `XYH_HU_CONTRACT` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `HumanID` varchar(127) NOT NULL DEFAULT '',/*人事id*/
  `ContentPath` varchar(255) NOT NULL DEFAULT '',
  `ContentInfo` varchar(255) DEFAULT '',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_HU_ATTENDANCE
-- ----------------------------
DROP TABLE IF EXISTS `XYH_HU_ATTENDANCE`;/*考勤*/
CREATE TABLE `XYH_HU_ATTENDANCE` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `Time` datetime NOT NULL, /*考勤时间*/
  `Name` varchar(32) NOT NULL DEFAULT '',/*姓名*/
  `IDCard` varchar(127) NOT NULL DEFAULT '',/*身份*/
  `History` varchar(255) NOT NULL DEFAULT '',/*签到记录*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_POSITION`;/*职位管理*/
CREATE TABLE `XYH_HU_POSITION` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `PositionName` varchar(127) NOT NULL DEFAULT '',/*职位名称*/
  `ParentID` varchar(127) NOT NULL DEFAULT '',/*上属部门*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_SALARY`;/*薪酬管理*/
CREATE TABLE `XYH_HU_SALARY` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `Organize` varchar(127) NOT NULL DEFAULT '',/*组织(分公司)*/
  `Position` varchar(127) NOT NULL DEFAULT '',/*职位名*/
  `PositionID` varchar(127) NOT NULL DEFAULT '',/*职位id*/
  `BaseSalary` int(11) DEFAULT 0,/*基本工资*/
  `Subsidy`int(11) DEFAULT 0,/*岗位补贴*/
  `ClothesBack` int(11) DEFAULT 0,/*工装扣款*/
  `AdministrativeBack` int(11) DEFAULT 0,/*行政扣款*/
  `PortBack` int(11) DEFAULT 0,/*端口扣款*/
  `OtherBack` int(11) DEFAULT 0,/*其它扣款*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_MONTH`;/*月结*/
CREATE TABLE `XYH_HU_MONTH` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `SettleTime` datetime DEFAULT NULL,/*月结时间*/
  `OperName` varchar(127) NOT NULL DEFAULT '',/*操作人*/
  `SalaryForm` varchar(127) NOT NULL DEFAULT '',/*工资表*/
  `AttendanceForm` varchar(127) NOT NULL DEFAULT '',/*考勤表*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
