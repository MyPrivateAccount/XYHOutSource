using GatewayInterface.Dto;
using System;
using System.Threading.Tasks;

namespace GatewayInterface
{
    public interface IExamineInterface
    {
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <returns></returns>
        Task<ResponseMessage> Submit(UserInfo user, ExamineSubmitRequest examineSubmitRequest);
        
    }
}
