using AutoMapper;

namespace ExamineCenterInterfacePlugin
{
    public class ExamineInterfaceMappingProfile : Profile
    {
        public ExamineInterfaceMappingProfile()
        {
            CreateMap<ExamineCenterPlugin.Dto.ExamineSubmitRequest, GatewayInterface.Dto.ExamineSubmitRequest>();
            CreateMap<GatewayInterface.Dto.ExamineSubmitRequest, ExamineCenterPlugin.Dto.ExamineSubmitRequest>();

            CreateMap<ApplicationCore.Dto.UserInfo, GatewayInterface.Dto.UserInfo>();
            CreateMap<GatewayInterface.Dto.UserInfo, ApplicationCore.Dto.UserInfo>();

            CreateMap<ApplicationCore.ResponseMessage, GatewayInterface.Dto.ResponseMessage>();
            CreateMap<GatewayInterface.Dto.ResponseMessage, ApplicationCore.ResponseMessage>();

            CreateMap<ApplicationCore.ResponseMessage<bool>, GatewayInterface.Dto.ResponseMessage<bool>>();
            CreateMap<GatewayInterface.Dto.ResponseMessage<bool>, ApplicationCore.ResponseMessage<bool>>();
        }
    }
}
