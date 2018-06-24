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
    }
}
