using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XYHChargePlugin.Dto;
using XYHChargePlugin.Models;

namespace XYHChargePlugin
{
    public class ChargeMappingProfile : Profile
    {
        public ChargeMappingProfile()
        {
            CreateMap<ChargeInfoRequest, ChargeInfo>();
            CreateMap<CostInfoRequest, CostInfo > ();
            CreateMap<ReceiptInfoRequest, ReceiptInfo>();

            CreateMap<ChargeInfoResponse, ChargeInfo>();
            CreateMap<ChargeInfo, ChargeInfoResponse>().AfterMap((s,t)=>
            {
                if (s.OrganizationExpansion != null && !String.IsNullOrEmpty(s.OrganizationExpansion.FullName))
                {
                    t.ReimburseDepartmentName = s.OrganizationExpansion.FullName;
                }
                else if (s.Organizations != null)
                {
                    t.ReimburseDepartmentName = s.Organizations.OrganizationName;
                }

                if (s.BranchInfo != null)
                {
                    t.BranchName = s.BranchInfo.OrganizationName;
                }

            });


            CreateMap<FileInfoRequest, FileInfo>();
            CreateMap<FileScopeRequest, FileScopeInfo>();
            CreateMap<FileInfo, FileInfoResponse>();
            CreateMap<FileScopeInfo, FileScopeResponse>().AfterMap((s,t)=>
            {
                if(s.FileList!=null && s.FileList.Count > 0)
                {
                    t.FileItem = this.ConvertToFileItem(s.FileGuid, s.FileList);
                }
            });

            CreateMap<CostInfo, CostInfoResponse>();
            CreateMap<CostInfoResponse, CostInfo>();

            //CreateMap<CostInfo, CostInfoResponseEx>();
            //CreateMap<CostInfoResponseEx, CostInfo>();

            CreateMap<ReceiptInfo, ReceiptInfoResponse>();
            CreateMap<ReceiptInfoResponse, ReceiptInfo>();

            CreateMap<UserInfo, SimpleUser>();
            CreateMap<HumanInfo, UserInfo>()
                 .ForPath(x=>x.Id, y=>y.MapFrom(x=>x.ID))
                 .ForPath(x=>x.UserName, y=>y.MapFrom(x=>x.Name))
                 .ForPath(x=>x.OrganizationId, y=>y.MapFrom(x=>x.DepartmentId));
            CreateMap<SimpleUser, UserInfo>();

            //CreateMap<FileInfoRequest, FileScopeInfo>();
            //CreateMap<FileScopeInfo, FileInfoRequest>();

            //CreateMap<LimitInfo, LimitInfoResponse>();
            //CreateMap<LimitInfoResponse, LimitInfo>();

            //CreateMap<FileInfoCallbackRequest, FileInfo>();
            //CreateMap<FileInfo, FileInfoCallbackRequest>();

           
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



        private FileItemResponse ConvertToFileItem(string fileGuid, List<FileInfo> fl)
        {
            FileItemResponse fi = new FileItemResponse();
            fi.FileGuid = fileGuid;
            fi.Group = fl.FirstOrDefault()?.Group;
            fi.Icon = fl.FirstOrDefault(x => x.Type == "ICON")?.Uri;
            fi.Original = fl.FirstOrDefault(x => x.Type == "ORIGINAL")?.Uri;
            fi.Medium = fl.FirstOrDefault(x => x.Type == "MEDIUM")?.Uri;
            fi.Small = fl.FirstOrDefault(x => x.Type == "SMALL")?.Uri;

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');
            if (!String.IsNullOrEmpty(fi.Icon))
            {
                fi.Icon = fr + "/" + fi.Icon.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Original))
            {
                fi.Original = fr + "/" + fi.Original.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Medium))
            {
                fi.Medium = fr + "/" + fi.Medium.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Small))
            {
                fi.Small = fr + "/" + fi.Small.TrimStart('/');
            }
            return fi;
        }
    }
}
