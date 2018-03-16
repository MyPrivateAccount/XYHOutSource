using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace XYHShopsPlugin.Models
{
    public class BuildingBaseInfo
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string City { get; set; }
        [MaxLength(255)]
        public string District { get; set; }
        [MaxLength(255)]
        public string Area { get; set; }

        public int? HouseHolds { get; set; }

        public DateTime? OpenDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public int? PropertyTerm { get; set; }

        public DateTime? LandExpireDate { get; set; }

        public double? FloorSurface { get; set; }

        public double? BuiltupArea { get; set; }

        public double? PlotRatio { get; set; }

        public double? GreeningRate { get; set; }

        public int? BasementParkingSpace { get; set; }

        public int? ParkingSpace { get; set; }

        public int? BuildingNum { get; set; }

        public int? Shops { get; set; }
        [MaxLength(255)]
        public string PMC { get; set; }

        public decimal? PMF { get; set; }
        [MaxLength(255)]
        public string Developer { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
        

        [NotMapped]
        public AreaDefine CityDefine { get; set; }

        [NotMapped]
        public AreaDefine DistrictDefine { get; set; }

        [NotMapped]
        public AreaDefine AreaDefine { get; set; }

    }
}
