using AspNet.Security.OAuth.Validation;
using AspNet.Security.OpenIdConnect.Primitives;
using AuthorizationCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OpenIddict.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using AuthorizationCenter.Filters;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Dto;
using AutoMapper;
using AuthorizationCenter.Dto.Response;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/user")]
    public class UserinfoController : Controller
    {
        private readonly ExtendUserManager<Users> _extendUserManager;
        private readonly ApplicationDbContext _context;
        private readonly OrganizationsManager _organizationsManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly IMapper _mapper;


        public UserinfoController(ApplicationDbContext context,
            OrganizationsManager organizationsManager,
            ExtendUserManager<Users> extendUserManager,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            _context = context;
            _organizationsManager = organizationsManager;
            _extendUserManager = extendUserManager;
            _permissionExpansionManager = permissionExpansionManager;
            _mapper = mapper;
        }

        //
        // GET: /api/userinfo
        [Route("~/api/userinfo")]
        [HttpGet, Produces("application/json")]
        public async Task<IActionResult> Userinfo()
        {
            var user = await _extendUserManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest(new OpenIdConnectResponse
                {
                    Error = OpenIdConnectConstants.Errors.InvalidGrant,
                    ErrorDescription = "The user profile is no longer available."
                });
            }

            var claims = new JObject();

            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            claims[OpenIdConnectConstants.Claims.Subject] = await _extendUserManager.GetUserIdAsync(user);
            claims[OpenIdConnectConstants.Claims.Name] = await _extendUserManager.GetUserNameAsync(user);
            claims[OpenIdConnectConstants.Claims.Picture] = user.Avatar;
            claims[OpenIdConnectConstants.Claims.Nickname] = user.TrueName;
            claims["Organization"] = user.OrganizationId;
            claims["City"] = _organizationsManager.FindByIdAsync(user.FilialeId, HttpContext.RequestAborted)?.Result?.City ?? "";

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Email))
            {
                claims[OpenIdConnectConstants.Claims.Email] = await _extendUserManager.GetEmailAsync(user);
                claims[OpenIdConnectConstants.Claims.EmailVerified] = await _extendUserManager.IsEmailConfirmedAsync(user);
            }

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIdConnectConstants.Scopes.Phone))
            {
                claims[OpenIdConnectConstants.Claims.PhoneNumber] = await _extendUserManager.GetPhoneNumberAsync(user);
                claims[OpenIdConnectConstants.Claims.PhoneNumberVerified] = await _extendUserManager.IsPhoneNumberConfirmedAsync(user);
            }

            if (User.HasClaim(OpenIdConnectConstants.Claims.Scope, OpenIddictConstants.Scopes.Roles))
            {
                claims[OpenIddictConstants.Claims.Roles] = JArray.FromObject(await _extendUserManager.GetRolesAsync(user));
            }

            // Note: the complete list of standard claims supported by the OpenID Connect specification
            // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

            return Json(claims);
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoRetrieve" })]
        public async Task<ResponseMessage<UserInfoResponse>> Get(string userId, [FromRoute]string id)
        {
            var r = new ResponseMessage<UserInfoResponse>();
            r.Extension = await _extendUserManager.FindByIdAsync(userId, HttpContext.RequestAborted);
            if (r.Extension == null)
            {
                r.Code = ResponseCodeDefines.NotFound;
                return r;
            }
            if (!await _permissionExpansionManager.HavePermission(userId, "UserInfoRetrieve", r.Extension.OrganizationId))
            {
                r.Code = ResponseCodeDefines.NotAllow;
                return r;
            }
            return r;
        }

        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoRetrieve" })]
        public async Task<PagingResponseMessage<UserInfoResponse>> GetList(string userId, [FromBody]UserSearchCondition condition)
        {
            PagingResponseMessage<UserInfoResponse> pagingResponse = new PagingResponseMessage<UserInfoResponse>();

            var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "UserInfoRetrieve");
            if (organizationIds?.Count == 0)
            {
                pagingResponse.Code = ResponseCodeDefines.NotAllow;
                pagingResponse.Message = "没有权限";
                return pagingResponse;
            }
            if (condition == null)
            {
                pagingResponse.Code = ResponseCodeDefines.ArgumentNullError;
                return pagingResponse;
            }
            if (condition.OrganizationIds?.Count > 0)
            {
                condition.OrganizationIds = condition.OrganizationIds.Where(a => organizationIds.Contains(a)).ToList();
            }
            return await _extendUserManager.Search(condition, HttpContext.RequestAborted);
        }

        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoCreate" })]
        public async Task<ResponseMessage> Post(string userId, [FromBody]UserInfoRequest model)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            //权限控制
            if (!await _permissionExpansionManager.HavePermission(userId, "UserInfoCreate", model.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var user = _mapper.Map<Users>(model);

            user.FilialeId = await _organizationsManager.FindFilialeIdAsync(user.OrganizationId);
            if (string.IsNullOrEmpty(user.FilialeId))
            {
                user.FilialeId = user.OrganizationId;
            }
            user.IsDeleted = false;
            var result = await _extendUserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return response;
            }
            response.Code = ResponseCodeDefines.ServiceError;
            return response;
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoUpdate" })]
        public async Task<ResponseMessage> Put(string userId, [FromRoute]string id, [FromBody]UserInfoRequest userInfoRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            var user = await _extendUserManager.FindByIdAsync(id);
            if (user == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            //权限控制
            if (!await _permissionExpansionManager.HavePermission(userId, "UserInfoUpdate", user.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            user.Avatar = userInfoRequest.Avatar;
            user.Email = userInfoRequest.Email;
            user.FilialeId = userInfoRequest.FilialeId;
            user.OrganizationId = userInfoRequest.OrganizationId;
            user.PhoneNumber = userInfoRequest.PhoneNumber;
            user.Position = userInfoRequest.Position;
            user.TrueName = userInfoRequest.TrueName;
            user.UserName = userInfoRequest.UserName;
            var result = await _extendUserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return response;
            }
            response.Code = ResponseCodeDefines.ServiceError;
            return response;
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoDelete" })]
        public async Task<ResponseMessage> Delete(string userId, [FromRoute]string id)
        {
            ResponseMessage response = new ResponseMessage();

            var user = await _extendUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            if (!await _permissionExpansionManager.HavePermission(userId, "UserInfoDelete", user.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }

            user.IsDeleted = true;
            user.TrueName = user.TrueName + "(已删除)";
            var result = await _extendUserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _permissionExpansionManager.RemoveUserAsync(userId);
                return response;
            }
            response.Code = ResponseCodeDefines.ServiceError;
            return response;
        }

        [HttpPost("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoDelete" })]
        public async Task<ResponseMessage> DeleteList(string userId, [FromBody]List<string> userIds)
        {
            ResponseMessage response = new ResponseMessage();
            if (userIds == null || userIds.Count == 0)
            {
                return response;
            }
            var uIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "UserInfoRetrieve");
            if (uIds == null || uIds.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            foreach (var item in userIds)
            {
                var user = await _extendUserManager.FindByIdAsync(item);
                if (user != null)
                {
                    if (!uIds.Contains(user.OrganizationId))
                    {
                        continue;
                    }
                    user.IsDeleted = true;
                    user.TrueName = user.TrueName + "(已删除)";
                    var result = await _extendUserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        await _permissionExpansionManager.RemoveUserAsync(userId);
                    }
                    //_extendUserManager.DeleteAsync(user).Wait();
                    //await _permissionExpansionManager.RemoveUserAsync(item);
                }
            }
            //await _extendUserManager.DeleteListAsync(userIds, HttpContext.RequestAborted);
            return response;
        }

        [HttpPost("initpassword")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "InitUserPassword" })]
        public async Task<ResponseMessage> InitPassword(string userId, [FromBody]InitUserPasswordRequest initUserPasswordRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            var user = await _extendUserManager.FindByNameAsync(initUserPasswordRequest.UserName);
            if (user == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "用户未找到";
                return response;
            }
            if (!await _permissionExpansionManager.HavePermission(userId, "InitUserPassword", user.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var password = _extendUserManager.PasswordHasher.HashPassword(user, initUserPasswordRequest.Password);
            user.PasswordHash = password;
            var result = await _extendUserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            return response;
        }



        [HttpPost("resetpassword")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserResetPassword" })]
        public async Task<ResponseMessage> ResetPassword(string userId, [FromBody]ResetPasswordRequest resetPasswordRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            var user = await _extendUserManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "用户未找到";
                return response;
            }
            //if (!await _permissionExpansionManager.HavePermission(userId, "UserResetPassword", user.OrganizationId))
            //{
            //    response.Code = ResponseCodeDefines.NotAllow;
            //    return response;
            //}
            //var uIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "UserInfoUpdate");
            //if (uIds == null || uIds.Count == 0 || !uIds.Contains(user.OrganizationId))
            //{
            //    response.Code = ResponseCodeDefines.NotAllow;
            //    return response;
            //}

            if (!await _extendUserManager.CheckPasswordAsync(user, resetPasswordRequest.OldPassword))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "原密码不正确";
                return response;
            }
            var result = await _extendUserManager.ChangePasswordAsync(user, resetPasswordRequest.OldPassword, resetPasswordRequest.Password);
            if (!result.Succeeded)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
            return response;
        }

        [AllowAnonymous]
        [HttpGet("~/api/userinfo/openidexist/{id}")]
        public async Task<ResponseMessage<bool>> GetOpenId([FromRoute]string id)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();

            var users = await _context.Users.FirstOrDefaultAsync(a => a.WXOpenId == id);
            if (users == null)
            {
                response.Extension = false;
                return response;
            }
            response.Extension = true;
            return response;
        }


        [HttpPut("~/api/userinfo/openid/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoUpdate" })]
        public async Task<ResponseMessage> UpdateOpenId([FromRoute]string id, [FromBody]string avatar)
        {
            ResponseMessage response = new ResponseMessage();
            var users = await _extendUserManager.GetUserAsync(User);
            users.Avatar = avatar;
            users.WXOpenId = id;
            try
            {
                await _extendUserManager.UpdateAsync(users);
            }
            catch
            {
                response.Code = ResponseCodeDefines.ServiceError;
            }
            return response;
        }

        [HttpDelete("~/api/userinfo/openid")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoUpdate" })]
        public async Task<ResponseMessage> ClearOpenId()
        {
            ResponseMessage response = new ResponseMessage();
            var users = await _extendUserManager.GetUserAsync(User);
            users.WXOpenId = null;
            try
            {
                await _extendUserManager.UpdateAsync(users);
            }
            catch
            {
                response.Code = ResponseCodeDefines.ServiceError;
            }
            return response;
        }

        [HttpPost("~/api/userinfo/openid/list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { })]
        public async Task<ResponseMessage<List<OpenIDResponse>>> GetOpenIDs(ClaimsUserInfo ClaimsUserInfo, [FromBody]List<string> userIds)
        {
            ResponseMessage<List<OpenIDResponse>> r = new ResponseMessage<List<OpenIDResponse>>();
            if (ClaimsUserInfo.grant_type != "client_credentials")
            {
                r.Code = "401";
                r.Message = "仅允许内部应用访问";
                return r;
            }

            try
            {
                var q = from u in _context.Users.AsNoTracking()
                        select new OpenIDResponse()
                        {
                            UserID = u.Id,
                            OpenID = u.WXOpenId
                        };
                if (userIds != null && userIds.Count > 0)
                {
                    q = q.Where(u => userIds.Contains(u.UserID));
                }

                r.Extension = await q.ToListAsync();
            }
            catch (Exception e)
            {
                r.Code = "1";
                r.Message = e.Message;
            }
            return r;
        }



    }
}
