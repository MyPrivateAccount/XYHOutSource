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
                templst.Add(new AttendanceSettingInfoResponse() { Type = 1, Times = 1, Money = 0 });//调休
                templst.Add(new AttendanceSettingInfoResponse() { Type = 2, Times = 1, Money = 0 });//事假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 3, Times = 1, Money = 0 });//病假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 4, Times = 1, Money = 0 });//年假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 5, Times = 1, Money = 0 });//婚假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 6, Times = 1, Money = 0 });//丧假
                templst.Add(new AttendanceSettingInfoResponse() { Type = 7, Times = 1, Money = 0 });//迟到
                templst.Add(new AttendanceSettingInfoResponse() { Type = 8, Times = 1, Money = 0 });//旷工
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

        public virtual async Task AddAttendence(List<AttendanceInfoRequest> lst, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (lst == null || lst.Count < 1)
            {
                throw new ArgumentNullException(nameof(lst));
            }

            await _Store.AddAttendanceAsync(_mapper.Map<List<AttendanceInfo>>(lst), cancellationToken);
        }

        public virtual async Task DeleteAttendence(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            await _Store.DeleteAttendenceAsync(new AttendanceInfo() {ID = id}, cancellationToken);
        }

        public virtual async Task<HumanSearchResponse<AttendanceInfoResponse>> SearchAttendenceInfo(UserInfo user, AttendenceSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var Response = new HumanSearchResponse<AttendanceInfoResponse>();
            var sql = @"SELECT a.* from XYH_HU_ATTENDANCE as a where";

            string connectstr = " ";
            if (!string.IsNullOrEmpty(condition?.KeyWord))
            {
                sql += connectstr + @"LOCATE('" + condition.KeyWord + "', a.`Name`)>0";
                connectstr = " and ";
            }
            else
            {
                sql += connectstr + @"a.`ID`!=''";
                connectstr = " and ";
            }

            if (condition?.CreateDate != null)
            {
                sql += connectstr + @"(a.`CreateTime`='" + condition.CreateDate + "'";
                connectstr = " and ";
            }
           
            try
            {
                var query = _Store.DapperSelect<AttendanceInfo>(sql).ToList();

                Response.ValidityContractCount = query.Count;
                Response.TotalCount = query.Count;

                List<AttendanceInfo> result = new List<AttendanceInfo>();
                var begin = (condition.pageIndex) * condition.pageSize;
                var end = (begin + condition.pageSize) > query.Count ? query.Count : (begin + condition.pageSize);

                for (; begin < end; begin++)
                {
                    result.Add(query.ElementAt(begin));
                }

                Response.PageIndex = condition.pageIndex;
                Response.PageSize = condition.pageSize;
                Response.Extension = _mapper.Map<List<AttendanceInfoResponse>>(result);
            }
            catch (Exception e)
            {

                throw;
            }

            return Response;
        }
    }
}
