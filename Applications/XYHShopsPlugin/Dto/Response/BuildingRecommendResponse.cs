using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto.Response
{
    public class BuildingRecommendResponse
    {
        public string OrganName { get; set; }

        public List<BuildingRecommendItem> Source { get; set; }
    }
    public class BuildingRecommendItem
    {
        public string Id { get; set; }

        public string BuildingId { get; set; }

        public string RecommendUserId { get; set; }

        public DateTime RecommendTime { get; set; }

        public int RecommendDays { get; set; }

        public int Order { get; set; }

        public string UserNikeName { get; set; }

        public bool IsRegion { get; set; }

        public bool IsOutDate { get; set; }
        
        public string MainAreaName { get; set; }

        public BuildingSearchResponse BuildingSearchResponse { get; set; }
    }
}
