using ApplicationCore;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XYH.Core.Log;
using XYHHumanPlugin.Managers;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/rewardpunishment")]
    public class RewardPunishmentController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumanrewardpunishment");
        private readonly RewardPunishmentManager _rpManage;
        public RewardPunishmentController( RewardPunishmentManager sta)
        {
            _rpManage = sta;
        }


    }
}
