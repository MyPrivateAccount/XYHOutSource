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

        [HttpPost("listcontract")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ContractSearchResponse<ContractInfoResponse>> SearchContractList(UserInfo user, [FromBody]ContractSearchRequest condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询合同条件(condition)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new ContractSearchResponse<ContractInfoResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询合同条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "SEARCH_CONTRACT"))
                {
                    pagingResponse = await _contractListManager.SearchContract(user, condition, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                    pagingResponse.Message = "权限不足";
                }

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return pagingResponse;
        }
    }
}
