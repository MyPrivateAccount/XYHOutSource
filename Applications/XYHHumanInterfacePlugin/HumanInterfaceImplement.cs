using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ApplicationCore;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using GatewayInterface.Dto;
using XYHHumanPlugin.Controllers;

namespace XYHHumanInterfacePlugin
{
    public class HumanInterfaceImplement : IHumanInterface
    {
        public async Task<GatewayInterface.Dto.ResponseMessage> SubmitHumanCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<HumanInfoController>().SubmitContractCallback(Mapper.Map<XYHHumanPlugin.Dto.Response.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }

        public async Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordHumanCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<HumanInfoController>().UpdateRecordContractCallback(Mapper.Map<XYHHumanPlugin.Dto.Response.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }
    }
}
