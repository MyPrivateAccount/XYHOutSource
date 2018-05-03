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
            month.OperName = user.Id;
            month.SalaryForm = Guid.NewGuid().ToString();
            month.AttendanceForm = Guid.NewGuid().ToString();

            await _Store.CreateMonthAsync(_mapper.Map<SimpleUser>(user), month, cancellationToken);


            List<HumanInfo> humanlist = await _Store.GetHumanListAsync(a => a.Where(b => b.StaffStatus > 1 && b.ID != ""));//入职员工
            if (await CreateMonthSalaryForm(month.ID, month.SalaryForm, humanlist))
            {
                if (await CreateMonthAttendanceForm(month.ID, month.AttendanceForm, humanlist))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> CreateMonthSalaryForm(string monthid, string salaryid, List<HumanInfo> humaninfolist, CancellationToken cle = default(CancellationToken))
        {
            try
            {
                foreach (var item in humaninfolist)
                {
                    SalaryFormInfo salary = new SalaryFormInfo();
                    salary.ID = salaryid;
                    salary.MonthID = monthid;
                    salary.HumanID = item.ID;
                    salary.BaseSalary = item.BaseSalary;
                    salary.Subsidy = item.Subsidy;
                    salary.ClothesBack = item.ClothesBack;
                    salary.AdministrativeBack = item.AdministrativeBack;
                    salary.PortBack = item.PortBack;
                    salary.OtherBack = item.OtherBack;

                    await _Store.CreateMonthSalaryAsync(salary, cle);
                }
                return false;
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
