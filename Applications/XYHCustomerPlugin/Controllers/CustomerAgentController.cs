using ApplicationCore;
using ApplicationCore.Dto;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto;

namespace XYHCustomerPlugin.Controllers
{
    /// <summary>
    /// 客户委托
    /// </summary>
    [Produces("application/json")]
    [Route("api/customeragent")]
    public class CustomerAgentController : Controller
    {
        private readonly RestClient _restClient;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerAgentAdd");
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");

        public CustomerAgentController(RestClient restClient)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        /// <summary>
        /// 查询楼盘下的推荐客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="buildingid"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseMessage> PostCustomerAgent([FromBody]CustomerAgentRequest customerAgentRequest)
        {
            Logger.Trace($"客户委托(PostCustomerList)：\r\n请求参数为：\r\n" + (customerAgentRequest != null ? JsonHelper.ToJson(customerAgentRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"客户委托(PostCustomerList)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerAgentRequest != null ? JsonHelper.ToJson(customerAgentRequest) : ""));
                return response;
            }
            Logger.Error("新增客户委托：" + (customerAgentRequest != null ? JsonHelper.ToJson(customerAgentRequest) : ""));
            //发送通知消息
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.MessageTypeCode = "CustomerAgentAdd";
            MessageItem messageItem = new MessageItem();
            messageItem.UserIds = new List<string> { "2543" };
            messageItem.MessageTypeItems = new List<TypeItem> {
                    new TypeItem{ Key="PHONE",Value=customerAgentRequest.CustomerPhone },
                    new TypeItem { Key="NAME",Value=customerAgentRequest.CustomerName},
                    new TypeItem{ Key="TIME",Value=DateTime.Now.ToString("MM-dd hh:mm")}
                };
            sendMessageRequest.MessageList = new List<MessageItem> { messageItem };
            try
            {
                MessageLogger.Info("发送通知消息协议：\r\n{0}", JsonHelper.ToJson(sendMessageRequest));
                _restClient.Post(ApplicationContext.Current.MessageServerUrl, sendMessageRequest, "POST", new NameValueCollection());
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误:" + e.ToString();
                MessageLogger.Error("发送通知消息出错：\r\n{0}", e.ToString());
            }
            return response;
        }


    }
}
