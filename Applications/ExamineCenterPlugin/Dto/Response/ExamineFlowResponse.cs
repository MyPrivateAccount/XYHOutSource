using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class ExamineFlowResponse
    {
        public string Id { get; set; }

        public string TaskName { get; set; }

        public DateTime SubmitTime { get; set; }

        public string SubmitUserId { get; set; }

        public string SubmitUserName { get; set; }

        public ExamineStatus ExamineStatus { get; set; }

        public string ContentType { get; set; }

        public string ContentId { get; set; }

        public string ContentName { get; set; }
        public string Content { get; set; }
        public string Desc { get; set; }

        public string SubmitDefineId { get; set; }
        public string CallbackUrl { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public string Ext3 { get; set; }
        public string Ext4 { get; set; }
        public string Ext5 { get; set; }
        public string Ext6 { get; set; }
        public string Ext7 { get; set; }
        public string Ext8 { get; set; }
        public IEnumerable<ExamineRecordResponse> ExamineRecordResponses { get; set; }
    }
}
