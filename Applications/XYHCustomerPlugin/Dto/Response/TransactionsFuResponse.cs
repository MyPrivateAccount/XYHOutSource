using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    public class TransactionsFuResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 跟进信息
        /// </summary>
        public List<TransactionsFollowUpResponse> TransactionsFollowUpResponse { get; set; }

        /// <summary>
        /// 报备规则
        /// </summary>
        public string ReportTime { get; set; }

        /// <summary>
        /// 楼栋名称
        /// </summary>
        public string BuildingName { get; set; }
        
        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string UserTrueName { get; set; }

        /// <summary>
        /// 预计带看时间
        /// </summary>
        public DateTime? ExpectedBeltTime { get; set; }

        /// <summary>
        /// 业务员手机号
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// 业务员组别
        /// </summary>
        public string DepartmentName { get; set; }
    }
}
