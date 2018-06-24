using ApplicationCore.Dto;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHChargePlugin.Dto
{
    /// <summary>
    /// 修改记录
    /// </summary>
    public class ModifyInfoResponse
    {
       
        public string Id { get; set; }

        public int Seq { get; set; }

        public string ChargeId { get; set; }

        public int Type { get; set; }

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


       
        public UserInfo CreateUserInfo { get; set; }

        public string DepartmentName { get; set; }

    }


}
