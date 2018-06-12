using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Models;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Controllers
{

    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/assistant")]
    public class AssistantController : Controller
    {
        private readonly PublicDataExecute _publicDataExecute;
        public AssistantController(PublicDataExecute publicDataExecute)
        {
            _publicDataExecute = publicDataExecute;
        }

        [HttpGet("getmywork/{date}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AssistantList" })]
        public async Task<ResponseMessage<List<GetMyWorkResponse>>> GetMyWork(UserInfo user, [FromRoute] DateTime date)
        {
            ResponseMessage<List<GetMyWorkResponse>> response = new ResponseMessage<List<GetMyWorkResponse>>();
            try
            {
                return await _publicDataExecute.GetMyWork(user, date, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
            }
            return response;
        }

        [HttpPost("getmyworktimes")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AssistantList" })]
        public async Task<ResponseMessage<List<GetMyWorkTimesResponse>>> GetMyWork(UserInfo user, [FromBody] List<DateTime> dates)
        {
            ResponseMessage<List<GetMyWorkTimesResponse>> response = new ResponseMessage<List<GetMyWorkTimesResponse>>();
            if (dates == null || dates.Count == 0)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入数据为空";
                return response;
            }
            try
            {
                return await _publicDataExecute.GetMyWorksTimes(user, dates, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
            }
            return response;
        }
    }
}
