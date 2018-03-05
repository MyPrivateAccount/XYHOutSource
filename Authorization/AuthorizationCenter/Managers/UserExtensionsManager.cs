using AuthorizationCenter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationCenter.Dto;
using System.Threading;
using AuthorizationCenter.Stores;
using AuthorizationCenter.Models;
using AutoMapper;

namespace AuthorizationCenter.Managers
{
    public class UserExtensionsManager : IUserExtensionsManager
    {
        IUserExtensionsStore store = null;
        IMapper mapper = null;
        public UserExtensionsManager(IUserExtensionsStore ues, IMapper _mapper)
        {
            store = ues;
            mapper = _mapper;
        }

        public async Task<bool> DeleteUserExtensions(ClaimsUserInfo ClaimsUserInfo, List<string> parNames, CancellationToken cancellationToken)
        {
            List<UserExtensions> extensions = new List<UserExtensions>();
            if(parNames!=null)
            {
                parNames.ForEach(p =>
                {
                    extensions.Add(new UserExtensions()
                    {
                        UserId = ClaimsUserInfo.Id,
                        ParName = p
                    });
                });
            }
            return await store.DeleteUserParamaters(extensions, cancellationToken);
        }

        public async Task<List<UserExtensionsResponse>> GetUserExtensions(ClaimsUserInfo ClaimsUserInfo, List<string> parNames, CancellationToken cancellationToken)
        {
            var list =await store.GetUserParameters(ClaimsUserInfo.Id, parNames, cancellationToken);
            if(list !=null && list.Count > 0)
            {
                return mapper.Map<List<UserExtensionsResponse>>(list);
            }


            return new List<UserExtensionsResponse>();
        }


        public async Task<List<UserExtensionsResponse>> GetUserExtensions(ClaimsUserInfo ClaimsUserInfo, string parNames, List<string> userIds, CancellationToken cancellationToken)
        {
            var list = await store.GetUserParameters(userIds, parNames, cancellationToken);
            if (list != null && list.Count > 0)
            {
                return mapper.Map<List<UserExtensionsResponse>>(list);
            }


            return new List<UserExtensionsResponse>();
        }

        public async Task<bool> SaveUserExtensions(ClaimsUserInfo ClaimsUserInfo, List<UserExtensionsRequest> request, CancellationToken cancellationToken)
        {
            List<UserExtensions> list = mapper.Map<List<UserExtensions>>(request);
            if (list != null)
            {
                list.ForEach(p => p.UserId = ClaimsUserInfo.Id);
                return await store.SaveUserParamaters(list, cancellationToken);
            }
            return false;
        }
    }
}
