using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using ApplicationCore.Models;
using ApplicationCore.Stores;
using AutoMapper;
using ExamineCenterPlugin.Dto;
using ExamineCenterPlugin.Models;
using ExamineCenterPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
    public class ExamineFlowManager
    {
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");
        public ExamineFlowManager(IExamineFlowStore examineFlowStore,
            PermissionExpansionManager permissionExpansionManager,
            IOrganizationExpansionStore organizationExpansionStore,
            IUserStore userStore,
            RestClient restClient,
            IMapper mapper)
        {
            Store = examineFlowStore ?? throw new ArgumentNullException(nameof(examineFlowStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _organizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        protected IExamineFlowStore Store { get; }
        protected IMapper _mapper { get; }
        protected IUserStore _userStore { get; }
        protected IOrganizationExpansionStore _organizationExpansionStore { get; }
        protected PermissionExpansionManager _permissionExpansionManager { get; }
        protected RestClient _restClient { get; }

        public Task<ExamineFlow> SaveExamineFlow(UserInfo user, string taskGuid, ExamineSubmitRequest examineSubmitRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineSubmitRequest == null)
            {
                throw new ArgumentNullException(nameof(examineSubmitRequest));
            }
            var examineFlow = _mapper.Map<ExamineFlow>(examineSubmitRequest);

            examineFlow.ExamineStatus = ExamineStatus.Examining;
            examineFlow.SubmitTime = DateTime.Now;
            examineFlow.SubmitOrganizationId = user.OrganizationId;
            examineFlow.SubmitUserId = user.Id;
            examineFlow.TaskGuid = taskGuid;
            examineFlow.IsDeleted = false;
            examineFlow.Id = Guid.NewGuid().ToString();

            return Store.CreateExamineFlowAsync(examineFlow, cancellationToken);
        }


        /// <summary>
        /// 根据内容Id和审核流程名获取一个审核
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<ExamineFlow> GetExamineFlow(string contentId, string action)
        {
            var flow = await Store.ExamineFlowGetAsync(a => a.Where(b => b.ContentId == contentId && b.Action == action && b.ExamineStatus == ExamineStatus.Examining));
            if (flow != null)
            {
                return flow;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取用户待审核列表（待修正）
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagingResponseMessage<ExamineFlowListResponse>> GetUserWaitingExamineFlowList(string userId, UserWaitingExamineFlowListCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<ExamineFlowListResponse> pagingResponse = new PagingResponseMessage<ExamineFlowListResponse>();
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var q = Store.GetRecordQuery().Where(a => a.ExamineUserId == userId && a.RecordStstus == RecordStatus.Waiting);

            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.OrderByDescending(a => a.RecordTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var resulte = qlist.Select(a => new ExamineFlowListResponse()
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


        /// <summary>
        /// 获取用户提交的审核列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagingResponseMessage<ExamineFlowListResponse>> GetUserSubmitExamineFlowList(string userId, UserExamineFlowListCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<ExamineFlowListResponse> pagingResponse = new PagingResponseMessage<ExamineFlowListResponse>();
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var q = Store.GetFlowQuery().Where(a => !a.IsDeleted && a.SubmitUserId == userId);
            if (condition.ExamineStatus?.Count > 0)
            {
                q = q.Where(a => condition.ExamineStatus.Contains(a.ExamineStatus));
            }
            if (condition.ContentTypes?.Count > 0)
            {
                q = q.Where(a => condition.ContentTypes.Contains(a.ContentType));
            }
            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = q.OrderByDescending(a => a.SubmitTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize);
            var resulte = qlist.Select(a => new ExamineFlowListResponse()
            {
                ContentId = a.ContentId,
                ContentName = a.ContentName,
                ContentType = a.ContentType,
                Desc = a.Desc,
                ExamineStatus = a.ExamineStatus,
                Id = a.Id,
                SubmitOrganizationId = a.SubmitOrganizationId,
                SubmitOrganizationName = a.SubmitOrganizationName,
                SubmitTime = a.SubmitTime,
                SubmitUserId = a.SubmitUserId,
                SubmitUserName = a.SubmitUserName,
                TaskName = a.TaskName,
                Ext1 = a.Ext1,
                Ext2 = a.Ext2,
                Ext3 = a.Ext3,
                Ext4 = a.Ext4,
                Ext5 = a.Ext5,
                Ext6 = a.Ext6,
                Ext7 = a.Ext7,
                Ext8 = a.Ext8
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = await resulte.ToListAsync(cancellationToken);
            return pagingResponse;
        }



        /// <summary>
        /// 获取用户参与审核的列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="condition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PagingResponseMessage<ExamineFlowListResponse>> GetUserExamineFlowList(string userId, UserExamineFlowListCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<ExamineFlowListResponse> pagingResponse = new PagingResponseMessage<ExamineFlowListResponse>();
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var q = Store.GetRecordQuery().Where(a => !a.IsDeleted && a.ExamineUserId == userId);
            if (condition.ExamineStatus?.Count > 0)
            {
                q = q.Where(a => condition.ExamineStatus.Contains(a.ExamineFlow.ExamineStatus));
            }
            if (condition.ContentTypes?.Count > 0)
            {
                q = q.Where(a => condition.ContentTypes.Contains(a.ExamineFlow.ContentType));
            }
            var q1 = q.Distinct((a, b) => a.FlowId == b.FlowId);
            pagingResponse.TotalCount = q1.Count();
            var qlist = q1.OrderByDescending(x => x.RecordTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize);
            var resulte = qlist.Select(a => new ExamineFlowListResponse()
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
                SubmitDefineId = a.ExamineFlow.SubmitDefineId,
                TaskName = a.ExamineFlow.TaskName,
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




        /// <summary>
        /// 根据流程id获取流程审核记录
        /// </summary>
        /// <param name="flowId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ExamineRecordResponse>> GetExamineRecordListByFlowId(string flowId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = Store.GetRecordQuery().Where(a => a.FlowId == flowId && !a.IsDeleted && a.IsCurrent);

            var resulte = q.Select(a => new ExamineRecordResponse()
            {
                Id = a.Id,
                Sort = a.Sort,
                ExamineContents = a.ExamineContents,
                ExamineTime = a.ExamineTime,
                ExamineUserId = a.ExamineUserId,
                RecordType = a.RecordType,
                ExamineUserName = a.ExamineUserName,
                FlowId = a.FlowId,
                RecordStstus = a.RecordStstus,
                RecordTime = a.RecordTime
            });
            return await resulte.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 获取待审核数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> GetExamineCount(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var examineRecord = Store.ExamineRecords.Where(a => a.RecordStstus == RecordStatus.Waiting && a.ExamineUserId == userId);
            return await examineRecord?.CountAsync(cancellationToken);
        }


        /// <summary>
        /// 审核步骤回调
        /// </summary>
        /// <param name="examineCallbackRequest"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> StepCallback(ExamineCallbackRequest examineCallbackRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (examineCallbackRequest == null)
            {
                throw new ArgumentNullException(nameof(examineCallbackRequest));
            }
            //找到本地审核流程
            var examineFlow = await Store.ExamineFlowGetAsync(a => a.Where(b => b.TaskGuid == examineCallbackRequest.TaskGuid));
            if (examineFlow == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "未在本地找到该审核流程";
                return response;
            }
            //找到所有能审核的人
            var userIds = await _permissionExpansionManager.GetPermissionUserIds(examineCallbackRequest.PermissionItemId);
            if (userIds == null || userIds?.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "未找到有权限审核人";
                return response;
            }
            //找到第一个符合条件的审核人
            string examineUserId = "";
            if (string.IsNullOrEmpty(examineCallbackRequest.OrganizationId))
            {
                examineUserId = await FindExamineUserId(examineFlow.SubmitOrganizationId, userIds);
            }
            else
            {
                examineUserId = await FindExamineUserIdByOrganization(examineCallbackRequest.OrganizationId, userIds);
            }
            if (string.IsNullOrEmpty(examineUserId))
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "未找到有权限并且符合组织条件的审核人";
                return response;
            }
            examineFlow.CurrentStepId = examineCallbackRequest.CurrentStepId;
            await Store.StepCallBackUpdateExamineFlowAsync(examineUserId, examineFlow);


            //发送通知消息
            SendMessageRequest sendMessageRequest = new SendMessageRequest();
            sendMessageRequest.MessageTypeCode = "ExamineWaiting";
            MessageItem messageItem = new MessageItem();
            messageItem.UserIds = new List<string> { examineUserId };
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

        /// <summary>
        /// 审核流程回调
        /// </summary>
        /// <param name="examineCallbackRequest"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> FlowCallback(ExamineCallbackRequest examineCallbackRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (examineCallbackRequest == null)
            {
                throw new ArgumentNullException(nameof(examineCallbackRequest));
            }
            //找到本地审核流程
            var examineFlow = await Store.ExamineFlowGetAsync(a => a.Where(b => b.TaskGuid == examineCallbackRequest.TaskGuid));
            if (examineFlow == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "未在本地找到该审核流程";
            }
            examineFlow.ExamineStatus = ExamineStatus.Examined;
            examineFlow.CurrentStepId = examineCallbackRequest.CurrentStepId;
            await Store.FlowCallBackUpdateExamineFlowAsync(examineFlow);
            return response;
        }

        /// <summary>
        /// 通过审核
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recordId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ExaminePass(string userId, string recordId, string desc)
        {
            ResponseMessage response = new ResponseMessage();
            var record = await Store.ExamineRecordGetAsync(a => a.Where(b => b.Id == recordId));
            if (record.ExamineUserId != userId)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "当前用户非审核用户";
                return response;
            }
            record.ExamineTime = DateTime.Now;
            record.ExamineContents = desc;
            record.RecordStstus = RecordStatus.Examined;
            await Store.UpdateExamineRecordAsync(record);
            return response;
        }

        /// <summary>
        /// 驳回审核
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recordId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public async Task<ResponseMessage> ExamineReject(string userId, string recordId, string desc)
        {
            ResponseMessage response = new ResponseMessage();
            var record = await Store.ExamineRecordGetAsync(a => a.Where(b => b.Id == recordId));
            if (record.ExamineUserId != userId)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "当前用户非审核用户";
                return response;
            }
            record.ExamineTime = DateTime.Now;
            record.ExamineContents = desc;
            record.RecordStstus = RecordStatus.Reject;
            await Store.UpdateExamineRejectAsync(record);
            return response;
        }
        public async Task<PagingResponseMessage<ExamineFlow>> Search(ExamineFlowSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            PagingResponseMessage<ExamineFlow> pagingResponse = new PagingResponseMessage<ExamineFlow>();

            var q = Store.ExamineFlows.Where(a => !a.IsDeleted);
            if (!string.IsNullOrEmpty(condition.ContentId))
            {
                q = q.Where(a => a.ContentId == condition.ContentId);
            }
            if (!string.IsNullOrEmpty(condition.ContentType))
            {
                q = q.Where(a => a.ContentType == condition.ContentType);
            }
            if (!string.IsNullOrEmpty(condition.ExaminAction))
            {
                q = q.Where(a => a.Action == condition.ExaminAction);
            }
            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.OrderByDescending(a => a.SubmitTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = qlist;
            return pagingResponse;
        }

        public async Task<List<ExamineStatusListResponse>> GetCurrentExamineStatus(string contentid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var examineflow = await Store.ExamineFlowListAsync(a => a.Where(b => b.ContentId == contentid && b.ExamineStatus != ExamineStatus.Examined));
            if (examineflow == null || examineflow?.Count == 0)
            {
                return null;
            }
            return examineflow.Select(a => new ExamineStatusListResponse
            {
                Action = a.Action,
                ContentId = a.ContentId,
                ContentType = a.ContentType,
                ExamineStatus = a.ExamineStatus
            }).ToList();
        }



        public async Task<ExamineFlowResponse> FindExamineFlowById(string flowId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = Store.GetFlowQuery().Where(a => a.Id == flowId);
            var result = q.Select(a =>
                new ExamineFlowResponse()
                {
                    Id = a.Id,
                    ContentId = a.ContentId,
                    ContentName = a.ContentName,
                    ContentType = a.ContentType,
                    Desc = a.Desc,
                    ExamineStatus = a.ExamineStatus,
                    SubmitTime = a.SubmitTime,
                    SubmitUserId = a.SubmitUserId,
                    SubmitUserName = a.SubmitUserName,
                    CallbackUrl = a.CallbackUrl,
                    SubmitDefineId = a.SubmitDefineId,
                    TaskName = a.TaskName,
                    Content = a.Content,
                    Ext1 = a.Ext1,
                    Ext2 = a.Ext2,
                    Ext3 = a.Ext3,
                    Ext4 = a.Ext4,
                    Ext5 = a.Ext5,
                    Ext6 = a.Ext6,
                    Ext7 = a.Ext7,
                    Ext8 = a.Ext8,
                    ExamineRecordResponses = a.ExamineRecords.Select(o => new ExamineRecordResponse()
                    {
                        ExamineContents = o.ExamineContents,
                        ExamineTime = o.ExamineTime,
                        ExamineUserId = o.ExamineUserId,
                        Sort = o.Sort,
                        ExamineUserName = o.ExamineUserName,
                        FlowId = o.FlowId,
                        RecordType = o.RecordType,
                        IsCurrent = o.IsCurrent,
                        Id = o.Id,
                        RecordStstus = o.RecordStstus,
                        RecordTime = o.RecordTime
                    })
                });
            return await result.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<ExamineFlowResponse> FindExamineFlowByTaskGuid(string taskGuid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = Store.GetFlowQuery().Where(a => a.TaskGuid == taskGuid);
            var result = q.Select(a =>
                new ExamineFlowResponse()
                {
                    Id = a.Id,
                    ContentId = a.ContentId,
                    ContentName = a.ContentName,
                    ContentType = a.ContentType,
                    Content = a.Content,
                    Desc = a.Desc,
                    ExamineStatus = a.ExamineStatus,
                    SubmitTime = a.SubmitTime,
                    SubmitUserId = a.SubmitUserId,
                    SubmitUserName = a.SubmitUserName,
                    CallbackUrl = a.CallbackUrl,
                    SubmitDefineId = a.SubmitDefineId,
                    TaskName = a.TaskName,
                    Ext1 = a.Ext1,
                    Ext2 = a.Ext2,
                    Ext3 = a.Ext3,
                    Ext4 = a.Ext4,
                    Ext5 = a.Ext5,
                    Ext6 = a.Ext6,
                    Ext7 = a.Ext7,
                    Ext8 = a.Ext8,
                    ExamineRecordResponses = a.ExamineRecords.Select(o => new ExamineRecordResponse()
                    {
                        ExamineContents = o.ExamineContents,
                        ExamineTime = o.ExamineTime,
                        ExamineUserId = o.ExamineUserId,
                        ExamineUserName = o.ExamineUserName,
                        Sort = o.Sort,
                        IsCurrent = o.IsCurrent,
                        RecordType = o.RecordType,
                        FlowId = o.FlowId,
                        Id = o.Id,
                        RecordStstus = o.RecordStstus,
                        RecordTime = o.RecordTime
                    })
                });
            return await result.FirstOrDefaultAsync(cancellationToken);
        }



        public async Task<ExamineRecord> FindExamineRecordById(string recordId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Store.GetRecordQuery().FirstOrDefaultAsync(a => a.Id == recordId, cancellationToken);
        }


        /// <summary>
        /// 依次找到提交用户所在组织以及上级组织中有审核权限的第一个用户
        /// </summary>
        /// <param name="userOrganizationId"></param>
        /// <param name="havePermissionUserIds"></param>
        /// <returns></returns>
        private async Task<string> FindExamineUserId(string userOrganizationId, List<string> havePermissionUserIds)
        {
            var users = await _userStore.ListAsync(a => a.Where(b => b.OrganizationId == userOrganizationId && havePermissionUserIds.Contains(b.Id)));
            if (users != null && users.Count > 0)
            {
                return users.FirstOrDefault().Id;
            }
            var organization = await _organizationExpansionStore.GetAsync(a => a.Where(b => b.SonId == userOrganizationId && b.IsImmediate));
            if (organization != null)
            {
                return await FindExamineUserId(organization.OrganizationId, havePermissionUserIds);
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// 通过指定组织找到用审核权限的第一个用户
        /// </summary>
        /// <param name="userOrganizationId"></param>
        /// <param name="havePermissionUserIds"></param>
        /// <returns></returns>
        private async Task<string> FindExamineUserIdByOrganization(string organizationId, List<string> havePermissionUserIds)
        {
            var users = await _userStore.ListAsync(a => a.Where(b => b.OrganizationId == organizationId && havePermissionUserIds.Contains(b.Id)));
            if (users != null && users.Count > 0)
            {
                return users.FirstOrDefault().Id;
            }
            else
            {
                return "";
            }
        }

    }

}
