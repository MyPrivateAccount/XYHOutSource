using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationCenter.Models
{
    public class UserToken : IdentityUserToken<string>
    {
    }
}
