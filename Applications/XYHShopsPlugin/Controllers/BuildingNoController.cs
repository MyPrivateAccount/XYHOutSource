using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Managers;

namespace XYHShopsPlugin.Controllers
{
    /// <summary>
    /// 楼栋批次
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildingNo")]
    public class BuildingNoController : Controller
    {
        #region 成员

        private readonly BuildingNoManager _buildingNoManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingNo");

        #endregion

        /// <summary>
        /// 楼盘批次信息
        /// </summary>
        public BuildingNoController(BuildingNoManager buildingNoManager, IMapper mapper)
        {
            _buildingNoManager = buildingNoManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 根据楼盘Id查询信息
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpGet("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingNoById" })]
        public async Task<ResponseMessage<List<BuildingNoCreateResponse>>> GetBuildingNo(UserInfo user, [FromRoute] string buildingId)
        {
            var response = new ResponseMessage<List<BuildingNoCreateResponse>>();
            if (string.IsNullOrEmpty(buildingId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "参数不能为空";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据楼盘Id查询信息(GetBuildingNo)模型验证失败：参数不能空,\r\n请求的参数为：\r\n");
                return response;
            }
            try
            {
                response.Extension = await _buildingNoManager.FindBuildingIdAsync(buildingId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据楼盘Id查询信息(GetBuildingNo)报错：{e.ToString()},\r\n请求的参数为：\r\n(buildingId){buildingId}");
            }
            return response;
        }

        /// <summary>
        /// 修改楼盘批次信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="buildingNoRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut("{BuildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingNoUpdate" })]
        public async Task<ResponseMessage> PutBuildingNo(UserInfo user, [FromRoute]string BuildingId, [FromBody]List<BuildingNoCreateRequest> buildingNoRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘批次信息(PutBuildingNo)：\r\n请求的参数为：\r\n" + (buildingNoRequest != null ? JsonHelper.ToJson(buildingNoRequest) : ""));

            var response = new ResponseMessage();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘批次信息(PutBuildingNo)模型验证失败：\r\n{error},\r\n请求的参数为：\r\n" + (buildingNoRequest != null ? JsonHelper.ToJson(buildingNoRequest) : ""));
                return response;
            }
            try
            {
                response = await _buildingNoManager.UpdateAsync(user, BuildingId, buildingNoRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘批次信息(PutBuildingNo)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (buildingNoRequest != null ? JsonHelper.ToJson(buildingNoRequest) : ""));
            }
            return response;
        }

    }
}
