using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Stores;
using System.Threading.Tasks;
using ApplicationCore.Dto;
using HumanInfRequest = XYHHumanPlugin.Dto.Response.HumanInfoResponse;
using AutoMapper;
using ApplicationCore.Models;
using XYHHumanPlugin.Models;
using System.Threading;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using System.Linq;

namespace XYHHumanPlugin.Managers
{
    public class HumanManager
    {
        public static int NotOnBoard = 1;//未入职
        public static int OnBoardinged = 2;//已入职


        public HumanManager(IHumanManageStore stor, IMapper mapper)
        {
            _Store = stor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IHumanManageStore _Store { get; }
        protected IMapper _mapper { get; }

        
        public virtual async Task AddHuman(UserInfo info, HumanInfRequest req, string modify, string checktion, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (req.ID == null)
            {
                req.ID = Guid.NewGuid().ToString();
            }

            await _Store.CreateAsync(_mapper.Map<SimpleUser>(info), _mapper.Map<HumanInfo>(req), modify, checktion, cancellationToken);
        }

        public virtual async Task CreateFileScopeAsync(string userId,  string humanid, FileInfoRequest fileInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
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
            var f = await _Store.GetScopeFileAsync(a => a.Where(b => b.ID == humanid));
            var fileinfo = await _Store.GetFileAsync(a => a.Where(b => b.FileGuid == f.FileGuid));
            return ConvertToFileItem(f.FileGuid, new List<FileInfo> { fileinfo});
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

        public virtual async Task SubmitAsync(string modifyid, ExamineStatusEnum ext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (modifyid == null)
            {
                throw new ArgumentNullException(nameof(modifyid));
            }

            if (ext == ExamineStatusEnum.Approved)
            {
                await _Store.UpdateExamineStatus(modifyid, ext, OnBoardinged, cancellationToken);
            }
            else if (ext == ExamineStatusEnum.Reject)
            {
                await _Store.UpdateExamineStatus(modifyid, ext, NotOnBoard, cancellationToken);
            }

        }

        #region 检索
        public virtual async Task<HumanSearchResponse<HumanInfoResponse>> SearchHumanInfo(UserInfo user, HumanSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var Response = new HumanSearchResponse<HumanInfoResponse>();
            var sql = @"SELECT a.* from XYH_HU_HUMANMANAGE as a where";

            if (condition?.CheckStatu > 0 || condition?.HumanType > 0)
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


            if (condition?.SearchTimeType > 0)
            {
                switch (condition?.SearchTimeType)
                {
                    case 1: {
                            sql += connectstr + @"(a.`CreateTime`<='" + condition.CreateDateStart.Value + "'";
                            connectstr = " and ";
                            sql += connectstr + @"a.`CreateTime`>='" + condition.CreateDateEnd.Value + "')";
                        } break;
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

            if (!string.IsNullOrEmpty(condition?.Organizate))
            {
                sql += connectstr + @"a.`OrganizateID`='" + condition.Organizate + "'";
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
                var query = _Store.DapperSelect<HumanInfo>(sql).ToList();
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
                Response.Extension = _mapper.Map<List<HumanInfoResponse>>(result);
            }
            catch (Exception e)
            {

                throw;
            }
            
            return Response;
        }
        #endregion
    }
}
