using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Users, UserInfoResponse>();
            CreateMap<UserInfoResponse, Users>();

            CreateMap<UserInfoRequest, Users>();
            CreateMap<Users, UserInfoRequest>();

            CreateMap<UserExtensionsRequest, UserExtensions>();
            CreateMap<UserExtensions, UserExtensionsResponse > ();
        }


    }
}
