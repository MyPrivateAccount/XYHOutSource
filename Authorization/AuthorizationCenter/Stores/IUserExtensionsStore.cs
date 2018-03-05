using AuthorizationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IUserExtensionsStore
    {
        Task<bool> SaveUserParamaters(List<UserExtensions> parameters, CancellationToken cancellationToken);

        Task<bool> DeleteUserParamaters(List<UserExtensions> parameters, CancellationToken cancellationToken);

        Task<List<UserExtensions>> GetUserParameters(string userId, List<string> parameterNames, CancellationToken cancellationToken);


        Task<List<UserExtensions>> GetUserParameters(List<string> userIds, string parameterNames, CancellationToken cancellationToken);
    }
}
