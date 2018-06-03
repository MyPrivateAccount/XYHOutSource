using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHChargePlugin.Dto.Request;
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

            CreateMap<CostInfo, CostInfoResponse>();
            CreateMap<CostInfoResponse, CostInfo>();

            CreateMap<CostInfo, CostInfoResponseEx>();
            CreateMap<CostInfoResponseEx, CostInfo>();

            CreateMap<ReceiptInfo, ReceiptInfoResponse>();
            CreateMap<ReceiptInfoResponse, ReceiptInfo>();

            CreateMap<FileInfoRequest, FileScopeInfo>();
            CreateMap<FileScopeInfo, FileInfoRequest>();

            CreateMap<LimitInfo, LimitInfoResponse>();
            CreateMap<LimitInfoResponse, LimitInfo>();

            CreateMap<FileInfoCallbackRequest, FileInfo>();
            CreateMap<FileInfo, FileInfoCallbackRequest>();

            CreateMap<ReceiptInfoRequest, ReceiptInfo>();
            CreateMap<ReceiptInfo, ReceiptInfoRequest>();
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
