using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto.Response
{
    /// <summary>
    /// 公客池返回体
    /// </summary>
    public class CustomerPoolResponse
    {
        /// <summary>
        /// 公客池Id
        /// </summary>
        public string PoolId { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime JoinDate { get; set; }

    }
}
