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
            CreateMap<HumanInfo, HumanInfoResponse1>();
            CreateMap<HumanInfoResponse1, HumanInfo>();

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

            CreateMap<AttendanceSettingInfoResponse, AttendanceSettingInfo>();
            CreateMap<AttendanceSettingInfo, AttendanceSettingInfoResponse>();

            CreateMap<HumanEducationInfo, HumanEducationInfoRequest>();
            CreateMap<HumanEducationInfoRequest, HumanEducationInfo>();

            CreateMap<HumanEducationInfo, HumanEducationInfoResponse>();
            CreateMap<HumanEducationInfoResponse, HumanEducationInfo>();

            CreateMap<HumanTitleInfo, HumanTitleInfoRequest>();
            CreateMap<HumanTitleInfoRequest, HumanTitleInfo>();

            CreateMap<HumanTitleInfo, HumanTitleInfoResponse>();
            CreateMap<HumanTitleInfoResponse, HumanTitleInfo>();

            CreateMap<HumanSalaryStructure, HumanSalaryStructureRequest>();
            CreateMap<HumanSalaryStructureRequest, HumanSalaryStructure>();

            CreateMap<HumanSalaryStructure, HumanSalaryStructureResponse>();
            CreateMap<HumanSalaryStructureResponse, HumanSalaryStructure>();

            CreateMap<HumanSocialSecurity, HumanSocialSecurityRequest>();
            CreateMap<HumanSocialSecurityRequest, HumanSocialSecurity>();

            CreateMap<HumanSocialSecurity, HumanSocialSecurityResponse>();
            CreateMap<HumanSocialSecurityResponse, HumanSocialSecurity>();

            CreateMap<HumanWorkHistory, HumanWorkHistoryRequest>();
            CreateMap<HumanWorkHistoryRequest, HumanWorkHistory>();

            CreateMap<HumanWorkHistory, HumanWorkHistoryResponse>();
            CreateMap<HumanWorkHistoryResponse, HumanWorkHistory>();


            CreateMap<HumanContractInfo, HumanContractInfoRequest>();
            CreateMap<HumanContractInfoRequest, HumanContractInfo>();

            CreateMap<HumanContractInfo, HumanContractInfoResponse>();
            CreateMap<HumanContractInfoResponse, HumanContractInfo>();


            CreateMap<IEnumerable<HumanEducationInfo>, IEnumerable<HumanEducationInfoRequest>>().ReverseMap();
            CreateMap<IEnumerable<HumanEducationInfo>, IEnumerable<HumanEducationInfoResponse>>().ReverseMap();
            CreateMap<IEnumerable<HumanEducationInfo>, IEnumerable<HumanTitleInfoRequest>>().ReverseMap();
            CreateMap<IEnumerable<HumanEducationInfo>, IEnumerable<HumanTitleInfoResponse>>().ReverseMap();
            CreateMap<IEnumerable<HumanEducationInfo>, IEnumerable<HumanWorkHistoryRequest>>().ReverseMap();
            CreateMap<IEnumerable<HumanEducationInfo>, IEnumerable<HumanWorkHistoryResponse>>().ReverseMap();

            CreateMap<HumanInfo, HumanInfoResponse>().ReverseMap();
            CreateMap<HumanInfo, HumanInfoRequest>();
            CreateMap<HumanInfoRequest, HumanInfo>();


            CreateMap<HumanInfoAdjustment, HumanInfoAdjustmentResponse>().ReverseMap();
            CreateMap<HumanInfoAdjustment, HumanInfoAdjustmentResponse>().ReverseMap();

            CreateMap<HumanInfoChange, HumanInfoChangeRequest>().ReverseMap();
            CreateMap<HumanInfoChange, HumanInfoChangeResponse>().ReverseMap();

            CreateMap<HumanInfoLeave, HumanInfoLeaveRequest>().ReverseMap();
            CreateMap<HumanInfoLeave, HumanInfoLeaveResponse>().ReverseMap();

            CreateMap<HumanInfoPartPosition, HumanInfoPartPositionRequest>().ReverseMap();
            CreateMap<HumanInfoPartPosition, HumanInfoPartPositionResponse>().ReverseMap();

            CreateMap<HumanInfoRegular, HumanInfoRegularRequest>().ReverseMap();
            CreateMap<HumanInfoRegular, HumanInfoRegularResponse>().ReverseMap();
        }
    }
}
