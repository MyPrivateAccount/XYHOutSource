using ExamineCenterPlugin.Dto;
using GatewayInterface;
using System;
using System.Threading.Tasks;
using GatewayInterface.Dto;
using ExamineCenterPlugin.Controllers;
using ApplicationCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace ExamineCenterInterfacePlugin
{
    public class ExamineInterfaceImplement : IExamineInterface
    {
        //protected ExamineFlowController ExamineFlowController { get; set; }
        //protected IMapper Mapper { get; set; }

        public ExamineInterfaceImplement()
        {
            //ExamineFlowController = ApplicationContext.Current.Provider.GetRequiredService<ExamineFlowController>();
            //Mapper = ApplicationContext.Current.Provider.GetRequiredService<IMapper>();
        }

        public async Task<GatewayInterface.Dto.ResponseMessage> Submit(UserInfo user, GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest)
        {
            using (var scope = ApplicationContext.Current.Provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var response = await scope.ServiceProvider.GetRequiredService<ExamineFlowController>().Submit(Mapper.Map<ApplicationCore.Dto.UserInfo>(user), Mapper.Map<ExamineCenterPlugin.Dto.ExamineSubmitRequest>(examineSubmitRequest));
                return Mapper.Map<GatewayInterface.Dto.ResponseMessage>(response);
            }
        }
    }
}
