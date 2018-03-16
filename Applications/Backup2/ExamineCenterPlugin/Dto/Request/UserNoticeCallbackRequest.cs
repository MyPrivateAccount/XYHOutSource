using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class UserNoticeCallbackRequest
    {
        public string TaskGuid { get; set; }
        public string CurrentStepId { get; set; }

        public string PermissionItemId { get; set; }
    }

}
