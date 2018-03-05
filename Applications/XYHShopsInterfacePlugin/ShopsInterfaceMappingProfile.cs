using AutoMapper;
using GatewayInterface.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsInterfacePlugin
{
    public class ShopsInterfaceMappingProfile : Profile
    {
        public ShopsInterfaceMappingProfile()
        {
            //CreateMap<ApplicationCore.Dto.UserInfo, GatewayInterface.Dto.UserInfo>();
            //CreateMap<GatewayInterface.Dto.UserInfo, ApplicationCore.Dto.UserInfo>();

            CreateMap<XYHShopsPlugin.Dto.ExamineResponse, GatewayInterface.Dto.ExamineResponse>();
            CreateMap<GatewayInterface.Dto.ExamineResponse, XYHShopsPlugin.Dto.ExamineResponse>();

            CreateMap<XYHShopsPlugin.Dto.Request.SaleShopsStatusRquest, GatewayInterface.Dto.SaleShopsStatusRquest>();
            CreateMap<GatewayInterface.Dto.SaleShopsStatusRquest, XYHShopsPlugin.Dto.Request.SaleShopsStatusRquest>();
        }
    }
}
