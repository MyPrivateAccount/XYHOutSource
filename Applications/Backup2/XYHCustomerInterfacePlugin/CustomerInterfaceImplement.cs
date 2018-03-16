using ApplicationCore;
using AutoMapper;
using GatewayInterface;
using GatewayInterface.Dto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XYHCustomerPlugin.Controllers;

namespace XYHCustomerInterfacePlugin
{
    public class CustomerInterfaceImplement : ICustomerInterface
    {

        //protected CustomerInfoController CustomerInfoController { get; set; }
        //protected IMapper Mapper { get; set; }
        public CustomerInterfaceImplement()
        {
            //CustomerInfoController = ApplicationContext.Current.Provider.GetRequiredService<CustomerInfoController>();
            //Mapper = ApplicationContext.Current.Provider.GetRequiredService<IMapper>();
        }

        public async Task<GatewayInterface.Dto.ResponseMessage> TransferCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<CustomerInfoController>().TransferCallback(Mapper.Map<XYHCustomerPlugin.Dto.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }
        public async Task<GatewayInterface.Dto.ResponseMessage> CustomerDealCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<CustomerDealController>().CustomerDealCallback(Mapper.Map<XYHCustomerPlugin.Dto.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }
        public async Task<GatewayInterface.Dto.ResponseMessage> CustomerDealBackCallback(ExamineResponse examineResponse)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<CustomerDealController>().CustomerDealBackSellCallback(Mapper.Map<XYHCustomerPlugin.Dto.ExamineResponse>(examineResponse));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }

    }
}
