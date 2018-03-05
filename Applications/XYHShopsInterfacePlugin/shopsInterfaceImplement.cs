using GatewayInterface;
using System;
using System.Collections.Generic;
using System.Text;
using GatewayInterface.Dto;
using System.Threading.Tasks;
using XYHShopsPlugin.Controllers;
using AutoMapper;
using ApplicationCore;
using Microsoft.Extensions.DependencyInjection;

namespace XYHShopsInterfacePlugin
{
    public class ShopsInterfaceImplement : IShopsInterface
    {
        public ShopsInterfaceImplement()
        {

        }

        public async Task<GatewayInterface.Dto.ResponseMessage> SubmitBuildingCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<BuildingsController>().SubmitBuildingCallback(Mapper.Map<XYHShopsPlugin.Dto.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }

        public async Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordSubmitCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<UpdateRecordController>().UpdateRecordSubmitCallback(Mapper.Map<XYHShopsPlugin.Dto.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }

        public async Task<GatewayInterface.Dto.ResponseMessage> SubmitShopsCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<ShopsController>().SubmitShopsCallback(Mapper.Map<XYHShopsPlugin.Dto.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }

        public async Task<GatewayInterface.Dto.ResponseMessage> BuildingsOnSiteCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<BuildingsController>().BuildingsOnSiteCallback(Mapper.Map<XYHShopsPlugin.Dto.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }

        /// <summary>
        /// 提交成交信息后 将商铺销售状态改为已售
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async Task<GatewayInterface.Dto.ResponseMessage<bool>> UpdateShopSaleStatus(string userid, string shopsid)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var ss = new List<string>();
                ss.Add(shopsid);
                var response = await scope.ServiceProvider.GetRequiredService<ShopsController>().PutShopSaleStatus(userid, Mapper.Map<XYHShopsPlugin.Dto.Request.SaleShopsStatusRquest>(new SaleShopsStatusRquest
                {
                    ShopsIds = ss,
                    SaleStatus = "10",
                    Mark = "成交信息修改"
                }));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage<bool>>(response);
            }
        }

        /// <summary>
        /// 获取当前用户是否为楼盘驻场用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async Task<GatewayInterface.Dto.ResponseMessage<bool>> IsResidentUser(string userId, string buildingId)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<BuildingsController>().IsResidentUser(userId, buildingId);
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage<bool>>(response);
            }
        }

        /// <summary>
        /// 获取当前用户是否能够查看该楼盘
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async Task<GatewayInterface.Dto.ResponseMessage<bool>> IsManagerSiteUser(string userid, string buildingId)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = new ApplicationCore.ResponseMessage<bool>();
                response = await scope.ServiceProvider.GetRequiredService<BuildingsController>().IsManagerSiteUser(userid, buildingId);
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage<bool>>(response);
            }
        }

        /// <summary>
        /// 获取当前楼盘报备规则
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async Task<GatewayInterface.Dto.ResponseMessage<GatewayInterface.Dto.Response.BuildingRuleInfoResponse>> GetBuilidngRule(string buildingId)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = new ApplicationCore.ResponseMessage<XYHShopsPlugin.Dto.Response.BuildingRuleInfoResponse>();
                response = await scope.ServiceProvider.GetRequiredService<BuildingRuleController>().GetBuildingRule(buildingId);
                if (response.Extension == null) return null;
                var res = new GatewayInterface.Dto.ResponseMessage<GatewayInterface.Dto.Response.BuildingRuleInfoResponse>();
                //res.Extension = response.Extension.ValidityDay < 1 ? 1: response.Extension.ValidityDay;
                //return res;
                var ss = new GatewayInterface.Dto.Response.BuildingRuleInfoResponse();
                ss.Id = response.Extension.Id;
                ss.AdvanceTime = response.Extension.AdvanceTime;
                ss.BeltProtectDay = response.Extension.BeltProtectDay < 1 ? 30 : response.Extension.ValidityDay; 
                ss.IsCompletenessPhone = response.Extension.IsCompletenessPhone;
                ss.IsUse = response.Extension.IsUse;
                ss.LiberatingEnd = response.Extension.LiberatingEnd;
                ss.LiberatingStart = response.Extension.LiberatingStart;
                ss.Mark = response.Extension.Mark;
                ss.MaxCustomer = response.Extension.MaxCustomer;
                ss.ReportedTemplate = response.Extension.ReportedTemplate;
                ss.ReportTime = response.Extension.ReportTime;
                ss.ValidityDay = response.Extension.ValidityDay < 1 ? 1 : response.Extension.ValidityDay;
                res.Extension = ss;
                //未找到映射类型 很烦
                return /*Mapper.Map<GatewayInterface.Dto.ResponseMessage<GatewayInterface.Dto.Response.BuildingRuleInfoResponse>>(response)*/res;
            }
        }
    }
}
