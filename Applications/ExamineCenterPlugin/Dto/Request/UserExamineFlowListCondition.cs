using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class UserExamineFlowListCondition
    {
        public List<ExamineStatus> ExamineStatus { get; set; }
        public List<string> ContentTypes { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
