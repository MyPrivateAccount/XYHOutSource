using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class ShopsRequest
    {
        [StringLength(127)]
        public string Id { get; set; }
        [StringLength(127)]
        public string BuildingId { get; set; }

        public string Summary { get; set; }

        public string OrganizationId { get; set; }

        public ExamineStatusEnum ExamineStatus { get; set; }

        [StringLength(512)]
        public string Icon { get; set; }
        //来源
        public string Source { get; set; }

        //来源ID
        public string SourceId { get; set; }
    }
}
