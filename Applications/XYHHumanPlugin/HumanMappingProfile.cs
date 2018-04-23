using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Dto.Request;

namespace XYHHumanPlugin
{
    public class HumanMappingProfile : Profile
    {
        public HumanMappingProfile()
        {
            CreateMap<HumanInfo, HumanInfoResponse>();
            CreateMap<HumanInfoResponse, HumanInfo>();

            CreateMap<BlackInfo, BlackInfoResponse>();
            CreateMap<BlackInfoResponse, BlackInfo>();

            CreateMap<AnnexInfo, FileInfoRequest>();
            CreateMap<FileInfoRequest, AnnexInfo>();

            CreateMap<AttendanceInfo, AttendanceInfoResponse>();
            CreateMap<AttendanceInfoResponse, AttendanceInfo>();

            CreateMap<PositionInfo, PositionInfoResponse>();
            CreateMap<PositionInfoResponse, PositionInfo>();

            CreateMap<SalaryInfo, SalaryInfoResponse>();
            CreateMap<SalaryInfoResponse, SalaryInfo>();
        }
    }
}
