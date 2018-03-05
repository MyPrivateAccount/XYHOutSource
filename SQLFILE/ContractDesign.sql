CREATE DATABASE IF NOT EXISTS `contractdb` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
CREATE USER 'contractdbuser'@'%' IDENTIFIED BY 'contractdb';
GRANT ALL ON `contractdb`.* TO 'contractdbuser'@'%';
USE `contractdb`;

/*
Navicat MySQL Data Transfer

Source Server         : dd
Source Server Version : 50629
Source Host           : 172.16.148.224:3306
Source Database       : Hello

Target Server Type    : MYSQL
Target Server Version : 50629
File Encoding         : 65001

Date: 2016-08-04 19:18:35
*/
SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for _SEQUENCE
-- ----------------------------
DROP TABLE IF EXISTS `_SEQUENCE`;
CREATE TABLE `_SEQUENCE` (
  `name` varchar(100) NOT NULL,
  `current_val` int(11) NOT NULL DEFAULT '0',
  `increment_size` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of _SEQUENCE
-- ----------------------------
INSERT INTO `_SEQUENCE` VALUES ('DBP_OBJCOUNT', '0', '1');
INSERT INTO `_SEQUENCE` VALUES ('DBP_CONTRACTCOUNT', '0', '1');

-- ----------------------------
-- Table structure for DBP_CONTRACTINFO
-- ----------------------------
DROP TABLE IF EXISTS `DBP_CONTRACTINFO`;
CREATE TABLE `DBP_CONTRACTINFO` (
  `ID` int(11) NOT NULL, /*数据库id*/
  `NUMBER` varchar(256) DEFAULT '',/*合同id*/
  `TYPE` int(11) NOT NULL,/*合同类型*/
  `RELATION` int(11) NOT NULL,/*合同关系*/
  `CONTRACTCONTENT` int(11) NOT NULL,/*合同内容-连接其它表*/
  `CONTRACTCOMPLEMENT` int(11) NOT NULL,/*合同补充内容-连接其它表*/
  `CONTRACTANNEX` int(11) NOT NULL,/*合同附件-连接其它表*/
  `CONTRACTCHECK` int(11) NOT NULL,/*合同审核流程-连接其它表*/
  `CONTRACTESTATE` varchar(256) DEFAULT '',/*合同楼盘内容-连接其它表;多个楼盘分号*/
  `FOLLOW` int(11) NOT NULL,/*合同续签id*/
  `REMARK` varchar(4000) DEFAULT '',/*合同备注*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for DBP_CONTRACTCONTENT
-- ----------------------------
DROP TABLE IF EXISTS `DBP_CONTRACTCONTENT`;
CREATE TABLE `DBP_CONTRACTCONTENT` (
  `ID` int(11) NOT NULL,/*数据库id*/
  `PROJECTNAME` varchar(32) DEFAULT '',/*项目名称*/
  `PROJECTTYPE` int(11) NOT NULL,/*项目类型*/
  `COMPANYA` varchar(32) DEFAULT '',/*甲方公司全称*/
  `PRINCIPALPEPOLEA` varchar(32) DEFAULT '',/*甲方负责人*/
  `PRINCIPALPEPOLEB` varchar(32) DEFAULT '',/*乙方负责人*/
  `PROPRINCIPALPEPOLE` varchar(32) DEFAULT '',/*项目负责人*/
  `COMMITPEPOLE` varchar(32) DEFAULT '',/*申请人*/
  `COMMITTIME` datetime DEFAULT NULL,/*申请时间*/
  `COMMITDEPARTMENT` varchar(32) DEFAULT '',/*申请部门*/
  `COMMISIONTYPE` int(11) DEFAULT '0',/*佣金方式*/
  `STARTTTIME` datetime DEFAULT NULL,/*合同开始时间*/
  `ENDTTIME` datetime DEFAULT NULL,/*合同结束时间*/
  `COUNT` int(11) NOT NULL,/*份数*/
  `RETURNORIGIN` datetime DEFAULT NULL,/*返回原件*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for DBP_CONTRACTCOMPLEMENT
-- ----------------------------
DROP TABLE IF EXISTS `DBP_CONTRACTCOMPLEMENT`;/*不明，未继续*/
CREATE TABLE `DBP_CONTRACTCOMPLEMENT` (
  `ID` int(11) NOT NULL,
  `CONTENTID` int(11) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for DBP_CAPTUREPARAMTEMPLATE
-- ----------------------------
DROP TABLE IF EXISTS `DBP_CONTRACTANNEX`;
CREATE TABLE `DBP_CONTRACTANNEX` (
  `ID` int(11) NOT NULL,
  `TYPE` int(11) NOT NULL,/*附件类型*/
  `PATH` varchar(255) NOT NULL DEFAULT '',/*附件路径*/
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for DBP_CONTRACTCHECK
-- ----------------------------
DROP TABLE IF EXISTS `DBP_CONTRACTCHECK`;/*类型不明，暂定*/
CREATE TABLE `DBP_CONTRACTCHECK` (
  `ID` int(11) NOT NULL,
  `CHECKID` varchar(20) NOT NULL DEFAULT '',
  `FROM` varchar(20) NOT NULL DEFAULT '',
  `TO` varchar(20) NOT NULL DEFAULT '',
  `CURRENT` varchar(20) NOT NULL DEFAULT '',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for DBP_CONTRACTESTATE
-- ----------------------------
DROP TABLE IF EXISTS `DBP_CONTRACTESTATE`; /*不明，未继续*/
CREATE TABLE `DBP_CONTRACTESTATE` (
  `ID` int(11) NOT NULL,
  `ESTATENAME` varchar(255) NOT NULL DEFAULT '',
  `DEVELOPER` varchar(255) NOT NULL DEFAULT '',
  `ADDRESS` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Function structure for curr_val
-- ----------------------------
DROP FUNCTION IF EXISTS `curr_val`;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `curr_val`(seq_name VARCHAR(50)) RETURNS int(11)
    READS SQL DATA
    DETERMINISTIC
BEGIN  
DECLARE NU2 INT;  
SET NU2 = 0;  
SELECT current_val INTO NU2 FROM _SEQUENCE WHERE name = seq_name;  
RETURN NU2;  
END
;;
DELIMITER ;

-- ----------------------------
-- Function structure for next_val
-- ----------------------------
DROP FUNCTION IF EXISTS `next_val`;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` FUNCTION `next_val`(seq_name VARCHAR(50)) RETURNS int(11)
    DETERMINISTIC
BEGIN  
UPDATE _SEQUENCE SET current_val = current_val + increment_size WHERE name = seq_name;  
RETURN curr_val(seq_name);  
END
;;
DELIMITER ;
