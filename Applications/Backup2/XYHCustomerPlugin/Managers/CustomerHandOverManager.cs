using ApplicationCore;
using ApplicationCore.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    /// <summary>
    /// 客户移交
    /// </summary>
    public class CustomerHandOverManager
    {
        #region 成员

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerInfoStore _icustomerInfoStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerDemandStore _icustomerDemandStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerReportStore _icustomerReportStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected IAboutLookStore _iaboutLookStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected IBeltLookStore _ibeltLookStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerFollowUpStore _icustomerFollowUpStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerPoolStore _icustomerPoolStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerPoolDefineStore _icustomerPoolDefineStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerTransactionsStore _icustomerTransactionsStore { get; }

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerTransactionsFollowUpStore _icustomerTransactionsFollowUpStore { get; }

        /// <summary>
        /// 映射
        /// </summary>
        protected IMapper _mapper { get; }

        #endregion

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="icustomerInfoStore"></param>
        /// <param name="icustomerDemandStore"></param>
        /// <param name="icustomerReportStore"></param>
        /// <param name="iaboutLookStore"></param>
        /// <param name="ibeltLookStore"></param>
        /// <param name="icustomerFollowUpStore"></param>
        /// <param name="icustomerPoolStore"></param>
        /// <param name="icustomerPoolDefineStore"></param>
        /// <param name="mapper"></param>
        public CustomerHandOverManager(ICustomerInfoStore icustomerInfoStore,
            ICustomerDemandStore icustomerDemandStore,
            ICustomerReportStore icustomerReportStore,
            IAboutLookStore iaboutLookStore,
            IBeltLookStore ibeltLookStore,
            ICustomerFollowUpStore icustomerFollowUpStore,
            ICustomerPoolStore icustomerPoolStore,
            ICustomerPoolDefineStore icustomerPoolDefineStore,
            IMapper mapper)
        {
            _icustomerInfoStore = icustomerInfoStore ?? throw new ArgumentNullException(nameof(icustomerInfoStore));
            _icustomerDemandStore = icustomerDemandStore ?? throw new ArgumentNullException(nameof(icustomerDemandStore));
            _icustomerReportStore = icustomerReportStore ?? throw new ArgumentNullException(nameof(icustomerReportStore));
            _iaboutLookStore = iaboutLookStore ?? throw new ArgumentNullException(nameof(iaboutLookStore));
            _ibeltLookStore = ibeltLookStore ?? throw new ArgumentNullException(nameof(ibeltLookStore));
            _icustomerFollowUpStore = icustomerFollowUpStore ?? throw new ArgumentNullException(nameof(icustomerFollowUpStore));
            _icustomerPoolStore = icustomerPoolStore ?? throw new ArgumentNullException(nameof(icustomerPoolStore));
            _icustomerPoolDefineStore = icustomerPoolDefineStore ?? throw new ArgumentNullException(nameof(icustomerPoolDefineStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 转移客户
        /// </summary>
        /// <param name="simpleUser"></param>
        /// <param name="condition">移交条件</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage> HandOver(UserInfo simpleUser, HandOverCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = new ResponseMessage();
            response.Code = ResponseCodeDefines.ArgumentNullError;
            response.Message = "未完成客户转移";


            try
            {
                var userinfo = await _icustomerInfoStore.ListAsync(a => a.Where(b => b.UserId == condition.OldUserId && !b.IsDeleted), cancellationToken);


                if (condition.IsHandOverFU)
                {
                    //用户信息
                    userinfo = userinfo.Select(x =>
                    {
                        x.UserId = condition.NewUserId;
                        x.DepartmentId = condition.NewDepatmentId;
                        return x;
                    }).ToList();
                    //需求信息
                    var demand = await _icustomerDemandStore.ListAsync(a => a.Where(b => b.CustomerId == condition.OldUserId && !b.IsDeleted), cancellationToken);
                    demand = demand.Select(x =>
                    {
                        x.UserId = condition.NewUserId;
                        x.DepartmentId = condition.NewDepatmentId;
                        return x;
                    }).ToList();
                    ////报备信息
                    //var report = await _icustomerReportStore.ListAsync(a => a.Where(b => b.CustomerId == condition.OldUserId && !b.IsDeleted), cancellationToken);
                    //report = report.Select(x =>
                    //{
                    //    x.UserId = condition.NewUserId;
                    //    x.DepartmentId = condition.NewDepatmentId;
                    //    return x;
                    //}).ToList();
                    ////约看信息
                    //var aboutlook = await _iaboutLookStore.ListAsync(a => a.Where(b => b.CustomerId == condition.OldUserId && !b.IsDeleted), cancellationToken);
                    //aboutlook = aboutlook.Select(x =>
                    //{
                    //    x.UserId = condition.NewUserId;
                    //    x.DepartmentId = condition.NewDepatmentId;
                    //    return x;
                    //}).ToList();
                    //带看信息
                    var beltlook = await _ibeltLookStore.ListAsync(a => a.Where(b => b.CustomerId == condition.OldUserId && !b.IsDeleted), cancellationToken);
                    beltlook = beltlook.Select(x =>
                    {
                        x.UserId = condition.NewUserId;
                        x.DepartmentId = condition.NewDepatmentId;
                        return x;
                    }).ToList();
                    //跟进信息
                    var followup = await _icustomerFollowUpStore.ListAsync(a => a.Where(b => b.CustomerId == condition.OldUserId && !b.IsDeleted), cancellationToken);
                    followup = followup.Select(x =>
                    {
                        x.UserId = condition.NewUserId;
                        x.DepartmentId = condition.NewDepatmentId;
                        return x;
                    }).ToList();

                    //成交跟进信息
                    var tranfollowups = new List<CustomerTransactionsFollowUp>();
                    //成交信息
                    var transactions = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => b.CustomerId == condition.OldUserId && !b.IsDeleted), cancellationToken);


                    foreach (var item in transactions)
                    {
                        item.UserId = condition.NewUserId;
                        item.DepartmentId = condition.NewDepatmentId;

                        //获取每条的跟进信息
                        var tranfollowup = await _icustomerTransactionsFollowUpStore.ListAsync(a => a.Where(b => b.CustomerTransactionsId == item.Id && !b.IsDeleted), cancellationToken);
                        tranfollowup = tranfollowup.Select(f =>
                        {
                            f.UserId = condition.NewUserId;
                            return f;
                        }).ToList();
                        tranfollowups.AddRange(tranfollowup);
                    }



                    await _icustomerInfoStore.HandOverAsync(userinfo, demand, transactions, tranfollowups, cancellationToken);
                }
                else
                {
                    var newuserinfo = userinfo.Select(p =>
                    {
                        p.Id = Guid.NewGuid().ToString();
                        p.UserId = condition.NewUserId;
                        p.DepartmentId = condition.NewDepatmentId;
                        p.BeltNum = 0;
                        p.AboutNum = 0;
                        p.FollowUpNum = 0;
                        p.SourceId = p.Id;
                        return p;
                    }).ToList();

                    //删除以前的用户
                    await _icustomerInfoStore.HandOverAsync(simpleUser, userinfo, newuserinfo, cancellationToken);
                }
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "移交完成";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
            }



            return response;
        }

        /// <summary>
        /// 将一个或多个客户转移到公客池
        /// </summary>
        /// <param name="simpleUser"></param>
        /// <param name="CustomerIds">用户id数组s</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage<List<CustomerPoolResponse>>> HandOverCustomerPool(SimpleUser simpleUser, List<string> CustomerIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = new ResponseMessage<List<CustomerPoolResponse>>();
            response.Code = ResponseCodeDefines.ArgumentNullError;
            response.Message = "未完成客户转移";


            try
            {
                var customer = await _icustomerInfoStore.ListAsync(a => a.Where(x => CustomerIds.Contains(x.Id) && !x.IsDeleted));

                var customerfu = new List<CustomerFollowUp>();

                var customerpools = new List<CustomerPool>();

                var mig = new List<MigrationPoolHistory>();

                foreach (var item in customer)
                {
                    customerpools.Add(new CustomerPool
                    {
                        Id = Guid.NewGuid().ToString(),
                        DepartmentId = item.DepartmentId,
                        CustomerId = item.Id,
                        CreateTime = DateTime.Now,
                        JoinDate = DateTime.Now,
                        CreateUser = simpleUser.Id
                    });

                    mig.Add(new MigrationPoolHistory
                    {
                        CustomerId = item.Id,
                        MigrationTime = DateTime.Now,
                        OriginalDepartment = item.DepartmentId,
                        TargetDepartment = item.DepartmentId
                    });

                    customerfu.Add(new CustomerFollowUp
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = item.Id,
                        TypeId = CustomerFollowUpType.JoinPool,
                        UserId = item.UserId,
                        DepartmentId = item.DepartmentId,
                        FollowUpTime = DateTime.Now,
                        TrueName = item.CustomerName,
                        FollowUpContents = "加入公客池",
                        CustomerNo = item.CustomerNo,
                        IsRealFollow = false,
                        CreateTime = DateTime.Now,
                        CreateUser = simpleUser.Id
                    });
                }

                var respons = await _icustomerPoolStore.CreateListAsync(customerpools, mig, customerfu, cancellationToken);


                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "转移完成";
                response.Extension = _mapper.Map<List<CustomerPoolResponse>>(respons);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
            }



            return response;
        }
    }
}
