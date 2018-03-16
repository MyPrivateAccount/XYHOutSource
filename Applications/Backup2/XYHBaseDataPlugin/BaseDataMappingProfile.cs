using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XYHBaseDataPlugin.Dto;
using XYHBaseDataPlugin.Dto.Request;
using XYHBaseDataPlugin.Dto.Response;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin
{
    public class BaseDataMappingProfile : Profile
    {
        public BaseDataMappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            //CreateMap<DictionaryGroup, DictionaryGroupRequest>();

            CreateMap<DictionaryDefineResponse, DictionaryDefine>();
            CreateMap<DictionaryDefine, DictionaryDefineResponse>();

            CreateMap<AreaDefineRequest, AreaDefine>();
            CreateMap<AreaDefine, AreaDefineRequest>();
            CreateMap<AreaDefineResponse, AreaDefine>();
            CreateMap<AreaDefine, AreaDefineResponse>();

            #region 用户自定义

            CreateMap<UserTypeValueResponse, UserTypeValue>();
            CreateMap<UserTypeValue, UserTypeValueResponse>();

            CreateMap<UserTypeValueRequest, UserTypeValue>();
            CreateMap<UserTypeValue, UserTypeValueRequest>();

            #endregion
        }



    }
}
