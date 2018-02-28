using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopPortal.Controllers
{
    public class CheckController : Controller
    {
        [HttpHead]
        public IActionResult Index()
        {
            return Content("OK");
        }
    }
}
