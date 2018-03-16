using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using ApplicationCore.Stores;
using AutoMapper;
using ExamineCenterPlugin.Dto;
using ExamineCenterPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;

namespace ExamineCenterPlugin.Managers
{
    public class ExamineNoticeManager
    {
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");
        public ExamineNoticeManager(IExamineNoticeStore examineNoticeStore,
            IExamineFlowStore examineFlowStore,
            PermissionExpansionManager permissionExpansionManager,
            IOrganizationExpansionStore organizationExpansionStore,
            IUserStore userStore,
            RestClient restClient,
            IMapper mapper)
        {
            Store = examineNoticeStore ?? throw new ArgumentNullException(nameof(examineNoticeStore));
            _examineFlowStore = examineFlowStore ?? throw new ArgumentNullException(nameof(examineFlowStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        protected IExamineNoticeStore Store { get; }
        protected IMapper _mapper { get; }
        protected IUserStore _userStore { get; }
        protected IOrganizationExpansionStore _organizationExpansionStore { get; }
        protected PermissionExpansionManager _permissionExpansionManager { get; }
        protected IExamineFlowStore _examineFlowStore { get; }
        protected RestClient _restClient { get; }

        public async Task<ResponseMessage> NoticeCallbackAsync(UserNoticeCallbackRequest userNoticeCallbackRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (userNoticeCallbackRequest == null)
            {
                throw new ArgumentNullException(nameof(userNoticeCallbackRequest));
            }
            //找到本地审核流程
            var examineFlow = await _examineFlowStore.ExamineFlowGetAsync(a => a.Where(b => b.TaskGuid == userNoticeCallbackRequest.TaskGuid));
            if (examineFlow == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "未在本地找到该审核流程";
            }
            //找到所有通知的人
            var userIds = await _permissionExpansionManager.GetPermissionUserIds(userNoticeCallbackRequest.PermissionItemId);
            if (userIds == null || userIds?.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "未找到通知的人";
            }
            examineFlow.CurrentStepId = userNoticeCallbackRequest.CurrentStepId;
            await Store.NoticeCallbackAsync(userIds, examineFlow);

            //发送通知消息
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.MessageTypeCode = "ExamineNotice";
            MessageItem messageItem = new MessageItem();
            messageItem.UserIds = userIds;
            messageItem.MessageTypeItems = new List<TypeItem> {
                new TypeItem{ Key="NOTICETYPE",Value=ExamineContentTypeConvert.GetContentTypeString(examineFlow.ContentType) },
                    new TypeItem { Key="NAME",Value=examineFlow.ContentName},
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
                MessageLogger.Error("发送通知消息出错：\r\n{0}", e.ToString());
            }

            return response;

        }


        public async Task<int> GetUserNoticeCount(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = Store.GetQuery().Where(a => a.NoticeUserId == userId && a.NoticeStatus == Models.NoticeStatus.Noticed && !a.IsDeleted);
            return await q.CountAsync(cancellationToken);
        }


        public async Task<PagingResponseMessage<ExamineNoticeResponse>> UserNoticeList(string userId, UserExamineNoticeListConditon condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<ExamineNoticeResponse> pagingResponse = new PagingResponseMessage<ExamineNoticeResponse>();
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var q = Store.GetQuery().Where(a => a.NoticeUserId == userId && !a.IsDeleted);

            if (condition.Status?.Count > 0)
            {
                q = q.Where(a => condition.Status.Contains(a.NoticeStatus));
            }
            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.OrderByDescending(x => x.NoticeTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var resulte = qlist.Select(a => new ExamineNoticeResponse()
            {
                ContentId = a.ExamineFlow.ContentId,
                ContentName = a.ExamineFlow.ContentName,
                ContentType = a.ExamineFlow.ContentType,
                Desc = a.ExamineFlow.Desc,
                ExamineStatus = a.ExamineFlow.ExamineStatus,
                Id = a.ExamineFlow.Id,
                SubmitOrganizationId = a.ExamineFlow.SubmitOrganizationId,
                SubmitOrganizationName = a.ExamineFlow.SubmitOrganizationName,
                SubmitTime = a.ExamineFlow.SubmitTime,
                SubmitUserId = a.ExamineFlow.SubmitUserId,
                SubmitUserName = a.ExamineFlow.SubmitUserName,
                TaskName = a.ExamineFlow.TaskName,
                SubmitDefineId = a.ExamineFlow.SubmitDefineId,
                FlowId = a.FlowId,
                NoticeStatus = a.NoticeStatus,
                RecordId = a.RecordId,
                Ext1 = a.ExamineFlow.Ext1,
                Ext2 = a.ExamineFlow.Ext2,
                Ext3 = a.ExamineFlow.Ext3,
                Ext4 = a.ExamineFlow.Ext4,
                Ext5 = a.ExamineFlow.Ext5,
                Ext6 = a.ExamineFlow.Ext6,
                Ext7 = a.ExamineFlow.Ext7,
                Ext8 = a.ExamineFlow.Ext8
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }


        public async Task<ExamineNoticeResponse> GetNoticeById(string userId, string nitoceId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = await Store.GetQuery().FirstOrDefaultAsync(a => a.Id == nitoceId && !a.IsDeleted, cancellationToken);
            var resulte = new ExamineNoticeResponse()
            {
                ContentId = q.ExamineFlow.ContentId,
                ContentName = q.ExamineFlow.ContentName,
                ContentType = q.ExamineFlow.ContentType,
                Desc = q.ExamineFlow.Desc,
                ExamineStatus = q.ExamineFlow.ExamineStatus,
                Id = q.ExamineFlow.Id,
                SubmitOrganizationId = q.ExamineFlow.SubmitOrganizationId,
                SubmitOrganizationName = q.ExamineFlow.SubmitOrganizationName,
                SubmitTime = q.ExamineFlow.SubmitTime,
                SubmitUserId = q.ExamineFlow.SubmitUserId,
                SubmitUserName = q.ExamineFlow.SubmitUserName,
                TaskName = q.ExamineFlow.TaskName,
                SubmitDefineId = q.ExamineFlow.SubmitDefineId,
                FlowId = q.FlowId,
                NoticeStatus = q.NoticeStatus,
                RecordId = q.RecordId,
                Ext1 = q.ExamineFlow.Ext1,
                Ext2 = q.ExamineFlow.Ext2,
                Ext3 = q.ExamineFlow.Ext3,
                Ext4 = q.ExamineFlow.Ext4,
                Ext5 = q.ExamineFlow.Ext5,
                Ext6 = q.ExamineFlow.Ext6,
                Ext7 = q.ExamineFlow.Ext7,
                Ext8 = q.ExamineFlow.Ext8
            };
            await Store.SetReadingStatus(nitoceId);
            return resulte;
        }



    }
}
