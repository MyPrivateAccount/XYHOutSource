using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

//由于前端封装接口,自动匹配结构体通过forin来的，所以必须排序属性
public class MonthFormResponse
{
    public int A1 { get; set; }//序号
    public string A2 { get; set; }//身份证号
    public string A3 { get; set; }//工号
    public string A4 { get; set; }//姓名
    public string A5 { get; set; }//部门
    public string A6 { get; set; }//职位
    public int A7 { get; set; }//正常出勤天数
    public decimal? A8 { get; set; }//基本工资
    public decimal? A81 { get; set; }//岗位补贴
    public decimal? A9 { get; set; }//交通补贴
    public decimal? A10 { get; set; }//通信补贴
    public decimal? A11 { get; set; }//其它补贴
    public int A12 { get; set; }//加班
    public int A13 { get; set; }//效绩奖励
    public int A131 {get;set;}//行政奖励
    public int A14 { get; set; }//迟到
    public int A15 { get; set; }//事假
    public int A16 { get; set; }//旷工
    public int A161 {get; set;}//行政惩罚 
    public int A17 { get; set; }//行政扣款
    public int A18 { get; set; }//端口扣款
    public float? A19 { get; set; }//应发合计
    public int A20 { get; set; }//意外险
    public int A21 { get; set; }//工作服
    public decimal? A22 { get; set; }//养老
    public decimal? A23 { get; set; }//失业
    public decimal? A24 { get; set; }//医疗
    public decimal? A25 { get; set; }//工伤
    public int? A26 { get; set; }//公积金
    public float? A27 { get; set; }//实发工资
}