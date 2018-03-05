using AutoMapper;
using ExamineCenterPlugin.Dto;
using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin
{
    public class ExamineMappingProfile : Profile
    {
        public ExamineMappingProfile()
        {
            CreateMap<ExamineSubmitRequest, ExamineFlow>();

            
            

        }

    }
}
