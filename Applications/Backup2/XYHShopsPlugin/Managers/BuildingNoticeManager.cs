using ApplicationCore;
using ApplicationCore.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class BuildingNoticeManager
    {

        public BuildingNoticeManager(IBuildingNoticeStore buildingNoticeStore, IMapper mapper)
        {
            Store = buildingNoticeStore;
            _mapper = mapper;
        }
        protected IBuildingNoticeStore Store { get; }
        protected IMapper _mapper { get; }


        public async Task<BuildingNoticeResponse> FindByIdAsync(string noticeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildingNotice = await Store.GetQuery().FirstOrDefaultAsync(a => a.Id == noticeId, cancellationToken);
            if (buildingNotice == null)
            {
                return null;
            }
            BuildingNoticeResponse updateRecordResponse = _mapper.Map<BuildingNoticeResponse>(buildingNotice);

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');
            updateRecordResponse.Icon = string.IsNullOrEmpty(updateRecordResponse.Icon) ? "" : fr + "/" + updateRecordResponse.Icon.TrimStart('/');

            updateRecordResponse.FileList = new List<FileItemResponse>();
            updateRecordResponse.AttachmentList = new List<AttachmentResponse>();

            if (buildingNotice.RecordFileInfos?.Count() > 0)
            {
                var f = buildingNotice.RecordFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = buildingNotice.RecordFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        updateRecordResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = buildingNotice.RecordFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
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
                    Group = item.Group,
                    Summary = item.Summary,
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

        public async Task<PagingResponseMessage<BuildingNoticeListResponse>> GetListAsync(BuildingNoticeListCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<BuildingNoticeListResponse> pagingResponse = new PagingResponseMessage<BuildingNoticeListResponse>();
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var q = Store.GetQuery().Where(a => !a.IsDeleted);
            if (condition.OrganizationIds?.Count > 0)
            {
                q = q.Where(a => condition.OrganizationIds.Contains(a.OrganizationId));
            }
            if (condition.UserIds?.Count > 0)
            {
                q = q.Where(a => condition.UserIds.Contains(a.UserId));
            }
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                q = q.Where(a => a.Title.Contains(condition.KeyWord) || a.Content.Contains(condition.KeyWord));
            }
            if (condition.StartTime != null)
            {
                q = q.Where(a => a.CreateTime >= condition.StartTime);
            }
            if (condition.EndTime != null)
            {
                q = q.Where(a => a.CreateTime <= condition.EndTime);
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
            var qlist = await q.OrderByDescending(a => a.CreateTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var resulte = qlist.Select(a => new BuildingNoticeListResponse
            {
                BuildingId = a.BuildingId,
                Id = a.Id,
                Icon = string.IsNullOrEmpty(a.Icon) ? "" : fr + "/" + a.Icon.TrimStart('/'),
                CreateTime = a.CreateTime,
                Ext1 = a.Ext1,
                Ext2 = a.Ext2,
                Title = a.Title,
                OrganizationId = a.OrganizationId,
                OrganizationName = a.OrganizationName,
                UserId = a.UserId,
                UserName = a.UserName,
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


        public async Task<BuildingNoticeResponse> CreateAsync(UserInfo user, BuildingNoticeRequest buildingNoticeRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNoticeRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingNoticeRequest));
            }
            var notice = _mapper.Map<BuildingNotice>(buildingNoticeRequest);
            //updateRecord.Id = Guid.NewGuid().ToString();
            notice.UserId = user.Id;
            notice.OrganizationId = user.OrganizationId;
            notice.CreateTime = DateTime.Now;
            notice.IsDeleted = false;
            return _mapper.Map<BuildingNoticeResponse>(await Store.CreateAsync(notice, cancellationToken));
        }


        public async Task DeleteAsync(string userId, string noticeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var notice = await Store.GetAsync(a => a.Where(b => b.Id == noticeId));
            if (notice == null)
            {
                throw new Exception("删除的对象实体未找到");
            }
            notice.IsDeleted = true;
            notice.DeleteTime = DateTime.Now;
            notice.DeleteUser = userId;
            await Store.UpdateAsync(notice, cancellationToken);
        }


        public async Task UpdateAsync(string noticeId, BuildingNoticeRequest buildingNoticeRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingNoticeRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingNoticeRequest));
            }
            var notice = await Store.GetAsync(a => a.Where(b => b.Id == noticeId));
            if (notice == null)
            {
                throw new Exception("更新的对象实体未找到");
            }
            notice.BuildingId = buildingNoticeRequest.BuildingId;
            notice.Content = buildingNoticeRequest.Content;
            notice.Ext2 = buildingNoticeRequest.Ext2;
            notice.Ext1 = buildingNoticeRequest.Ext1;
            notice.Icon = buildingNoticeRequest.Icon;
            notice.Title = buildingNoticeRequest.Title;
            await Store.UpdateAsync(notice, cancellationToken);
        }



    }
}
