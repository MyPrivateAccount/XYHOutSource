using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHContractPlugin.Dto.Response;
using XYHContractPlugin.Models;
using XYHContractPlugin.Stores;

namespace XYHContractPlugin.Controllers
{
    [Produces("application/json")]
    [Route("api/contractnotice")]
    public class ContractNoticeController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("ContractNoticeController");
        private readonly RestClient _restClient;
        private readonly PermissionExpansionManager permissionExpansionManager;
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");

        public ContractNoticeController(IContractInfoStore icontractstore, PermissionExpansionManager pr, RestClient rt, IMapper mapper)
        {
            _icontractInfoStore = icontractstore;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            permissionExpansionManager = pr;
            _restClient = rt;
        }

        protected IContractInfoStore _icontractInfoStore { get; }
        protected IMapper _mapper { get; }



        private async Task NoticeUserByMsgType(string sql, string msgType)
        {
            try
            {
                 

                List<ContractInfo> contractList = new List<ContractInfo>();
                contractList = _icontractInfoStore.DapperSelect<ContractInfo>(sql).ToList();
                SendMessageRequest sendMessageRequest = new SendMessageRequest();
                sendMessageRequest.MessageList = new List<MessageItem>();
                List<string> recordUserList = new List<string>();
                recordUserList = await permissionExpansionManager.GetPermissionUserIds("RECORD_FUC");
                Logger.Trace("查询到符合条件的合同信息的个数为" + contractList.Count.ToString());
                for (int i = 0; i < contractList.Count; i++)
                {

                    ContractInfo info = contractList[i];
                    //驻场经理
                    List<string> magrUseList = await permissionExpansionManager.GetUseridsHaveOrganPermission(info.OrganizateID, "MyManagerContracts");
                  //未判断是否作废
                    if (info.EndTime.HasValue)
                    {
                        DateTime now = DateTime.Now;
                        int nSpanDay = (info.EndTime.Value - now).Days;
                        if (msgType == "ContractHaveNoOriginal")
                        {
                            if (info.ReturnOrigin == 2)
                            {


                                if (nSpanDay > 0 && nSpanDay < 15)
                                {
                                    MessageItem messageItem = new MessageItem();
                                    sendMessageRequest.MessageTypeCode = "ContractHaveNoOriginal";

                                    List<string> userList = recordUserList.Union(magrUseList).ToList<string>();
                                    messageItem.UserIds = userList;
                                    messageItem.MessageTypeItems = new List<TypeItem> {
                                //new TypeItem{ Key="NOTICETYPE",Value= "ContractNotice" },
                                new TypeItem { Key="NAME",Value=info.Name},
                                new TypeItem{ Key="TIME",Value=DateTime.Now.ToString("MM-dd hh:mm")}
                            };
                                    sendMessageRequest.MessageList.Add(messageItem);

                                }
                            }
                        }


                        if (msgType == "ContractWillExpire")
                        {
                            if (nSpanDay > 0 && nSpanDay < 15)
                            {
                                MessageItem messageItem = new MessageItem();
                                sendMessageRequest.MessageTypeCode = "ContractWillExpire";
                                messageItem.UserIds = recordUserList;
                                messageItem.MessageTypeItems = new List<TypeItem> {
                               // new TypeItem{ Key="NOTICETYPE",Value= "ContractNotice" },
                                new TypeItem { Key="NAME",Value=info.Name},
                                new TypeItem{ Key="TIME",Value=DateTime.Now.ToString("MM-dd hh:mm")}
                                };
                                sendMessageRequest.MessageList.Add(messageItem);
                            }
                        }

                    }

                    if (sendMessageRequest.MessageList.Count > 0)
                    {
                        try
                        {
                            MessageLogger.Info("发送通知消息协议：\r\n{0}", JsonHelper.ToJson(sendMessageRequest));
                            string x = await _restClient.Post(ApplicationContext.Current.MessageServerUrl, sendMessageRequest, "POST", new NameValueCollection());
                        }
                        catch (Exception e)
                        {
                            MessageLogger.Error("发送通知消息出错：\r\n{0}", e.ToString());
                        }
                    }

                }
            }catch(Exception e)
            {
                Logger.Error("发送合同通知消息出错:\r\n{0}", e.ToString());
            }
            return;
        }

        /// <summary>
        /// 合同通知定时任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("basicinfo")]
        public async Task<ResponseMessage> NoticeUserContractInfo()
        {
            Logger.Trace("Recv notice command!");
            var response = new ResponseMessage();
            try
            {
                Dictionary<string, string> msyTypeCodeList = new Dictionary<string, string>();


                string sql = "";
                DateTime now = DateTime.Now;
                sql = string.Format("select * from XYH_DT_CONTRACTINFO as a where ReturnOrigin=2 and TIMESTAMPDIFF(DAY,  '{0}',a.EndTime) > 0 and  TIMESTAMPDIFF(DAY, '{1}',a.EndTime) < 15", now, now);
                msyTypeCodeList.Add("ContractHaveNoOriginal", sql);
                sql = string.Format("select * from XYH_DT_CONTRACTINFO as a where  TIMESTAMPDIFF(DAY,'{0}', a.EndTime) > 0 and  TIMESTAMPDIFF(DAY, '{1}',a.EndTime) < 5", now, now);
                msyTypeCodeList.Add("ContractWillExpire", sql);

                foreach (KeyValuePair<string, string> kvp in msyTypeCodeList)
                {
                    await NoticeUserByMsgType(kvp.Value, kvp.Key);
                }
                response.Code = "0";
                response.Message = "通知发送完毕";
            }
            catch (Exception e)
            {
                response.Code = "1";
                response.Message = "通知发送异常，异常信息:" + e.ToString();
                Logger.Error("发送合同通知消息出错:\r\n{0}", e.ToString());
            }
            return response;
        }
    }
}
