using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Dto.Request;
using ApplicationCore.Models;
using ApplicationCore.Dto;

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


            CreateMap<HumanInfo, HumanInfoResponse>()
                .ForMember(a => a.HumanSocialSecurityResponse, (map) => map.MapFrom(b => b.HumanSocialSecurity))
                .ForMember(a => a.HumanTitleInfosResponse, (map) => map.MapFrom(b => b.HumanTitleInfos))
                .ForMember(a => a.HumanWorkHistoriesResponse, (map) => map.MapFrom(b => b.HumanWorkHistories))
                .ForMember(a => a.HumanEducationInfosResponse, (map) => map.MapFrom(b => b.HumanEducationInfos))
                .ForMember(a => a.HumanSalaryStructureResponse, (map) => map.MapFrom(b => b.HumanSalaryStructure))
                    .ForMember(a => a.HumanContractInfoResponse, (map) => map.MapFrom(b => b.HumanContractInfo));

            CreateMap<HumanInfoResponse, HumanInfo>()
                    .ForMember(a => a.HumanContractInfo, (map) => map.MapFrom(b => b.HumanContractInfoResponse))
            .ForMember(a => a.HumanEducationInfos, (map) => map.MapFrom(b => b.HumanEducationInfosResponse))
            .ForMember(a => a.HumanSalaryStructure, (map) => map.MapFrom(b => b.HumanSalaryStructureResponse))
            .ForMember(a => a.HumanTitleInfos, (map) => map.MapFrom(b => b.HumanTitleInfosResponse))
            .ForMember(a => a.HumanWorkHistories, (map) => map.MapFrom(b => b.HumanWorkHistoriesResponse))
            .ForMember(a => a.HumanSocialSecurity, (map) => map.MapFrom(b => b.HumanSocialSecurityResponse));

            CreateMap<HumanInfo, HumanInfoRequest>()
                .ForMember(a => a.HumanSocialSecurityRequest, (map) => map.MapFrom(b => b.HumanSocialSecurity))
                .ForMember(a => a.HumanTitleInfosRequest, (map) => map.MapFrom(b => b.HumanTitleInfos))
                .ForMember(a => a.HumanWorkHistoriesRequest, (map) => map.MapFrom(b => b.HumanWorkHistories))
                .ForMember(a => a.HumanEducationInfosRequest, (map) => map.MapFrom(b => b.HumanEducationInfos))
                .ForMember(a => a.HumanSalaryStructureRequest, (map) => map.MapFrom(b => b.HumanSalaryStructure))
                    .ForMember(a => a.HumanContractInfoRequest, (map) => map.MapFrom(b => b.HumanContractInfo));
            CreateMap<HumanInfoRequest, HumanInfo>()
                 .ForMember(a => a.HumanContractInfo, (map) => map.MapFrom(b => b.HumanContractInfoRequest))
            .ForMember(a => a.HumanEducationInfos, (map) => map.MapFrom(b => b.HumanEducationInfosRequest))
            .ForMember(a => a.HumanSalaryStructure, (map) => map.MapFrom(b => b.HumanSalaryStructureRequest))
            .ForMember(a => a.HumanTitleInfos, (map) => map.MapFrom(b => b.HumanTitleInfosRequest))
            .ForMember(a => a.HumanWorkHistories, (map) => map.MapFrom(b => b.HumanWorkHistoriesRequest))
            .ForMember(a => a.HumanSocialSecurity, (map) => map.MapFrom(b => b.HumanSocialSecurityRequest));

            CreateMap<SimpleUser, UserInfo>().ReverseMap();

            CreateMap<RewardPunishmentInfo, RewardPunishmentResponse>();
            CreateMap<RewardPunishmentResponse, RewardPunishmentInfo>();

            CreateMap<HumanInfoAdjustment, HumanInfoAdjustmentRequest>().ReverseMap();
            CreateMap<HumanInfoAdjustment, HumanInfoAdjustmentResponse>().ReverseMap();

            CreateMap<HumanInfoChange, HumanInfoChangeRequest>().ReverseMap();
            CreateMap<HumanInfoChange, HumanInfoChangeResponse>().ReverseMap();

            CreateMap<HumanInfoLeave, HumanInfoLeaveRequest>().ReverseMap();
            CreateMap<HumanInfoLeave, HumanInfoLeaveResponse>().ReverseMap();

            CreateMap<HumanInfoPartPosition, HumanInfoPartPositionRequest>().ReverseMap();
            CreateMap<HumanInfoPartPosition, HumanInfoPartPositionResponse>().ReverseMap();

            CreateMap<HumanInfoRegular, HumanInfoRegularRequest>().ReverseMap();
            CreateMap<HumanInfoRegular, HumanInfoRegularResponse>().ReverseMap();

            CreateMap<HumanInfoBlack, HumanInfoBlackRequest>().ReverseMap();
            CreateMap<HumanInfoBlack, HumanInfoBlackResponse>().ReverseMap();


            CreateMap<PositionSalary, PositionSalaryRequest>().ReverseMap();
            CreateMap<PositionSalary, PositionSalaryResponse>().ReverseMap();

            CreateMap<HumanPosition, HumanPositionRequest>().ReverseMap();
            CreateMap<HumanPosition, HumanPositionResponse>().ReverseMap();
        }
    }
}
