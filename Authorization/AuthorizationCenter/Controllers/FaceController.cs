using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;
using Microsoft.Extensions.Configuration;
using AuthorizationCenter.Dto;
using Newtonsoft.Json.Linq;
using AuthorizationCenter.Helpers;
using AuthorizationCenter.Filters;
using AuthorizationCenter.Interface;
using AuthorizationCenter.Models;

namespace AuthorizationCenter.Controllers
{
    //人脸识别相关
    
    [Produces("application/json")]
    [Route("api/user/face")]
    public class FaceController : Controller
    {
        private static readonly string ENABLE_FACE_LOGIN = nameof(ENABLE_FACE_LOGIN);
        private static readonly string FACE_REGISTERED = nameof(FACE_REGISTERED);
        private static readonly string ENABLE_FACE_SOUND_TIP = nameof(ENABLE_FACE_SOUND_TIP);

        private readonly string serviceApiUrl = "";
        private RestClient restClient = null;
        private IUserExtensionsManager userExtensionsManager = null;
        private readonly ApplicationDbContext _dbContext = null;
        private XYH.Core.Log.ILogger Logger = XYH.Core.Log.LoggerManager.GetLogger("FaceController");
        
        public FaceController(IConfigurationRoot config, IUserExtensionsManager uem, ApplicationDbContext dbContext)
        {
            serviceApiUrl = config["ServiceApi"];
            restClient = new RestClient(serviceApiUrl);
            userExtensionsManager = uem;
            _dbContext = dbContext;
        }


        //人脸注册
        [HttpPost("register")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission))]
        public async Task<ResponseMessage<BDFaceResponseBase>> FaceRegister(ClaimsUserInfo ClaimsUserInfo, [FromBody]BDFaceRegisterRequest request, [FromQuery]bool isFirst=false)
        {
            ResponseMessage<BDFaceResponseBase> r = new ResponseMessage<BDFaceResponseBase>();
            try
            {
                request.user_info = ClaimsUserInfo.Id;
                request.uid = ClaimsUserInfo.UserName;
                r = await restClient.Post<ResponseMessage<BDFaceResponseBase>>("/baidu/face", request);
                if(r.IsSuccess())
                {
                    //设置参数
                    if (isFirst)
                    {
                        List<UserExtensionsRequest> pars = new List<UserExtensionsRequest>();
                        pars.Add(new UserExtensionsRequest()
                        {
                            ParName = ENABLE_FACE_LOGIN,
                            ParValue = "1"
                        });
                        pars.Add(new UserExtensionsRequest()
                        {
                            ParName = ENABLE_FACE_SOUND_TIP,
                            ParValue = "1"
                        });
                        pars.Add(new UserExtensionsRequest()
                        {
                            ParName = FACE_REGISTERED,
                            ParValue = "1"
                        });
                        await userExtensionsManager.SaveUserExtensions(ClaimsUserInfo, pars, HttpContext.RequestAborted);
                    }

                }
            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("注册用户人脸失败：\r\n{0}", e.ToString());
            }

            return r;
        }

        //更新人脸
        [HttpPost("update")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission))]
        public async Task<ResponseMessage<BDFaceResponseBase>> FaceUpdate(ClaimsUserInfo ClaimsUserInfo, [FromBody]BDFaceRegisterRequest request)
        {
            ResponseMessage<BDFaceResponseBase> r = new ResponseMessage<BDFaceResponseBase>();
            try
            {
                request.user_info = ClaimsUserInfo.Id;
                request.uid = ClaimsUserInfo.UserName;
                r = await restClient.Post<ResponseMessage<BDFaceResponseBase>>("/baidu/face", request, "PUT");
               
            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("更新用户人脸失败：\r\n{0}", e.ToString());
            }

            return r;
        }


        //取消刷脸登录
        [HttpPost("switch")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission))]
        public async Task<ResponseMessage<BDFaceResponseBase>> SwitchFaceLogin(ClaimsUserInfo ClaimsUserInfo, [FromQuery]bool enable)
        {
            ResponseMessage<BDFaceResponseBase> r = new ResponseMessage<BDFaceResponseBase>();
            try
            {

                List<UserExtensionsRequest> pars = new List<UserExtensionsRequest>();
                pars.Add(new UserExtensionsRequest()
                {
                    ParName = ENABLE_FACE_LOGIN,
                    ParValue = enable ? "1" : "0"
                });

                await userExtensionsManager.SaveUserExtensions(ClaimsUserInfo, pars, HttpContext.RequestAborted);



            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("开启用户刷脸登录失败：\r\n{0}", e.ToString());
            }

            return r;
        }


        //人脸认证
        [HttpPost("verify")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission))]
        public async Task<ResponseMessage<BDFaceVerifyResponse>> FaceVerify(ClaimsUserInfo ClaimsUserInfo, [FromBody]BDFaceVerifyRequest request)
        {
            ResponseMessage<BDFaceVerifyResponse> r = new ResponseMessage<BDFaceVerifyResponse>();
            try
            {
                request.uid = ClaimsUserInfo.UserName;
                request.topNum = 1;
                
                r = await restClient.Post<ResponseMessage<BDFaceVerifyResponse>>("/baidu/face/verify", request);

            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("更新用户人脸失败：\r\n{0}", e.ToString());
            }

            return r;
        }


        [HttpGet("par")]
        public async Task<ResponseMessage<List<UserExtensionsResponse>>> GetFaceLoginParameter(string username)
        {
            ResponseMessage<List<UserExtensionsResponse>> r = new ResponseMessage<List<UserExtensionsResponse>>();
            try
            {
               var user = _dbContext.Users.Where(x => x.UserName.ToLower() == username.ToLower()).Select(x => new Users() { Id = x.Id, UserName = x.UserName }).FirstOrDefault();
                if(user!=null)
                {
                    r.Extension = await userExtensionsManager.GetUserExtensions(new ClaimsUserInfo()
                    {
                        UserName = user.UserName,
                        Id = user.Id
                    }, new List<string>() { ENABLE_FACE_LOGIN, ENABLE_FACE_SOUND_TIP, FACE_REGISTERED }, HttpContext.RequestAborted);
                }
               
            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("获取用户人脸登录参数失败：\r\n{0}", e.ToString());
            }

            return r;
        }


    }
}