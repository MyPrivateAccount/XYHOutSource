using AuthorizationCenter.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Interface
{
    public interface IUserExtensionsManager
    {
        Task<bool> SaveUserExtensions(ClaimsUserInfo ClaimsUserInfo, List<UserExtensionsRequest> request, CancellationToken cancellationToken);


        Task<List<UserExtensionsResponse>> GetUserExtensions(ClaimsUserInfo ClaimsUserInfo,List<string> parNames,  CancellationToken cancellationToken);

        Task<List<UserExtensionsResponse>> GetUserExtensions(ClaimsUserInfo ClaimsUserInfo,string parName, List<string> userIds, CancellationToken cancellationToken);


        Task<bool> DeleteUserExtensions(ClaimsUserInfo ClaimsUserInfo, List<string> parNames, CancellationToken cancellationToken);
    }
}
