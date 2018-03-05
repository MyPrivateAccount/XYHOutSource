using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthorizationCenter.DataSync
{
    public class OaDepartment
    {
        [Key]
        public int DEPT_ID { get; set; }
        public string DEPT_NAME { get; set; }
        public string DEPT_NO { get; set; }
        public int DEPT_PARENT { get; set; }
        public string MANAGER { get; set; }
        public string ASSISTANT_ID { get; set; }
        public string LEADER1 { get; set; }

    }
}
