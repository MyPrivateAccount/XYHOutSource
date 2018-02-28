using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayInterface.Dto.Response
{
    public class BuildingRuleInfoResponse
    {
        /// <summary>
        /// 楼盘Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 报备有效期
        /// </summary>
        public int ValidityDay { get; set; }

        /// <summary>
        /// 带看保护期
        /// </summary>
        public int BeltProtectDay { get; set; }

        /// <summary>
        /// 报备开始时间
        /// </summary>
        public DateTime ReportTime { get; set; }

        /// <summary>
        /// 提前报备时间
        /// </summary>
        public int AdvanceTime { get; set; }

        /// <summary>
        /// 接访时间
        /// </summary>
        public string LiberatingStart { get; set; }

        /// <summary>
        /// 接访时间
        /// </summary>
        public string LiberatingEnd { get; set; }

        /// <summary>
        /// 最大客户数
        /// </summary>
        public int MaxCustomer { get; set; }

        /// <summary>
        /// 是否提供完整电话
        /// </summary>
        public bool IsCompletenessPhone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 报备模板
        /// </summary>
        public string ReportedTemplate { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUse { get; set; }
    }
}
