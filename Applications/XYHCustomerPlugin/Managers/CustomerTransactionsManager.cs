using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using ApplicationCore.Models;
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
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    /// <summary>
    /// 成交信息
    /// </summary>
    public class CustomerTransactionsManager
    {
        #region 成员

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerPhoneStore _icustomerPhoneStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerInfoStore _icustomerInfoStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerFollowUpStore _icustomerFollowUpStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerDemandStore _icustomerDemandStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerTransactionsStore _icustomerTransactionsStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerTransactionsFollowUpStore _icustomerTransactionsFollowUpStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerDealStore _icustomerDealStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected IBeltLookStore _ibeltLookStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected IOrganizationExpansionStore _iorganizationExpansionStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected PermissionExpansionManager _permissionExpansionManager { get; }

        /// <summary>
        /// 映射
        /// </summary>
        protected IMapper _mapper { get; }

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="icustomerTransactionsStore"></param>
        /// <param name="beltLookStore"></param>
        /// <param name="icustomerFollowUpStore"></param>
        /// <param name="icustomerInfoStore"></param>
        /// <param name="icustomerDemandStore"></param>
        /// <param name="icustomerDealStore"></param>
        /// <param name="organizationExpansionStore"></param>
        /// <param name="icustomerTransactionsFollowUpStore"></param>
        /// <param name="mapper"></param>
        /// <param name="permissionExpansionManager"></param>
        public CustomerTransactionsManager(ICustomerTransactionsStore icustomerTransactionsStore,
            IBeltLookStore beltLookStore, ICustomerFollowUpStore icustomerFollowUpStore,
            ICustomerTransactionsFollowUpStore icustomerTransactionsFollowUpStore,
            ICustomerInfoStore icustomerInfoStore,
            ICustomerDemandStore icustomerDemandStore,
            ICustomerDealStore icustomerDealStore,
            IOrganizationExpansionStore organizationExpansionStore,
            ICustomerPhoneStore customerPhoneStore,
            IMapper mapper,
             PermissionExpansionManager permissionExpansionManager)
        {
            _icustomerPhoneStore = customerPhoneStore ?? throw new ArgumentNullException(nameof(customerPhoneStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _icustomerInfoStore = icustomerInfoStore ?? throw new ArgumentNullException(nameof(icustomerInfoStore));
            _ibeltLookStore = beltLookStore ?? throw new ArgumentNullException(nameof(beltLookStore));
            _icustomerFollowUpStore = icustomerFollowUpStore ?? throw new ArgumentNullException(nameof(icustomerFollowUpStore));
            _icustomerTransactionsStore = icustomerTransactionsStore ?? throw new ArgumentNullException(nameof(icustomerTransactionsStore));
            _icustomerDemandStore = icustomerDemandStore ?? throw new ArgumentNullException(nameof(icustomerDemandStore));
            _icustomerTransactionsFollowUpStore = icustomerTransactionsFollowUpStore ?? throw new ArgumentNullException(nameof(icustomerTransactionsFollowUpStore));
            _icustomerDealStore = icustomerDealStore ?? throw new ArgumentNullException(nameof(icustomerDealStore));
            _iorganizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 根据Id获取成交信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="request">request</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<TransactionsFollowUpResponse>> FindBuildingFollowupByCustomerIdAsync(string userid, BuildingFollowUpRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => x.BuildingId == request.BuildingId && x.CustomerId == request.CustomerId && !x.IsDeleted).ToListAsync(cancellationToken);
            var fus = new List<TransactionsFollowUpResponse>();

            foreach (var asda in response)
            {
                fus.AddRange(_mapper.Map<List<TransactionsFollowUpResponse>>(asda.CustomerTransactionsFollowUps.ToList()));
            }
            return fus;
        }

        /// <summary>
        /// 根据Id获取成交信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<TransactionsResponse> FindByIdAsync(string userid, string id, CancellationToken cancellationToken = default(CancellationToken))
        {

            var response = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(b => b.Id == id && !b.IsDeleted).SingleOrDefaultAsync(cancellationToken);



            response.MainPhone = System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(response.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            response.DepartmentName = _iorganizationExpansionStore.GetFullName(response.DepartmentId);
            return _mapper.Map<TransactionsResponse>(response);
        }



        /// <summary>
        /// 获取业务员的组织名 到大区
        /// </summary>
        /// <returns></returns>
        public string GetMerchandiserOrganization(List<Organizations> Organizations, string oraganid)
        {
            var respons = "";

            var organization = Organizations.Where(x => x.Id == oraganid);
            var organ = organization.FirstOrDefault();
            if (organ == null)
            {
                return "未找到相应部门信息";
            }
            else
            {
                if (string.IsNullOrEmpty(organ.Type))
                {
                    var result = GetMerchandiserOrganization(Organizations, organ.ParentId);
                    if (result == "Top")
                    {
                        respons = organ.OrganizationName;
                    }
                    else
                    {
                        respons += result + "-" + organ.OrganizationName;
                    }
                }
                else
                {
                    return "Top";
                }
            }

            return respons;
        }

        /// <summary>
        /// 根据userid获取成交跟进信息
        /// </summary>
        /// <param name="userid">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<TransactionsResponse>> FindByUserIdAsync(string userid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => x.UserId == userid && !x.IsDeleted).OrderByDescending(x => x.FollowUpTime).ToListAsync(cancellationToken);
            response = response.Select(x =>
            {
                x.MainPhone = System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                return x;
            }).ToList();

            return _mapper.Map<List<TransactionsResponse>>(response);
        }

        /// <summary>
        /// 根据customerid获取成交跟进信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="customerid">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<TransactionsResponse>> FindByCustomerIdAsync(string userid, string customerid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => x.CustomerId == customerid && !x.IsDeleted && x.UserId == userid).OrderByDescending(x => x.FollowUpTime).ToListAsync(cancellationToken);
            response = response.Select(x =>
            {
                x.MainPhone = System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                return x;
            }).ToList();

            return _mapper.Map<List<TransactionsResponse>>(response);
        }

        /// <summary>
        /// 根据customerids获取成交跟进信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="transids">Id</param>
        /// <param name="valphone"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<TransactionsResponse>> FindByCustomerIdsAsync(string userid, List<string> transids, bool valphone, CancellationToken cancellationToken = default(CancellationToken))
        {
            var oragans = await _permissionExpansionManager.GetLowerDepartments(userid);

            var response = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => transids.Contains(x.Id) && !x.IsDeleted && oragans.Contains(x.DepartmentId)).OrderByDescending(x => x.FollowUpTime).ToListAsync(cancellationToken);
            response = response.Select(x =>
            {
                if (!valphone)
                    x.MainPhone = System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                else
                    x.MainPhone = new Help.EncDncHelper().Decrypt(x.MainPhone);
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                return x;
            }).ToList();

            return _mapper.Map<List<TransactionsResponse>>(response);
        }

        /// <summary>
        /// 根据buildingid获取驻场报备信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="buildingid">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<TransactionsResponse>> FindByBuildingIdAsync(string userid, string buildingid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, buildingid));
            if (!val.Extension)
                return null;

            var response = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => x.BuildingId == buildingid && !x.IsDeleted).OrderByDescending(x => x.FollowUpTime).ToListAsync(cancellationToken);

            response = response.Select(x =>
            {
                x.MainPhone = System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                return x;
            }).ToList();
            return _mapper.Map<List<TransactionsResponse>>(response);
        }

        /// <summary>
        /// 根据buildingid和报备状态获取报备信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="tranStatusByBuildingRequest">状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<TransactionsResponse>> FindCustomerByBuildingIdStatusAsync(string userid, TranStatusByBuildingRequest tranStatusByBuildingRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(tranStatusByBuildingRequest.BuildingId))
                return null;
            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, tranStatusByBuildingRequest.BuildingId));
            if (!val.Extension)
                return null;


            var response = _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(tranStatusByBuildingRequest.BuildingId))
            {
                response = response.Where(x => x.BuildingId == tranStatusByBuildingRequest.BuildingId);
            }
            if (tranStatusByBuildingRequest.TransactionsStatus != null && tranStatusByBuildingRequest.TransactionsStatus.Count != 0)
            {
                response = response.Where(x => tranStatusByBuildingRequest.TransactionsStatus.Contains(x.TransactionsStatus));
            }
            //查询主键信息
            if (!string.IsNullOrEmpty(tranStatusByBuildingRequest.KeyWord))
            {
                response = response.Where(x => x.MainPhone == new Help.EncDncHelper().Encrypt(tranStatusByBuildingRequest.KeyWord) || x.CustomerName.Contains(tranStatusByBuildingRequest.KeyWord) || x.UserTrueName.Contains(tranStatusByBuildingRequest.KeyWord) || x.UserPhone.Contains(tranStatusByBuildingRequest.KeyWord));
            }

            var query = await response.OrderByDescending(x => x.FollowUpTime).ToListAsync(cancellationToken);
            query = query.Select(x =>
            {
                x.MainPhone = System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                return x;
            }).ToList();
            return _mapper.Map<List<TransactionsResponse>>(query);
        }

        /// <summary>
        /// 根据buildingid获取驻场报备各状态行数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="buildingid">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<TransactionsStatisticsResponse> FindStatusNumByBuildingIdAsync(string userid, TransactionsStatusCountRequest transactionsStatusCountRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, transactionsStatusCountRequest.Buildingid));
            if (!val.Extension)
                return null;

            var all = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => x.BuildingId == transactionsStatusCountRequest.Buildingid && !x.IsDeleted).OrderByDescending(x => x.FollowUpTime).ToListAsync(cancellationToken);
            var response = new TransactionsStatisticsResponse();
            response.SubmitCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Submit);
            if (transactionsStatusCountRequest.IsToday != null && transactionsStatusCountRequest.IsToday == false)
            {
                response.ConfirmCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.AddDays(-1) <= DateTime.Now && x.ExpectedBeltTime.Value.AddDays(1) >= DateTime.Now && x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now);
            }
            else
            {
                response.ConfirmCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.Date == DateTime.Now.Date && x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now);
            }
            response.ReportCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Report);
            response.BeltLookCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.BeltLook);
            response.EndDealCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.EndDeal);
            response.OverTimeLapseCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.NoBeltLapse);
            response.ManualLapseCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.NoDealLapse);
            return response;
        }

        /// <summary>
        /// 根据buildingid获取驻场报备各状态行数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="transactionsStatusCountRequest">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<TransactionsStatisticsResponse> FindStatusNumByBuildingId2Async(string userid, TransactionsStatusCountRequest transactionsStatusCountRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, transactionsStatusCountRequest.Buildingid));
            if (!val.Extension)
                return null;

            var all = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => x.BuildingId == transactionsStatusCountRequest.Buildingid && !x.IsDeleted).OrderByDescending(x => x.FollowUpTime).ToListAsync(cancellationToken);
            var response = new TransactionsStatisticsResponse();
            response.SubmitCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Submit);
            if (transactionsStatusCountRequest.IsToday != null && transactionsStatusCountRequest.IsToday == false)
            {
                var dt1 = DateTime.Now.AddDays((int)transactionsStatusCountRequest.ReportEffectiveTime);
                var dt2 = DateTime.Now.AddDays(-(int)transactionsStatusCountRequest.ReportEffectiveTime);
                response.ConfirmCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value <= dt1 && x.ExpectedBeltTime.Value >= dt2 /*&& x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now*/);
            }
            else
            {
                var dt1 = DateTime.Now;
                var dt2 = DateTime.Now.AddDays(-(int)transactionsStatusCountRequest.ReportEffectiveTime);
                response.ConfirmCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.Date <= dt1 && x.ExpectedBeltTime.Value.Date >= dt2 /*&& x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now*/);
            }
            response.ReportCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Report);
            response.BeltLookCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.BeltLook);
            response.EndDealCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.EndDeal);
            response.OverTimeLapseCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.NoBeltLapse);
            response.ManualLapseCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.NoDealLapse);
            return response;
        }


        /// <summary>
        /// 根据buildingId获取驻场待确认行数
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="buildingid">Id</param>
        /// <param name="completephone"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<TransactionsFieldResponse> FindStatusNumByFieldAsync(string userid, string buildingid, bool completephone, CancellationToken cancellationToken = default(CancellationToken))
        {
            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, buildingid));
            if (!val.Extension)
                return null;

            var all = await _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => x.BuildingId == buildingid && !x.IsDeleted).OrderByDescending(x => x.FollowUpTime).ToListAsync(cancellationToken);

            var submintid = from ids in all
                            where ids.TransactionsStatus == TransactionsStatus.Submit
                            select ids.Id;

            var report = all.Where(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.Date == DateTime.Now.Date).ToList();
            report = report.Select(x =>
            {
                if (completephone)
                    x.MainPhone = new Help.EncDncHelper().Decrypt(x.MainPhone);
                else
                    x.MainPhone = System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                return x;
            }).ToList();


            var response = new TransactionsFieldResponse()
            {
                SubmitCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Submit),
                //ConfirmCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Confirm),
                TodayReportCount = all.Count(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.Date == DateTime.Now.Date),
                //TodayEndReportCount = all.Count(x => x.CustomerTransactionsFollowUps.Where(a => a.CreateTime.Value.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd") && a.TransactionsStatus == TransactionsStatus.Report).Count() != 0),
                SubmintIds = submintid.ToList(),
                TransactionsResponses = _mapper.Map<List<TransactionsResponse>>(report)
            };

            return response;
        }

        /// <summary>
        /// 验证报备重复
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="transactionValidation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<string>> ValidationTransaction(string userid, TransactionValidation transactionValidation, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = await _icustomerTransactionsStore.ListAsync(x => x.Where(y => y.BuildingId == transactionValidation.BuildingId && transactionValidation.CustomerIds.Contains(y.CustomerId) && y.ExpectedBeltTime <= transactionValidation.ExpectedLookTime && (y.TransactionsStatus == TransactionsStatus.Submit || y.TransactionsStatus == TransactionsStatus.Confirm || y.TransactionsStatus == TransactionsStatus.Report)));
            var response = query.GroupBy(x => x.CustomerId).Select(x => x.FirstOrDefault().CustomerName).ToList();
            return response;
        }

        /// <summary>
        /// 验证带看重复
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="transactionValidation"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<string>> ValidationBeltLook(string userid, TransactionValidation transactionValidation, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = new List<string>();
            var query = _icustomerTransactionsStore.CustomerTransactions.Where((y => y.BuildingId == transactionValidation.BuildingId && !y.IsDeleted && y.TransactionsStatus == TransactionsStatus.BeltLook)).Select(x => x.CustomerId);
            var phones = _icustomerPhoneStore.CustomerPhones.Where(y => transactionValidation.CustomerIds.Contains(y.CustomerId) && !y.IsDeleted).Select(x => x.Phone);
            var customerphones = await _icustomerPhoneStore.ListAsync(x => x.Where(y => query.Contains(y.CustomerId) && phones.Contains(y.Phone) && !y.IsDeleted));
            if (customerphones?.Count > 0)
            {
                response.Add((await _icustomerInfoStore.ListAsync(x => x.Where(y => transactionValidation.CustomerIds.Contains(y.Id)))).FirstOrDefault()?.CustomerName);
            }
            return response;
        }

        /// <summary>
        /// 查询报备信息接口
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="searchcondition"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<TransactionsResponse>> SearchSalesman(string userid, CustomerTransactionsSearchRequest searchcondition, CancellationToken cancellationToken = default(CancellationToken))
        {

            var response = _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => !x.IsDeleted && x.UserId == userid);

            var pagingResponse = new PagingResponseMessage<TransactionsResponse>();

            if (!string.IsNullOrEmpty(searchcondition.KeyWord))
            {
                response = response.Where(x => x.UserTrueName.Contains(searchcondition.KeyWord) || x.UserPhone.Contains(searchcondition.KeyWord) || x.CustomerName.Contains(searchcondition.KeyWord) || x.MainPhone == new Help.EncDncHelper().Encrypt(searchcondition.KeyWord));
            }
            if (searchcondition.Status != null)
            {
                response = response.Where(x => searchcondition.Status.Contains((int)x.TransactionsStatus));
            }
            if (!string.IsNullOrEmpty(searchcondition.BuildingId))
            {
                response = response.Where(x => x.BuildingId == searchcondition.BuildingId);
            }
            if (searchcondition.IsToDay != null)
            {
                //当预计带看时间小于当前时间时过滤 2018-3-1修改
                if (searchcondition.IsToDay == true)
                {
                    if (searchcondition.ReportEffectiveTime == null)
                    {
                        var dt = DateTime.Now.Date;
                        //默认为当天天
                        response = response.Where(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.Date == dt /*&& x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now*/);
                    }
                    else
                    {
                        var dt1 = DateTime.Now;
                        var dt2 = DateTime.Now.AddDays(-(int)searchcondition.ReportEffectiveTime);
                        response = response.Where(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.Date <= dt1 && x.ExpectedBeltTime.Value.Date >= dt2 /*&& x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now*/);
                    }
                }
                else
                {
                    if (searchcondition.ReportEffectiveTime == null)
                    {
                        searchcondition.ReportEffectiveTime = 24;
                    }
                    var dt1 = DateTime.Now.AddHours((int)searchcondition.ReportEffectiveTime);
                    var dt2 = DateTime.Now.AddHours(-(int)searchcondition.ReportEffectiveTime);
                    response = response.Where(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value <= dt1 && x.ExpectedBeltTime.Value >= dt2 /*&& x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now*/);

                }

            }
            var result = await response.OrderByDescending(x => x.CreateTime).Skip(searchcondition.PageIndex * searchcondition.PageSize).Take(searchcondition.PageSize).ToListAsync(cancellationToken);

            result = result.Select(x =>
            {
                x.MainPhone = new Help.EncDncHelper().Decrypt(x.MainPhone);
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                return x;
            }).OrderBy(a => a.FollowUpTime).ToList();


            pagingResponse.TotalCount = await response.CountAsync(cancellationToken);
            pagingResponse.PageIndex = searchcondition.PageIndex;
            pagingResponse.PageSize = searchcondition.PageSize;

            pagingResponse.Extension = _mapper.Map<List<TransactionsResponse>>(result);

            return pagingResponse;
        }

        /// <summary>
        /// 查询报备信息接口
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="searchcondition"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<TransactionsResponse>> Search(string userid, CustomerTransactionsSearchRequest searchcondition, CancellationToken cancellationToken = default(CancellationToken))
        {

            var response = _icustomerTransactionsStore.CustomerTransactionsAll().Where(x => !x.IsDeleted);

            if (string.IsNullOrEmpty(searchcondition.BuildingId))
                return new PagingResponseMessage<TransactionsResponse>();

            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, searchcondition.BuildingId));
            if (!val.Extension)
            {
                return null;
                //response = response.Where(x => x.UserId == userid);
            }
            var pagingResponse = new PagingResponseMessage<TransactionsResponse>();


            if (!string.IsNullOrEmpty(searchcondition.KeyWord))
            {
                response = response.Where(x => x.UserTrueName.Contains(searchcondition.KeyWord) || x.UserPhone.Contains(searchcondition.KeyWord) || x.CustomerName.Contains(searchcondition.KeyWord) || x.MainPhone == new Help.EncDncHelper().Encrypt(searchcondition.KeyWord));
            }
            if (searchcondition.Status != null)
            {
                response = response.Where(x => searchcondition.Status.Contains((int)x.TransactionsStatus));
            }
            if (!string.IsNullOrEmpty(searchcondition.BuildingId))
            {
                response = response.Where(x => x.BuildingId == searchcondition.BuildingId);
            }
            if (searchcondition.IsToDay != null)
            {
                //当预计带看时间小于当前时间时过滤 2018-3-1修改
                if (searchcondition.IsToDay == true)
                {
                    if (searchcondition.ReportEffectiveTime == null)
                    {
                        var dt = DateTime.Now.Date;
                        //默认为当天天
                        response = response.Where(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.Date == dt /*&& x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now*/);
                    }
                    else
                    {
                        var dt1 = DateTime.Now;
                        var dt2 = DateTime.Now.AddDays(-(int)searchcondition.ReportEffectiveTime);
                        response = response.Where(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value.Date <= dt1 && x.ExpectedBeltTime.Value.Date >= dt2 /*&& x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now*/);
                    }
                }
                else
                {
                    if (searchcondition.ReportEffectiveTime == null)
                    {
                        searchcondition.ReportEffectiveTime = 24;
                    }
                    var dt1 = DateTime.Now.AddHours((int)searchcondition.ReportEffectiveTime);
                    var dt2 = DateTime.Now.AddHours(-(int)searchcondition.ReportEffectiveTime);
                    response = response.Where(x => x.TransactionsStatus == TransactionsStatus.Confirm && x.ExpectedBeltTime.Value <= dt1 && x.ExpectedBeltTime.Value >= dt2 /*&& x.ExpectedBeltTime.Value.AddHours(12) >= DateTime.Now*/);

                }

            }
            var result = await response.OrderByDescending(x => x.CreateTime).Skip(searchcondition.PageIndex * searchcondition.PageSize).Take(searchcondition.PageSize).ToListAsync(cancellationToken);

            result = result.Select(x =>
            {
                if (searchcondition.ValPhone)
                {
                    x.TruePhone = new Help.EncDncHelper().Decrypt(x.MainPhone);
                    x.TruePhones = x.Phones.Select(a => new Help.EncDncHelper().Decrypt(a));
                }
                x.MainPhone = System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                x.Phones = x.Phones.Select(a => System.Text.RegularExpressions.Regex.Replace(new Help.EncDncHelper().Decrypt(a), "(\\d{3})\\d{4}(\\d{4})", "$1****$2"));
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                return x;
            }).OrderBy(a => a.FollowUpTime).ToList();


            pagingResponse.TotalCount = await response.CountAsync(cancellationToken);
            pagingResponse.PageIndex = searchcondition.PageIndex;
            pagingResponse.PageSize = searchcondition.PageSize;

            pagingResponse.Extension = _mapper.Map<List<TransactionsResponse>>(result);

            return pagingResponse;
        }


        /// <summary>
        /// 新增成交信息
        /// </summary>
        /// <param name="user">创建者</param>
        /// <param name="transactionsCreateRequest">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task CreateAsync(UserInfo user, TransactionsCreateRequest transactionsCreateRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            //业务员新增的时候会被拒绝掉
            //var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            //var val = (await _shopsInterface.IsResidentUser(user.Id, transactionsCreateRequest.BuildingId));
            //if (!val.Extension)
            //    return;

            if (transactionsCreateRequest == null)
            {
                throw new ArgumentNullException(nameof(transactionsCreateRequest));
            }
            var customerinfo = new List<CustomerInfo>();

            var customerfu = new List<CustomerFollowUp>();

            var customerta = new List<CustomerTransactions>();

            //需要更改的报备信息
            var customertaup = new List<CustomerTransactions>();

            var listfu = new List<CustomerTransactionsFollowUp>();

            foreach (var item in transactionsCreateRequest.CustomerIds)
            {
                var customerInfo = await _icustomerInfoStore.GetAsync(a => a.Where(b => b.Id == item), cancellationToken);
                if (customerInfo == null) break;
                customerInfo.FollowUpNum++;
                customerInfo.FollowupTime = DateTime.Now;
                customerInfo.RateProgress = RateProgress.TakeALookAt;
                customerinfo.Add(customerInfo);


                //插入跟进信息
                customerfu.Add(new CustomerFollowUp
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = item,
                    TypeId = CustomerFollowUpType.CustomerReported,
                    UserId = user.Id,
                    DepartmentId = user.OrganizationId,
                    FollowUpTime = DateTime.Now,
                    TrueName = customerInfo.CustomerName,
                    FollowUpContents = "客户报备",
                    CustomerNo = customerInfo.CustomerNo,
                    IsRealFollow = false,
                    CreateTime = DateTime.Now,
                    CreateUser = user.Id

                });
                var customertransactions = _mapper.Map<CustomerTransactions>(transactionsCreateRequest);

                //var oldcustomertran = await _icustomerTransactionsStore.GetAsync(x => x.Where(y => y.UserId == user.Id && y.CustomerId == item && !y.IsDeleted && y.BuildingId == transactionsCreateRequest.BuildingId && y.TransactionsStatus != TransactionsStatus.BeltLook && y.TransactionsStatus != TransactionsStatus.EndDeal && y.TransactionsStatus != TransactionsStatus.NoBeltLapse && y.TransactionsStatus != TransactionsStatus.NoDealLapse && y.ExpectedBeltTime == transactionsCreateRequest.ExpectedBeltTime));


                //if (oldcustomertran != null)
                //{
                //    //报备基本信息
                //    customertransactions.Id = oldcustomertran.Id;
                //    customertransactions.CreateUser = user.Id;
                //    customertransactions.CreateTime = DateTime.Now;
                //    customertransactions.UserId = user.Id;
                //    customertransactions.DepartmentId = user.OrganizationId;
                //    customertransactions.ReportTime = oldcustomertran.ReportTime;
                //    customertransactions.CustomerName = customerInfo.CustomerName;
                //    customertransactions.MainPhone = customerInfo.MainPhone;
                //    customertransactions.FollowUpTime = DateTime.Now;
                //    customertransactions.CustomerId = item;

                //    customertaup.Add(customertransactions);
                //}
                //else
                //{
                //报备基本信息
                customertransactions.Id = Guid.NewGuid().ToString();
                customertransactions.CreateUser = user.Id;
                customertransactions.CreateTime = DateTime.Now;
                customertransactions.UserId = user.Id;
                customertransactions.DepartmentId = user.OrganizationId;
                customertransactions.ReportTime = DateTime.Now;
                customertransactions.CustomerName = customerInfo.CustomerName;
                customertransactions.MainPhone = customerInfo.MainPhone;
                customertransactions.FollowUpTime = DateTime.Now;
                customertransactions.CustomerId = item;



                customerta.Add(customertransactions);


                //}

                //新增一条成交跟进信息

                listfu.Add(new CustomerTransactionsFollowUp()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerTransactionsId = customertransactions.Id,
                    Contents = "申请报备",
                    CreateTime = DateTime.Now,
                    UserId = user.Id,
                    CreateUser = user.Id,
                    TransactionsStatus = customertransactions.TransactionsStatus,
                    MarkTime = customertransactions.ExpectedBeltTime

                });




            }
            await _icustomerTransactionsStore.CreateAndUpdateUserInfoAsync(customerfu, customerinfo, customerta, customertaup, cancellationToken);

            //放在同一个事务一起就会报错 很奇怪
            await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);



            //transactionsCreateRequest.CustomerInfoCreateRequest.MainPhone = new Help.EncDncHelper().Encrypt(transactionsCreateRequest.CustomerInfoCreateRequest.MainPhone);
            //string truename = "";
            //if (string.IsNullOrEmpty(transactionsCreateRequest.CustomerId))
            //{
            //    //信息是否完善
            //    if (string.IsNullOrEmpty(transactionsCreateRequest.CustomerInfoCreateRequest.MainPhone) && string.IsNullOrEmpty(transactionsCreateRequest.CustomerInfoCreateRequest.CustomerName))
            //    {
            //        throw new ArgumentNullException(nameof(transactionsCreateRequest));
            //    }
            //    else
            //    {
            //        var customerInfo = await _icustomerInfoStore.CustomerInfoAll().Where(b => b.UserId == user.Id && b.MainPhone == transactionsCreateRequest.CustomerInfoCreateRequest.MainPhone && !b.IsDeleted).FirstOrDefaultAsync(cancellationToken);

            //        if (customerInfo != null)
            //        {
            //            //修改核心意向

            //            var demand = await _icustomerDemandStore.GetAsync(a => a.Where(b => b.Id == customerInfo.CustomerDemand.Id), cancellationToken);


            //            if (transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceEnd == null)
            //            {
            //                demand.PriceEnd = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceEnd;
            //            }
            //            if (transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceStart == null)
            //            {
            //                demand.PriceStart = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceStart;
            //            }
            //            if (transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageEnd == null)
            //            {
            //                demand.AcreageEnd = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageEnd;
            //            }
            //            if (transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageStart == null)
            //            {
            //                demand.AcreageStart = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageStart;
            //            }
            //            await _icustomerDemandStore.UpdateAsync(demand, cancellationToken);

            //            transactionsCreateRequest.CustomerId = customerInfo.Id;
            //            truename = customerInfo.CustomerName;
            //        }
            //        else
            //        {
            //            var customerinfo = _mapper.Map<CustomerInfo>(transactionsCreateRequest.CustomerInfoCreateRequest);
            //            //客户基本信息
            //            customerinfo.Id = Guid.NewGuid().ToString();
            //            customerinfo.CreateUser = user.Id;
            //            customerinfo.CreateTime = DateTime.Now;
            //            customerinfo.UserId = user.Id;
            //            customerinfo.DepartmentId = user.OrganizationId;
            //            //最后跟进时间为创建时间
            //            customerinfo.FollowupTime = customerinfo.CreateTime;
            //            if (customerinfo.Birthday != null)
            //            {
            //                customerinfo.Age = DateTime.Now.Year - customerinfo.Birthday.Value.Year;
            //            }
            //            customerinfo.CustomerStatus = CustomerStatus.ExistingCustomers;


            //            //客户需求信息
            //            customerinfo.CustomerDemand.Id = Guid.NewGuid().ToString();
            //            customerinfo.CustomerDemand.UserId = user.Id;
            //            customerinfo.CustomerDemand.DepartmentId = user.OrganizationId;
            //            customerinfo.CustomerDemand.CustomerId = customerinfo.Id;
            //            customerinfo.CustomerDemand.CreateUser = user.Id;
            //            customerinfo.CustomerDemand.CreateTime = DateTime.Now;

            //            //客户电话信息
            //            customerinfo.CustomerPhones = customerinfo.CustomerPhones.Select(q =>
            //            {
            //                q.Id = Guid.NewGuid().ToString();
            //                q.CustomerId = customerinfo.Id;
            //                q.CreateUser = user.Id;
            //                q.CreateTime = DateTime.Now;

            //                return q;
            //            });


            //            await _icustomerInfoStore.CreateAsync(customerinfo, cancellationToken);


            //            truename = customerinfo.CustomerName;
            //            transactionsCreateRequest.CustomerId = customerinfo.Id;
            //        }
            //    }
            //}
            //else
            //{
            //    //更新原来的用户数据跟进时间
            //    var customerinfo = await _icustomerInfoStore.GetAsync(a => a.Where(b => b.Id == transactionsCreateRequest.CustomerId && !b.IsDeleted));

            //    if (!string.IsNullOrEmpty(transactionsCreateRequest.CustomerInfoCreateRequest.Mark))
            //    {
            //        customerinfo.Mark = transactionsCreateRequest.CustomerInfoCreateRequest.Mark;
            //    }


            //    customerinfo.FollowUpNum++;
            //    customerinfo.FollowupTime = DateTime.Now;

            //    customerinfo.Sex = transactionsCreateRequest.CustomerInfoCreateRequest.Sex;
            //    customerinfo.CustomerName = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerName;
            //    customerinfo.Mark = transactionsCreateRequest.CustomerInfoCreateRequest.Mark;
            //    //暂未写修改电话逻辑

            //    //修改核心意向
            //    var customerdemand = await _icustomerDemandStore.GetAsync(a => a.Where(b => b.Id == transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.Id && !b.IsDeleted));
            //    customerdemand.PriceStart = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceStart;
            //    customerdemand.PriceEnd = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceEnd;
            //    customerdemand.AcreageStart = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageStart;
            //    customerdemand.AcreageEnd = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageEnd;
            //    customerdemand.AreaFullName = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AreaFullName;
            //    customerdemand.AreaId = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AreaId;
            //    customerdemand.CityId = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.CityId;
            //    customerdemand.ProvinceId = transactionsCreateRequest.CustomerInfoCreateRequest.CustomerDemandRequest.ProvinceId;


            //    //插入跟进信息
            //    var followup = new CustomerFollowUp
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        CustomerId = transactionsCreateRequest.CustomerId,
            //        TypeId = CustomerFollowUpType.CustomerReported,
            //        UserId = user.Id,
            //        DepartmentId = user.OrganizationId,
            //        FollowUpTime = DateTime.Now,
            //        TrueName = customerinfo.CustomerName,
            //        FollowUpContents = "客户报备",
            //        CustomerNo = customerinfo.CustomerNo,
            //        IsRealFollow = false,
            //        CreateTime = DateTime.Now,
            //        CreateUser = user.Id

            //    };
            //    await _icustomerFollowUpStore.CreateAndUpdateUserInfoAsync(followup, customerinfo, customerdemand, cancellationToken);
            //    truename = customerinfo.CustomerName;
            //}

            //var customertransactions = _mapper.Map<CustomerTransactions>(transactionsCreateRequest);
            ////报备基本信息
            //customertransactions.Id = Guid.NewGuid().ToString();
            //customertransactions.CreateUser = user.Id;
            //customertransactions.CreateTime = DateTime.Now;
            //customertransactions.UserId = user.Id;
            //customertransactions.DepartmentId = user.OrganizationId;
            //customertransactions.ReportTime = DateTime.Now;
            //customertransactions.CustomerName = truename;
            //customertransactions.MainPhone = transactionsCreateRequest.CustomerInfoCreateRequest.MainPhone;
            //customertransactions.FollowUpTime = DateTime.Now;


            ////新增一条成交跟进信息
            //var listfu = new List<CustomerTransactionsFollowUp>();
            //listfu.Add(new CustomerTransactionsFollowUp()
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    CustomerTransactionsId = customertransactions.Id,
            //    Contents = "预计带看",
            //    CreateTime = DateTime.Now,
            //    UserId = user.Id,
            //    CreateUser = user.Id,
            //    TransactionsStatus = customertransactions.TransactionsStatus,
            //    MarkTime = customertransactions.ExpectedBeltTime

            //});

            //customertransactions.CustomerTransactionsFollowUps = listfu;

            //await _icustomerTransactionsStore.CreateAsync(customertransactions, cancellationToken);

            //return _mapper.Map<TransactionsCreateResponse>(customertransactions);


            //if (transactionsRequest == null)
            //{
            //    throw new ArgumentNullException(nameof(transactionsRequest));
            //}

            //var respons = new ResponseMessage<TransactionsResponse>();

            //var tsfollowUp = _mapper.Map<CustomerTransactions>(transactionsRequest);
            //tsfollowUp.Id = Guid.NewGuid().ToString();
            //tsfollowUp.UserId = user.Id;
            //tsfollowUp.CreateTime = DateTime.Now;
            //tsfollowUp.CreateUser = user.Id;
            //tsfollowUp.TransactionsStatus = TransactionsStatus.Submit;


            //try
            //{

            //    await _icustomerTransactionsStore.CreateAsync(tsfollowUp, cancellationToken);

            //    respons.Code = ResponseCodeDefines.SuccessCode;
            //    respons.Message = "创建成功";
            //    respons.Extension = _mapper.Map<TransactionsResponse>(tsfollowUp);
            //}
            //catch (Exception e)
            //{
            //    respons.Code = ResponseCodeDefines.ServiceError;
            //    respons.Message = "服务器错误：" + e.ToString();
            //}

            //return respons;
        }

        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="transactionsids">请求数据</param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns> 
        public virtual async Task UpdateStatusConAsync(UserInfo user, List<string> transactionsids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerReports = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => transactionsids.Contains(b.Id) && b.TransactionsStatus == TransactionsStatus.Submit), cancellationToken);
            if (customerReports == null) return;

            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();


            var listfu = new List<CustomerTransactionsFollowUp>();

            var listbf = new List<BeltLook>();

            foreach (var item in customerReports)
            {
                var val = (await _shopsInterface.IsResidentUser(user.Id, item.BuildingId));
                if (val.Extension)
                {
                    item.TransactionsStatus = TransactionsStatus.Confirm;
                    item.FollowUpTime = DateTime.Now;

                    //新增一条成交跟进信息
                    listfu.Add(new CustomerTransactionsFollowUp()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerTransactionsId = item.Id,
                        Contents = JudgmentTransactionsStatus(TransactionsStatus.Confirm),
                        CreateTime = DateTime.Now,
                        UserId = user.Id,
                        CreateUser = user.Id,
                        TransactionsStatus = TransactionsStatus.Confirm,
                        MarkTime = DateTime.Now
                    });
                }
            }


            await _icustomerTransactionsStore.UpdateListAsync(customerReports, cancellationToken);


            await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);
        }

        /// <summary>
        /// 按楼盘将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="buildingid">请求数据</param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateStatusConByBuildingIdAsync(UserInfo user, string buildingid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(user.Id, buildingid));
            if (!val.Extension)
                return;

            var customerReports = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => b.BuildingId == buildingid && b.TransactionsStatus == TransactionsStatus.Submit), cancellationToken);
            if (customerReports == null) return;



            var listfu = new List<CustomerTransactionsFollowUp>();


            var coontents = string.Empty;


            customerReports = customerReports.Select(q =>
            {
                q.TransactionsStatus = TransactionsStatus.Confirm;
                q.FollowUpTime = DateTime.Now;
                //新增一条成交跟进信息
                listfu.Add(new CustomerTransactionsFollowUp()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerTransactionsId = q.Id,
                    Contents = JudgmentTransactionsStatus(TransactionsStatus.Confirm),
                    CreateTime = DateTime.Now,
                    UserId = user.Id,
                    CreateUser = user.Id,
                    TransactionsStatus = TransactionsStatus.Confirm,
                    MarkTime = q.ExpectedBeltTime
                });

                return q;
            }).ToList();
            await _icustomerTransactionsStore.UpdateListAsync(customerReports, cancellationToken);

            await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);
        }



        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="transactionsids">请求数据</param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns> 
        public virtual async Task UpdateStatusRepAsync(UserInfo user, List<string> transactionsids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerReports = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => transactionsids.Contains(b.Id) && b.TransactionsStatus == TransactionsStatus.Confirm), cancellationToken);
            if (customerReports == null) return;

            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();


            var listfu = new List<CustomerTransactionsFollowUp>();

            var listbf = new List<BeltLook>();

            foreach (var item in customerReports)
            {
                var val = (await _shopsInterface.IsResidentUser(user.Id, item.BuildingId));
                if (val.Extension)
                {
                    item.TransactionsStatus = TransactionsStatus.Report;
                    item.FollowUpTime = DateTime.Now;

                    //新增一条成交跟进信息
                    listfu.Add(new CustomerTransactionsFollowUp()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerTransactionsId = item.Id,
                        Contents = JudgmentTransactionsStatus(TransactionsStatus.Report),
                        CreateTime = DateTime.Now,
                        UserId = user.Id,
                        CreateUser = user.Id,
                        TransactionsStatus = TransactionsStatus.Report,
                        MarkTime = DateTime.Now
                    });
                }
            }


            await _icustomerTransactionsStore.UpdateListAsync(customerReports, cancellationToken);


            await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);
        }

        /// <summary>
        /// 按楼盘将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="buildingid">请求数据</param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateStatusRepByBuildingIdAsync(UserInfo user, string buildingid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(user.Id, buildingid));
            if (!val.Extension)
                return;

            var customerReports = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => b.BuildingId == buildingid && b.TransactionsStatus == TransactionsStatus.Confirm), cancellationToken);
            if (customerReports == null) return;



            var listfu = new List<CustomerTransactionsFollowUp>();

            var listcfu = new List<CustomerFollowUp>();

            var coontents = string.Empty;


            customerReports = customerReports.Select(q =>
            {
                q.TransactionsStatus = TransactionsStatus.Report;
                q.FollowUpTime = DateTime.Now;

                //新增一条成交跟进信息
                listfu.Add(new CustomerTransactionsFollowUp()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerTransactionsId = q.Id,
                    Contents = JudgmentTransactionsStatus(TransactionsStatus.Report),
                    CreateTime = DateTime.Now,
                    UserId = user.Id,
                    CreateUser = user.Id,
                    TransactionsStatus = TransactionsStatus.Report,
                    MarkTime = q.ExpectedBeltTime
                });

                return q;
            }).ToList();
            await _icustomerTransactionsStore.UpdateListAsync(customerReports, cancellationToken);

            await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);
        }

        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="transactionsids">请求数据</param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateStatusBleAsync(UserInfo user, List<string> transactionsids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerReports = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => transactionsids.Contains(b.Id) && b.TransactionsStatus == TransactionsStatus.Report), cancellationToken);
            if (customerReports == null) return;

            var listfu = new List<CustomerTransactionsFollowUp>();

            var listbf = new List<BeltLook>();

            var listcfu = new List<CustomerFollowUp>();

            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();


            foreach (var q in customerReports)
            {
                var val = (await _shopsInterface.IsResidentUser(user.Id, q.BuildingId));
                if (val.Extension)
                {
                    q.FollowUpTime = DateTime.Now;
                    q.TransactionsStatus = TransactionsStatus.BeltLook;
                    q.BeltLookId = q.UserId;
                    q.BeltLookTime = DateTime.Now;
                    listbf.Add(new BeltLook
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = q.UserId,
                        BeltState = BeltLookState.WaitDeal,
                        CustomerId = q.CustomerId,
                        DepartmentId = q.DepartmentId,
                        BeltHouse = q.BuildingName,
                        CreateTime = DateTime.Now,
                        InSiteUser = user.Id
                    });

                    //新增一条客户跟进信息
                    listcfu.Add(new CustomerFollowUp()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = q.CustomerId,
                        TypeId = CustomerFollowUpType.BeltLook,
                        UserId = q.UserId,
                        DepartmentId = q.DepartmentId,
                        FollowUpTime = DateTime.Now,
                        TrueName = q.CustomerName,
                        FollowUpContents = "带看客户",
                        CustomerNo = q.CustomerNo,
                        IsRealFollow = false,
                        CreateTime = DateTime.Now,
                        CreateUser = user.Id
                    });

                    //新增一条成交跟进信息
                    listfu.Add(new CustomerTransactionsFollowUp()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerTransactionsId = q.Id,
                        Contents = JudgmentTransactionsStatus(TransactionsStatus.BeltLook),
                        CreateTime = DateTime.Now,
                        UserId = user.Id,
                        CreateUser = user.Id,
                        TransactionsStatus = TransactionsStatus.BeltLook,
                        MarkTime = DateTime.Now
                    });
                }
            }


            await _ibeltLookStore.CreateListAsync(listbf, customerReports, listfu, listcfu, cancellationToken);
        }

        /// <summary>
        /// 再次带看
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="transactionsid">请求数据</param>
        /// <param name="expectedbelt">预计带看时间</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateStatusAgainBleAsync(UserInfo user, string transactionsid, DateTime expectedbelt, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerReport = await _icustomerTransactionsStore.GetAsync(a => a.Where(b => b.Id == transactionsid && b.UserId == user.Id), cancellationToken);
            if (customerReport == null) return;

            customerReport.Id = Guid.NewGuid().ToString();
            customerReport.ExpectedBeltTime = expectedbelt;





            var fu = new CustomerTransactionsFollowUp()
            {
                Id = Guid.NewGuid().ToString(),
                CustomerTransactionsId = customerReport.Id,
                Contents = "再次报备",
                CreateTime = DateTime.Now,
                UserId = user.Id,
                CreateUser = user.Id,
                TransactionsStatus = TransactionsStatus.Submit,
                MarkTime = expectedbelt
            };

            var customerfu = new CustomerFollowUp()
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = customerReport.CustomerId,
                TypeId = CustomerFollowUpType.CustomerReported,
                UserId = user.Id,
                DepartmentId = user.OrganizationId,
                FollowUpTime = DateTime.Now,
                TrueName = customerReport.CustomerName,
                FollowUpContents = "再次报备",
                CustomerNo = customerReport.CustomerNo,
                IsRealFollow = false,
                CreateTime = DateTime.Now,
                CreateUser = user.Id
            };

            //放在上面 TransactionsID为NULL 会报错
            //if (customerReport.TransactionsStatus == TransactionsStatus.BeltLook || customerReport.TransactionsStatus == TransactionsStatus.NoDealLapse)
            //{
            //    var trans = await _icustomerTransactionsStore.ListAsync(x => x.Where(y => !y.IsDeleted && y.BuildingId == customerReport.BuildingId && y.CustomerId == customerReport.CustomerId && (y.TransactionsStatus == TransactionsStatus.Submit || y.TransactionsStatus == TransactionsStatus.Confirm || y.TransactionsStatus == TransactionsStatus.Report) && y.ExpectedBeltTime == customerReport.ExpectedBeltTime));

            //    if (trans.Count > 0)
            //    {
            //        customerReport.Id = trans.FirstOrDefault().Id;
            //    }
            //    else
            //    {
            //        //报备基本信息
            //        customerReport.Id = null;
            //    }

            //}
            customerReport.TransactionsStatus = TransactionsStatus.Submit;

            await _ibeltLookStore.CreateAgainBeltLookAsync(customerReport, fu, customerfu, cancellationToken);
        }

        public string JudgmentTransactionsStatus(TransactionsStatus mark)
        {
            switch (mark)
            {
                case TransactionsStatus.Submit: return "提交报备";
                case TransactionsStatus.Confirm: return "报备确认";
                case TransactionsStatus.Report: return "向开发商报备";
                case TransactionsStatus.BeltLook: return "带看客户";
                case TransactionsStatus.EndDeal: return "成交客户";
                case TransactionsStatus.NoBeltLapse: return "未带看已失效";
                case TransactionsStatus.NoDealLapse: return "未成交已失效";
            }
            return "";
        }
    }
}
