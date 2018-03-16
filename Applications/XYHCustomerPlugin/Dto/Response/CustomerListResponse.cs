using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Response
{
    /// <summary>
    /// 返回的客户列表数据
    /// </summary>
    public class CustomerListResponse
    {
        /// <summary>
        /// 客户Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }

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
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 跟进日期
        /// </summary>
        public DateTime? FollowupTime { get; set; }

        /// <summary>
        /// 意向
        /// </summary>
        public CustomerDemandResponse Deamand { get; set; }

        /// <summary>
        /// 报备
        /// </summary>
        public TransactionsFuResponse TransactionsResponse { get; set; }

        /// <summary>
        /// 客户备注
        /// </summary>
        public string Mark { get; set; }//主用电话号码
    }
}
