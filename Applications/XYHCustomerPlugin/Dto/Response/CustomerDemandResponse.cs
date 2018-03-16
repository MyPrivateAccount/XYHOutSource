using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    public class CustomerDemandResponse
    {

        public string Id { get; set; }

        public string CustomerId { get; set; }

        public NeedHouseType TypeId { get; set; }

        public string RequirementType { get; set; }

        public Importance? Importance { get; set; }

        public int? RoomStart { get; set; }

        public int? RoomEnd { get; set; }

        public int? LivingRoomStart { get; set; }

        public int? LivingRoomEnd { get; set; }

        public decimal? AcreageStart { get; set; }

        public decimal? AcreageEnd { get; set; }

        public int? FloorNumStart { get; set; }

        public int? FloorNumEnd { get; set; }

        public string Renovation { get; set; }

        public string Orientation { get; set; }

        public string AreaId { get; set; }

        public string DistrictId { get; set; }

        public decimal? PriceStart { get; set; }

        public decimal? PriceEnd { get; set; }

        public string PurchaseWay { get; set; }

        public decimal? DownPayment { get; set; }

        public string OtherNeeds { get; set; }

        public string PurchaseMotivation { get; set; }

        public string DepartmentId { get; set; }

        public string UserId { get; set; }

        public int? FlatShareIntention { get; set; }

        public int? LeaseTerm { get; set; }

        public int? PayRent { get; set; }

        public EmergencyDegree? EmergencyDegree { get; set; }

        public string HomeAppliances { get; set; }

        public string Furniture { get; set; }

        public string Remark { get; set; }


        public DemandLevel? DemandLevel { get; set; }

        public string SmallAreaId { get; set; }

        public string ProvinceId { get; set; }

        public string CityId { get; set; }

        public decimal DoorWithStart { get; set; }

        public decimal DoorWithEnd { get; set; }

        public int UnitPrice { get; set; }

        public string UsesType { get; set; }

        public string PropertyType { get; set; }

        public string PositionType { get; set; }
        public string AreaFullName { get; set; }
    }
}
