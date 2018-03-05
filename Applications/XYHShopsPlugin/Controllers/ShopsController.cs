using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using ApplicationCore.Models;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using GatewayInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Managers;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Shops")]
    public class ShopsController : Controller
    {
        private readonly ShopsManager _shopsManager;
        private readonly IMapper _mapper;
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("Shops");

        public ShopsController(ShopsManager shopsManager,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            _shopsManager = shopsManager;
            _mapper = mapper;
            _permissionExpansionManager = permissionExpansionManager;
        }

        /// <summary>
        /// 获取商铺列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsRetrieve" })]
        public async Task<PagingResponseMessage<ShopInfoResponse>> GetShopsList(UserInfo user, [FromBody]ShopSearchCondition condition)
        {
            PagingResponseMessage<ShopInfoResponse> response = new PagingResponseMessage<ShopInfoResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取商铺列表(GetShopsList)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                return await _shopsManager.SimpleSearch(user, condition);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取商铺列表(GetShopsList)请求失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 查询商铺列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsRetrieve" })]
        public async Task<PagingResponseMessage<ShopListSearchResponse>> Search(UserInfo user, [FromBody]ShopListSearchCondition condition)
        {
            PagingResponseMessage<ShopListSearchResponse> response = new PagingResponseMessage<ShopListSearchResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询商铺列表(Search)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                response = await _shopsManager.Search(user.Id, condition);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询商铺列表(Search)请求失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 查询商铺列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("searchrecommend")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsRetrieve" })]
        public async Task<PagingResponseMessage<ShopListSearchResponse>> SearchRecommend(UserInfo user, [FromBody]ShopListSearchCondition condition)
        {
            PagingResponseMessage<ShopListSearchResponse> response = new PagingResponseMessage<ShopListSearchResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询商铺列表(SearchRecommend)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                response = await _shopsManager.SearchRecommend(user.Id, condition);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询商铺列表(SearchRecommend)请求失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 根据商铺ID获取商铺详情
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpGet("{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsRetrieve" })]
        public async Task<ResponseMessage<ShopInfoResponse>> GetShops(UserInfo user, [FromRoute] string shopsId)
        {
            ResponseMessage<ShopInfoResponse> response = new ResponseMessage<ShopInfoResponse>();
            if (string.IsNullOrEmpty(shopsId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入值不符合规则";
                return response;
            }
            try
            {
                response.Extension = await _shopsManager.FindByIdAsync(user.Id, shopsId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据商铺ID获取商铺详情(GetShops)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
            }
            return response;
        }

        //[HttpPost]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsCreate" })]
        //public async Task<ResponseMessage<ShopInfoResponse>> PostShops(UserInfo user, [FromBody]ShopsRequest shopsRequest)
        //{
        //    ResponseMessage<ShopInfoResponse> response = new ResponseMessage<ShopInfoResponse>();
        //    if (!ModelState.IsValid)
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    response.Extension = await _shopsManager.CreateAsync(user, shopsRequest, HttpContext.RequestAborted);
        //    return response;
        //}

        /// <summary>
        /// 修改商铺信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsId"></param>
        /// <param name="shopsRequest"></param>
        /// <returns></returns>
        [HttpPut("{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsUpdate" })]
        public async Task<ResponseMessage> PutShops(UserInfo user, [FromRoute] string shopsId, [FromBody] ShopsRequest shopsRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺信息(PutShops)：\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}\r\n" + (shopsRequest != null ? JsonHelper.ToJson(shopsRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || shopsRequest.Id != shopsId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺信息(PutShops)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}\r\n" + (shopsRequest != null ? JsonHelper.ToJson(shopsRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _shopsManager.FindByIdAsync(user.Id, shopsId, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _shopsManager.CreateAsync(user, shopsRequest, HttpContext.RequestAborted);
                }
                await _shopsManager.UpdateAsync(user.Id, shopsId, shopsRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺信息(PutShops)请求失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}\r\n" + (shopsRequest != null ? JsonHelper.ToJson(shopsRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 修改销售状态
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="PutSaleStatus"></param>
        /// <returns></returns>
        [HttpPut("updateshopssalestatus")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsSaleStatusUpdate" })]
        public async Task<ResponseMessage<bool>> PutShopSaleStatus(string userid, [FromBody]SaleShopsStatusRquest PutSaleStatus)
        {
            Logger.Trace($"用户{userid ?? ""}修改销售状态(PutShopSaleStatus)：\r\n请求参数为：\r\n" + (PutSaleStatus != null ? JsonHelper.ToJson(PutSaleStatus) : ""));

            ResponseMessage<bool> response = new ResponseMessage<bool>();
            if (!ModelState.IsValid)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{userid ?? ""}修改销售状态(PutShopSaleStatus)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (PutSaleStatus != null ? JsonHelper.ToJson(PutSaleStatus) : ""));
                return response;
            }
            try
            {
                response.Extension = await _shopsManager.UpdateSaleStatusAsync(userid, PutSaleStatus);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{userid ?? ""}修改销售状态(PutShopSaleStatus)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (PutSaleStatus != null ? JsonHelper.ToJson(PutSaleStatus) : ""));
            }
            return response;
        }

        /// <summary>
        /// 销售统计
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [HttpGet("salestatistics/{buildingid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsSaleStatistics" })]
        public async Task<ResponseMessage<List<SaleStatisticsResponse>>> ShopsSaleStatistics(UserInfo user, [FromRoute]string buildingid)
        {
            ResponseMessage<List<SaleStatisticsResponse>> response = new ResponseMessage<List<SaleStatisticsResponse>>();
            if (string.IsNullOrEmpty(buildingid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入值不符合规则";
                return response;
            }
            try
            {
                response.Extension = await _shopsManager.SaleStatistics(buildingid);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})销售统计(ShopsSaleStatistics)报错：\r\n{e.Message}，\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 修改商铺总结
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="shopsId"></param>
        /// <param name="shopsRequest"></param>
        /// <returns></returns>
        [HttpPut("summary/{buildingId}/{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsUpdate" })]
        public async Task<ResponseMessage> PutShops(UserInfo user, [FromRoute]string buildingId, [FromRoute] string shopsId, [FromBody] ShopsRequest shopsRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺总结(PutShops)：\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}\r\n(shopsId){shopsId ?? ""}\r\n" + (shopsRequest != null ? JsonHelper.ToJson(shopsRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || shopsRequest.Id != shopsId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺总结(PutShops)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}\r\n(shopsId){shopsId ?? ""}\r\n" + (shopsRequest != null ? JsonHelper.ToJson(shopsRequest) : ""));
                return response;
            }
            try
            {
                await _shopsManager.SaveSummaryAsync(user, buildingId, shopsId, shopsRequest.Summary, shopsRequest.Source, shopsRequest.SourceId, HttpContext.RequestAborted);
                //var dictionaryGroup = await _shopsManager.FindByIdAsync(shopsId, HttpContext.RequestAborted);
                //if (dictionaryGroup == null)
                //{
                //    await _shopsManager.CreateAsync(userId, shopsRequest, HttpContext.RequestAborted);
                //}
                //await _shopsManager.UpdateAsync(userId, shopsId, shopsRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺总结(PutShops)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}\r\n(shopsId){shopsId ?? ""}\r\n" + (shopsRequest != null ? JsonHelper.ToJson(shopsRequest) : ""));
            }
            return response;
        }

        ////提交审核
        //[HttpPost("audit/submit/{shopsId}")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsUpdate" })]
        //public async Task<ResponseMessage<ExamineStatusEnum>> SubmitBuilding(UserInfo User, [FromRoute] string shopsId)
        //{
        //    ResponseMessage<ExamineStatusEnum> response = new ResponseMessage<ExamineStatusEnum>();
        //    if (!ModelState.IsValid)
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        response.Message = ModelState.GetAllErrors();
        //        Logger.Warn("模型验证失败：\r\n{0}", response.Message ?? "");
        //        return response;
        //    }
        //    try
        //    {
        //        //目前直接审核成功
        //        await _shopsManager.SubmitAsync(User, shopsId, Dto.ExamineStatusEnum.Approved, HttpContext.RequestAborted);
        //        response.Extension = ExamineStatusEnum.Approved;
        //        //var dictionaryGroup = await _buildingsManager.FindByIdAsync(buildingId, HttpContext.RequestAborted);
        //        //if (dictionaryGroup == null)
        //        //{
        //        //    await _buildingsManager.CreateAsync(userId, buildingRequest, HttpContext.RequestAborted);
        //        //}
        //        //await _buildingsManager.UpdateAsync(userId, buildingId, buildingRequest, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //        Logger.Error("提交审核失败：\r\n{0}", e.ToString());
        //    }
        //    return response;
        //}


        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpPost("audit/submit/{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { })]
        public async Task<ResponseMessage<Models.ExamineStatusEnum>> SubmitShops(UserInfo user, [FromRoute] string shopsId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})提交审核(SubmitShops)：\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");

            ResponseMessage<Models.ExamineStatusEnum> response = new ResponseMessage<Models.ExamineStatusEnum>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})提交审核(SubmitShops)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
                return response;
            }
            try
            {
                var shops = await _shopsManager.FindByIdAsync(user.Id, shopsId);
                if (shops == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "提交审核的商铺不存在：" + shopsId;
                    return response;
                }
                GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                examineSubmitRequest.ContentId = shopsId;
                examineSubmitRequest.ContentType = "shops";
                examineSubmitRequest.ContentName = shops.BasicInfo.Name;
                examineSubmitRequest.Source = "";
                examineSubmitRequest.CallbackUrl = "使用http时再启用";
                if (await _permissionExpansionManager.HavePermission(user.Id, "ShopsCreateQuick"))
                {
                    examineSubmitRequest.Action = "ShopsExaminePass";
                }
                else
                {
                    examineSubmitRequest.Action = "ShopsExamine";
                }
                examineSubmitRequest.TaskName = user.UserName + "提交的商铺：" + shopsId;
                examineSubmitRequest.Ext1 = shops.BasicInfo.BuildingNo;//楼栋
                examineSubmitRequest.Ext2 = shops.BasicInfo.FloorNo;//楼层
                examineSubmitRequest.Ext3 = shops.BasicInfo.Number;//商铺编号
                examineSubmitRequest.Ext4 = shops.Buildings.BasicInfo.Name;//楼盘名字

                GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                {
                    Id = user.Id,
                    KeyWord = user.KeyWord,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.OrganizationName,
                    UserName = user.UserName
                };

                //Logger.Info("nwf协议：\r\n{0}", JsonHelper.ToJson(examineSubmitRequest));
                //string result = await _restClient.Post(ApplicationContext.Current.ExamineUrl, examineSubmitRequest, "POST", new NameValueCollection());
                //Logger.Info("返回：\r\n{0}", result);
                var _examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();
                var response2 = await _examineInterface.Submit(userInfo, examineSubmitRequest);
                if (response2.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                    return response;
                }
                await _shopsManager.SubmitAsync(shopsId, Dto.ExamineStatusEnum.Auditing, HttpContext.RequestAborted);
                response.Extension = Models.ExamineStatusEnum.Auditing;
                //await _shopsManager.SubmitAsync(shopsId, Dto.ExamineStatusEnum.Approved, HttpContext.RequestAborted);
                //response.Extension = Models.ExamineStatusEnum.Approved;
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})提交审核(SubmitShops)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
            }
            return response;
        }


        //审核中心回调接口
        [HttpPost("audit/examinecallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> SubmitShopsCallback([FromBody] ExamineResponse examineResponse)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }
            try
            {
                var shops = await _shopsManager.FindByIdAsync("", examineResponse.ContentId);
                if (shops == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "商铺不存在：" + examineResponse.ContentId;
                    return response;
                }
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _shopsManager.SubmitAsync(examineResponse.ContentId, Dto.ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _shopsManager.SubmitAsync(examineResponse.ContentId, Dto.ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"审核中心回调接口(SubmitShopsCallback)报错：\r\n{e.ToString()}");

            }
            return response;
        }

        /// <summary>
        /// 删除商铺
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpDelete("{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsDelete" })]
        public async Task<ResponseMessage> DeleteShops(UserInfo user, [FromRoute] string shopsId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除商铺(DeleteShops)：\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");

            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(shopsId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入值不符合规则";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除商铺(DeleteShops)模型验证失败：\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
                return response;
            }
            try
            {
                await _shopsManager.DeleteAsync(user, shopsId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除商铺(DeleteShops)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
                return response;
            }
            return response;
        }

        /// <summary>
        /// 批量删除商铺
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsDelete" })]
        public async Task<ResponseMessage> DeleteShopss(UserInfo user, [FromBody] List<string> groupIds)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除商铺(DeleteShopss)：\r\n请求参数为：\r\n" + (groupIds != null ? JsonHelper.ToJson(groupIds) : ""));

            ResponseMessage response = new ResponseMessage();
            if (groupIds == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入值不符合规则";
                return response;
            }
            try
            {
                await _shopsManager.DeleteListAsync(user.Id, groupIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除商铺(DeleteShopss)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (groupIds != null ? JsonHelper.ToJson(groupIds) : ""));
            }
            return response;
        }

        /// <summary>
        /// 新增客源不感兴趣商铺信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="customerNotShops">增加实体</param>
        /// <returns></returns>
        [HttpPost("customernotshops")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerNotShopsCreate" })]
        public async Task<ResponseMessage<CustomerNotShops>> PostCustomerNotShops(UserInfo user, [FromBody]CustomerNotShops customerNotShops)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客源不感兴趣商铺信息(PostCustomerNotShops)：\r\n请求参数为：\r\n" + (customerNotShops != null ? JsonHelper.ToJson(customerNotShops) : ""));

            var response = new ResponseMessage<CustomerNotShops>();
            if (!ModelState.IsValid)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客源不感兴趣商铺信息(PostCustomerNotShops)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerNotShops != null ? JsonHelper.ToJson(customerNotShops) : ""));
                return response;
            }
            try
            {

                response = await _shopsManager.CreateCustomerNotShopAsync(user, customerNotShops, HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客源不感兴趣商铺信息(PostCustomerNotShops)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerNotShops != null ? JsonHelper.ToJson(customerNotShops) : ""));
            }
            return response;
        }


        /// <summary>
        /// 查询客源不感兴趣商铺信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="customerNotShops">增加实体</param>
        /// <returns></returns>
        [HttpGet("customernotshops")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerNotShopsGet" })]
        public async Task<PagingResponseMessage<CustomerNotShops>> GetCustomerNotShops(UserInfo user)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客源不感兴趣商铺信息(PostCustomerNotShops)\r\n");

            var response = new PagingResponseMessage<CustomerNotShops>();
            
            try
            {

                response = await _shopsManager.GetCustomerNotShopAsync(user, Request.HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客源不感兴趣商铺信息(PostCustomerNotShops)请求失败：\r\n{response.Message ?? ""}，\r\n" );
            }
            return response;
        }

    }
}
