using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHChargePlugin.Dto.Response;
using XYHChargePlugin.Models;

namespace XYHChargePlugin
{
    public class ChargeMappingProfile : Profile
    {
        public ChargeMappingProfile()
        {
            CreateMap<ChargeInfoResponse, ChargeInfo>();
            CreateMap<ChargeInfo, ChargeInfoResponse>();

            //CreateMap<MonthInfo, MonthInfoResponse>();
            //CreateMap<MonthInfoResponse, MonthInfo>();

            //CreateMap<BlackInfo, BlackInfoResponse>();
            //CreateMap<BlackInfoResponse, BlackInfo>();

            //CreateMap<AttendanceInfo, AttendanceInfoResponse>();
            //CreateMap<AttendanceInfoResponse, AttendanceInfo>();

            //CreateMap<PositionInfo, PositionInfoResponse>();
            //CreateMap<PositionInfoResponse, PositionInfo>();

            //CreateMap<SalaryInfo, SalaryInfoResponse>();
            //CreateMap<SalaryInfoResponse, SalaryInfo>();
        }
    }
}
