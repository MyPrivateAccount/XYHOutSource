using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto.Response
{
    public class ShopsFavoriteResponse
    {
        public string Id { get; set; }

        public string ShopsId { get; set; }

        public string UserId { get; set; }

        public DateTime FavoriteTime { get; set; }
        
        public string UserNikeName { get; set; }
        
        public ShopListSearchResponse ShopListSearchResponse { get; set; }
    }
}
