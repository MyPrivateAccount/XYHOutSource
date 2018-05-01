using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Dto.Response;

namespace XYHHumanPlugin
{
    public class HumanMappingProfile : Profile
    {
        public HumanMappingProfile()
        {
            CreateMap<HumanInfo, HumanInfoResponse>();
            CreateMap<HumanInfoResponse, HumanInfo>();

            CreateMap<MonthInfo, MonthInfoResponse>();
            CreateMap<MonthInfoResponse, MonthInfo>();

            CreateMap<BlackInfo, BlackInfoResponse>();
            CreateMap<BlackInfoResponse, BlackInfo>();

            CreateMap<AttendanceInfo, AttendanceInfoResponse>();
            CreateMap<AttendanceInfoResponse, AttendanceInfo>();

            CreateMap<PositionInfo, PositionInfoResponse>();
            CreateMap<PositionInfoResponse, PositionInfo>();

            CreateMap<SalaryInfo, SalaryInfoResponse>();
            CreateMap<SalaryInfoResponse, SalaryInfo>();
        }
    }
}
