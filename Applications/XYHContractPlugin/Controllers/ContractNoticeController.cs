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
using XYH.Core.Log;
using XYHContractPlugin.Dto.Response;
using XYHContractPlugin.Models;
using XYHContractPlugin.Stores;

namespace XYHContractPlugin.Controllers
{
    class ContractNoticeController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("ContractNoticeController");
        private readonly RestClient _restClient;
        private readonly PermissionExpansionManager permissionExpansionManager;
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");

        public ContractNoticeController(IContractInfoStore icontractstore, IMapper mapper)
        {
            {
                _icontractInfoStore = icontractstore;
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            }
        }

        protected IContractInfoStore _icontractInfoStore { get; }
        protected IMapper _mapper { get; }
    

        public async void NoticeUserByMsgType(string sql, string msgType)
        {
            List<ContractInfo> contractList = new List<ContractInfo>();
            contractList = _icontractInfoStore.DapperSelect<ContractInfo>(sql).ToList();
            SendMessageRequest sendMessageRequest = new SendMessageRequest();

            List<string> recordUserList = new List<string>();
            recordUserList = await permissionExpansionManager.GetPermissionUserIds("RECORD_FUC");
            for (int i = 0; i < contractList.Count; i++)
            {

                ContractInfo info = contractList[i];
                //驻场经理
                List<string> magrUseList = new List<string>();//GetUseridsHaveOrganPermission(string organizationId, string permissionId)
                //未判断是否作废
                if (info.EndTime.HasValue)
                {
                    DateTime now = DateTime.Now;
                    int nSpanDay = (info.EndTime.Value - now).Days;
                    if(msgType == "ContractHaveNoOriginal")
                    {
                        if (info.ReturnOrigin == 0)
                        {


                            if (nSpanDay > 0 && nSpanDay < 15)
                            {
                                MessageItem messageItem = new MessageItem();
                                sendMessageRequest.MessageTypeCode = "ContractHaveNoOriginal";

                                List<string> userList = recordUserList.Union(magrUseList).ToList<string>();
                                messageItem.UserIds = userList;
                                messageItem.MessageTypeItems = new List<TypeItem> {
                                new TypeItem{ Key="NOTICETYPE",Value= "ContractNotice" },
                                new TypeItem { Key="NAME",Value=info.Name},
                                new TypeItem{ Key="TIME",Value=DateTime.Now.ToString("MM-dd hh:mm")}
                            };
                                sendMessageRequest.MessageList.Add(messageItem);

                            }
                        }
                    }


                    if (msgType == "ContractWillEexpire")
                    {
                        if (nSpanDay > 0 && nSpanDay > 5)
                        {
                            MessageItem messageItem = new MessageItem();
                            sendMessageRequest.MessageTypeCode = "ContractWillEexpire";
                            messageItem.UserIds = recordUserList;
                            messageItem.MessageTypeItems = new List<TypeItem> {
                                new TypeItem{ Key="NOTICETYPE",Value= "ContractNotice" },
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
                        await _restClient.Post(ApplicationContext.Current.MessageServerUrl, sendMessageRequest, "POST", new NameValueCollection());
                    }
                    catch (Exception e)
                    {
                        MessageLogger.Error("发送通知消息出错：\r\n{0}", e.ToString());
                    }
                }

            }
        }
        public async void NoticeUserContractInfo()
        {
            try
            {
                Dictionary<string, string> msyTypeCodeList = new Dictionary<string, string>();


                string sql = "";
                DateTime now = DateTime.Now;
                sql = string.Format("select * from XYH_DT_CONTRACTINFO as a where ReturnOrigin=1 and DATEDIFF(DAY, a.EndTime, {0}) > 0 and  DATEDIFF(DAY, a.EndTime, {1}) < 15", now, now);
                msyTypeCodeList.Add("ContractHaveNoOriginal", sql);
                sql = string.Format("select * from XYH_DT_CONTRACTINFO as a where  DATEDIFF(DAY, a.EndTime, {0}) > 0 and  DATEDIFF(DAY, a.EndTime, {1}) < 5", now, now);
                msyTypeCodeList.Add("ContractWillEexpire", sql);

                foreach (KeyValuePair<string, string> kvp in msyTypeCodeList)
                {
                    NoticeUserByMsgType(kvp.Value, kvp.Key);
                }
            }
            catch(Exception e)
            {
                Logger.Error("发送合同通知消息出错:\r\n{0}", e.ToString());
            }

        }
    }
}
