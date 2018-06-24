using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Dto.Response;
using ApplicationCore.Models;

namespace XYHHumanPlugin.Stores
{
    public interface IHumanManageStore
    {
        IQueryable<HumanInfo> SimpleQuery { get; }

        IEnumerable<T> DapperSelect<T>(string sql);
        Task<HumanInfo> CreateAsync(SimpleUser userinfo, HumanInfo humaninfo, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateAsync(AnnexInfo humaninfo, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateAsync(FileInfo fileinfo, CancellationToken cancellationToken = default(CancellationToken));
        Task SetStationAsync(PositionInfo fileinfo, CancellationToken cancellationToken = default(CancellationToken));
        Task SetSalaryAsync(SalaryInfo salaryinfo, CancellationToken cle = default(CancellationToken));
        Task SetBlackAsync(BlackInfo salaryinfo, string id = null, CancellationToken cle = default(CancellationToken));
        Task SetAttendanceSettingAsync(AttendanceSettingInfo atteninfo, CancellationToken cle = default(CancellationToken));
        Task AddAttendanceAsync(List<AttendanceInfo> atteninfo, CancellationToken cle = default(CancellationToken));


        Task CreateMonthAsync(SimpleUser userinfo, MonthInfo monthinf, CancellationToken cancellationToken = default(CancellationToken));
        Task CreateMonthSalaryAsync(SalaryFormInfo forminfo, CancellationToken cle = default(CancellationToken));
        Task CreateMonthAttendanceAsync(AttendanceFormInfo forminfo , CancellationToken cle = default(CancellationToken));

        Task CreateListAsync(List<FileInfo> fileInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(HumanInfo userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAsync(MonthInfo monthinfo, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteStationAsync(PositionInfo monthinfo, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteSalaryAsync(SalaryInfo monthinfo, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteBlackAsync(BlackInfo monthinfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<HumanInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteListAsync(List<MonthInfo> monthList, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteListAsync(List<SalaryFormInfo> monthList, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteListAsync(List<AttendanceFormInfo> monthList, CancellationToken cancellationToken = default(CancellationToken));

        Task PreChangeHuman(SimpleUser userinfo, string modifyid, string huid, string info, string idcard, string checkaction, CancellationToken cancellationToken = default(CancellationToken));
        Task PreLeaveHuman(SimpleUser userinfo, string modifyid, string huid, string info, string idcard, string checkaction, CancellationToken cancellationToken = default(CancellationToken));
        Task PreBecomeHuman(SimpleUser userinfo, string modifyid, string huid, string info, string idcard, string checkaction, CancellationToken cancellationToken = default(CancellationToken));

        Task BecomeHuman(SocialInsurance info , string huid, CancellationToken cancellationToken = default(CancellationToken));
        Task LeaveHuman(LeaveInfo info, string huid, CancellationToken cancellationToken = default(CancellationToken));
        Task ChangeHuman(ChangeInfo info, string huid, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetOrganizationFullName(string departmentid);
        Task<List<Organizations>> GetAllOrganization();

        Task<SocialInsurance> GetSocialInfoAsync(string idcard);
        Task<TResult> GetHumanAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> GetMonthAsync<TResult>(Func<IQueryable<MonthInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> GetSalaryAsync<TResult>(Func<IQueryable<SalaryInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetFileListAsync<TResult>(Func<IQueryable<FileInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetScopeFileListAsync<TResult>(Func<IQueryable<AnnexInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> GetHumanListAsync<TResult>(Func<IQueryable<HumanInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<TResult> GetStationAsync<TResult>(Func<IQueryable<PositionInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetStationListAsync<TResult>(Func<IQueryable<PositionInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<TResult> GetModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetListModifyAsync<TResult>(Func<IQueryable<ModifyInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetListMonthAsync<TResult>(Func<IQueryable<MonthInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetListSalaryFormAsync<TResult>(Func<IQueryable<SalaryFormInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<TResult>> GetListAttendanceFormAsync<TResult>(Func<IQueryable<AttendanceFormInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<AttendanceSettingInfo>> GetListAttendanceSettingAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(HumanInfo buildingBase, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<HumanInfo> buildingBaseList, CancellationToken cancellationToken = default(CancellationToken));

        Task SaveAsync(HumanInfo humaninfo, CancellationToken cancellationToken = default(CancellationToken));
        Task<ModifyInfo> UpdateExamineStatus(string modifyId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken));

    }
}
