using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanWorkHistoryResponse
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 人事Id
        /// </summary>
        public string HumanId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>

        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 证明人
        /// </summary>
        public string Witness { get; set; }
        /// <summary>
        /// 证明人电话
        /// </summary>
        public string WitnessPhone { get; set; }

        public DateTime? CreateTime { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
