using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Models;
using System.Linq;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Dto.Request;

namespace XYHShopsPlugin
{
    public class ShopsMappingProfile : Profile
    {
        public ShopsMappingProfile()
        {
            CreateMap<UserInfo, SimpleUser>();
            CreateMap<SimpleUser, UserInfo>();
            CreateMap<BuildingRequest, Buildings>();
            CreateMap<BuildingBaseInfoResponse, BuildingBaseInfo>();
            CreateMap<BuildingFacilitiesInfoResponse, BuildingFacilities>();
            CreateMap<BuildingShopInfoResponse, BuildingShopInfo>();


            CreateMap<BuildingResponse, Buildings>();
            CreateMap<Buildings, BuildingResponse>()
                .ForMember(a => a.BasicInfo, (map) => map.MapFrom(b => b.BuildingBaseInfo))
                .ForMember(a => a.ShopInfo, (map) => map.MapFrom(b => b.BuildingShopInfo))
                .ForMember(a => a.FacilitiesInfo, (map) => map.MapFrom(b => b.BuildingFacilities))
                .ForMember(a => a.BuildingNoInfos, (map) => map.MapFrom(b => b.BuildingNoInfo))
                .ForMember(a => a.RuleInfo, (map) => map.MapFrom(b => b.BuildingRule))
                .ForMember(a => a.UpdateRecordResponses, (map) => map.MapFrom(b => b.UpdateRecords))
                .AfterMap((a, b) =>
                {
                    if (b.BasicInfo != null && a.BuildingBaseInfo != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        if (a.BuildingBaseInfo.CityDefine != null && !String.IsNullOrEmpty(a.BuildingBaseInfo.CityDefine.Name))
                        {
                            sb.Append(a.BuildingBaseInfo.CityDefine.Name);
                            if (a.BuildingBaseInfo.DistrictDefine != null && !String.IsNullOrEmpty(a.BuildingBaseInfo.DistrictDefine.Name))
                            {
                                sb.Append("-").Append(a.BuildingBaseInfo.DistrictDefine.Name);
                                if (a.BuildingBaseInfo.AreaDefine != null && !String.IsNullOrEmpty(a.BuildingBaseInfo.AreaDefine.Name))
                                {
                                    sb.Append("-").Append(a.BuildingBaseInfo.AreaDefine.Name);

                                }
                            }
                        }
                        b.BasicInfo.AreaFullName = sb.ToString();
                    }



                });

            CreateMap<BuildingSiteResponse, Buildings>();
            CreateMap<Buildings, BuildingSiteResponse>()
                .AfterMap((a, b) =>
                {
                    if (a.BuildingBaseInfo != null)
                    {
                        b.BuildingName = a.BuildingBaseInfo.Name;
                    }
                });

            CreateMap<BuildingBaseInfoRequest, BuildingBaseInfo>();
            CreateMap<BuildingBaseInfo, BuildingBaseInfoRequest>();

            CreateMap<BuildingFacilitiesRequest, BuildingFacilities>();
            CreateMap<BuildingFacilities, BuildingFacilitiesRequest>();

            CreateMap<BuildingShopInfoRequest, BuildingShopInfo>()
                .AfterMap((a, b) =>
                {
                    StringBuilder sb = new StringBuilder();
                    if (a.TradeMixPlanning != null && a.TradeMixPlanning.Count > 0)
                    {
                        sb.Append(",");
                        a.TradeMixPlanning.ForEach(x =>
                        {
                            sb.Append(x);
                            sb.Append(",");
                        });

                    }
                    b.TradeMixPlanning = sb.ToString();
                });
            CreateMap<BuildingShopInfoResponse, BuildingShopInfoRequest>();


            CreateMap<Buildings, BuildingRequest>();
            CreateMap<BuildingBaseInfo, BuildingBaseInfoResponse>().AfterMap((a, b) =>
            {

                if (a.CityDefine != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(a.CityDefine.Name);
                    if (a.DistrictDefine != null)
                    {
                        sb.Append("-").Append(a.DistrictDefine.Name);
                    }
                    if (a.AreaDefine != null)
                    {
                        sb.Append("-").Append(a.AreaDefine.Name);
                    }
                    b.AreaFullName = sb.ToString();
                }

            });
            CreateMap<BuildingFacilities, BuildingFacilitiesInfoResponse>();
            CreateMap<BuildingShopInfo, BuildingShopInfoResponse>().AfterMap((a, b) =>
            {
                if (!String.IsNullOrEmpty(a.TradeMixPlanning))
                {
                    b.TradeMixPlanning = a.TradeMixPlanning.Split(',').Where(x => !String.IsNullOrWhiteSpace(x)).ToList();
                }
            });

            CreateMap<AreaDefineRequest, AreaDefine>();
            CreateMap<AreaDefine, AreaDefineRequest>();
            CreateMap<AreaDefineResponse, AreaDefine>();
            CreateMap<AreaDefine, AreaDefineResponse>();

            CreateMap<ShopsRequest, Shops>();
            CreateMap<Shops, ShopsRequest>();
            CreateMap<ShopBaseInfoRequest, ShopBaseInfo>();
            CreateMap<ShopBaseInfo, ShopBaseInfoRequest>();
            CreateMap<ShopFacilitiesRequest, ShopFacilities>();
            CreateMap<ShopFacilities, ShopFacilitiesRequest>();
            CreateMap<ShopLeaseInfoRequest, ShopLeaseInfo>();
            CreateMap<ShopLeaseInfo, ShopLeaseInfoRequest>();

            CreateMap<ShopBaseInfoResponse, ShopBaseInfo>();
            CreateMap<ShopBaseInfo, ShopBaseInfoResponse>();
            CreateMap<ShopFacilitiesResponse, ShopFacilities>();
            CreateMap<ShopFacilities, ShopFacilitiesResponse>();
            CreateMap<ShopLeaseInfoResponse, ShopLeaseInfo>();
            CreateMap<ShopLeaseInfo, ShopLeaseInfoResponse>();
            CreateMap<ShopInfoResponse, Shops>();
            CreateMap<Shops, ShopInfoResponse>()
                .ForMember(a => a.BasicInfo, (map) => map.MapFrom(b => b.ShopBaseInfo))
                .ForMember(a => a.LeaseInfo, (map) => map.MapFrom(b => b.ShopLeaseInfo))
                .ForMember(a => a.FacilitiesInfo, (map) => map.MapFrom(b => b.ShopFacilities));


            CreateMap<FileInfoRequest, BuildingFileScope>();
            CreateMap<FileInfoRequest, ShopsFileScope>();

            CreateMap<FileInfoCallbackRequest, FileInfo>();
            CreateMap<FileInfo, FileInfoCallbackRequest>();

            CreateMap<FileInfoResponse, FileInfo>();
            CreateMap<FileInfo, FileInfoResponse>();

            CreateMap<ShopInfoResponse, ShopsRequest>();
            CreateMap<ShopsRequest, ShopInfoResponse>();

            CreateMap<BuildingResponse, BuildingRequest>();
            CreateMap<BuildingRequest, BuildingResponse>();


            CreateMap<UpdateRecord, UpdateRecordRequest>();
            CreateMap<UpdateRecordRequest, UpdateRecord>();

            CreateMap<UpdateRecordResponse, UpdateRecord>();
            CreateMap<UpdateRecord, UpdateRecordResponse>()
                .AfterMap((a, b) =>
                {
                    
                        StringBuilder sb = new StringBuilder();
                        if (a.CityDefine != null && !String.IsNullOrEmpty(a.CityDefine.Name))
                        {
                            sb.Append(a.CityDefine.Name);
                            if (a.DistrictDefine != null && !String.IsNullOrEmpty(a.DistrictDefine.Name))
                            {
                                sb.Append("-").Append(a.DistrictDefine.Name);
                                if (a.AreaDefine != null && !String.IsNullOrEmpty(a.AreaDefine.Name))
                                {
                                    sb.Append("-").Append(a.AreaDefine.Name);

                                }
                            }
                        }
                        b.AreaFullName = sb.ToString();



                });

            CreateMap<BuildingNotice, BuildingNoticeRequest>();
            CreateMap<BuildingNoticeRequest, BuildingNotice>();

            CreateMap<BuildingNotice, BuildingNoticeResponse>()
                .AfterMap((a, b) =>
                {

                    StringBuilder sb = new StringBuilder();
                    if (a.CityDefine != null && !String.IsNullOrEmpty(a.CityDefine.Name))
                    {
                        sb.Append(a.CityDefine.Name);
                        if (a.DistrictDefine != null && !String.IsNullOrEmpty(a.DistrictDefine.Name))
                        {
                            sb.Append("-").Append(a.DistrictDefine.Name);
                            if (a.AreaDefine != null && !String.IsNullOrEmpty(a.AreaDefine.Name))
                            {
                                sb.Append("-").Append(a.AreaDefine.Name);

                            }
                        }
                    }
                    b.AreaFullName = sb.ToString();
                });
            CreateMap<BuildingNoticeResponse, BuildingNotice>();

            #region 楼盘收藏

            CreateMap<BuildingFavoriteResponse, BuildingFavorite>();
            CreateMap<BuildingFavorite, BuildingFavoriteResponse>();

            CreateMap<BuildingFavoriteRequest, BuildingFavorite>();
            CreateMap<BuildingFavorite, BuildingFavoriteRequest>();

            #endregion

            #region 商铺收藏

            CreateMap<ShopsFavoriteResponse, ShopsFavorite>();
            CreateMap<ShopsFavorite, ShopsFavoriteResponse>();

            CreateMap<ShopsFavoriteRequest, ShopsFavorite>();
            CreateMap<ShopsFavorite, ShopsFavoriteRequest>();

            #endregion

            #region 楼盘推荐

            CreateMap<BuildingRecommendResponse, BuildingRecommend>();
            CreateMap<BuildingRecommend, BuildingRecommendResponse>();

            CreateMap<BuildingRecommendRequest, BuildingRecommend>();
            CreateMap<BuildingRecommend, BuildingRecommendRequest>();

            #endregion

            #region 楼栋批次

            CreateMap<BuildingNoCreateRequest, BuildingNo>();
            CreateMap<BuildingNo, BuildingNoCreateRequest>();

            CreateMap<BuildingNoCreateResponse, BuildingNo>();
            CreateMap<BuildingNo, BuildingNoCreateResponse>();

            #endregion

            #region 报备规则

            CreateMap<BuildingRuleRequest, BuildingRule>();
            CreateMap<BuildingRule, BuildingRuleRequest>();

            CreateMap<BuildingRuleInfoResponse, BuildingRule>();
            CreateMap<BuildingRule, BuildingRuleInfoResponse>();

            #endregion
        }




    }



}
