using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Managers;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humanattendance")]
    public class AttendanceController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumanattendance");
        private readonly BlackManager _blackManage;
        public AttendanceController(BlackManager sta)
        {
            _blackManage = sta;
        }
    }
}
