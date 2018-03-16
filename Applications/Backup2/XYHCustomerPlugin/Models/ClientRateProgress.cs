using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客户成交进度信息
    /// </summary>
    public class ClientRateProgress : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 开始录入时间
        /// </summary>
        public DateTime? RecordTime { get; set; }
        /// <summary>
        /// 完成录入时间
        /// </summary>
        public DateTime? CompleteTime { get; set; }
        /// <summary>
        /// 约看时间
        /// </summary>
        public DateTime? AboutLookTime { get; set; }
        /// <summary>
        /// 首看时间
        /// </summary>
        public DateTime? FirstTime { get; set; }
        /// <summary>
        /// 二看时间
        /// </summary>
        public DateTime? SecondTime { get; set; }
        /// <summary>
        /// 诚意金时间
        /// </summary>
        public DateTime? MoneyTime { get; set; }
        /// <summary>
        /// 洽谈时间
        /// </summary>
        public DateTime? TalkTime { get; set; }
        /// <summary>
        /// 签约时间
        /// </summary>
        public DateTime? SigningTime { get; set; }
        /// <summary>
        /// 客户
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [MaxLength(127)]
        public string CustomerNo { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

    }
}
