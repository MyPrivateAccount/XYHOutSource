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
    public class ChargeInterfaceImplement : IChargeInterface
    {
        public async Task<GatewayInterface.Dto.ResponseMessage> SubmitChargeCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<ChargeInfoController>().SubmitChargeCallback(Mapper.Map<XYHChargePlugin.Dto.Response.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }

        public async Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordChargeCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<ChargeInfoController>().UpdateRecordChargeCallback(Mapper.Map<XYHChargePlugin.Dto.Response.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }
    }
}
