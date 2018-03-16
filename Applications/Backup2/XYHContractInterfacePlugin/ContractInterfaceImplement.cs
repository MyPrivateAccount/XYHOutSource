using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ApplicationCore;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using GatewayInterface.Dto;
using XYHContractPlugin.Controllers;

namespace XYHContractInterfacePlugin
{
    public class ContractInterfaceImplement : IContractInterface
    {
        public ContractInterfaceImplement()
        { }

        public async Task<GatewayInterface.Dto.ResponseMessage> SubmitContractCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<ContractInfoController>().SubmitContractCallback(Mapper.Map<XYHContractPlugin.Dto.Response.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }

        public async Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordContractCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<ContractInfoController>().UpdateRecordContractCallback(Mapper.Map<XYHContractPlugin.Dto.Response.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }


    }
}
