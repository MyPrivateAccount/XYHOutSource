using ApplicationCore;
using AutoMapper;
using GatewayInterface;
using GatewayInterface.Dto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYHChargePlugin.Controllers;

namespace XYHChargeInterfacePlugin
{
    public class ChargeInterfaceMappingProfile : Profile
    {
        public ChargeInterfaceMappingProfile()
        {
            CreateMap<XYHChargePlugin.Dto.Response.ExamineResponse, GatewayInterface.Dto.ExamineResponse>();
            CreateMap<GatewayInterface.Dto.ExamineResponse, XYHChargePlugin.Dto.Response.ExamineResponse>();

            CreateMap<ApplicationCore.Dto.UserInfo, GatewayInterface.Dto.UserInfo>();
            CreateMap<GatewayInterface.Dto.UserInfo, ApplicationCore.Dto.UserInfo>();

            CreateMap<ApplicationCore.ResponseMessage, GatewayInterface.Dto.ResponseMessage>();
            CreateMap<GatewayInterface.Dto.ResponseMessage, ApplicationCore.ResponseMessage>();

            CreateMap<ApplicationCore.ResponseMessage<bool>, GatewayInterface.Dto.ResponseMessage<bool>>();
            CreateMap<GatewayInterface.Dto.ResponseMessage<bool>, ApplicationCore.ResponseMessage<bool>>();
        }
    }
}
