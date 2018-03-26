using System;
using System.Collections.Generic;
using System.Text;
using GatewayInterface.Dto;
using System.Threading.Tasks;

namespace GatewayInterface
{
    public interface IHumanInterface
    {
        Task<GatewayInterface.Dto.ResponseMessage> SubmitContractCallback(ExamineResponse examineResponse);
        Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordContractCallback(ExamineResponse examineResponse);
    }
}
