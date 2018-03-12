using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerPoolResponse
    {
        /// <summary>
        /// 公客池Id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 加入时间
        /// </summary>
        public DateTime JoinDate { get; set; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string MainPhone { get; set; }//主用电话号码

        /// <summary>
        /// 客户性别
        /// </summary>
        public bool Sex { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 跟进日期
        /// </summary>
        public DateTime? FollowupTime { get; set; }

        /// <summary>
        /// 客户备注
        /// </summary>
        public string Mark { get; set; }//主用电话号码
    }
}
