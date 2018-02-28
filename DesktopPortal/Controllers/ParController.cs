using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopPortal.Controllers
{
    public class ParController : Controller
    {
        private IConfiguration config = null;
        public ParController(IConfiguration config)
        {
            this.config = config;
        }

        [HttpGet]
        public ActionResult Index()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"var _authUrl = '{this.config["AuthUrl"]}'");
            sb.AppendLine($"var _appRoot = '{Url.Content("~/")}'");
            sb.AppendLine($"var _basicDataUrl = '{this.config["BasicDataUrl"]}'");//基础数据接口地址
            sb.AppendLine($"var _flowChartUrl='{this.config["FlowChartUrl"]}'");
            sb.AppendLine($"var _uploadUrl='{this.config["UploadUrl"]}'");
            return Content(sb.ToString(), "application/javascript");
        }
    }
}
