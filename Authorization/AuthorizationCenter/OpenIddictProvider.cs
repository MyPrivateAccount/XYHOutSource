using AuthorizationCenter.Models;
using Microsoft.Extensions.Logging;
using OpenIddict.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.Extensions.Configuration;

namespace AuthorizationCenter
{
    /// <summary>
    /// 支持配置多个Issuer
    /// </summary>
    public class OpenIddictProvider : OpenIddict.OpenIddictProvider<Applications, Authorization, OpenIddict.Models.OpenIddictScope, Token>
    {
        private List<String> _validIssuers = null;

        public OpenIddictProvider(
            ILogger<OpenIddictProvider> logger,
            OpenIddictApplicationManager<Applications> applications,
            OpenIddictAuthorizationManager<Authorization> authorizations,
            OpenIddictScopeManager<OpenIddict.Models.OpenIddictScope> scopes,
            OpenIddictTokenManager<Token> tokens,
            IConfigurationRoot config) :base(logger, applications, authorizations, scopes, tokens)
        {
            // config.GetSection("ValidIssuers").getva
            _validIssuers = config.GetSection("ValidIssuers")?.Get<List<String>>();
        }

        public override Task DeserializeAccessToken(DeserializeAccessTokenContext context)
        {
            if(_validIssuers!=null && _validIssuers.Count > 0)
            {
                context.TokenValidationParameters.ValidIssuers = _validIssuers.ToList();
            }
            return base.DeserializeAccessToken(context);
        }
    }
}
