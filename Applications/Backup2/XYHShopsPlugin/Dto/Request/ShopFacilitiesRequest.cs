using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class ShopFacilitiesRequest
    {
        [StringLength(127)]
        public string Id { get; set; }
        public bool? UpperWater { get; set; }// 是否有上水
        public bool? DownWater { get; set; }// 是否有下水
        public bool? Gas { get; set; }// 是否有天然气
        public bool? Chimney { get; set; }// 是否有烟管道
        public bool? Blowoff { get; set; }//排污管道
        public bool? Split { get; set; }// 可分割
        public bool? Elevator { get; set; }//电梯
        public bool? Staircase { get; set; }//扶梯
        public bool? Outside { get; set; }//外摆区
        public bool? OpenFloor { get; set; }// 架空层
        public bool? ParkingSpace { get; set; }//停车位
        public int? Voltage { get; set; }// 电压  220 / 380
        public int? Capacitance { get; set; }// 电容量  法拉
    }
}
