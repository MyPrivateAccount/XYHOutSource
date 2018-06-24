using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHChargePlugin.Models
{
    /// <summary>
    /// 修改记录
    /// </summary>
    public class ModifyInfo
    {
        [Key]
        public string Id { get; set; }

        public int Seq { get; set; }

        public string ChargeId { get; set; }

        public ModifyTypeEnum Type { get; set; }

        public string TypeMemo { get; set; }

        public string Memo { get; set; }

        public int Status { get; set; }


        public string Department { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }

        public string RelativeId { get; set; }

        public string Ext1 { get; set; }

        public string Ext2 { get; set; }

        public string Ext3 { get; set; }


        [NotMapped]
        public SimpleUser CreateUserInfo { get; set; }

        [NotMapped]
        public Organizations Organizations { get; set; }

        [NotMapped]
        public OrganizationExpansion OrganizationExpansion { get; set; }
    }


    public enum ModifyTypeEnum
    {
        Add = 0, //新增
        Modify = 2, //修改
        Submit = 4, //提交
        Confirm = 8, //确认
        Reject = 12, //驳回
        Backup = 16, //后补发票
        Payment = 32, //付款
        Deleted = 128, //作废
        SubmitBill = 256, //提交发票
        ConfirmBill = 260,
        RejectBill = 170,
        
    }

    public class ModifyTypeConstans
    {
        public const string Add = "新增";

        public const string Modify = "修改";

        public const string Submit = "提交";

        public const string Confirm = "确认";

        public const string Reject = "驳回";

        public const string Backup = "后补发票";

        public const string Payment = "付款";

        public const string Deleted = "作废";

        public const string SubmitBill = "提交后补发票";

        public const string ConfirmBill = "后补发票确认";

        public const string RejectBill = "驳回发票";
    }
}
