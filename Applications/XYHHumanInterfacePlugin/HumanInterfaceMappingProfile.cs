using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanInterfacePlugin
{
    public class HumanInterfaceMappingProfile : Profile
    {
        public HumanInterfaceMappingProfile()
        {
            CreateMap<XYHHumanPlugin.Dto.Response.ExamineResponse, GatewayInterface.Dto.ExamineResponse>();
            CreateMap<GatewayInterface.Dto.ExamineResponse, XYHHumanPlugin.Dto.Response.ExamineResponse>();

            CreateMap<ApplicationCore.Dto.UserInfo, GatewayInterface.Dto.UserInfo>();
            CreateMap<GatewayInterface.Dto.UserInfo, ApplicationCore.Dto.UserInfo>();

            CreateMap<ApplicationCore.ResponseMessage, GatewayInterface.Dto.ResponseMessage>();
            CreateMap<GatewayInterface.Dto.ResponseMessage, ApplicationCore.ResponseMessage>();

            CreateMap<ApplicationCore.ResponseMessage<bool>, GatewayInterface.Dto.ResponseMessage<bool>>();
            CreateMap<GatewayInterface.Dto.ResponseMessage<bool>, ApplicationCore.ResponseMessage<bool>>();
        }
    }
}
