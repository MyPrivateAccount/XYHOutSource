using ApplicationCore.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            #region 用户基本信息

            CreateMap<UserInfo, SimpleUser>();
            CreateMap<SimpleUser, UserInfo>();

            #endregion

            #region 客户信息

            CreateMap<CustomerInfoCreateResponse, CustomerInfo>()
                .ForMember(a => a.CustomerDemand, (map) => map.MapFrom(b => b.CustomerDemandResponse))
                //.ForMember(a => a.CustomerReport, (map) => map.MapFrom(b => b.CustomerReportResponse))
                .ForMember(a => a.BeltLook, (map) => map.MapFrom(b => b.BeltLookResponse))
                .ForMember(a => a.HousingResources, (map) => map.MapFrom(b => b.RelationHouseResponse))
                .ForMember(a => a.CustomerPhones, (map) => map.MapFrom(b => b.CustomerPhoneResponse));
            CreateMap<CustomerInfo, CustomerInfoCreateResponse>()
                .ForMember(a => a.CustomerDemandResponse, (map) => map.MapFrom(b => b.CustomerDemand))
                //.ForMember(a => a.CustomerReportResponse, (map) => map.MapFrom(b => b.CustomerReport))
                .ForMember(a => a.BeltLookResponse, (map) => map.MapFrom(b => b.BeltLook))
                .ForMember(a => a.RelationHouseResponse, (map) => map.MapFrom(b => b.HousingResources))
                .ForMember(a => a.CustomerFollowUpResponse, (map) => map.MapFrom(b => b.CustomerFollowUp))
                 .ForMember(a => a.TransactionsResponse, (map) => map.MapFrom(b => b.CustomerTransactions))
                 .ForMember(a => a.CustomerPhoneResponse, (map) => map.MapFrom(b => b.CustomerPhones))
                 .ForMember(a => a.CustomerDealResponse, (map) => map.MapFrom(b => b.CustomerDeal))
                  .ForMember(a => a.CustomerLossResponse, (map) => map.MapFrom(b => b.CustomerLoss));

            CreateMap<CustomerInfo, CustomerSearchSaleman>()
                .ForMember(a => a.CustomerDemandResponse, (map) => map.MapFrom(b => b.CustomerDemand))
                .ForMember(a => a.CustomerDealResponse, (map) => map.MapFrom(b => b.CustomerDeal));

            CreateMap<CustomerInfo, CustomerSearchPool>()
                .ForMember(a => a.CustomerDemandResponse, (map) => map.MapFrom(b => b.CustomerDemand))
                .ForMember(a => a.CustomerPoolResponse, (map) => map.MapFrom(b => b.CustomerPool));

            CreateMap<CustomerInfo, CustomerSearchLoss>()
                .ForMember(a => a.CustomerDemandResponse, (map) => map.MapFrom(b => b.CustomerDemand))
                .ForMember(a => a.CustomerLossResponse, (map) => map.MapFrom(b => b.CustomerLoss));

            CreateMap<CustomerInfoCreateRequest, CustomerInfo>()
                .ForMember(a => a.CustomerDemand, (map) => map.MapFrom(b => b.CustomerDemandRequest))
                .ForMember(a => a.HousingResources, (map) => map.MapFrom(b => b.HousingResourcesRequest))
                .ForMember(a => a.CustomerPhones, (map) => map.MapFrom(b => b.Phones));
            CreateMap<CustomerInfo, CustomerInfoCreateRequest>()
                .ForMember(a => a.CustomerDemandRequest, (map) => map.MapFrom(b => b.CustomerDemand))
                .ForMember(a => a.HousingResourcesRequest, (map) => map.MapFrom(b => b.HousingResources))
                .ForMember(a => a.Phones, (map) => map.MapFrom(b => b.CustomerPhones));

            CreateMap<CustomerInfoSearchResponse, CustomerInfo>()
                .ForMember(a => a.CustomerDemand, (map) => map.MapFrom(b => b.CustomerDemandResponse))
                //.ForMember(a => a.CustomerReport, (map) => map.MapFrom(b => b.CustomerReportResponse))
                .ForMember(a => a.BeltLook, (map) => map.MapFrom(b => b.BeltLookResponse))
                .ForMember(a => a.HousingResources, (map) => map.MapFrom(b => b.RelationHouseResponse))
                .ForMember(a => a.CustomerTransactions, (map) => map.MapFrom(b => b.TransactionsResponse));
            CreateMap<CustomerInfo, CustomerInfoSearchResponse>()
                .ForMember(a => a.CustomerDemandResponse, (map) => map.MapFrom(b => b.CustomerDemand))
                //.ForMember(a => a.CustomerReportResponse, (map) => map.MapFrom(b => b.CustomerReport))
                .ForMember(a => a.BeltLookResponse, (map) => map.MapFrom(b => b.BeltLook))
                .ForMember(a => a.RelationHouseResponse, (map) => map.MapFrom(b => b.HousingResources))
                .ForMember(a => a.TransactionsResponse, (map) => map.MapFrom(b => b.CustomerTransactions));


            CreateMap<DealFileInfoRequest, CustomerFileScope>();
            CreateMap<CustomerFileScope, DealFileInfoRequest>();
            #endregion

            #region 需求信息

            CreateMap<CustomerDemandResponse, CustomerDemand>();
            CreateMap<CustomerDemand, CustomerDemandResponse>();

            CreateMap<CustomerDemandRequest, CustomerDemand>();
            CreateMap<CustomerDemand, CustomerDemandRequest>();

            #endregion

            #region 约看

            CreateMap<AboutLookResponse, AboutLook>();
            CreateMap<AboutLook, AboutLookResponse>();

            CreateMap<AboutLookRequest, AboutLook>();
            CreateMap<AboutLook, AboutLookRequest>();

            #endregion

            #region 报备

            CreateMap<CustomerReportResponse, CustomerReport>();
            CreateMap<CustomerReport, CustomerReportResponse>();

            CreateMap<CustomerReportRequest, CustomerReport>().ForMember(a => a.CustomerInfo, (map) => map.MapFrom(b => b.CustomerInfoCreateRequest));
            CreateMap<CustomerReport, CustomerReportRequest>().ForMember(a => a.CustomerInfoCreateRequest, (map) => map.MapFrom(b => b.CustomerInfo));

            #endregion

            #region 带看

            CreateMap<BeltLookResponse, BeltLook>();
            CreateMap<BeltLook, BeltLookResponse>();

            CreateMap<BeltLookRequest, BeltLook>();
            CreateMap<BeltLook, BeltLookRequest>();

            #endregion

            #region 需求信息

            CreateMap<RelationHouseRequest, RelationHouse>();
            CreateMap<RelationHouse, RelationHouseRequest>();

            CreateMap<RelationHouseResponse, RelationHouse>();
            CreateMap<RelationHouse, RelationHouseResponse>();

            #endregion

            #region 电话信息

            //CreateMap<RelationHouseRequest, RelationHouse>();
            //CreateMap<RelationHouse, RelationHouseRequest>();

            CreateMap<CustomerPhoneResponse, CustomerPhone>();
            CreateMap<CustomerPhone, CustomerPhoneResponse>();

            #endregion

            #region 客户失效
            CreateMap<CustomerLossResponse, CustomerLoss>();
            CreateMap<CustomerLoss, CustomerLossResponse>();
            CreateMap<CustomerLossRequest, CustomerLoss>();
            CreateMap<CustomerLoss, CustomerLossRequest>();
            CreateMap<CustomerLossResponse, CustomerLossRequest>();
            CreateMap<CustomerLossRequest, CustomerLossResponse>();
            #endregion

            #region 跟进信息

            CreateMap<FollowUpRequest, CustomerFollowUp>();
            CreateMap<CustomerFollowUp, FollowUpRequest>();

            CreateMap<FollowUpResponse, CustomerFollowUp>();
            CreateMap<CustomerFollowUp, FollowUpResponse>();

            #endregion

            #region 成交跟进信息

            CreateMap<TransactionsFollowUpRequest, CustomerTransactionsFollowUp>();
            CreateMap<CustomerTransactionsFollowUp, TransactionsFollowUpRequest>();

            CreateMap<TransactionsFollowUpResponse, CustomerTransactionsFollowUp>();
            CreateMap<CustomerTransactionsFollowUp, TransactionsFollowUpResponse>();

            #endregion

            #region 成交信息

            CreateMap<TransactionsRequest, CustomerTransactions>();
            CreateMap<CustomerTransactions, TransactionsRequest>();

            CreateMap<TransactionsResponse, CustomerTransactions>()
                /*.ForMember(a => a.CustomerTransactionsFollowUps, (map) => map.MapFrom(b => b.TransactionsFollowUpResponse))*/;
            CreateMap<CustomerTransactions, TransactionsResponse>()
               /* .ForMember(a => a.TransactionsFollowUpResponse, (map) => map.MapFrom(b => b.CustomerTransactionsFollowUps))*/;


            CreateMap<TransactionsFuResponse, CustomerTransactions>()
                .ForMember(a => a.CustomerTransactionsFollowUps, (map) => map.MapFrom(b => b.TransactionsFollowUpResponse));
            CreateMap<CustomerTransactions, TransactionsFuResponse>()
                .ForMember(a => a.TransactionsFollowUpResponse, (map) => map.MapFrom(b => b.CustomerTransactionsFollowUps));

            CreateMap<TransactionsCreateResponse, CustomerTransactions>();
            CreateMap<CustomerTransactions, TransactionsCreateResponse>();

            CreateMap<TransactionsCreateRequest, CustomerTransactions>();
            CreateMap<CustomerTransactions, TransactionsCreateRequest>();

            CreateMap<CustomerDealRequest, CustomerDeal>();
            CreateMap<CustomerDeal, CustomerDealRequest>();

            CreateMap<CustomerDealResponse, CustomerDeal>();
            CreateMap<CustomerDeal, CustomerDealResponse>();

            CreateMap<DealFileInfoRequest, DealFileScope>();
            CreateMap<DealFileScope, DealFileInfoRequest>();
            #endregion

            #region 公客池

            CreateMap<CustomerPoolResponse, CustomerPool>();
            CreateMap<CustomerPool, CustomerPoolResponse>();

            CreateMap<CustomerPoolDefineResponse, CustomerPoolDefine>();
            CreateMap<CustomerPoolDefine, CustomerPoolDefineResponse>();

            CreateMap<CustomerPoolDefineRequest, CustomerPoolDefine>();
            CreateMap<CustomerPoolDefine, CustomerPoolDefineRequest>();
            #endregion

            #region 电话

            CreateMap<CustomerPhoneResponse, CustomerPhone>();
            CreateMap<CustomerPhone, CustomerPhoneResponse>();


            CreateMap<CustomerPhoneRequest, CustomerPhone>();
            CreateMap<CustomerPhone, CustomerPhoneRequest>();
            #endregion

            #region 文件上传回调

            CreateMap<CustomerDealFileInfo, DealFileInfoCallbackRequest>();
            CreateMap<DealFileInfoCallbackRequest, CustomerDealFileInfo>();
            #endregion
        }
    }
}
