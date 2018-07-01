using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Stores;

namespace XYHHumanPlugin.Managers
{
    public class MonthManager
    {
        public MonthManager(IHumanManageStore stor, IMapper mapper)
        {
            _Store = stor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IHumanManageStore _Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<MonthInfo> GetLastMonth(CancellationToken cancellationToken = default(CancellationToken))
        {
            //var result = await _Store.GetMonthAsync(a => a.Where(b => b.ID == "000"));
            string sql = @"SELECT a.* from XYH_HU_MONTH as a where a.`ID`!='' order by a.`SettleTime` ASC LIMIT 0,2";
            var query = _Store.DapperSelect<MonthInfo>(sql).ToList();
            if (query.Count < 1)
            {
                return null;
            }
             return query[0];
        }

        public virtual async Task DeleteMonth(DateTime date, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _Store.GetListMonthAsync(a => a.Where(b => b.SettleTime.Value.ToString("Y") == date.ToString("Y")));
            foreach (var item in result)
            {
                await DeleteMonthSalaryForm(item.ID, cancellationToken);
                await DeleteMonthAttendanceForm(item.ID, cancellationToken);
            }

            if (result.Count > 0)
            {
                await _Store.DeleteListAsync(result, cancellationToken);
            }
        }

        public async Task<List<MonthFormResponse>> GetMonthForm(DateTime tf)
        {
            //var tf = await GetLastMonth();
            if (tf != null)
            {
                var lst = new List<MonthFormResponse>();
                var month = await _Store.GetMonthAsync(a => a.Where(b => b.SettleTime.Value.ToString("Y") == tf.ToString("Y")));

                if (month != null)
                {
                    if (await GetMonthSalaryFormAndAttendanceForm(lst, month.ID, month.SettleTime.GetValueOrDefault()))
                    {
                        return lst;
                    }
                }
                
            }
            return null;
        }

        public virtual async Task<SearchMonthInfoResponse> GetAllMonthInfo(XYHHumanPlugin.Dto.Request.MonthRequest req, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await _Store.GetListMonthAsync(a => a.Where(b => b.ID != ""), cancellationToken);

            List<MonthInfo> info = new List<MonthInfo>();
            var begin = (req.pageIndex) * req.pageSize;
            var end = (begin + req.pageSize) > list.Count ? list.Count : (begin + req.pageSize);
            for (; begin < end; begin++)
            {
                info.Add(list.ElementAt(begin));
            }

            if (list != null)
            {
                SearchMonthInfoResponse result = new SearchMonthInfoResponse();
                result.extension = _mapper.Map<List<MonthInfoResponse>>(info);
                result.pageIndex = req.pageIndex;
                result.pageSize = req.pageSize;
                result.totalCount = list.Count;

                var tf = await GetLastMonth();
                if (tf != null)
                {
                    result.lastTime = tf.SettleTime;
                }
                else
                    result.lastTime = DateTime.Now;
                

                return result;
            }
            return null;
        }

        public virtual async Task<bool> CreateMonth(UserInfo user, DateTime date, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<MonthInfo> monthlist = new List<MonthInfo>();
            MonthInfo month = new MonthInfo();
            month.ID = Guid.NewGuid().ToString();
            month.SettleTime = date;
            month.OperName = user.UserName;
            month.OperID = user.Id;
            
            await _Store.CreateMonthAsync(_mapper.Map<SimpleUser>(user), month, cancellationToken);

            List<HumanInfo> humanlist = await _Store.GetHumanListAsync(a => a.Where(b => b.StaffStatus == StaffStatus.Regular && b.Id != ""));//入职员工
            if (await CreateMonthSalaryForm(month.ID, humanlist))
            {
                if (await CreateMonthAttendanceForm(month.ID, month.AttendanceForm, humanlist))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> GetMonthSalaryFormAndAttendanceForm(List<MonthFormResponse> lst, string monthid, DateTime settletime, CancellationToken cle = default(CancellationToken))
        {
            var salarylst = await _Store.GetListSalaryFormAsync(a => a.Where(b => b.MonthID == monthid));
            foreach (var item in salarylst)
            {
                var human = await _Store.GetHumanAsync(a => a.Where(b => b.Id == item.HumanID));
                if (human != null)
                {
                    var tf = await _Store.GetStationAsync(a => a.Where(b => b.ID == human.Position));
                    //var sc = await _Store.GetSocialInfoAsync(human.IDCard);
                    if (tf != null)
                    {
                        var it = new MonthFormResponse();
                        it.A1 = lst.Count;
                        it.A2 = human.IDCard;
                        it.A3 = human.Id;
                        it.A4 = human.Name;
                        it.A5 = await _Store.GetOrganizationFullName(human.DepartmentId);
                        it.A6 = tf.PositionName;
                        it.A8 = item.BaseSalary.GetValueOrDefault();
                        it.A9 = 0;//交通补贴暂无
                        it.A10 = 0;//通信补贴暂无
                        it.A11 = item.Subsidy.GetValueOrDefault();//岗位补贴
                        var trp = await _Store.GetRewardPunishmentListAsync(a => a.Where(b => b.UserID == human.Id&&(b.WorkDate.Value.Year==settletime.Year&& b.WorkDate.Value.Month == settletime.Month)));
                        it.A131 = trp.Sum(a => { if (a.Type == 1) { return a.Money; } return 0; });
                        it.A161 = trp.Sum(a => { if (a.Type == 2) { return a.Money; } return 0; });
                        it.A17 = trp.Sum(a => { if (a.Type == 3) { return a.Money; } return 0; });
                        it.A18 = human.PortBack.GetValueOrDefault();
                        it.A20 = 0;////意外险暂无
                        it.A21 = human.ClothesBack.GetValueOrDefault();
                        it.A22 = human.HumanSocialSecurity!=null? human.HumanSocialSecurity.EndowmentInsurance:0;
                        it.A23 = human.HumanSocialSecurity != null ? human.HumanSocialSecurity.UnemploymentInsurance:0;
                        it.A24 = human.HumanSocialSecurity != null ? human.HumanSocialSecurity.MedicalInsurance:0;
                        it.A25 = human.HumanSocialSecurity != null ? human.HumanSocialSecurity.EmploymentInjuryInsurance:0;
                        it.A26 = 0;////公积金暂无

                        var att = await _Store.GetAttendenceListAsync(a => a.Where(b => b.UserID == human.Id&& b.Date.Value.Year == settletime.Year&& b.Date.Value.Month == settletime.Month));
                        if (att!= null && att.Count > 0)
                        {
                            it.A7 = att[0].Normal;
                            it.A12 = 0;//加班暂无
                            it.A13 = 0;//效绩奖励暂无
                            it.A14 = att[0].Late;
                            it.A15 = att[0].Matter;
                            it.A16 = att[0].Absent;
                        }

                        float a1 = 0, a2 = 0, a3 = 0;
                        //只算迟到 事假 旷工的扣薪
                        var atts = await _Store.GetListAttendanceSettingAsync();
                        if (atts != null && atts.Count > 0)
                        {
                            foreach (var attsitem in atts)
                            {
                                if (attsitem.Type == 7)
                                {
                                    a1 = (attsitem.Money * it.A14) / attsitem.Times;
                                }
                                else if (attsitem.Type == 2)
                                {
                                    a2 = (attsitem.Money * it.A15) / attsitem.Times;
                                }
                                else if (attsitem.Type == 8)
                                {
                                    a3 = (attsitem.Money * it.A16) / attsitem.Times;
                                }
                            }
                        }

                        //算总
                        it.A19 = it.A8 + it.A9 + it.A10 + it.A11 + it.A12 + it.A13 + it.A131 - it.A14 - it.A15 - it.A16 - it.A161 - it.A17 - it.A18-a1-a2-a3;
                        it.A27 = it.A19 - it.A20 - float.Parse(it.A21.ToString()) - float.Parse(it.A22.ToString())
                            - float.Parse(it.A23.ToString()) - float.Parse(it.A24.ToString()) - float.Parse(it.A25.ToString()) - it.A26;

                        lst.Add(it);
                    }
                }
            }

            return lst.Count > 0;
        }
        

        private async Task<bool> CreateMonthSalaryForm(string monthid,List<HumanInfo> humaninfolist, CancellationToken cle = default(CancellationToken))
        {
            try
            {
                List<SalaryFormInfo> lst = new List<SalaryFormInfo>();
                foreach (var item in humaninfolist)
                {
                    SalaryFormInfo salary = new SalaryFormInfo();
                    salary.ID = Guid.NewGuid().ToString();
                    salary.MonthID = monthid;
                    salary.HumanID = item.Id;
                    salary.ClothesBack = item.ClothesBack;
                    salary.AdministrativeBack = item.AdministrativeBack;
                    salary.PortBack = item.PortBack;
                    salary.OtherBack = item.OtherBack;

                    lst.Add(salary);
                }

                await _Store.CreateMonthSalaryListAsync(lst, cle);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
            return false;
        }

        private async Task<bool> CreateMonthAttendanceForm(string monthid, string attendanceid, List<HumanInfo> humaninfolist, CancellationToken cle = default(CancellationToken))
        {
            
            return true;
        }

        private async Task<bool> DeleteMonthSalaryForm(string monthid, CancellationToken cle = default(CancellationToken))
        {
            var result = await _Store.GetListSalaryFormAsync(a => a.Where(b => b.MonthID == monthid));
            await _Store.DeleteListAsync(result, cle);
            return true;
        }

        private async Task<bool> DeleteMonthAttendanceForm(string monthid, CancellationToken cle = default(CancellationToken))
        {
            return false;
        }
    }
}
