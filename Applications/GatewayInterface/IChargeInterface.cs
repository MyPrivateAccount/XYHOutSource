using GatewayInterface.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GatewayInterface
{
    public interface IChargeInterface
    {
        Task<GatewayInterface.Dto.ResponseMessage> SubmitChargeCallback(ExamineResponse examineResponse);
        Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordChargeCallback(ExamineResponse examineResponse);
    }
}
