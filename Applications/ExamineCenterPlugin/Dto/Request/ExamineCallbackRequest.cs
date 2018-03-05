using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class ExamineCallbackRequest
    {
        [MaxLength(127)]
        public string TaskGuid { get; set; }
        [MaxLength(127)]
        public string CurrentStepId { get; set; }
        [MaxLength(127)]
        public string PermissionItemId { get; set; }
        [MaxLength(127)]
        public string OrganizationId { get; set; }
    }
}
