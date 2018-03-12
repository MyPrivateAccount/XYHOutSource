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
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    public class CustomerDealManager
    {
        public CustomerDealManager(
            ICustomerInfoStore customerInfoStore,
            ICustomerDealStore customerDealStore,
            IDealFileScopeStore dealFileScopeStore,
            IFileInfoStore fileInfoStore,
            ICustomerTransactionsStore customerTransactionsStore,
            IOrganizationExpansionStore organizationExpansionStore,
             PermissionExpansionManager permissionExpansionManager,
        IMapper mapper
            )
        {
            _icustomerInfoStore = customerInfoStore ?? throw new ArgumentNullException(nameof(customerInfoStore));
            _icustomerDealStore = customerDealStore ?? throw new ArgumentNullException(nameof(customerDealStore));
            _icustomerTransactionsStore = customerTransactionsStore ?? throw new ArgumentNullException(nameof(customerTransactionsStore));
            _dealFileScopeStore = dealFileScopeStore ?? throw new ArgumentNullException(nameof(dealFileScopeStore));
            _iorganizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _fileInfoStore = fileInfoStore;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected ICustomerInfoStore _icustomerInfoStore { get; }
        protected ICustomerDealStore _icustomerDealStore { get; }
        protected ICustomerTransactionsStore _icustomerTransactionsStore { get; }
        protected IDealFileScopeStore _dealFileScopeStore { get; }
        protected IFileInfoStore _fileInfoStore { get; }
        protected IOrganizationExpansionStore _iorganizationExpansionStore { get; }
        protected IMapper _mapper { get; }
        private readonly PermissionExpansionManager _permissionExpansionManager;


        public virtual async Task<CustomerDealResponse> FindByIdSimpleAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = await _icustomerDealStore.GetAsync(a => a.Where(x => x.Id == id && !x.IsDeleted));
            return _mapper.Map<CustomerDealResponse>(query);
        }

        public virtual async Task<CustomerDealResponse> FindByIdAsync(string userid, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _icustomerDealStore.CustomerDealAll().Where(x => x.Id == id);


            if (query.Count() == 0)
            {
                return null;
            }
            var deal = await query.FirstOrDefaultAsync(cancellationToken);


            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, deal.ProjectId));
            if (!val.Extension)
                return null;

            deal.DepartmentName = _iorganizationExpansionStore.GetFullName(deal.DepartmentId);

            var phone = new Help.EncDncHelper().Decrypt(deal.CustomerPhone);
            if (!string.IsNullOrEmpty(phone))
            {
                deal.CustomerPhone = System.Text.RegularExpressions.Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }


            CustomerDealResponse customerDealResponse = _mapper.Map<CustomerDealResponse>(deal);
            customerDealResponse.FileList = new List<DealFileItemResponse>();
            customerDealResponse.AttachmentList = new List<DealAttachmentResponse>();

            if (deal.DealFileInfos != null && deal.DealFileInfos.Count() > 0)
            {
                var f = deal.DealFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = deal.DealFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        customerDealResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = deal.DealFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                    if (f2?.Count > 0)
                    {
                        customerDealResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                    }
                }
            }
            return customerDealResponse;
        }

        public virtual async Task<CustomerDealResponse> FindByIdManagerAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _icustomerDealStore.CustomerDealAll().Where(x => x.Id == id);
            if (query.Count() == 0)
            {
                return null;
            }
            var deal = await query.FirstOrDefaultAsync(cancellationToken);

            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(user.Id, deal.ProjectId));
            if (!val.Extension)
                return null;

            deal.DepartmentName = _iorganizationExpansionStore.GetFullName(deal.DepartmentId);
            var phone = new Help.EncDncHelper().Decrypt(deal.CustomerPhone);
            if (!string.IsNullOrEmpty(phone))
            {
                deal.CustomerPhone = System.Text.RegularExpressions.Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            CustomerDealResponse customerDealResponse = _mapper.Map<CustomerDealResponse>(deal);
            customerDealResponse.FileList = new List<DealFileItemResponse>();
            customerDealResponse.AttachmentList = new List<DealAttachmentResponse>();

            if (deal.DealFileInfos != null && deal.DealFileInfos.Count() > 0)
            {
                var f = deal.DealFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = deal.DealFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        customerDealResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = deal.DealFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                    if (f2?.Count > 0)
                    {
                        customerDealResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                    }
                }
            }
            return customerDealResponse;
        }

        public virtual async Task<CustomerDealResponse> FindByTransactionsIdAsync(string userid, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _icustomerDealStore.CustomerDealAll();
            query = query.Where(x => x.FlowId == id && !x.IsDeleted);
            var deal = await query.FirstOrDefaultAsync(cancellationToken);
            if (deal == null)
            {
                return null;
            }


            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, deal.ProjectId));
            if (!val.Extension && deal.Salesman != userid)
                return null;

            deal.DepartmentName = _iorganizationExpansionStore.GetFullName(deal.DepartmentId);
            var phone = new Help.EncDncHelper().Decrypt(deal.CustomerPhone);
            if (!string.IsNullOrEmpty(phone))
            {
                deal.CustomerPhone = System.Text.RegularExpressions.Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            CustomerDealResponse customerDealResponse = _mapper.Map<CustomerDealResponse>(deal);
            customerDealResponse.FileList = new List<DealFileItemResponse>();
            customerDealResponse.AttachmentList = new List<DealAttachmentResponse>();

            if (deal.DealFileInfos != null && deal.DealFileInfos.Count() > 0)
            {
                var f = deal.DealFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = deal.DealFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        customerDealResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = deal.DealFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                    if (f2?.Count > 0)
                    {
                        customerDealResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                    }
                }
            }
            return customerDealResponse;


        }

        public virtual async Task<CustomerDealResponse> FindByTransactionsIdSaleManAsync(string userid, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _icustomerDealStore.CustomerDealAll();
            query = query.Where(x => x.FlowId == id && x.Salesman == userid && !x.IsDeleted);
            var deal = await query.FirstOrDefaultAsync(cancellationToken);
            if (deal == null)
            {
                return null;
            }
            deal.DepartmentName = _iorganizationExpansionStore.GetFullName(deal.DepartmentId);
            var phone = new Help.EncDncHelper().Decrypt(deal.CustomerPhone);
            if (!string.IsNullOrEmpty(phone))
            {
                deal.CustomerPhone = System.Text.RegularExpressions.Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            CustomerDealResponse customerDealResponse = _mapper.Map<CustomerDealResponse>(deal);
            customerDealResponse.FileList = new List<DealFileItemResponse>();
            customerDealResponse.AttachmentList = new List<DealAttachmentResponse>();

            if (deal.DealFileInfos != null && deal.DealFileInfos.Count() > 0)
            {
                var f = deal.DealFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = deal.DealFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        customerDealResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = deal.DealFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                    if (f2?.Count > 0)
                    {
                        customerDealResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                    }
                }
            }
            return customerDealResponse;
        }

        public virtual async Task<CustomerDealResponse> FindByTransactionsIdSaleManagerAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _icustomerDealStore.CustomerDealAll();
            var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
            query = query.Where(x => x.FlowId == id && !x.IsDeleted && organs.Contains(x.DepartmentId));
            var deal = await query.FirstOrDefaultAsync(cancellationToken);
            if (deal == null)
            {
                return null;
            }
            //var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            //var val = (await _shopsInterface.IsResidentUser(userid, deal.ProjectId));
            //if (!val.Extension)
            //{

            //}


            deal.DepartmentName = _iorganizationExpansionStore.GetFullName(deal.DepartmentId);
            var phone = new Help.EncDncHelper().Decrypt(deal.CustomerPhone);
            if (!string.IsNullOrEmpty(phone))
            {
                deal.CustomerPhone = System.Text.RegularExpressions.Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            }
            CustomerDealResponse customerDealResponse = _mapper.Map<CustomerDealResponse>(deal);
            customerDealResponse.FileList = new List<DealFileItemResponse>();
            customerDealResponse.AttachmentList = new List<DealAttachmentResponse>();

            if (deal.DealFileInfos != null && deal.DealFileInfos.Count() > 0)
            {
                var f = deal.DealFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = deal.DealFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        customerDealResponse.FileList.Add(ConvertToFileItem(item, f1));
                    }
                    var f2 = deal.DealFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                    if (f2?.Count > 0)
                    {
                        customerDealResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                    }
                }
            }
            return customerDealResponse;
        }


        public virtual async Task<List<CustomerDealResponse>> FindByShopsIdAsync(string userid, string Id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = await _icustomerDealStore.CustomerDealAll().Where(x => x.ShopId == Id).ToListAsync(cancellationToken);
            if (query.Count() == 0)
            {
                return null;
            }

            var response = new List<CustomerDealResponse>();
            query = query.Select(x =>
            {
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                var phone = new Help.EncDncHelper().Decrypt(x.CustomerPhone);
                if (!string.IsNullOrEmpty(phone))
                    x.CustomerPhone = System.Text.RegularExpressions.Regex.Replace(phone, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                else
                    x.CustomerPhone = x.Mobile;

                CustomerDealResponse customerDealResponse = _mapper.Map<CustomerDealResponse>(x);
                customerDealResponse.FileList = new List<DealFileItemResponse>();
                customerDealResponse.AttachmentList = new List<DealAttachmentResponse>();

                if (x.DealFileInfos != null && x.DealFileInfos.Count() > 0)
                {
                    var f = x.DealFileInfos.Select(a => a.FileGuid).Distinct();
                    foreach (var item in f)
                    {
                        var f1 = x.DealFileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                        if (f1?.Count > 0)
                        {
                            customerDealResponse.FileList.Add(ConvertToFileItem(item, f1));
                        }
                        var f2 = x.DealFileInfos.Where(a => a.Type == "Attachment" && a.FileGuid == item).ToList();
                        if (f2?.Count > 0)
                        {
                            customerDealResponse.AttachmentList.AddRange(ConvertToAttachmentItem(f2));
                        }
                    }
                }
                response.Add(customerDealResponse);

                return x;
            }).ToList();

            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userid, query.FirstOrDefault().ProjectId));
            if (!val.Extension)
                return null;




            return response;
        }

        public virtual async Task<BuildingDealStatisticsResponse> BuildingDealStatistics(string userid, BuildingDealStatisticsRequest buildingDealStatisticsRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = await _icustomerDealStore.CustomerDealAll().Where(x => x.ProjectId == buildingDealStatisticsRequest.BuildingId && x.CreateTime >= buildingDealStatisticsRequest.StartTime && x.CreateTime < buildingDealStatisticsRequest.EndTime && !x.IsDeleted && x.ExamineStatus == (int)ExamineStatusEnum.Approved).ToListAsync(cancellationToken);
            if (query.Count() == 0)
            {
                return null;
            }
            var response = new BuildingDealStatisticsResponse()
            {
                DealCount = query.Count(),
                Commission = query.Sum(x => x.Commission),
                DealPrice = query.Sum(x => x.TotalPrice)
            };
            return response;
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
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="customerDealRequest">请求数据</param>
        /// <param name="dealid"></param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage<CustomerDeal>> CustiomerDealSubmitAsync(UserInfo user, string dealId, CustomerDealRequest customerDealRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = new ResponseMessage<CustomerDeal>();

            try
            {
                //if (string.IsNullOrEmpty(customerDealRequest.ProjectId))
                //    return response;

                var oldcustomerdeal = await _icustomerDealStore.GetAsync(a => a.Where(b => b.ShopId == customerDealRequest.ShopId && b.Customer == customerDealRequest.Customer));
                if (oldcustomerdeal != null)
                {
                    if (oldcustomerdeal.ExamineStatus == (int)ExamineStatusEnum.Auditing)
                    {
                        response.Extension = oldcustomerdeal;
                        return response;
                    }
                    else
                    {
                        oldcustomerdeal.ExamineStatus = 1;
                        await _icustomerDealStore.UpdateAsync(oldcustomerdeal);
                        response.Extension = oldcustomerdeal;
                        return response;
                    }
                }
                var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
                var val = (await _shopsInterface.IsResidentUser(user.Id, customerDealRequest.ProjectId));
                if (!val.Extension)
                {
                    return null;
                }
                if (!customerDealRequest.IsTwoHand)
                {
                    if ((await _icustomerDealStore.ListAsync(a => a.Where(b => b.ShopId == customerDealRequest.ShopId), cancellationToken)).FirstOrDefault() != null)
                    {
                        return null;
                    }
                }
                var customerdeal = _mapper.Map<CustomerDeal>(customerDealRequest);
                customerdeal.Id = dealId;
                customerdeal.UserId = user.Id;
                customerdeal.CreateTime = DateTime.Now;
                customerdeal.CreateUser = user.Id;
                customerdeal.ExamineStatus = (int)ExamineStatusEnum.Auditing;
                response.Extension = await _icustomerDealStore.SubmitCreateAsync(customerdeal, cancellationToken);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器发生错误：" + e.ToString();
                throw;
            }
            return response;
        }

        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateShopStatusAsync(string userId, string buildingid, string shopsid, string status, CancellationToken cancellationToken = default(CancellationToken))
        {
            var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
            var val = (await _shopsInterface.IsResidentUser(userId, buildingid));
            if (!val.Extension)
                return false;
            var aa = (await _shopsInterface.UpdateShopSaleStatus(userId, shopsid, status));
            if (!aa.Extension)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="customerDealRequest">请求数据</param>
        /// <param name="dealid"></param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage<CustomerDealResponse>> UpdateStatusDealAsync(string userId, string organizationId, string customerDealId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = new ResponseMessage<CustomerDealResponse>();
            try
            {
                var customerdeal = await _icustomerDealStore.GetAsync(a => a.Where(b => b.Id == customerDealId));
                if (customerdeal == null)
                {
                    throw new Exception("未找到成交记录");
                }
                customerdeal.ExamineStatus = (int)ExamineStatusEnum.Approved;
                var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
                var val = (await _shopsInterface.IsResidentUser(userId, customerdeal.ProjectId));
                if (!val.Extension)
                    return null;

                if ((await _icustomerDealStore.ListAsync(a => a.Where(b => b.ShopId == customerdeal.ShopId && b.ExamineStatus == (int)ExamineStatusEnum.Approved), cancellationToken)).FirstOrDefault() != null)
                {
                    throw new Exception("该商铺已经成交过");
                }

                //修改报备流程状态
                var customertran = await _icustomerTransactionsStore.GetAsync(a => a.Where(b => b.Id == customerdeal.FlowId), cancellationToken);
                var reportfollowup = new CustomerTransactionsFollowUp();

                if (customerdeal.SellerType == SellerType.SinceSale && customertran == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "未找到报备信息";
                    return response;
                }

                if (customertran != null)
                {
                    customertran.TransactionsStatus = TransactionsStatus.EndDeal;
                    //添加一条报备跟进信息
                    reportfollowup.Id = Guid.NewGuid().ToString();
                    reportfollowup.CustomerTransactionsId = customerdeal.FlowId;
                    reportfollowup.Contents = "成交客户";
                    reportfollowup.CreateTime = DateTime.Now;
                    reportfollowup.UserId = userId;
                    reportfollowup.CreateUser = userId;
                    reportfollowup.TransactionsStatus = TransactionsStatus.EndDeal;
                }

                //将客户状态改为已成交
                var customerinfo = await _icustomerInfoStore.GetAsync(a => a.Where(x => x.Id == customerdeal.Customer));
                if (customerinfo != null)
                {
                    //成交后不改变客户的状态，只有在确认没有购买意向后才置为已成交
                    //customerinfo.CustomerStatus = CustomerStatus.EndDeal;
                    customerinfo.RateProgress = RateProgress.ClinchADeal;
                    //将客户的仍有购买意向置为null
                    customerinfo.IsSellIntention = null;
                }

                var customerfu = customerinfo == null ? null : new CustomerFollowUp()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customertran.CustomerId,
                    TypeId = CustomerFollowUpType.EndDeal,
                    UserId = userId,
                    DepartmentId = organizationId,
                    FollowUpTime = DateTime.Now,
                    TrueName = customertran.CustomerName,
                    FollowUpContents = "已成交",
                    CustomerNo = customertran.CustomerNo,
                    IsRealFollow = false,
                    CreateTime = DateTime.Now,
                    CreateUser = userId
                };

                response.Extension = _mapper.Map<CustomerDealResponse>(await _icustomerDealStore.CreateAsync(customerdeal, customerinfo, reportfollowup, customertran, customerfu, cancellationToken));

                var aa = (await _shopsInterface.UpdateShopSaleStatus(userId, customerdeal.ShopId, "10"));
                if (!aa.Extension)
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器发生错误：" + e.ToString();
                throw;
            }
            return response;
        }

        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="customerDealRequest">请求数据</param>
        /// <param name="dealid"></param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task UpdateStatusDealAsync(string customerDealId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerdeal = await _icustomerDealStore.GetAsync(a => a.Where(b => b.Id == customerDealId));
            if (customerdeal == null)
            {
                throw new Exception("未找到成交记录");
            }
            customerdeal.ExamineStatus = 16;
            await _icustomerDealStore.UpdateAsync(customerdeal);
        }

        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="customerDealId"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<bool> UpdateStatusDealBackAsync(string customerDealId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerdeal = await _icustomerDealStore.GetAsync(a => a.Where(b => b.Id == customerDealId));
            if (customerdeal == null)
            {
                throw new Exception("未找到成交记录");
            }
            customerdeal.ExamineStatus = (int)ExamineStatusEnum.Auditing;
            try
            {
                await _icustomerDealStore.UpdateAsync(customerdeal);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="customerDealRequest">请求数据</param>
        /// <param name="dealid"></param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task UpdateStatusDealBackRejectAsync(string customerDealId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerdeal = await _icustomerDealStore.GetAsync(a => a.Where(b => b.Id == customerDealId));
            if (customerdeal == null)
            {
                throw new Exception("未找到成交记录");
            }
            customerdeal.ExamineStatus = (int)ExamineStatusEnum.Approved;
            await _icustomerDealStore.UpdateAsync(customerdeal);
        }

        /// <summary>
        /// 将报备状态改为对应状态
        /// </summary>
        /// <param name="user">请求者Id</param>
        /// <param name="customerDealRequest">请求数据</param>
        /// <param name="dealid"></param>
        /// <param name="mark">修改的状态</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task UpdateStatusDealBackApprovedAsync(string customerDealId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerdeal = await _icustomerDealStore.GetAsync(a => a.Where(b => b.Id == customerDealId));
            if (customerdeal == null)
            {
                throw new Exception("未找到成交记录");
            }
            customerdeal.IsDeleted = true;
            customerdeal.ExamineStatus = (int)ExamineStatusEnum.Reject;
            await _icustomerDealStore.UpdateAsync(customerdeal);

            var customertran = await _icustomerTransactionsStore.GetAsync(a => a.Where(b => b.Id == customerdeal.FlowId));
            if (customertran != null)
            {
                customertran.TransactionsStatus = TransactionsStatus.NoDealLapse;
                var transfollowup = new CustomerTransactionsFollowUp()
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerTransactionsId = customertran.Id,
                    Contents = "已失效",
                    CreateTime = DateTime.Now,
                    UserId = "",
                    CreateUser = "",
                    TransactionsStatus = customertran.TransactionsStatus,
                    MarkTime = DateTime.Now

                };


                await _icustomerTransactionsStore.UpdateAsync(customertran, transfollowup);
            }
        }


        private List<DealAttachmentResponse> ConvertToAttachmentItem(List<CustomerDealFileInfo> f2)
        {
            List<DealAttachmentResponse> list = new List<DealAttachmentResponse>();
            string fr = ApplicationContext.Current.FileServerRoot;
            foreach (var item in f2)
            {
                list.Add(new DealAttachmentResponse
                {
                    FileGuid = item.FileGuid,
                    FileName = item.Name,
                    Summary = item.Summary,
                    Url = fr + "/" + item.Uri.TrimStart('/')
                });
            }
            return list;
        }


        private DealFileItemResponse ConvertToFileItem(string fileGuid, List<CustomerDealFileInfo> fl)
        {
            DealFileItemResponse fi = new DealFileItemResponse();
            fi.FileGuid = fileGuid;
            fi.Icon = fl.FirstOrDefault(x => x.Type == "ICON")?.Uri;
            fi.Original = fl.FirstOrDefault(x => x.Type == "ORIGINAL")?.Uri;
            fi.Medium = fl.FirstOrDefault(x => x.Type == "MEDIUM")?.Uri;
            fi.Small = fl.FirstOrDefault(x => x.Type == "SMALL")?.Uri;

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');
            if (!String.IsNullOrEmpty(fi.Icon))
            {
                fi.Icon = fr + "/" + fi.Icon.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Original))
            {
                fi.Original = fr + "/" + fi.Original.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Medium))
            {
                fi.Medium = fr + "/" + fi.Medium.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Small))
            {
                fi.Small = fr + "/" + fi.Small.TrimStart('/');
            }
            return fi;
        }


        public virtual async Task UpdateAsync(string userId, string id, CustomerDealRequest customerDealRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerDealRequest == null)
            {
                throw new ArgumentNullException(nameof(customerDealRequest));
            }
            var building = await _icustomerDealStore.GetAsync(a => a.Where(b => b.Id == id && !b.IsDeleted && b.UserId == userId));
            if (building == null)
            {
                return;
            }
            var newdeal = _mapper.Map<CustomerDeal>(customerDealRequest);
            newdeal.IsDeleted = building.IsDeleted;
            newdeal.CreateTime = building.CreateTime;
            newdeal.CreateUser = building.CreateUser;
            newdeal.UpdateTime = DateTime.Now;
            newdeal.UpdateUser = userId;
            await _icustomerDealStore.UpdateAsync(newdeal, cancellationToken);
        }
    }
}
