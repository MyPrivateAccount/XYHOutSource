using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Dto;

namespace XYHCustomerInterfacePlugin
{
    public class CustomerInterfaceMappingProfile : Profile
    {
        public CustomerInterfaceMappingProfile()
        {
            CreateMap<ExamineResponse, GatewayInterface.Dto.ExamineResponse>();
            CreateMap<GatewayInterface.Dto.ExamineResponse, ExamineResponse>();
        }

    }
}
