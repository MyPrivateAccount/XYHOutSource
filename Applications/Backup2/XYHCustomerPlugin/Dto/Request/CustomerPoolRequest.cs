using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerPoolRequest
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
