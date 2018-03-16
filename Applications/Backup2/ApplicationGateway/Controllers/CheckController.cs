using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ApplicationGateway.Controllers
{
    [Route("[controller]")]
    public class CheckController : Controller
    {
        // GET api/values
        [HttpGet]
        [HttpHead]
        public string Get()
        {
            return "OK";
        }

      
    }
}
