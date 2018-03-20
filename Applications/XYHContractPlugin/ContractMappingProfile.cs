using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHContractPlugin.Dto.Response;
using XYHContractPlugin.Models;
using XYHContractPlugin.Dto.Request;

namespace XYHContractPlugin
{
    public class ContractMappingProfile : Profile
    {
        public ContractMappingProfile()
        {
            CreateMap<ContractAnnexResponse, AnnexInfo>();
            CreateMap<AnnexInfo, ContractAnnexResponse>();

            CreateMap<ContractComplementResponse, ComplementInfo>();
            CreateMap<ComplementInfo, ContractComplementResponse>();

            CreateMap<ContractInfoResponse, ContractInfo>();
            CreateMap<ContractInfo, ContractInfoResponse>();

            CreateMap<ContractModifyResponse, ModifyInfo>();
            CreateMap<ModifyInfo, ContractModifyResponse>();

            CreateMap<FileInfoRequest, ContractFileScope>();

            CreateMap<FileInfoCallbackRequest, FileInfo>();
            CreateMap<FileInfo, FileInfoCallbackRequest>();

            CreateMap<FileInfoResponse, FileInfo>();
            CreateMap<FileInfo, FileInfoResponse>();

//            CreateMap<FileInfo, FileInfoResponse>();
//            CreateMap<FileInfoResponse,FileInfo>();

            CreateMap<ContractContentResponse, ContractInfo>()
                .ForMember(d => d.ID, m=>m.MapFrom(src => src.BaseInfo.ID))
                .ForMember(d => d.Type, m => m.MapFrom(src => src.BaseInfo.Type))
                .ForMember(d => d.Settleaccounts, m => m.MapFrom(src => src.BaseInfo.Settleaccounts))
                .ForMember(d => d.IsSubmmitShop, m => m.MapFrom(src => src.BaseInfo.IsSubmmitShop))
                .ForMember(d => d.IsSubmmitRelation, m => m.MapFrom(src => src.BaseInfo.IsSubmmitRelation))
                .ForMember(d => d.Commission, m => m.MapFrom(src => src.BaseInfo.Commission))
                .ForMember(d => d.Relation, m => m.MapFrom(src => src.BaseInfo.Relation))
                .ForMember(d => d.Name, m => m.MapFrom(src => src.BaseInfo.Name))
                .ForMember(d => d.ContractEstate, m => m.MapFrom(src => src.EstateInfo.ID))
                .ForMember(d => d.Modify, m => m.MapFrom(src => src.Modifyinfo==null?0: src.Modifyinfo.Count))
                .ForMember(d => d.Annex, m => m.MapFrom(src => src.AnnexInfo==null?0: src.AnnexInfo.Count))
                .ForMember(d => d.Complement, m => m.MapFrom(src => src.ComplementInfo==null?0: src.ComplementInfo.Count))
                .ForMember(d => d.Follow, m => m.MapFrom(src => src.BaseInfo.Follow))
                .ForMember(d => d.Remark, m => m.MapFrom(src => src.BaseInfo.Remark))
                .ForMember(d => d.ProjectName, m => m.MapFrom(src => src.BaseInfo.ProjectName))
                .ForMember(d => d.ProjectType, m => m.MapFrom(src => src.BaseInfo.ProjectType))
                .ForMember(d => d.CompanyA, m => m.MapFrom(src => src.BaseInfo.CompanyA))
                .ForMember(d => d.CompanyAT, m => m.MapFrom(src => src.BaseInfo.CompanyAT))
                .ForMember(d => d.PrincipalpepoleA, m => m.MapFrom(src => src.BaseInfo.PrincipalpepoleA))
                .ForMember(d => d.PrincipalpepoleB, m => m.MapFrom(src => src.BaseInfo.PrincipalpepoleB))
                .ForMember(d => d.ProprincipalPepole, m => m.MapFrom(src => src.BaseInfo.ProprincipalPepole))
                .ForMember(d => d.CreateUser, m => m.MapFrom(src => src.BaseInfo.CreateUser))
                .ForMember(d => d.CreateTime, m => m.MapFrom(src => src.BaseInfo.CreateTime))
                .ForMember(d => d.CreateDepartment, m => m.MapFrom(src => src.BaseInfo.CreateDepartment))
                .ForMember(d => d.Organizete, m => m.MapFrom(src => src.BaseInfo.Organizete))
                .ForMember(d => d.CommisionType, m => m.MapFrom(src => src.BaseInfo.CommisionType))
                .ForMember(d => d.StartTime, m => m.MapFrom(src => src.BaseInfo.StartTime))
                .ForMember(d => d.EndTime, m => m.MapFrom(src => src.BaseInfo.EndTime))
                .ForMember(d => d.Count, m => m.MapFrom(src => src.BaseInfo.Count))
                .ForMember(d => d.ReturnOrigin, m => m.MapFrom(src => src.BaseInfo.ReturnOrigin));

            CreateMap<ContractInfo, ContractContentResponse>()
                .ForMember(d => d.Discard, m => m.MapFrom(src => src.IsDelete))
                .ForMember(d => d.BaseInfo, m => m.MapFrom(src => new BaseInfoResponse
                {
                    ID = src.ID,
                    Type = src.Type,
                    Settleaccounts = src.Settleaccounts,
                    IsSubmmitShop = src.IsSubmmitShop,
                    IsSubmmitRelation = src.IsSubmmitRelation,
                    Commission = src.Commission,
                    Relation = src.Relation,
                    Name = src.Name,
                    Follow = src.Follow,
                    Remark = src.Remark,
                    ProjectName = src.ProjectName,
                    ProjectType = src.ProjectType,
                    CompanyA = src.CompanyA,
                    CompanyAT = src.CompanyAT,
                    PrincipalpepoleA = src.PrincipalpepoleA,
                    PrincipalpepoleB = src.PrincipalpepoleB,
                    ProprincipalPepole = src.ProprincipalPepole,
                    CreateUser = src.CreateUser,
                    CreateTime = src.CreateTime,
                    CreateDepartment = src.CreateDepartment,
                    Organizete = src.Organizete,
                    CommisionType = src.CommisionType,
                    StartTime = src.StartTime,
                    EndTime = src.EndTime,
                    Count = src.Count,
                    ReturnOrigin = src.ReturnOrigin
                }));
        }
    }
}
