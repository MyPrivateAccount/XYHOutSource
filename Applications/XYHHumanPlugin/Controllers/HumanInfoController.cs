using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNet.Security.OAuth.Validation;
using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using XYH.Core.Log;
using XYHHumanPlugin.Dto.Request;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humaninfo")]
   public class HumanInfoController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("Humaninfo");
        private readonly IHumanManageStore _humanManage;
        private string _lastDate;
        private int  _lastNumber;

        HumanInfoController(IHumanManageStore human)
        {
            _humanManage = human;
            _lastNumber = 0;
        }

        [HttpGet("testinfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<int>>> GetTestInfo()//[FromRoute]string testinfo
        {
            var Response = new ResponseMessage<List<int>>();
            //if (string.IsNullOrEmpty(testinfo))
            //{
            //    Response.Code = ResponseCodeDefines.ModelStateInvalid;
            //    Response.Message = "请求参数不正确";
            //}
            try
            {
                //Response.Extension = await _userTypeValueManager.FindByTypeAsync(user.Id, type, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("humaninfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<HumanInfoResponse>>> SearchHumanInfo(UserInfo User, [FromBody]ContractSearchRequest condition)
        {
            var Response = new ResponseMessage<List<HumanInfoResponse>>();
            try
            {
                //await _contractInfoManager.DiscardAsync(user, contractId, HttpContext.RequestAborted);
                Response.Message = $"searchhumaninfo sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpGet("jobnumber")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<string>> GetJobNumber(UserInfo User)
        {
            var Response = new ResponseMessage<string>();
            try
            {
                var td = DateTime.Now;
                if (td.Month.ToString()+td.Day.ToString() == _lastDate)
                {
                    _lastNumber++;
                }
                else 
                {
                    _lastNumber = 0;
                }
                Response = $"XYH-{td.Year.ToString()}{td.Month.ToString()}{td.Day.ToString()}-{_lastNumber}"
                Response.Message = $"getjobnumber {contractId} sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }
    }
}
