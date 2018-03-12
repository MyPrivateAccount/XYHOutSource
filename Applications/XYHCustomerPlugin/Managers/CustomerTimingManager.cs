using ApplicationCore;
using ApplicationCore.Stores;
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
using XYHCustomerPlugin.Dto.Common;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    public class CustomerTimingManager
    {
        #region 成员

        protected ICustomerInfoStore _icustomerInfoStore { get; }

        protected IBeltLookStore _ibeltLookStore { get; }

        protected ICustomerDealStore _icustomerDealStore { get; }

        protected ICustomerPoolStore _icustomerPoolStore { get; }

        protected ICustomerPoolDefineStore _icustomerPoolDefineStore { get; }

        protected ICustomerTransactionsStore _icustomerTransactionsStore { get; }

        protected ICustomerTransactionsFollowUpStore _icustomerTransactionsFollowUpStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public CustomerTimingManager(
            ICustomerInfoStore icustomerInfoStore,
            IBeltLookStore ibeltLookStore,
            ICustomerDealStore icustomerDealStore,
            ICustomerPoolStore icustomerPoolStore,
            ICustomerPoolDefineStore icustomerPoolDefineStore,
            Stores.IOrganizationExpansionStore organizationExpansionStore,
            ICustomerTransactionsStore icustomerTransactionsStore,
            ICustomerTransactionsFollowUpStore icustomerTransactionsFollowUpStore,
            IMapper mapper)
        {
            _icustomerInfoStore = icustomerInfoStore ?? throw new ArgumentNullException(nameof(icustomerInfoStore));
            _ibeltLookStore = ibeltLookStore ?? throw new ArgumentNullException(nameof(ibeltLookStore));
            _icustomerDealStore = icustomerDealStore ?? throw new ArgumentNullException(nameof(icustomerDealStore));

            _icustomerPoolStore = icustomerPoolStore ?? throw new ArgumentNullException(nameof(icustomerPoolStore));
            _icustomerPoolDefineStore = icustomerPoolDefineStore ?? throw new ArgumentNullException(nameof(icustomerPoolDefineStore));
            _icustomerTransactionsStore = icustomerTransactionsStore ?? throw new ArgumentNullException(nameof(icustomerTransactionsStore));
            _icustomerTransactionsFollowUpStore = icustomerTransactionsFollowUpStore ?? throw new ArgumentNullException(nameof(icustomerTransactionsFollowUpStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 报备未确认但未进行报备预计带看时间大于24小时超期失效
        /// </summary>
        /// <returns></returns>
        public virtual async Task TimedTaskConfirmReport(CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => b.TransactionsStatus == TransactionsStatus.Submit && !b.IsDeleted && b.ExpectedBeltTime.Value.AddHours(24) < DateTime.Now), cancellationToken);

            if (query.Count == 0) return;

            var listfu = new List<CustomerTransactionsFollowUp>();


            var coontents = string.Empty;


            query = query.Select(q =>
            {
                q.TransactionsStatus = TransactionsStatus.NoBeltLapse;

                //新增一条成交跟进信息
                listfu.Add(new CustomerTransactionsFollowUp()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerTransactionsId = q.Id,
                    Contents = "已失效",
                    CreateTime = DateTime.Now,
                    UserId = "定时任务",
                    CreateUser = "定时任务",
                    TransactionsStatus = TransactionsStatus.NoBeltLapse
                });

                return q;
            }).ToList();
            await _icustomerTransactionsStore.UpdateListAsync(query, cancellationToken);

            await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);
        }

        /// <summary>
        /// 未向开发商报备超期失效
        /// </summary>
        /// <returns></returns>
        public virtual async Task TimedTaskSubmitReport(CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => (b.TransactionsStatus == TransactionsStatus.Confirm || b.TransactionsStatus == TransactionsStatus.Report) && !b.IsDeleted), cancellationToken);

            var customerReports = new List<CustomerTransactions>();

            var group = query.GroupBy(x => x.BuildingId);
            foreach (var item in group)
            {
                var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
                var val = (await _shopsInterface.GetBuilidngRule(item.Key)); if (val == null) continue;
                var lapeses = new List<CustomerTransactions>();
                if (val.Extension.EffectiveType == GatewayInterface.Dto.Response.EffectiveType.Days)
                {
                    lapeses = item.Where(x => x.ExpectedBeltTime.Value.AddDays(val.Extension.ValidityDay).Date <= DateTime.Now.Date && x.ExpectedBeltTime.Value.AddDays(val.Extension.ValidityDay).Date >= DateTime.Now.Date).ToList();
                }
                else if (val.Extension.EffectiveType == GatewayInterface.Dto.Response.EffectiveType.Hours)
                {
                    lapeses = item.Where(x => x.ExpectedBeltTime.Value.AddHours(-val.Extension.ValidityDay) <= DateTime.Now && x.ExpectedBeltTime.Value.AddHours(val.Extension.ValidityDay) >= DateTime.Now).ToList();
                }
                customerReports.AddRange(lapeses);

            }
            if (customerReports.Count == 0) return;

            var listfu = new List<CustomerTransactionsFollowUp>();


            var coontents = string.Empty;


            customerReports = customerReports.Select(q =>
            {
                q.TransactionsStatus = TransactionsStatus.NoBeltLapse;

                //新增一条成交跟进信息
                listfu.Add(new CustomerTransactionsFollowUp()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerTransactionsId = q.Id,
                    Contents = "已失效",
                    CreateTime = DateTime.Now,
                    UserId = "定时任务",
                    CreateUser = "定时任务",
                    TransactionsStatus = TransactionsStatus.NoBeltLapse
                });

                return q;
            }).ToList();
            await _icustomerTransactionsStore.UpdateListAsync(customerReports, cancellationToken);

            await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);
        }

        /// <summary>
        /// 报备未带看超期失效
        /// </summary>
        /// <returns></returns>
        public virtual async Task TimedTaskBeltLookReport(CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => b.TransactionsStatus == TransactionsStatus.BeltLook && !b.IsDeleted), cancellationToken);

            var customerReports = new List<CustomerTransactions>();

            var group = query.GroupBy(x => x.BuildingId);
            foreach (var item in group)
            {
                var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
                var val = (await _shopsInterface.GetBuilidngRule(item.Key));
                var lapeses = item.Where(x => x.BeltLookTime.Value.AddDays(val.Extension.BeltProtectDay).Date < DateTime.Now.Date).ToList();
                customerReports.AddRange(lapeses);
            }
            if (customerReports.Count == 0) return;

            var listfu = new List<CustomerTransactionsFollowUp>();


            var coontents = string.Empty;


            customerReports = customerReports.Select(q =>
            {
                q.TransactionsStatus = TransactionsStatus.NoDealLapse;

                //新增一条成交跟进信息
                listfu.Add(new CustomerTransactionsFollowUp()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerTransactionsId = q.Id,
                    Contents = "已失效",
                    CreateTime = DateTime.Now,
                    UserId = "定时任务",
                    CreateUser = "定时任务",
                    TransactionsStatus = TransactionsStatus.NoDealLapse
                });

                return q;
            }).ToList();
            await _icustomerTransactionsStore.UpdateListAsync(customerReports, cancellationToken);

            await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);
        }

        /// <summary>
        /// 定时任务超期失效
        /// </summary>
        /// <returns></returns>
        //public virtual async Task TimedTaskBeyondTheStatus(CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var customerReports = await _icustomerTransactionsStore.ListAsync(a => a.Where(b => b.TransactionsStatus != TransactionsStatus.NoDealLapse && b.TransactionsStatus != TransactionsStatus.NoBeltLapse && b.TransactionsStatus != TransactionsStatus.TheLapse && b.TransactionsStatus != TransactionsStatus.EndDeal && !b.IsDeleted), cancellationToken);
        //    customerReports = customerReports.Where(b => (DateTime.Now - b.ReportTime).Days > 30).ToList();
        //    if (customerReports == null) return;

        //    var listfu = new List<CustomerTransactionsFollowUp>();


        //    var coontents = string.Empty;


        //    customerReports = customerReports.Select(q =>
        //    {
        //        q.TransactionsStatus = TransactionsStatus.TheLapse;

        //        //新增一条成交跟进信息
        //        listfu.Add(new CustomerTransactionsFollowUp()
        //        {
        //            Id = Guid.NewGuid().ToString(),
        //            CustomerTransactionsId = q.Id,
        //            Contents = "已失效",
        //            CreateTime = DateTime.Now,
        //            UserId = "定时任务",
        //            CreateUser = "定时任务",
        //            TransactionsStatus = TransactionsStatus.TheLapse
        //        });

        //        return q;
        //    }).ToList();
        //    await _icustomerTransactionsStore.UpdateListAsync(customerReports, cancellationToken);

        //    await _icustomerTransactionsFollowUpStore.CreateListAsync(listfu, cancellationToken);
        //}

        /// <summary>
        /// 定时任务客户加入公客池
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task HandOverCustomerPool(CancellationToken cancellationToken = default(CancellationToken))
        {
            var filepath = System.IO.Path.Combine(AppContext.BaseDirectory, "pooldefault.json");
            var cfg = new List<PoolDefine>();
            if (System.IO.File.Exists(filepath))
            {
                string json = System.IO.File.ReadAllText(filepath);
                cfg = JsonHelper.ToObject<List<PoolDefine>>(json);
            }
            var customerfu = new List<CustomerFollowUp>();

            var customerpools = new List<CustomerPool>();

            var mig = new List<MigrationPoolHistory>();

            //跳到部门级
            var customers = await _icustomerInfoStore.ListAsync(a => a.Where(x => (x.CustomerStatus == CustomerStatus.ExistingCustomers || (x.CustomerStatus == CustomerStatus.EndDeal && x.IsSellIntention == true)) && !x.IsDeleted));

            var organizations = _icustomerInfoStore.OrganizationsAll().ToList();

            var organids = customers.GroupBy(x => x.DepartmentId).Select(x => x.Key);

            foreach (var item in organizations.Where(x => organids.Contains(x.Id)))
            {
                var department = item;
                int PoolDay = item.PoolDay;
                while (PoolDay == 0)
                {
                    if (cfg?.Count != 0)
                    {
                        PoolDay = cfg.FirstOrDefault(x => x.Code == department.Type)?.Value ?? 0;
                    }
                    if (PoolDay == 0)
                    {
                        department = organizations.SingleOrDefault(x => x.Id == department.ParentId);
                        PoolDay = department?.PoolDay ?? 0;
                    }
                }



                var quest = customers.Where(x => x.DepartmentId == item.Id && (DateTime.Now - x.FollowupTime).Value.Days > PoolDay).ToList();

                foreach (var cus in quest)
                {
                    customerpools.Add(new CustomerPool
                    {
                        Id = Guid.NewGuid().ToString(),
                        DepartmentId = department.Id,
                        CustomerId = cus.Id,
                        CreateTime = DateTime.Now,
                        JoinDate = DateTime.Now,
                        CreateUser = "定时任务"
                    });


                    mig.Add(new MigrationPoolHistory
                    {
                        CustomerId = item.Id,
                        MigrationTime = DateTime.Now,
                        OriginalDepartment = cus.DepartmentId,
                        TargetDepartment = department.Id,
                    });


                    customerfu.Add(new CustomerFollowUp
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = cus.Id,
                        TypeId = CustomerFollowUpType.JoinPool,
                        UserId = cus.UserId,
                        DepartmentId = cus.DepartmentId,
                        FollowUpTime = DateTime.Now,
                        TrueName = cus.CustomerName,
                        FollowUpContents = "加入公客池",
                        CustomerNo = cus.CustomerNo,
                        IsRealFollow = false,
                        CreateTime = DateTime.Now,
                        CreateUser = "定时任务"
                    });
                }
            }

            await _icustomerPoolStore.CreateListAsync(customerpools, mig, customerfu, cancellationToken);

        }

        /// <summary>
        /// 定时任务公客池内调客
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task CustomerPoolHandOver(CancellationToken cancellationToken = default(CancellationToken))
        {
            var filepath = System.IO.Path.Combine(AppContext.BaseDirectory, "pooldefault.json");
            var cfg = new List<PoolDefine>();
            if (System.IO.File.Exists(filepath))
            {
                string json = System.IO.File.ReadAllText(filepath);
                cfg = JsonHelper.ToObject<List<PoolDefine>>(json);
            }

            //跳到上一级
            var organizations = _icustomerInfoStore.OrganizationsAll().ToList();

            var customerpools1 = new List<CustomerPool>();

            var mig = new List<MigrationPoolHistory>();

            //var customers = new List<CustomerInfo>();

            foreach (var item in organizations)
            {
                if (item.Type != "Filiale")
                {
                    var customerspool = await _icustomerPoolStore.ListAsync(x => x.Where(y => y.DepartmentId == item.Id), cancellationToken);
                    if (customerspool.Count() == 0) continue;


                    var parentdepartment = organizations.SingleOrDefault(x => x.Id == item.ParentId);


                    int PoolDay = parentdepartment.PoolDay;

                    while (PoolDay == 0)
                    {
                        if (cfg?.Count != 0)
                        {
                            PoolDay = cfg.FirstOrDefault(x => x.Code == parentdepartment.Type)?.Value ?? 0;
                        }
                        if (PoolDay == 0)
                        {
                            parentdepartment = organizations.SingleOrDefault(x => x.Id == parentdepartment.ParentId);
                            PoolDay = parentdepartment?.PoolDay ?? 0;
                        }
                    }
                    customerspool = customerspool.Where(y => (DateTime.Now - y.JoinDate).Days > PoolDay).ToList();

                    foreach (var cuspool in customerspool)
                    {
                        cuspool.DepartmentId = parentdepartment.Id;

                        cuspool.JoinDate = DateTime.Now;

                        customerpools1.Add(cuspool);


                        mig.Add(new MigrationPoolHistory
                        {
                            CustomerId = cuspool.CustomerId,
                            MigrationTime = DateTime.Now,
                            OriginalDepartment = cuspool.DepartmentId,
                            TargetDepartment = parentdepartment.Id
                        });

                        //var customerinfo = await _icustomerInfoStore.GetAsync(x => x.Where(y => y.Id == cuspool.CustomerId));

                        //customerinfo.DepartmentId = parentdepartment.Id;
                        //customerinfo.FollowupTime = DateTime.Now;

                        //customers.Add(customerinfo);
                    }
                }

            }
            if (customerpools1.Count != 0)
            {
                await _icustomerPoolStore.UpdateListAsync(customerpools1, mig, cancellationToken);
            }
            //if (customers.Count != 0)
            //{
            //    await _icustomerInfoStore.UpdateListAsync(customers, cancellationToken);
            //}
        }
    }
}
