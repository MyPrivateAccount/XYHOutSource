using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHHumanPlugin.Models;

public class ModifyInfoResponse
{
    public string ID { get; set; }
    public string IDCard { get; set; }
    public int? Type { get; set; }
    public ExamineStatusEnum ExamineStatus { get; set; } = ExamineStatusEnum.UnSubmit;
    public DateTime? ExamineTime { get; set; }
    public string ModifyPepole { get; set; }
    public DateTime? ModifyStartTime { get; set; }
    public string ModifyCheck { get; set; }
    public string Ext1 { get; set; }
    public string Ext2 { get; set; }
    public string Ext3 { get; set; }
    public string Ext4 { get; set; }
}