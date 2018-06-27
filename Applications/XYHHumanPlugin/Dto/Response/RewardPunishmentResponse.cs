using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class RewardPunishmentResponse
    {
        public string ID { get; set; }
        public int Type { get; set; }
        public int Detail { get; set; }
        public string DepartmentID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public DateTime? WorkDate { get; set; }
        public int Money { get; set; }
        public string Comments { get; set; }
    }
}
