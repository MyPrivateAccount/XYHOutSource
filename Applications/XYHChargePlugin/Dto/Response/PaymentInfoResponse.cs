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
    /// 付款信息
    /// </summary>
    public class PaymentInfoResponse
    {
       
        public string Id { get; set; }

        public string ChargeId { get; set; }

        public string BranchId { get; set; }

        public string BranchPrefix { get; set; }

        public int Seq { get; set; }

       
        public string PaymentNo { get; set; }

        public DateTime? PaymentDate { get; set; }

        public Decimal PaymentAmount { get; set; }

        public int Status { get; set; }

        public string Payee { get; set; }

        public string Memo { get; set; }

        public string Department { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }

        
        public UserInfo CreateUserInfo { get; set; }

        public ChargeInfoResponse ChargeInfo { get; set; }

       
    }
}
