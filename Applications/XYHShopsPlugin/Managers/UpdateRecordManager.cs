using ApplicationCore;
using ApplicationCore.Dto;
using AutoMapper;
using GatewayInterface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class UpdateRecordManager
    {
        public UpdateRecordManager(IUpdateRecordStore updateRecordStore, IMapper mapper)
        {
            Store = updateRecordStore;
            _mapper = mapper;
        }
        protected IUpdateRecordStore Store { get; }
        protected IMapper _mapper { get; }

        public async Task<UpdateRecordResponse> CreateUpdateRecordAsync(UserInfo user, UpdateRecordRequest updateRecordRequest, Models.ExamineStatusEnum examineStatusEnum, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecordRequest == null)
            {
                throw new ArgumentNullException(nameof(UpdateRecordRequest));
            }
            var updateRecord = _mapper.Map<UpdateRecord>(updateRecordRequest);
            //updateRecord.Id = Guid.NewGuid().ToString();
            updateRecord.UserId = user.Id;
            updateRecord.OrganizationId = user.OrganizationId;
            updateRecord.ExamineStatus = examineStatusEnum;
            updateRecord.IsCurrent = true;
            updateRecord.UpdateTime = DateTime.Now;
            updateRecord.SubmitTime = DateTime.Now;
            return _mapper.Map<UpdateRecordResponse>(await Store.CreateUpdateRecordAsync(updateRecord, cancellationToken));
        }

        public async Task UpdateAsync(string updateId, UpdateRecordRequest updateRecordRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (updateRecordRequest == null)
            {
                throw new ArgumentNullException(nameof(UpdateRecordRequest));
            }
            var updateRecord = await Store.GetAsync(a => a.Where(b => b.Id == updateId));
            if (updateRecord == null)
            {
                throw new Exception("更新的对象实体未找到");
            }
            updateRecord.Content = updateRecordRequest.Content;
            updateRecord.ContentId = updateRecordRequest.ContentId;
            updateRecord.ContentType = updateRecordRequest.ContentType;
            updateRecord.Ext1 = updateRecordRequest.Ext1;
            updateRecord.Ext2 = updateRecordRequest.Ext2;
            updateRecord.Ext3 = updateRecordRequest.Ext3;
            updateRecord.Ext4 = updateRecordRequest.Ext4;
            updateRecord.Ext5 = updateRecordRequest.Ext5;
            updateRecord.Ext6 = updateRecordRequest.Ext6;
            updateRecord.Ext7 = updateRecordRequest.Ext7;
            updateRecord.Ext8 = updateRecordRequest.Ext8;
            updateRecord.Icon = updateRecordRequest.Image;
            updateRecord.Title = updateRecordRequest.Title;
            updateRecord.UpdateContent = updateRecordRequest.UpdateContent;
            updateRecord.UpdateType = updateRecordRequest.UpdateType;
            await Store.UpdateUpdateRecordAsync(updateRecord, cancellationToken);
        }

        public async Task<UpdateRecordResponse> FindByIdAsync(string updateId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var updaterecord = await Store.GetQuery().FirstOrDefaultAsync(a => a.Id == updateId, cancellationToken);
            if (updaterecord == null)
            {
                return null;
            }
            UpdateRecordResponse updateRecordResponse = _mapper.Map<UpdateRecordResponse>(updaterecord);

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');
            updateRecordResponse.Icon = string.IsNullOrEmpty(updateRecordResponse.Icon) ? "" : fr + "/" + updateRecordResponse.Icon.TrimStart('/');

            updateRecordResponse.FileList = new List<FileItemResponse>();
            updateRecordResponse.AttachmentList = new List<AttachmentResponse>();

            if (updaterecord.RecordFileInfos?.Count() > 0)
            {
                var f = updaterecord.RecordFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = updaterecord.RecordFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        updateRecordResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = updaterecord.RecordFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                    if (f2?.Count > 0)
                    {
                        updateRecordResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                    }
                }
            }
            return updateRecordResponse;
        }

        private List<AttachmentResponse> ConvertToAttachmentItem(List<FileInfo> f2)
        {
            List<AttachmentResponse> list = new List<AttachmentResponse>();
            string fr = ApplicationContext.Current.FileServerRoot;
            foreach (var item in f2)
            {
                list.Add(new AttachmentResponse
                {
                    FileGuid = item.FileGuid,
                    FileName = item.Name,
                    Summary = item.Summary,
                    Group = item.Group,
                    Url = fr + "/" + item.Uri.TrimStart('/')
                });
            }
            return list;
        }

        private FileItemResponse ConvertToFileItem(string fileGuid, List<FileInfo> fl)
        {
            FileItemResponse fi = new FileItemResponse();
            fi.FileGuid = fileGuid;
            fi.Group = fl.FirstOrDefault()?.Group;
            fi.Icon = fl.FirstOrDefault(x => x.Type == "ICON")?.Uri;
            fi.Original = fl.FirstOrDefault(x => x.Type == "ORIGINAL")?.Uri;
            fi.Medium = fl.FirstOrDefault(x => x.Type == "MEDIUM")?.Uri;
            fi.Small = fl.FirstOrDefault(x => x.Type == "SMALL")?.Uri;

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');
            if (!String.IsNullOrEmpty(fi.Icon))
            {
                fi.Icon = fr + "/" + fi.Icon.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Original))
            {
                fi.Original = fr + "/" + fi.Original.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Medium))
            {
                fi.Medium = fr + "/" + fi.Medium.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Small))
            {
                fi.Small = fr + "/" + fi.Small.TrimStart('/');
            }
            return fi;
        }


        public async Task<PagingResponseMessage<UpdateRecordListResponse>> GetListAsync(UpdateRecordListCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<UpdateRecordListResponse> pagingResponse = new PagingResponseMessage<UpdateRecordListResponse>();
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(UpdateRecordListCondition));
            }
            var q = Store.GetDetail().Where(a => !a.IsDeleted);
            if (condition.ContentTypes?.Count > 0)
            {
                q = q.Where(a => condition.ContentTypes.Contains(a.ContentType));
            }
            if (condition.ContentIds?.Count > 0)
            {
                q = q.Where(a => condition.ContentIds.Contains(a.ContentId));
            }
            if (condition.ExamineStatus?.Count > 0)
            {
                q = q.Where(a => condition.ExamineStatus.Contains(a.ExamineStatus));
            }
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                q = q.Where(a => a.Title.Contains(condition.KeyWord) || a.Content.Contains(condition.KeyWord));
            }
            if (condition.UpdateTypes?.Count > 0)
            {
                q = q.Where(a => condition.UpdateTypes.Contains(a.UpdateType));
            }
            if (!string.IsNullOrEmpty(condition.UserId))
            {
                q = q.Where(a => a.UserId == condition.UserId);
            }
            if (condition.IsCurrent != null)
            {
                q = q.Where(a => a.IsCurrent == condition.IsCurrent);
            }
            if (!string.IsNullOrEmpty(condition.AreaCode))
            {
                q = q.Where(a => a.AreaDefine.Code == condition.AreaCode);
            }
            if (!string.IsNullOrEmpty(condition.DistrictCode))
            {
                q = q.Where(a => a.DistrictDefine.Code == condition.DistrictCode);
            }
            if (!string.IsNullOrEmpty(condition.CityCode))
            {
                q = q.Where(a => a.CityDefine.Code == condition.CityCode);
            }

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);

            var qlist = await q.OrderByDescending(a => a.SubmitTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var resulte = qlist.Select(a => new UpdateRecordListResponse
            {
                ContentId = a.ContentId,
                UserName = a.UserName,
                Id = a.Id,
                Icon = string.IsNullOrEmpty(a.Icon) ? "" : fr + "/" + a.Icon.TrimStart('/'),
                SubmitTime = a.SubmitTime,
                ContentType = a.ContentType,
                ExamineStatus = a.ExamineStatus,
                Title = a.Title,
                UpdateTime = a.UpdateTime,
                UpdateType = a.UpdateType,
                IsCurrent = a.IsCurrent,
                UserId = a.UserId,
                Ext1 = a.Ext1,
                Ext2 = a.Ext2,
                Ext3 = a.Ext3,
                Ext4 = a.Ext4,
                Ext5 = a.Ext5,
                Ext6 = a.Ext6,
                Ext7 = a.Ext7,
                Ext8 = a.Ext8,
                BuildingName = a.BuildingName,
                AreaFullName = (a.CityDefine != null && !string.IsNullOrEmpty(a.CityDefine.Name) ? a.CityDefine.Name + "-" : "") +
                               (a.DistrictDefine != null && !string.IsNullOrEmpty(a.DistrictDefine.Name) ? a.DistrictDefine.Name + "-" : "") +
                               (a.AreaDefine != null && !string.IsNullOrEmpty(a.AreaDefine.Name) ? a.AreaDefine.Name : "")
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }

        public async Task<PagingResponseMessage<UpdateRecordListResponse>> GetFollowListAsync(string userId, UpdateRecordFollowListCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<UpdateRecordListResponse> pagingResponse = new PagingResponseMessage<UpdateRecordListResponse>();

            var q = Store.GetFollowDetail(userId).Where(a => !a.IsDeleted && a.ExamineStatus == Models.ExamineStatusEnum.Approved);

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');

            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);

            var qlist = await q.OrderByDescending(a => a.SubmitTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var resulte = qlist.Select(a => new UpdateRecordListResponse
            {
                ContentId = a.ContentId,
                UserName = a.UserName,
                Id = a.Id,
                Icon = string.IsNullOrEmpty(a.Icon) ? "" : fr + "/" + a.Icon.TrimStart('/'),
                SubmitTime = a.SubmitTime,
                ContentType = a.ContentType,
                ExamineStatus = a.ExamineStatus,
                Title = a.Title,
                UpdateTime = a.UpdateTime,
                UpdateType = a.UpdateType,
                IsCurrent = a.IsCurrent,
                UserId = a.UserId,
                Ext1 = a.Ext1,
                Ext2 = a.Ext2,
                Ext3 = a.Ext3,
                Ext4 = a.Ext4,
                Ext5 = a.Ext5,
                Ext6 = a.Ext6,
                Ext7 = a.Ext7,
                Ext8 = a.Ext8,
                BuildingName = a.BuildingName,
                AreaFullName = (a.CityDefine != null && !string.IsNullOrEmpty(a.CityDefine.Name) ? a.CityDefine.Name + "-" : "") +
                               (a.DistrictDefine != null && !string.IsNullOrEmpty(a.DistrictDefine.Name) ? a.DistrictDefine.Name + "-" : "") +
                               (a.AreaDefine != null && !string.IsNullOrEmpty(a.AreaDefine.Name) ? a.AreaDefine.Name : "")
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }




        public async Task DeleteAsync(string userId, string updateId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var updateRecord = await Store.GetAsync(a => a.Where(b => b.Id == updateId));
            if (updateRecord == null)
            {
                throw new Exception("删除的对象实体未找到");
            }
            updateRecord.IsDeleted = true;
            updateRecord.DeleteTime = DateTime.Now;
            updateRecord.DeleteUserId = userId;
            await Store.UpdateUpdateRecordAsync(updateRecord, cancellationToken);
        }



        public async Task UpdateRecordSubmitCallback(ExamineResponse examineResponse, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineResponse == null)
            {
                throw new ArgumentNullException(nameof(ExamineResponse));
            }
            switch (examineResponse.ContentType)
            {
                case "ShopsHot":
                    await Store.ShopsHotCallbackAsync(examineResponse);
                    break;
                case "ShopsAdd":
                    await Store.ShopsAddCallbackAsync(examineResponse);
                    break;
                case "ReportRule":
                    await Store.ReportRuleCallbackAsync(examineResponse);
                    break;
                case "CommissionType":
                    await Store.CommissionTypeCallbackAsync(examineResponse);
                    break;
                case "BuildingNo":
                    await Store.BuildingNoCallbackAsync(examineResponse);
                    break;
                case "DiscountPolicy":
                    await Store.DiscountPolicyCallbackAsync(examineResponse);
                    break;
                case "Price":
                    await Store.PriceCallbackAsync(examineResponse);
                    break;
                default:
                    throw new NotImplementedException("未实现该内容类型的回调处理：" + examineResponse.ContentType);
            }
        }
    }
}
