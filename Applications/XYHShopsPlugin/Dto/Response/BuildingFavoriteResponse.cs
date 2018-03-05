using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto.Response
{
    public class BuildingFavoriteResponse
    {
        public string Id { get; set; }

        public string BuildingId { get; set; }
        
        public string UserId { get; set; }

        public string UserNikeName { get; set; }

        public DateTime FavoriteTime { get; set; }

        public BuildingSearchResponse BuildingSearchResponse { get; set; }
    }
}
