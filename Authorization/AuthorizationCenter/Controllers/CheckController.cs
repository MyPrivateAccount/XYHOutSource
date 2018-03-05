using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationCenter.Controllers
{
    [Route("[controller]")]
    public class CheckController : Controller
    {
        [HttpHead]
        public IActionResult Index()
        {
            return Content("OK");
        }
    }
}