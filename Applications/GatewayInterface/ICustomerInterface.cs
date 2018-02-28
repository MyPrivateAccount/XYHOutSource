using GatewayInterface.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GatewayInterface
{
    public interface ICustomerInterface
    {

        Task<Dto.ResponseMessage> TransferCallback(ExamineResponse examineResponse);


        Task<GatewayInterface.Dto.ResponseMessage> CustomerDealCallback(ExamineResponse examineResponse);


    }
}
