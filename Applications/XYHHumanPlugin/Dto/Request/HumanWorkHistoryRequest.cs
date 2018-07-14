using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanWorkHistoryRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 人事Id
        /// </summary>
        [StringLength(127)]
        public string HumanId { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        [StringLength(255)]
        public string Company { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(255)]
        public string Position { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>

        public DateTime StartTime { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 证明人
        /// </summary>
        [StringLength(50)]
        public string Witness { get; set; }
        /// <summary>
        /// 证明人电话
        /// </summary>
        [StringLength(50)]
        public string WitnessPhone { get; set; }
    }
}
