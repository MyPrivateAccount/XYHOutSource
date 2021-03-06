﻿USE `xyhdb`;

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
  `DepartmentId` varchar(127) DEFAULT '',/*部门*/
  `Position` varchar(127) DEFAULT '',/*职位--外链*/
  `CreateUser` varchar(127) DEFAULT '',/*创建人*/
  `Modify` int(11) DEFAULT '0',/*修改个数*/
  `Picture` varchar(256) DEFAULT '',/*员工照片*/
  `RecentModify` varchar(127) DEFAULT '',/*最近修改:创建kl入职 离职--外链表*/
  `StaffStatus` int(11) DEFAULT 0,/*-1黑名单 0 未入职 1离职 2入职 3转正 */
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

DROP TABLE IF EXISTS `XYH_HU_SOCIALINSURANCE`;/*社保*/
CREATE TABLE `XYH_HU_SOCIALINSURANCE` (
  `IDCard` varchar(127) NOT NULL DEFAULT '',/*身份证号*/
  `IsSocial` tinyint DEFAULT 0,/*是否社保*/
  `Giveup` tinyint DEFAULT 0,/*放弃社保*/
  `GiveupSign` tinyint DEFAULT 0,/*放弃陈诺书*/
  `EnTime` datetime(6) DEFAULT NULL,/*转正时间*/
  `SureTime` datetime(6) DEFAULT NULL,/*参保时间*/
  `EnPlace` varchar(255) DEFAULT NULL,
  `Pension` int(11) DEFAULT 0,/*养老*/
  `Medical` int(11) DEFAULT 0,/*医疗*/
  `WorkInjury` int(11) DEFAULT 0,/*工伤*/
  `Unemployment` int(11) DEFAULT 0,/*失业*/
  `Fertility` int(11) DEFAULT 0,/*生育*/
  PRIMARY KEY (`IDCard`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_LEAVEINFO`;/*离职信息*/
CREATE TABLE `XYH_HU_LEAVEINFO` (
  `IDCard` varchar(127) NOT NULL DEFAULT '',/*身份证号*/
  `LeaveTime` datetime(6) DEFAULT NULL,/*离职时间*/
  `IsFormalities` tinyint DEFAULT 0,
  `IsReduceSocialEnsure` tinyint DEFAULT 0,
  PRIMARY KEY (`IDCard`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for XYH_HU_ATTENDANCE
-- ----------------------------
DROP TABLE IF EXISTS `XYH_HU_ATTENDANCE`;/*考勤*/
CREATE TABLE `XYH_HU_ATTENDANCE` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `UserID` varchar(127) NOT NULL DEFAULT '',/*工号*/
  `Date` datetime NOT NULL, /*考勤日期*/
  `Name` varchar(32) NOT NULL DEFAULT '',/*姓名*/
  `Comments` varchar(255) DEFAULT '',/*备注*/
  `Normal` int(11) DEFAULT 0,/*正常出勤天数*/
  `NormalDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  `Relaxation` int(11) DEFAULT 0,/*调休天数*/
  `RelaxationDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  `Matter` int(11) DEFAULT 0,/*事假天数*/
  `MatterDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  `Illness` int(11) DEFAULT 0,/*病假天数*/
  `IllnessDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  `Annual` int(11) DEFAULT 0,/*年假天数*/
  `AnnualDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  `Marry` int(11) DEFAULT 0,/*婚假天数*/
  `MarryDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  `Funeral` int(11) DEFAULT 0,/*丧假天数*/
  `FuneralDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  `Late` int(11) DEFAULT 0,/*迟到天数*/
  `LateDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  `Absent` int(11) DEFAULT 0,/*旷工天数*/
  `AbsentDate` varchar(400) DEFAULT '',/*json字符串，记录详细日期*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_ATTENDANCESETTING`;/*考勤设置表*/
CREATE TABLE `XYH_HU_ATTENDANCESETTING` (
  `Type` int(11) DEFAULT 0,/*限定类型*/
  `Times` int(11) DEFAULT 0,/*次数*/
  `Money` int(11) DEFAULT 0,
  PRIMARY KEY (`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_REWARDPUNISHMENT`;/*考勤设置表*/
CREATE TABLE `XYH_HU_REWARDPUNISHMENT` (
  `ID` varchar(127) NOT NULL DEFAULT '',
  `Type` int(11) DEFAULT 0,/*类型*/
  `Detail` int(11) DEFAULT 0,/*详细类型，对应字典*/
  `DepartmentID` varchar(127) NOT NULL DEFAULT '',/*部门id*/
  `UserID` varchar(127) NOT NULL DEFAULT '',/*用户id*/
  `Name` varchar(127) NOT NULL DEFAULT '',/*用户名*/
  `WorkDate` datetime(6) DEFAULT NULL,/*有效时间*/
  `Money` int(11) DEFAULT 0,/*金额*/
  `Comments` varchar(255) DEFAULT '',
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
  `PositionName` varchar(127) DEFAULT '',/*职位*/
  `BaseSalary` int(11) DEFAULT 0,/*基本工资*/
  `Subsidy`int(11) DEFAULT 0,/*岗位补贴*/
  `ClothesBack` int(11) DEFAULT 0,/*工装扣款*/
  `AdministrativeBack` int(11) DEFAULT 0,/*行政扣款*/
  `PortBack` int(11) DEFAULT 0,/*端口扣款*/
  `OtherBack` int(11) DEFAULT 0,/*其它扣款*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_CHANGE`;/*异动调薪*/
CREATE TABLE `XYH_HU_CHANGE` (
  `IDCard` varchar(127) NOT NULL DEFAULT '',/**/
  `ChangeTime` datetime NOT NULL, /*变动时间*/
  `ChangeType` int(11) NOT NULL,/*异动类型*/
  `ChangeReason` int(11) NOT NULL,/*异动原因*/
  `OtherReason` varchar(127) DEFAULT '',/*其它原因*/
  `OrgDepartmentId` varchar(127) DEFAULT '',/**/
  `OrgStation` varchar(127) DEFAULT '',/**/
  `NewStation` varchar(127) DEFAULT '',/**/
  `NewDepartmentId` varchar(127) DEFAULT '',/**/
  `BaseSalary` int(11) DEFAULT 0,/*基本工资*/
  `Subsidy`int(11) DEFAULT 0,/*岗位补贴*/
  `ClothesBack` int(11) DEFAULT 0,/*工装扣款*/
  `AdministrativeBack` int(11) DEFAULT 0,/*行政扣款*/
  `PortBack` int(11) DEFAULT 0,/*端口扣款*/
  `OtherBack` int(11) DEFAULT 0,/*其它扣款*/
  PRIMARY KEY (`IDCard`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_MONTH`;/*月结*/
CREATE TABLE `XYH_HU_MONTH` (
  `ID` varchar(127) NOT NULL DEFAULT '',/**/
  `SettleTime` datetime NOT NULL DEFAULT NULL,/*月结时间*/
  `OperName` varchar(127) NOT NULL DEFAULT '',/*操作人*/
  `OperID` varchar(127) NOT NULL DEFAULT '',/*操作人*/
  `AttendanceForm` varchar(127) DEFAULT '',/*月结考勤表-链接到XYH_HU_ATTENDANCEFORM.ID*/
  `SalaryForm` varchar(127) DEFAULT '',/*月结工资表-链接外面*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `XYH_HU_SALARYFORM`;/*月结工资表*/
CREATE TABLE `XYH_HU_SALARYFORM` (
  `ID` varchar(127) NOT NULL DEFAULT '',/*工资表ID*/
  `MonthID` varchar(127) NOT NULL DEFAULT '',
  `HumanID` varchar(127) NOT NULL DEFAULT '',/*存人事的guid*/
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
  `HumanID` varchar(127) NOT NULL DEFAULT '',/*存人事的guid*/
  `ComeTime` datetime DEFAULT NULL,/*到时间*/
  `LeaveTime` datetime DEFAULT NULL,/*离开时间*/
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