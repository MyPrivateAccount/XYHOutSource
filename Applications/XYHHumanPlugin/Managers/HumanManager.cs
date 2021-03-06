﻿using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Stores;
using System.Threading.Tasks;
using ApplicationCore.Dto;
using HumanInfRequest = XYHHumanPlugin.Dto.Response.HumanInfoResponse1;
using AutoMapper;
using ApplicationCore.Models;
using XYHHumanPlugin.Models;
using System.Threading;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using System.Linq;
using ApplicationCore.Managers;
using SocialInsuranceRequest = XYHHumanPlugin.Dto.Response.SocialInsuranceResponse;
using LeaveInfoRequest = XYHHumanPlugin.Dto.Response.LeaveInfoResponse;
using ChangeInfoRequest = XYHHumanPlugin.Dto.Response.ChangeInfoResponse;
using ApplicationCore;
using ApplicationCore.Stores;
using Microsoft.EntityFrameworkCore;

namespace XYHHumanPlugin.Managers
{
    public class HumanManager
    {

        public HumanManager(IHumanManageStore stor, IMapper mapper,
            IOrganizationExpansionStore organizationExpansionStore,
            PermissionExpansionManager permissionExpansionManager)
        {
            _Store = stor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _iorganizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
        }

        protected IHumanManageStore _Store { get; }
        protected IMapper _mapper { get; }
        protected IOrganizationExpansionStore _iorganizationExpansionStore { get; }
        protected PermissionExpansionManager _permissionExpansionManager { get; }

        public virtual async Task AddHuman(UserInfo info, HumanInfRequest req, string modify, string checktion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (req.ID == null)
            {
                req.ID = Guid.NewGuid().ToString();
            }

            await _Store.CreateAsync(_mapper.Map<SimpleUser>(info), _mapper.Map<HumanInfo>(req), modify, checktion, cancellationToken);
        }

        public virtual async Task CreateFileScopeAsync(string userId, string humanid, FileInfoRequest fileInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(fileInfoRequest));
            }

            var contractfile = _mapper.Map<AnnexInfo>(fileInfoRequest);
            if (string.IsNullOrEmpty(contractfile.ID))
            {
                contractfile.ID = humanid;
            }

            contractfile.CreateUser = userId;
            contractfile.CreateTime = DateTime.Now;

            await _Store.CreateAsync(contractfile, cancellationToken);
        }

        public virtual async Task<FileItemResponse> GetFilelistAsync(string humanid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var f = await _Store.GetScopeFileListAsync(a => a.Where(b => b.ID == humanid));
            if (f.Count > 0)
            {
                var fileinfo = await _Store.GetFileListAsync(a => a.Where(b => b.FileGuid == f[0].FileGuid));
                return ConvertToFileItem(f[0].FileGuid, fileinfo);
            }
            return null;
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
        public virtual async Task CreateFilelistAsync(string userid, List<FileInfoCallbackRequest> fileInfoCallbackRequestList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoCallbackRequestList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoCallbackRequestList));
            }

            var fileInfos = _mapper.Map<List<FileInfo>>(fileInfoCallbackRequestList);
            for (int i = 0; i < fileInfos.Count; i++)
            {
                fileInfos[i].IsDeleted = false;
                fileInfos[i].CreateTime = DateTime.Now;
                fileInfos[i].CreateUser = userid;
                fileInfos[i].Uri = fileInfoCallbackRequestList[i].FilePath;
            }
            await _Store.CreateListAsync(fileInfos, cancellationToken);
        }

        public virtual async Task<ModifyInfoResponse> SubmitAsync(string modifyid, ExamineStatusEnum ext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (modifyid == null)
            {
                throw new ArgumentNullException(nameof(modifyid));
            }

            if (ext == ExamineStatusEnum.Approved)
            {
                return _mapper.Map<ModifyInfoResponse>(await _Store.UpdateExamineStatus(modifyid, ext, cancellationToken));
            }
            else if (ext == ExamineStatusEnum.Reject)
            {
                return _mapper.Map<ModifyInfoResponse>(await _Store.UpdateExamineStatus(modifyid, ext, cancellationToken));
            }
            return null;
        }

        public virtual async Task PreBecomeHuman(UserInfo userinfo, string modifyid, SocialInsuranceRequest info, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userinfo == null)
            {
                throw new ArgumentNullException(nameof(userinfo));
            }

            await _Store.PreBecomeHuman(_mapper.Map<SimpleUser>(userinfo), modifyid, info.ID, JsonHelper.ToJson(_mapper.Map<SocialInsurance>(info)), info.IDCard, checkaction, cancellationToken);
        }

        public virtual async Task BecomeHuman(SocialInsuranceRequest hr, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (hr == null)
            {
                throw new ArgumentNullException(nameof(hr));
            }

            await _Store.BecomeHuman(_mapper.Map<SocialInsurance>(hr), hr.ID, cancellationToken);
        }

        public virtual async Task PreLeaveHuman(UserInfo userinfo, string modifyid, LeaveInfoRequest info, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userinfo == null)
            {
                throw new ArgumentNullException(nameof(userinfo));
            }

            await _Store.PreLeaveHuman(_mapper.Map<SimpleUser>(userinfo), modifyid, info.ID, JsonHelper.ToJson(_mapper.Map<LeaveInfo>(info)), info.IDCard, checkaction, cancellationToken);
        }

        public virtual async Task LeaveHuman(LeaveInfoRequest info, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            await _Store.LeaveHuman(_mapper.Map<LeaveInfo>(info), info.ID, cancellationToken);
        }

        public virtual async Task PreChangeHuman(UserInfo userinfo, string modifyid, ChangeInfoRequest info, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userinfo == null)
            {
                throw new ArgumentNullException(nameof(userinfo));
            }

            await _Store.PreChangeHuman(_mapper.Map<SimpleUser>(userinfo), modifyid, info.ID, JsonHelper.ToJson(_mapper.Map<ChangeInfo>(info)), info.IDCard, checkaction, cancellationToken);
        }

        public virtual async Task ChangeHuman(ChangeInfoRequest info, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            await _Store.ChangeHuman(_mapper.Map<ChangeInfo>(info), info.ID, cancellationToken);
        }

        #region 检索
        public virtual async Task<HumanSearchResponse<HumanInfoResponse1>> SearchHumanInfo(UserInfo user, HumanSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var Response = new HumanSearchResponse<HumanInfoResponse1>();
            var sql = @"SELECT a.* from XYH_HU_HUMANMANAGE as a where";

            if (condition?.CheckStatu > 0)
            {
                sql = @"SELECT a.* from XYH_HU_HUMANMANAGE as a LEFT JOIN XYH_HU_MODIFY as b ON a.`RecentModify`=b.`ID` where";
            }

            string connectstr = " ";
            if (!string.IsNullOrEmpty(condition?.KeyWord))
            {
                sql += connectstr + @"LOCATE('" + condition.KeyWord + "', a.`Name`)>0";
                connectstr = " and ";
            }
            else if (condition?.KeyWord != null)
            {
                sql += connectstr + @"a.`ID`!=''";
                connectstr = " and ";
            }

            if (condition?.HumanType > 0)//0不限 1未入职 2在职 3离职 4黑名单
            {
                switch (condition?.HumanType)
                {
                    case 1:
                        {
                            sql += connectstr + @"a.`StaffStatus`=0";
                            connectstr = " and ";
                        }
                        break;

                    case 2:
                        {
                            sql += connectstr + @"a.`StaffStatus`>1";
                            connectstr = " and ";
                        }
                        break;

                    case 3:
                        {
                            sql += connectstr + @"a.`StaffStatus`=1";
                            connectstr = " and ";
                        }
                        break;

                    case 4:
                        {
                            sql += connectstr + @"a.`StaffStatus`=-1";
                            connectstr = " and ";
                        }
                        break;
                }
            }

            if (condition?.SearchTimeType > 0)
            {
                switch (condition?.SearchTimeType)
                {
                    case 1:
                        {
                            sql += connectstr + @"(a.`CreateTime`<='" + condition.CreateDateStart.Value + "'";
                            connectstr = " and ";
                            sql += connectstr + @"a.`CreateTime`>='" + condition.CreateDateEnd.Value + "')";
                        }
                        break;
                    case 2:
                        {
                            sql += connectstr + @"(a.`EntryTime`<='" + condition.CreateDateStart.Value + "'";
                            connectstr = " and ";
                            sql += connectstr + @"a.`EntryTime`>='" + condition.CreateDateEnd.Value + "')";
                        }
                        break;
                    case 3:
                        {
                            sql += connectstr + @"(a.`BecomeTime`<='" + condition.CreateDateStart.Value + "'";
                            connectstr = " and ";
                            sql += connectstr + @"a.`BecomeTime`>='" + condition.CreateDateEnd.Value + "')";
                        }
                        break;
                    case 4:
                        {
                            sql += connectstr + @"(a.`LeaveTime`<='" + condition.CreateDateStart.Value + "'";
                            connectstr = " and ";
                            sql += connectstr + @"a.`LeaveTime`>='" + condition.CreateDateEnd.Value + "')";
                        }
                        break;
                    default:
                        break;
                }

            }

            if (condition?.CheckStatu > 0)
            {
                string head = "(", tail = ")";
                if ((condition?.CheckStatu & 0x01) > 0)//1 2 4 8 未提交 审核中 通过 驳回
                {
                    sql += connectstr + head + @"b.`ExamineStatus`=0";
                    connectstr = " or ";
                    head = "";
                }
                if ((condition?.CheckStatu & 0x02) > 0)
                {
                    sql += connectstr + head + @"b.`ExamineStatus`=1";
                    connectstr = " or ";
                    head = "";
                }
                if ((condition?.CheckStatu & 0x04) > 0)
                {
                    sql += connectstr + head + @"b.`ExamineStatus`=8";
                    connectstr = " or ";
                    head = "";
                }
                if ((condition?.CheckStatu & 0x08) > 0)
                {
                    sql += connectstr + head + @"b.`ExamineStatus`=16";
                    connectstr = " or ";
                    head = "";
                }

                sql += tail;
                connectstr = " and ";
            }

            if (condition?.AgeCondition > 0)
            {
                if (condition?.AgeCondition == 1)
                {
                    sql += connectstr + @"a.`Age`>=20";
                    connectstr = " and ";
                }
                else if (condition?.AgeCondition == 2)
                {
                    sql += connectstr + @"a.`Age`>=30";
                    connectstr = " and ";
                }
                else if (condition?.AgeCondition == 3)
                {
                    sql += connectstr + @"a.`Age`>=40";
                    connectstr = " and ";
                }
            }

            if (condition?.OrderRule == 0 || condition?.OrderRule == null)
            {
                sql += @" ORDER BY a.`Name`";
            }
            else if (condition?.OrderRule == 1)
            {
                sql += @" ORDER BY a.`ID`";
            }

            try
            {
                List<HumanInfo> query = new List<HumanInfo>();
                var sqlinfo = _Store.DapperSelect<HumanInfo>(sql).ToList();

                if (!string.IsNullOrEmpty(condition?.Organizate) && condition.LstChildren.Count > 0)
                {
                    foreach (var item in sqlinfo)
                    {
                        if (condition.LstChildren.Contains(item.DepartmentId))
                        {
                            query.Add(item);
                        }
                    }
                }
                else
                {
                    query = sqlinfo;
                }

                Response.ValidityContractCount = query.Count;
                Response.TotalCount = query.Count;

                List<HumanInfo> result = new List<HumanInfo>();
                var begin = (condition.pageIndex) * condition.pageSize;
                var end = (begin + condition.pageSize) > query.Count ? query.Count : (begin + condition.pageSize);

                for (; begin < end; begin++)
                {

                    result.Add(query.ElementAt(begin));
                }

                Response.PageIndex = condition.pageIndex;
                Response.PageSize = condition.pageSize;
                Response.Extension = _mapper.Map<List<HumanInfoResponse1>>(result);

                foreach (var item in Response.Extension)
                {
                    var tf = await _Store.GetStationAsync(a => a.Where(b => b.ID == item.Position));
                    if (tf != null)
                    {
                        item.PositionName = tf.PositionName;
                    }

                }
            }
            catch (Exception e)
            {

                throw;
            }

            return Response;
        }
        #endregion
        



        public virtual async Task<PagingResponseMessage<HumanInfoResponse>> SimpleSearch(UserInfo user, string permissionId, string keyword, string branchId, int pageSize, int pageIndex)
        {
            PagingResponseMessage<HumanInfoResponse> r = new PagingResponseMessage<HumanInfoResponse>();

            var orgIds = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, permissionId);
            if (!String.IsNullOrEmpty(branchId))
            {
                var lids = await _permissionExpansionManager.GetLowerDepartments(branchId);
                orgIds = lids.Where(x => orgIds.Contains(x)).ToList();
            }

            var query = _Store.SimpleQuery;
            query = query.Where(hr => orgIds.Contains(hr.DepartmentId));

            if (!String.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(hr => ( hr.Name.Contains(keyword) || hr.UserID.Contains(keyword) || hr.Id==keyword ));
            }
            if (pageSize > 0 && pageIndex > 0)
            {
                r.TotalCount = await query.CountAsync();
                r.PageIndex = pageIndex;
                r.PageSize = pageSize;
                query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            }
            var ul = await query.ToListAsync();
            r.Extension = new List<HumanInfoResponse>();
            ul.ForEach(u =>
            {
                var u2 = _mapper.Map<HumanInfoResponse>(u);
                if (u.OrganizationExpansion != null && !String.IsNullOrEmpty(u.OrganizationExpansion.FullName))
                {
                    u2.OrganizationFullName = u.OrganizationExpansion.FullName;
                }
                else if (u.Organizations != null)
                {
                    u2.OrganizationFullName = u.Organizations.OrganizationName;
                }
                r.Extension.Add(u2);
            });

            if (r.TotalCount == 0)
            {
                r.TotalCount = r.Extension.Count;
            }

            return r;




        }
    }
}
