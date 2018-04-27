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
  `Age` int(11) NOT NULL DEFAULT 0,
  `Sex` int(11) NOT NULL DEFAULT 0,
  `Position` varchar(127) DEFAULT '',/*职位--外链*/
  `CreateUser` varchar(127) DEFAULT '',/*创建人*/
  `Modify` int(11) DEFAULT '0',/*修改个数*/
  `Picture` varchar(256) DEFAULT '',/*员工照片*/
  `RecentModify` varchar(127) DEFAULT '',/*最近修改:创建 入职 离职--外链表*/
  `Contract` varchar(127) DEFAULT '',/*合同上传内容个数--外链*/
  `CreateTime` datetime DEFAULT NULL,/*创建时间*/
  `EntryTime` datetime DEFAULT NULL,/*入职时间*/
  `BecomeTime` datetime DEFAULT NULL,/*转正时间*/
  `LeaveTime` datetime DEFAULT NULL,/*离开时间*/
  `IsSocialInsurance` BOOLEAN DEFAULT FALSE,/*是否参加社保*/
  `SocialInsuranceInfo` varchar(127) DEFAULT '',/*社保具体信息*/
  `BaseSalary` int(11) DEFAULT 0,/*基本工资*/
  `Subsidy`int(11) DEFAULT 0,/*岗位补贴*/
  `ClothesBack` int(11) DEFAULT 0,/*工装扣款*/
  `AdministrativeBack` int(11) DEFAULT 0,/*行政扣款*/
  `PortBack` int(11) DEFAULT 0,/*端口扣款*/
  `OtherBack` int(11) DEFAULT 0,/*其它扣款*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_HU_ANNEX
-- ----------------------------
DROP TABLE IF EXISTS `XYH_HU_ANNEX`;
CREATE TABLE `XYH_HU_ANNEX` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `FileGuid` varchar(127) NOT NULL,
  `From` varchar(255) DEFAULT NULL,
  `Group` varchar(255) DEFAULT NULL,
  `CreateUser` varchar(127) DEFAULT NULL,
  `CreateTime` datetime(6) DEFAULT NULL,
  `UpdateUser` varchar(127) DEFAULT NULL,
  `UpdateTime` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/******************************图片文件具体信息********************************/
DROP TABLE IF EXISTS `XYH_HU_FILEINFOS`;
CREATE TABLE `XYH_HU_FILEINFOS` (
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
-- Table structure for XYH_HU_BLACKLIST
-- ----------------------------
DROP TABLE IF EXISTS `XYH_HU_BLACKLIST`;/*黑名单*/
CREATE TABLE `XYH_HU_BLACKLIST` (
  `IDCard` varchar(127) NOT NULL DEFAULT '',/*身份证号*/
  `Name` varchar(127) NOT NULL DEFAULT '',/*名字*/
  `Reason` varchar(255) DEFAULT '',/*理由*/
  PRIMARY KEY (`IDCard`)
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
  `OperTime` datetime DEFAULT NULL,/*操作时间*/
  `AttendanceForm` varchar(127) NOT NULL DEFAULT '',/*月结考勤表-链接到XYH_HU_ATTENDANCEFORM.ID*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_SALARYFORM`;/*月结工资表*/
CREATE TABLE `XYH_HU_SALARYFORM` (
  `ID` varchar(127) NOT NULL DEFAULT '',/*工资表ID*/
  `MonthID` varchar(127) NOT NULL DEFAULT '',
  `HumanID` varchar(127) NOT NULL DEFAULT '',/*数据库和员工id，取员工信息*/
  `BaseSalary` int(11) DEFAULT 0,/*基本工资*/
  `Subsidy`int(11) DEFAULT 0,/*岗位补贴*/
  `ClothesBack` int(11) DEFAULT 0,/*工装扣款*/
  `AdministrativeBack` int(11) DEFAULT 0,/*行政扣款*/
  `PortBack` int(11) DEFAULT 0,/*端口扣款*/
  `OtherBack` int(11) DEFAULT 0,/*其它扣款*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_ATTENDANCEFORM`;/*月结考勤表*/
CREATE TABLE `XYH_HU_ATTENDANCEFORM` (
  `ID` varchar(127) NOT NULL DEFAULT '',/*考勤表ID*/
  `MonthID` varchar(127) NOT NULL DEFAULT '',
  `HumanID` varchar(127) NOT NULL DEFAULT '',/*数据库和员工id*/
  `Late` int(11) DEFAULT 0,/*迟到*/
  `Leave` int(11) DEFAULT 0,/*旷工*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_HU_MODIFY
-- ----------------------------
DROP TABLE IF EXISTS `XYH_HU_MODIFY`;
CREATE TABLE `XYH_HU_MODIFY` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `IDCard` varchar(127) NOT NULL DEFAULT '',
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