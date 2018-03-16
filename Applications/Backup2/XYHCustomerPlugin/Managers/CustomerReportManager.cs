using ApplicationCore;
using ApplicationCore.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    public class CustomerReportManager
    {
        #region 成员
        protected ICustomerFollowUpStore _icustomerFollowUpStore { get; }

        protected ICustomerReportStore _customerReportStore { get; }

        protected ICustomerInfoStore _customerinfoStore { get; }

        protected ICustomerDemandStore _icustomerDemandStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        /// <summary>
        /// 报备
        /// </summary>
        /// <param name="icustomerReportStore"></param>
        /// <param name="mapper"></param>
        public CustomerReportManager(ICustomerReportStore icustomerReportStore, ICustomerFollowUpStore icustomerFollowUpStore, ICustomerInfoStore customerInfoStore, ICustomerDemandStore customerDemandStore, IMapper mapper)
        {
            _icustomerFollowUpStore = icustomerFollowUpStore ?? throw new ArgumentNullException(nameof(icustomerFollowUpStore));
            _customerReportStore = icustomerReportStore ?? throw new ArgumentNullException(nameof(icustomerReportStore));
            _customerinfoStore = customerInfoStore ?? throw new ArgumentNullException(nameof(customerInfoStore));
            _icustomerDemandStore = customerDemandStore ?? throw new ArgumentNullException(nameof(customerDemandStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 根据Id获取客源信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<CustomerReportResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _customerReportStore.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<CustomerReportResponse>(response);
        }

        /// <summary>
        /// 新增报备信息
        /// </summary>
        /// <param name="user">新增人基本信息</param>
        /// <param name="customerReportRequest">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<CustomerReportResponse> CreateAsync(UserInfo user, CustomerReportRequest customerReportRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerReportRequest == null)
            {
                throw new ArgumentNullException(nameof(customerReportRequest));
            }

            customerReportRequest.CustomerInfoCreateRequest.MainPhone = new Help.EncDncHelper().Encrypt(customerReportRequest.CustomerInfoCreateRequest.MainPhone);

            if (string.IsNullOrEmpty(customerReportRequest.CustomerId))
            {
                //信息是否完善
                if (string.IsNullOrEmpty(customerReportRequest.CustomerInfoCreateRequest.MainPhone) && string.IsNullOrEmpty(customerReportRequest.CustomerInfoCreateRequest.CustomerName))
                {
                    throw new ArgumentNullException(nameof(customerReportRequest));
                }
                else
                {
                    var customerInfo = await _customerinfoStore.CustomerInfoAll().Where(b => b.UserId == user.Id && b.MainPhone == customerReportRequest.CustomerInfoCreateRequest.MainPhone && b.CustomerName == customerReportRequest.CustomerInfoCreateRequest.CustomerName && !b.IsDeleted).FirstOrDefaultAsync(cancellationToken);

                    if (customerInfo != null)
                    {
                        //修改核心意向

                        var demand = await _icustomerDemandStore.GetAsync(a => a.Where(b => b.Id == customerInfo.CustomerDemand.Id), cancellationToken);
                        

                        if (customerReportRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceEnd == null)
                        {
                            demand.PriceEnd = customerReportRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceEnd;
                        }
                        if (customerReportRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceStart == null)
                        {
                            demand.PriceStart = customerReportRequest.CustomerInfoCreateRequest.CustomerDemandRequest.PriceStart;
                        }
                        if (customerReportRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageEnd == null)
                        {
                            demand.AcreageEnd = customerReportRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageEnd;
                        }
                        if (customerReportRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageStart == null)
                        {
                            demand.AcreageStart = customerReportRequest.CustomerInfoCreateRequest.CustomerDemandRequest.AcreageStart;
                        }
                        await _icustomerDemandStore.UpdateAsync(demand, cancellationToken);
                    }
                    else
                    {
                        var customerinfo = _mapper.Map<CustomerInfo>(customerReportRequest.CustomerInfoCreateRequest);
                        //客户基本信息
                        customerinfo.Id = Guid.NewGuid().ToString();
                        customerinfo.CreateUser = user.Id;
                        customerinfo.CreateTime = DateTime.Now;
                        customerinfo.UserId = user.Id;
                        customerinfo.DepartmentId = user.OrganizationId;
                        //最后跟进时间为创建时间
                        customerinfo.FollowupTime = customerinfo.CreateTime;
                        if (customerinfo.Birthday != null)
                        {
                            customerinfo.Age = DateTime.Now.Year - customerinfo.Birthday.Value.Year;
                        }
                        customerinfo.CustomerStatus = CustomerStatus.ExistingCustomers;


                        //客户需求信息
                        customerinfo.CustomerDemand.Id = Guid.NewGuid().ToString();
                        customerinfo.CustomerDemand.UserId = user.Id;
                        customerinfo.CustomerDemand.DepartmentId = user.OrganizationId;
                        customerinfo.CustomerDemand.CustomerId = customerinfo.Id;
                        customerinfo.CustomerDemand.CreateUser = user.Id;
                        customerinfo.CustomerDemand.CreateTime = DateTime.Now;

                        //客户电话信息
                        customerinfo.CustomerPhones = customerinfo.CustomerPhones.Select(q =>
                        {
                            q.Id = Guid.NewGuid().ToString();
                            q.CustomerId = customerinfo.Id;
                            q.CreateUser = user.Id;
                            q.CreateTime = DateTime.Now;

                            return q;
                        });


                        await _customerinfoStore.CreateAsync(customerinfo, cancellationToken);



                        customerReportRequest.CustomerId = customerinfo.Id;
                    }
                }
            }
            else
            {
                //更新原来的用户数据跟进时间
                var customerinfo = await _customerinfoStore.GetAsync(a => a.Where(b => b.Id == customerReportRequest.CustomerId && !b.IsDeleted));



                //插入跟进信息
                var followup = new CustomerFollowUp
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customerReportRequest.CustomerId,
                    TypeId = CustomerFollowUpType.CustomerReported,
                    UserId = user.Id,
                    DepartmentId = user.OrganizationId,
                    FollowUpTime = DateTime.Now,
                    TrueName = customerinfo.CustomerName,
                    FollowUpContents = "客户报备",
                    CustomerNo = customerinfo.CustomerNo,
                    IsRealFollow = false,
                    CreateTime = DateTime.Now,
                    CreateUser = user.Id

                };
                await _icustomerFollowUpStore.CreateAsync(followup, cancellationToken);
            }

            var customerreport = _mapper.Map<CustomerReport>(customerReportRequest);
            //报备基本信息
            customerreport.Id = Guid.NewGuid().ToString();
            customerreport.CreateUser = user.Id;
            customerreport.CreateTime = DateTime.Now;
            customerreport.UserId = user.Id;
            customerreport.DepartmentId = user.OrganizationId;
            customerreport.ReportTime = customerreport.CreateTime;
            await _customerReportStore.CreateAsync(customerreport, cancellationToken);

            return _mapper.Map<CustomerReportResponse>(customerreport);
        }

        /// <summary>
        /// 删除报备信息
        /// </summary>
        /// <param name="user">删除人基本信息</param>
        /// <param name="id">删除报备Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _customerReportStore.DeleteAsync(_mapper.Map<SimpleUser>(user), new CustomerReport() { Id = id });
        }

        /// <summary>
        /// 修改单个报备信息
        /// </summary>
        /// <param name="userId">请求者Id</param>
        /// <param name="customerReportRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(string userId, CustomerReportRequest customerReportRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerReportRequest == null)
            {
                throw new ArgumentNullException(nameof(customerReportRequest));
            }
            var customerInfo = await _customerReportStore.GetAsync(a => a.Where(b => b.Id == customerReportRequest.Id && !b.IsDeleted));
            if (customerInfo == null)
            {
                return;
            }
            var newcustomerInfo = _mapper.Map<CustomerReport>(customerReportRequest);
            //报备基本信息
            newcustomerInfo.IsDeleted = customerInfo.IsDeleted;
            newcustomerInfo.CreateTime = customerInfo.CreateTime;
            newcustomerInfo.CreateUser = customerInfo.CreateUser;
            newcustomerInfo.UpdateTime = DateTime.Now;
            newcustomerInfo.UpdateUser = userId;
            await _customerReportStore.UpdateAsync(newcustomerInfo, cancellationToken);
        }

        /// <summary>
        /// 修改单个报备状态
        /// </summary>
        /// <param name="userId">请求者Id</param>
        /// <param name="reportid">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsyncStatus(string userId, string reportid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerReport = await _customerReportStore.GetAsync(a => a.Where(b => b.Id == reportid), cancellationToken);
            if (customerReport == null) return;
            //customerReport.ReportStatus = ReportStatus.WatiBeltLook;
            await _customerReportStore.UpdateAsync(customerReport, cancellationToken);
        }

        /// <summary>
        /// 查询我的
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="condition">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerReportResponse>> FindMyCustomerReport(string id, CustomerPageCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var pagingResponse = new PagingResponseMessage<CustomerReportResponse>();

            var response = await _customerReportStore.ListAsync(a => a.Where(b => b.UserId == id && !b.IsDeleted), cancellationToken);

            var result = response.OrderBy(a => a.ReportStatus).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToList();

            pagingResponse.TotalCount = response.Count;
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;

            pagingResponse.Extension = _mapper.Map<List<CustomerReportResponse>>(result);

            return pagingResponse;
        }

        /// <summary>
        /// 查询报备信息接口
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        //public virtual async Task<PagingResponseMessage<CustomerReportResponse>> Search(string id, CustomerReportSearchRequest searchcondition, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var pagingResponse = new PagingResponseMessage<CustomerReportResponse>();

        //    var response = _customerReportStore.CustomerReportAll().Where(x => !x.IsDeleted);

        //    if (!string.IsNullOrEmpty(searchcondition.BuildingId))
        //    {
        //        response = response.Where(x => x.BuildingId == searchcondition.BuildingId);
        //    }


        //    var result = await response.OrderBy(a => a.ReportStatus).Skip(searchcondition.PageIndex * searchcondition.PageSize).Take(searchcondition.PageSize).ToListAsync(cancellationToken);

        //    pagingResponse.TotalCount = await response.CountAsync(cancellationToken);
        //    pagingResponse.PageIndex = searchcondition.PageIndex;
        //    pagingResponse.PageSize = searchcondition.PageSize;

        //    pagingResponse.Extension = _mapper.Map<List<CustomerReportResponse>>(result);

        //    return pagingResponse;
        //}
    }
}
