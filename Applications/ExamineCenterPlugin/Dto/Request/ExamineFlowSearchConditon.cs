using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class ExamineFlowSearchCondition
    {
        public string ContentType { get; set; }
        public string ContentId { get; set; }
        public string ExaminAction { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
