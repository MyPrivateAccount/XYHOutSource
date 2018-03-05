using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthorizationCenter.DataSync
{
    public class OaUsers
    {
        [Key]
        public int UID { get; set; }
        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string BYNAME { get; set; }
        public int USER_PRIV { get; set; }
        public int USER_PRIV_NO { get; set; }
        public string USER_PRIV_NAME { get; set; }
        public int DEPT_ID { get; set; }
        public string MOBIL_NO { get; set; }
        public string EMAIL { get; set; }
        public string AVATAR { get; set; }
    }
}

