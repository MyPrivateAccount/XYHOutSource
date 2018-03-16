using System;
using System.Collections.Generic;
using System.Text;
using GatewayInterface.Dto;
using System.Threading.Tasks;

namespace GatewayInterface
{
    public interface IContractInterface
    {
        Task<GatewayInterface.Dto.ResponseMessage> SubmitContractCallback(ExamineResponse examineResponse);
        Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordContractCallback(ExamineResponse examineResponse);
        
     
    }
}
