using ApplicationCore.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Stores;
using AttendanceSettingInfoRequest = XYHHumanPlugin.Dto.Response.AttendanceSettingInfoResponse;
using AttendanceInfoRequest = XYHHumanPlugin.Dto.Response.AttendanceInfoResponse;

namespace XYHHumanPlugin.Managers
{
    public class AttendanceManager
    {
        public AttendanceManager(IHumanManageStore stor, IMapper mapper)
        {
            _Store = stor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IHumanManageStore _Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<List<AttendanceSettingInfoResponse>> GetAttendenceSetting(CancellationToken cancellationToken = default(CancellationToken))
        {
            var tlst = _mapper.Map<List<AttendanceSettingInfoResponse>>(await _Store.GetListAttendanceSettingAsync(cancellationToken));
            if (tlst == null && tlst.Count < 1)//返回空的设置表
            {
                List<AttendanceSettingInfoResponse> templst = new List<AttendanceSettingInfoResponse>();
                templst.Add(new AttendanceSettingInfoResponse() { Type = 1, Times = 0, Money = 0 });//调休
                templst.Add(new AttendanceSettingInfoResponse() { Type = 2, Times = 0, Money = 0 });//事假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 3, Times = 0, Money = 0 });//病假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 4, Times = 0, Money = 0 });//年假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 5, Times = 0, Money = 0 });//婚假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 6, Times = 0, Money = 0 });//丧假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 7, Times = 0, Money = 0 });//迟到
                templst.Add(new AttendanceSettingInfoResponse() { Type = 8, Times = 0, Money = 0 });//旷工
                return templst;
            }
            return tlst;
        }

        public virtual async Task SetAttendenceSetting(List<AttendanceSettingInfoRequest> lst, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (lst==null || lst.Count<1)
            {
                throw new ArgumentNullException(nameof(lst));
            }

            foreach (var item in lst)
            {
                await _Store.SetAttendanceSettingAsync(_mapper.Map<AttendanceSettingInfo>(item), cancellationToken);
            }
        }

        public virtual async Task SetAttendence(List<AttendanceInfoResponse> lst, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (lst == null || lst.Count < 1)
            {
                throw new ArgumentNullException(nameof(lst));
            }

            await _Store.AddAttendanceAsync(_mapper.Map<List<AttendanceInfo>>(lst), cancellationToken);
        }

        public virtual async Task<HumanSearchResponse<HumanInfoResponse>> SearchAttendenceInfo(UserInfo user, HumanSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var Response = new HumanSearchResponse<HumanInfoResponse>();
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
                Response.Extension = _mapper.Map<List<HumanInfoResponse>>(result);

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
    }
}
