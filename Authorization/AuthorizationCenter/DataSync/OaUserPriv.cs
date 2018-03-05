using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthorizationCenter.DataSync
{
    public class OaUserPriv
    {
        [Key]
        public int USER_PRIV { get; set; }
        public string PRIV_NAME { get; set; }
    }
}
