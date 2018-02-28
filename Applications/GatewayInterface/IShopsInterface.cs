using GatewayInterface.Dto;
using System.Threading.Tasks;

namespace GatewayInterface
{
    public interface IShopsInterface
    {

        Task<GatewayInterface.Dto.ResponseMessage> SubmitBuildingCallback(ExamineResponse examineResponse);
        Task<GatewayInterface.Dto.ResponseMessage> UpdateRecordSubmitCallback(ExamineResponse examineResponse);
        Task<ResponseMessage> SubmitShopsCallback(ExamineResponse examineResponse);
        Task<ResponseMessage> BuildingsOnSiteCallback(ExamineResponse examineResponse);
        Task<ResponseMessage<bool>> UpdateShopSaleStatus(string userid, string shopsid);
        Task<ResponseMessage<bool>> IsResidentUser(string userId, string buildingId);
        Task<ResponseMessage<bool>> IsManagerSiteUser(string userid, string buildingId);
        Task<GatewayInterface.Dto.ResponseMessage<GatewayInterface.Dto.Response.BuildingRuleInfoResponse>> GetBuilidngRule(string buildingId);
    }
}
