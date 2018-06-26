using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class RewardPunishmentSearchRequest
    {
        public string KeyWord { get; set; }
        public DateTime CreateDate { get; set; }

        public int OrderRule { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
