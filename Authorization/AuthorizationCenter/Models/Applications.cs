using AuthorizationCenter.Stores;
using OpenIddict.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class Applications : OpenIddictApplication<string, Authorization, Token>
    {
        [MaxLength(255)]
        public string ApplicationType { get; set; }
    }

    public class Token : OpenIddictToken<string, Applications, Authorization>
    {

    }

    public class Authorization : OpenIddictAuthorization<string, Applications, Token>
    {

    }
}
