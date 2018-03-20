using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using XYHContractPlugin.Managers;
using XYHContractPlugin.Dto.Request;
using System.Threading.Tasks;
using XYHContractPlugin.Dto.Response;
using ApplicationCore.Dto;
using Microsoft.Extensions.Logging;
using XYH.Core.Log;
using ApplicationCore;
using ApplicationCore.Managers;

namespace XYHContractPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/contractlist")]
    public class ContractListController : Controller
    {
        private readonly ContractListManager _contractListManager;
        private readonly XYH.Core.Log.ILogger Logger = LoggerManager.GetLogger("ContractListInfo");
        protected PermissionExpansionManager _permissionExpansionManager { get; }

        public ContractListController(ContractListManager listmanager, PermissionExpansionManager permissionExpansionManager)
        {
            _contractListManager = listmanager;
            _permissionExpansionManager = permissionExpansionManager;
        }

        [HttpPost("searchcontract")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ContractSearchResponse<ContractInfoResponse>> SearchContractList(UserInfo User, [FromBody]ContractSearchRequest condition)
        {
            if (User.Id == null)
            {
                {
                    User.Id = "66df64cb-67c5-4645-904f-704ff92b3e81";
                    User.UserName = "wqtest";
                    User.KeyWord = "";
                    User.OrganizationId = "270";
                    User.PhoneNumber = "18122132334";
                };
            }

            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询合同条件(condition)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new ContractSearchResponse<ContractInfoResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询合同条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                //if (await _permissionExpansionManager.HavePermission(User.Id, "SEARCH_CONTRACT"))
                {
                    pagingResponse = await _contractListManager.SearchContract(User, condition, HttpContext.RequestAborted);
                }
                //else
                //{
                //    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                //    pagingResponse.Message = "权限不足";
                //}

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return pagingResponse;
        }
    }
}
