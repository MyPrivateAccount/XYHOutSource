using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class ShopLeaseInfo
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }//
        public bool? HasLease { get; set; }//是否包含租约
        [MaxLength(255)]
        public string CurrentOperation { get; set; }//: 当前经营
        public DateTime? StartDate { get; set; }//开始日期
        public DateTime? EndDate { get; set; }// 结束日期
        public decimal? Rental { get; set; }// 租金
        public decimal? Deposit { get; set; }//  押金
        [MaxLength(255)]
        public string PaymentTime { get; set; }// 付款时长 按年 按季度 按月
        public double? Upscale { get; set; }//递增比率  %
        public bool? HasLeaseback { get; set; }// 是否返租
        public int? BackMonth { get; set; }// 返租时间 月
        public double? BackRate { get; set; }//  返租比率
        public string Memo { get; set; }// 备注


        public int? DepositType { get; set; } //押金类型
        public int? UpscaleStartYear { get; set; } //递增起始年

        public int? UpscaleInterval { get; set; } //递增间隔

    }
}
