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


namespace XYHHumanPlugin.Controllers
{
    //[Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humaninfo")]
   public class HumanInfoController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("Humaninfo");


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
    }
}
