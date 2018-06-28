using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    public class RewardPunishmentInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        public int Type { get; set; }//1 奖励 2惩罚 3扣款
        public int Detail { get; set; }
        [MaxLength(127)]
        public string DepartmentID { get; set; }
        [MaxLength(127)]
        public string UserID { get; set; }
        [MaxLength(127)]
        public string Name { get; set; }
        public DateTime? WorkDate { get; set; }
        public int Money { get; set; }
        [MaxLength(255)]
        public string Comments { get; set; }
    }
}
