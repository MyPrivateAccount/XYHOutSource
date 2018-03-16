using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingShopsUpdateModel
    {
        public List<ShopsSimpleInfo> ShopList { get; set; }
        public string Momo { get; set; }
        public List<FileItemResponse> FileList { get; set; }
    }

    public class ShopsSimpleInfo
    {
        public string BuildingNo { get; set; }
        public string FloorNo { get; set; }
        public string Number { get; set; }
        public string Id { get; set; }
    }
}
