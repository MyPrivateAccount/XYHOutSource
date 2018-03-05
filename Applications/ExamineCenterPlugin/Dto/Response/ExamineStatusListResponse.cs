using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class ExamineStatusListResponse
    {
        public string ContentId { get; set; }
        public string ContentType { get; set; }
        public string Action { get; set; }
        public ExamineStatus ExamineStatus { get; set; }
    }
}
