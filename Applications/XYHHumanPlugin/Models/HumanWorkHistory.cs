using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 工作经历
    /// </summary>
    public class HumanWorkHistory
    {
        public string Id { get; set; }
        public string HumanId { get; set; }

        public string Company { get; set; }

        public string Position { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }
    }
}
