using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Managers;

namespace XYHCustomerPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/customerpooldefine")]
    public class CustomerPoolDefineController : Controller
    {
        //暂时未使用
        
        //private readonly PermissionExpansionManager _permissionExpansionManager;
        //private readonly CustomerPoolDefineManager _customerPoolDefineManager;

        //public CustomerPoolDefineController(CustomerPoolDefineManager customerPoolDefineManager,
        //     PermissionExpansionManager permissionExpansionManager)
        //{
        //    _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
        //    _customerPoolDefineManager = customerPoolDefineManager ?? throw new ArgumentNullException(nameof(customerPoolDefineManager));
        //}

        //[HttpGet("{poolId}")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        //public async Task<ResponseMessage<CustomerPoolDefineResponse>> GetCustomerLossByCoustomerId(string userid, [FromRoute] string poolId)
        //{
        //    ResponseMessage<CustomerPoolDefineResponse> response = new ResponseMessage<CustomerPoolDefineResponse>();
        //    try
        //    {
        //        response.Extension = await _customerPoolDefineManager.FindByIdAsync(poolId, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}

        //[HttpPost("list")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        //public async Task<ResponseMessage<List<CustomerPoolDefineResponse>>> GetList(string userId)
        //{
        //    ResponseMessage<List<CustomerPoolDefineResponse>> response = new ResponseMessage<List<CustomerPoolDefineResponse>>();
        //    try
        //    {
        //        response.Extension = await _customerPoolDefineManager.FindAllAsync(HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}



        //[HttpPost]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        //public async Task<ResponseMessage<CustomerPoolDefineResponse>> PostDictionaryGroup(UserInfo user, [FromBody]CustomerPoolDefineRequest customerPoolDefineRequest)
        //{
        //    ResponseMessage<CustomerPoolDefineResponse> response = new ResponseMessage<CustomerPoolDefineResponse>();
        //    if (!ModelState.IsValid)
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    try
        //    {
        //        response.Extension = await _customerPoolDefineManager.CreateAsync(user, customerPoolDefineRequest, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}

        //[HttpPut("{id}")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        //public async Task<ResponseMessage> PostDictionaryGroup(string userId, [FromRoute]string id, [FromBody]CustomerPoolDefineRequest customerPoolDefineRequest)
        //{
        //    ResponseMessage response = new ResponseMessage();
        //    if (!ModelState.IsValid && customerPoolDefineRequest.Id == id)
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    try
        //    {
        //        await _customerPoolDefineManager.UpdateAsync(userId, customerPoolDefineRequest, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}


        //[HttpDelete]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        //public async Task<ResponseMessage> DeleteDictionaryGroup(string userId, [FromBody]List<string> customerIdList)
        //{
        //    ResponseMessage response = new ResponseMessage();
        //    try
        //    {
        //        await _customerPoolDefineManager.DeleteListAsync(userId, customerIdList, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //        return response;
        //    }
        //    return response;
        //}



    }
}
