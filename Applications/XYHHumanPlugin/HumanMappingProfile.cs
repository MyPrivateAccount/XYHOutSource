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

            CreateMap<FileInfoRequest, AnnexInfo>();
            CreateMap<AnnexInfo, FileInfoRequest>();

            CreateMap<FileInfoCallbackRequest, FileInfo>();
            CreateMap<FileInfo, FileInfoCallbackRequest>();

            CreateMap<SocialInsurance, SocialInsuranceResponse>();
            CreateMap<SocialInsuranceResponse, SocialInsurance>();

            CreateMap<LeaveInfo, LeaveInfoResponse>();
            CreateMap<LeaveInfoResponse, LeaveInfo>();

            CreateMap<ChangeInfoResponse, ChangeInfo>();
            CreateMap<ChangeInfo, ChangeInfoResponse>();

            CreateMap<ModifyInfoResponse, ModifyInfo>();
            CreateMap<ModifyInfo, ModifyInfoResponse>();
            
        }
    }
}
