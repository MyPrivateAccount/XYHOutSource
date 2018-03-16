using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Common;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    public class CustomerInfoManager
    {
        #region 成员

        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerInfoManager");

        protected ICustomerDealStore _icustomerDealStore { get; }

        protected ICustomerFollowUpStore _icustomerFollowUpStore { get; }

        protected ICustomerPoolStore _icustomerPoolStore { get; }

        protected ICustomerPoolDefineStore _icustomerPoolDefineStore { get; }

        protected ICustomerTransactionsStore _icustomerTransactionsStore { get; }

        protected ICustomerTransactionsFollowUpStore _icustomerTransactionsFollowUpStore { get; }

        protected ICustomerDemandStore _icustomerDemandStore { get; }

        protected ICustomerInfoStore _icustomerInfoStore { get; }

        protected IAboutLookStore _iaboutLookStore { get; }

        protected IBeltLookStore _ibeltLookStore { get; }

        protected ICustomerPhoneStore _icustomerPhoneStore { get; }

        protected IOrganizationExpansionStore _iorganizationExpansionStore { get; }

        protected PermissionExpansionManager _permissionExpansionManager { get; }

        protected IMapper _mapper { get; }

        #endregion

        public CustomerInfoManager(
             ICustomerDealStore icustomerDealStore,
            ICustomerFollowUpStore icustomerFollowUpStore,
             ICustomerPoolStore customerPoolStore,
             ICustomerPoolDefineStore customerPoolDefineStore,
             ICustomerTransactionsStore customerTransactionsStore,
             ICustomerTransactionsFollowUpStore customerTransactionsFollowUpStore,
            ICustomerInfoStore customerInfoStore,
            ICustomerDemandStore icustomerDemandStore,
            IAboutLookStore iaboutLookStore,
            IBeltLookStore ibeltLookStore,
            ICustomerPhoneStore icustomerPhoneStore,
            IOrganizationExpansionStore organizationExpansionStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            _icustomerDealStore = icustomerDealStore ?? throw new ArgumentNullException(nameof(icustomerDealStore));
            _icustomerFollowUpStore = icustomerFollowUpStore ?? throw new ArgumentNullException(nameof(icustomerFollowUpStore));
            _icustomerPoolStore = customerPoolStore ?? throw new ArgumentNullException(nameof(customerPoolStore));
            _icustomerPoolDefineStore = customerPoolDefineStore ?? throw new ArgumentNullException(nameof(customerPoolDefineStore));
            _icustomerTransactionsStore = customerTransactionsStore ?? throw new ArgumentNullException(nameof(customerTransactionsStore));
            _icustomerTransactionsFollowUpStore = customerTransactionsFollowUpStore ?? throw new ArgumentNullException(nameof(customerTransactionsFollowUpStore));
            _icustomerDemandStore = icustomerDemandStore ?? throw new ArgumentNullException(nameof(icustomerDemandStore));
            _icustomerInfoStore = customerInfoStore ?? throw new ArgumentNullException(nameof(customerInfoStore));
            _iaboutLookStore = iaboutLookStore ?? throw new ArgumentNullException(nameof(iaboutLookStore));
            _ibeltLookStore = ibeltLookStore ?? throw new ArgumentNullException(nameof(ibeltLookStore));
            _icustomerPhoneStore = icustomerPhoneStore ?? throw new ArgumentNullException(nameof(icustomerPhoneStore));
            _iorganizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 新增客源信息
        /// </summary>
        /// <param name="user">创建者</param>
        /// <param name="customerInfoCreateRequest">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage<CustomerInfoCreateResponse>> CreateAsync(UserInfo user, CustomerInfoCreateRequest customerInfoCreateRequest, CancellationToken cancellationToken = default(CancellationToken))
        {


            var response = new ResponseMessage<CustomerInfoCreateResponse>();

            if (customerInfoCreateRequest == null)
            {
                throw new ArgumentNullException(nameof(customerInfoCreateRequest));
            }

            customerInfoCreateRequest.MainPhone = new Help.EncDncHelper().Encrypt(customerInfoCreateRequest.MainPhone);
            var customerinfo = _mapper.Map<CustomerInfo>(customerInfoCreateRequest);

            //判断电话号码是否重复
            var oldcustomerinfo = (await _icustomerInfoStore.ListAsync(a => a.Where(b => b.MainPhone == customerinfo.MainPhone))).FirstOrDefault();
            if (oldcustomerinfo != null)
            {
                if (oldcustomerinfo.UserId == user.Id)
                {

                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "该手机号用户已在您的用户列表中";
                    return response;
                }
                else
                {
                    customerinfo.UniqueId = oldcustomerinfo.UniqueId;
                }
            }
            else
            {
                customerinfo.UniqueId = Guid.NewGuid().ToString();
            }

            //客户基本信息
            //customerinfo.Id = Guid.NewGuid().ToString();
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
            //商机阶段
            customerinfo.RateProgress = RateProgress.Entry;
            //客户需求信息
            customerinfo.CustomerDemand.Id = Guid.NewGuid().ToString();
            customerinfo.CustomerDemand.UserId = user.Id;
            customerinfo.CustomerDemand.DepartmentId = user.OrganizationId;
            customerinfo.CustomerDemand.CustomerId = customerinfo.Id;
            customerinfo.CustomerDemand.CreateUser = user.Id;
            customerinfo.CustomerDemand.CreateTime = DateTime.Now;
            //客户电话信息
            customerinfo.CustomerPhones = customerinfo.CustomerPhones.Where(x => !string.IsNullOrEmpty(x.Phone)).Select(q =>
              {
                  q.Id = Guid.NewGuid().ToString();
                  q.CustomerId = customerinfo.Id;
                  q.CreateUser = user.Id;
                  q.CreateTime = DateTime.Now;
                  q.Phone = new Help.EncDncHelper().Encrypt(q.Phone);
                  return q;
              });
            //需求楼盘信息
            if (customerinfo.HousingResources != null)
            {
                foreach (var item in customerinfo.HousingResources)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.CustomerId = customerinfo.Id;
                    item.CreateUser = user.Id;
                    item.CreateTime = DateTime.Now;
                }
            }


            await _icustomerInfoStore.CreateAsync(customerinfo, cancellationToken);

            response.Extension = _mapper.Map<CustomerInfoCreateResponse>(customerinfo);
            return response;
        }

        /// <summary>
        /// 根据Id获取客源信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<CustomerInfoCreateResponse> FindByIdAsync(string userId, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            //3dd0e30c-0b02-442a-b942-25a4444067d8 使用singleordefault会出来两条数据
            var response = await _icustomerInfoStore.CustomerInfoAll().FirstOrDefaultAsync(x => x.Id == id && (x.UserId == userId || string.IsNullOrEmpty(x.UserId)), cancellationToken);
            if (response == null) return null;
            response.DepartmentName = _iorganizationExpansionStore.GetFullName(response.DepartmentId);

            response.MainPhone = new Help.EncDncHelper().Decrypt(response.MainPhone);
            response.CustomerPhones = response.CustomerPhones.Select(a =>
             {
                 a.Phone = new Help.EncDncHelper().Decrypt(a.Phone);
                 return a;
             });
            response.CustomerFollowUp = response.CustomerFollowUp.Where(x => x.TypeId != CustomerFollowUpType.TransferCustomer);
            var query = _mapper.Map<CustomerInfoCreateResponse>(response);


            var trans = new List<TransactionsFuResponse>();


            foreach (var item in response.CustomerTransactions.GroupBy(x => x.BuildingId))
            {
                var tran = _mapper.Map<TransactionsFuResponse>(item.OrderByDescending(x => x.ReportTime).FirstOrDefault());
                var fus = new List<TransactionsFollowUpResponse>();

                foreach (var asda in item)
                {
                    fus.AddRange(_mapper.Map<List<TransactionsFollowUpResponse>>(asda.CustomerTransactionsFollowUps.ToList()));
                }
                tran.TransactionsFollowUpResponse = fus;
                trans.Add(tran);
            }
            query.TransactionsResponse = trans;

            query.FileList = new List<DealFileItemResponse>();
            if (response.CustomerFileInfos != null && response.CustomerFileInfos.Count() > 0)
            {
                var f = response.CustomerFileInfos.Select(a => a.FileGuid).Distinct();
                foreach (var item in f)
                {
                    var f1 = response.CustomerFileInfos.Where(a => a.FileGuid == item).ToList();
                    if (f1?.Count > 0)
                    {
                        query.FileList.Add(ConvertToFileItem(item, f1));
                    }
                }
            }


            //Regex.Replace(new Help.EncDncHelper().Decrypt(response.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            return query;
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

        /// <summary>
        /// 根据Id获取客源信息部门
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<CustomerInfoCreateResponse> FindByIdOrganAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);

            //3dd0e30c-0b02-442a-b942-25a4444067d8 使用singleordefault会出来两条数据
            var response = await _icustomerInfoStore.CustomerInfoAll().FirstOrDefaultAsync(x => x.Id == id && organs.Contains(x.DepartmentId), cancellationToken);
            if (response == null) return null;
            response.DepartmentName = _iorganizationExpansionStore.GetFullName(response.DepartmentId);

            response.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(response.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            response.CustomerPhones = response.CustomerPhones.Select(a =>
            {
                a.Phone = Regex.Replace(new Help.EncDncHelper().Decrypt(response.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                return a;
            });
            response.CustomerTransactions = response.CustomerTransactions.Select(a =>
            {
                a.DepartmentName = _iorganizationExpansionStore.GetFullName(a.DepartmentId);
                return a;
            });
            //Regex.Replace(new Help.EncDncHelper().Decrypt(response.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
            return _mapper.Map<CustomerInfoCreateResponse>(response);
        }

        /// <summary>
        /// 根据CustomerId获取客源电话
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerid"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<CustomerPhoneResponse>> FindPhoneByCustomerAsync(UserInfo user, string customerid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var organs = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "CustomerRetrieve");


            var response = await _icustomerPhoneStore.CustomerPhoneAll().Where(x => x.CustomerId == customerid /*&& !x.IsDeleted*/ && organs.Contains(x.DepartmentId)).OrderBy(x => x.IsMain).ToListAsync(cancellationToken);

            var costomerinfo = await _icustomerInfoStore.GetAsync(a => a.Where(b => b.Id == customerid), cancellationToken);
            //插入查看电话信息
            var followup = new ChekPhoneHistory
            {
                Id = Guid.NewGuid().ToString(),
                CheckTime = DateTime.Now,
                CheckUserId = user.Id,
                CustomerId = customerid
                //Id = Guid.NewGuid().ToString(),
                //CustomerId = customerid,
                //TypeId = CustomerFollowUpType.SeePhone,
                //UserId = user.Id,
                //DepartmentId = user.OrganizationId,
                //FollowUpTime = DateTime.Now,
                //TrueName = costomerinfo.CustomerName,
                //FollowUpContents = "查看电话",
                //CustomerNo = costomerinfo.CustomerNo,
                //IsRealFollow = false,
                //CreateTime = DateTime.Now,
                //CreateUser = user.Id

            };
            await _icustomerInfoStore.CreateCheckPhone(followup, cancellationToken);

            response = response.Select(a =>
            {
                a.Phone = new Help.EncDncHelper().Decrypt(a.Phone);
                return a;
            }).ToList();

            return _mapper.Map<List<CustomerPhoneResponse>>(response);
        }

        /// <summary>
        /// 查看无效客户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageCondition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerListResponse>> FindByCustomerLapseAsync(string userId, CustomerPageCondition pageCondition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pageCondition == null)
            {
                throw new ArgumentNullException(nameof(pageCondition));
            }
            var pagingResponse = new PagingResponseMessage<CustomerListResponse>();

            var response = await _icustomerInfoStore.CustomerInfoAll().Where(b => b.UserId == userId && !b.IsDeleted && b.CustomerStatus == CustomerStatus.Lapse).ToListAsync(cancellationToken);

            var result = response.OrderByDescending(a => a.CreateTime).Skip(pageCondition.PageIndex * pageCondition.PageSize).Take(pageCondition.PageSize).ToList();

            pagingResponse.TotalCount = response.Count;
            pagingResponse.PageIndex = pageCondition.PageIndex;
            pagingResponse.PageSize = pageCondition.PageSize;

            var list = _mapper.Map<List<CustomerInfoCreateResponse>>(result).Select(a => new CustomerListResponse
            {
                Id = a.Id,
                CustomerName = a.CustomerName,
                Sex = a.Sex,
                MainPhone = new Help.EncDncHelper().Decrypt(a.MainPhone),
                Deamand = a.CustomerDemandResponse,
                CreateTime = a.CreateTime,
                FollowupTime = a.FollowupTime,
                Mark = a.Mark,
                TransactionsResponse = a.TransactionsResponse.FirstOrDefault(),
            });

            pagingResponse.Extension = list.ToList();

            return pagingResponse;
        }

        /// <summary>
        /// 根据Userid查询客户信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageCondition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerListResponse>> FindByUserIdAsync(string userId, CustomerListSearchCondition pageCondition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pageCondition == null)
            {
                throw new ArgumentNullException(nameof(pageCondition));
            }
            var pagingResponse = new PagingResponseMessage<CustomerListResponse>();

            var response = _icustomerInfoStore.CustomerInfoSimple().Where(x => !x.IsDeleted && x.UserId == userId && (x.CustomerStatus == CustomerStatus.ExistingCustomers || (x.IsSellIntention == true && x.CustomerStatus == CustomerStatus.EndDeal)));


            //查询主键信息
            if (!string.IsNullOrEmpty(pageCondition.KeyWord))
            {
                response = response.Where(x => x.MainPhone == new Help.EncDncHelper().Encrypt(pageCondition.KeyWord) || x.CustomerName.Contains(pageCondition.KeyWord) || x.UserName.Contains(pageCondition.KeyWord) || x.UserPhone.Contains(pageCondition.KeyWord));
            }


            pagingResponse.TotalCount = await response.CountAsync(cancellationToken);

            var result = await response.OrderBy(a => a.FollowupTime).Skip(pageCondition.pageIndex * pageCondition.pageSize).Take(pageCondition.pageSize).ToListAsync(cancellationToken);

            pagingResponse.PageIndex = pageCondition.pageIndex;
            pagingResponse.PageSize = pageCondition.pageSize;

            var list = _mapper.Map<List<CustomerInfoCreateResponse>>(result).Select(a => new CustomerListResponse
            {
                Id = a.Id,
                CustomerName = a.CustomerName,
                Sex = a.Sex,
                MainPhone = new Help.EncDncHelper().Decrypt(a.MainPhone),
                Deamand = a.CustomerDemandResponse,
                CreateTime = a.CreateTime,
                FollowupTime = a.FollowupTime,
                Mark = a.Mark
            });

            pagingResponse.Extension = list.ToList();

            return pagingResponse;
        }

        /// <summary>
        /// 修改单个客源信息
        /// </summary>
        /// <param name="userId">请求者Id</param>
        /// <param name="customerInfoCreateRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(string userId, CustomerInfoCreateRequest customerInfoCreateRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerInfoCreateRequest == null)
            {
                throw new ArgumentNullException(nameof(customerInfoCreateRequest));
            }
            var customerInfo = await _icustomerInfoStore.GetAsync(a => a.Where(b => b.Id == customerInfoCreateRequest.Id && !b.IsDeleted && b.UserId == userId));


            //如果没有查到该用户
            if (customerInfo == null)
                return;

            var customerDemand = await _icustomerDemandStore.GetAsync(a => a.Where(b => b.Id == customerInfoCreateRequest.CustomerDemandRequest.Id && !b.IsDeleted/* && b.UserId == userId*/));


            var newcustomerInfo = _mapper.Map<CustomerInfo>(customerInfoCreateRequest);
            //客户基本信息
            customerInfo.CustomerName = newcustomerInfo.CustomerName;
            customerInfo.Source = newcustomerInfo.Source;
            customerInfo.MainPhone = new Help.EncDncHelper().Encrypt(newcustomerInfo.MainPhone);
            customerInfo.QQ = newcustomerInfo.QQ;
            customerInfo.WeChat = newcustomerInfo.WeChat;
            customerInfo.Email = newcustomerInfo.Email;
            customerInfo.Sex = newcustomerInfo.Sex;
            customerInfo.Birthday = newcustomerInfo.Birthday;
            customerInfo.HeadImg = newcustomerInfo.HeadImg;
            if (customerInfo.Birthday != null)
            {
                customerInfo.Age = DateTime.Now.Year - customerInfo.Birthday.Value.Year;
            }
            customerInfo.Mark = newcustomerInfo.Mark;

            customerInfo.FollowupTime = DateTime.Now;
            customerInfo.FollowUpNum = customerInfo.FollowUpNum + 1;


            //客户需求信息
            customerDemand.CityId = newcustomerInfo.CustomerDemand.CityId;
            customerDemand.AreaId = newcustomerInfo.CustomerDemand.AreaId;
            customerDemand.DistrictId = newcustomerInfo.CustomerDemand.DistrictId;
            customerDemand.AreaFullName = newcustomerInfo.CustomerDemand.AreaFullName;
            customerDemand.Importance = newcustomerInfo.CustomerDemand.Importance;
            customerDemand.RequirementType = newcustomerInfo.CustomerDemand.RequirementType;
            customerDemand.DemandLevel = newcustomerInfo.CustomerDemand.DemandLevel;
            customerDemand.AcreageStart = newcustomerInfo.CustomerDemand.AcreageStart;
            customerDemand.AcreageEnd = newcustomerInfo.CustomerDemand.AcreageEnd;
            customerDemand.PriceStart = newcustomerInfo.CustomerDemand.PriceStart;
            customerDemand.PriceEnd = newcustomerInfo.CustomerDemand.PriceEnd;
            customerDemand.PurchaseWay = newcustomerInfo.CustomerDemand.PurchaseWay;
            customerDemand.PurchaseMotivation = newcustomerInfo.CustomerDemand.PurchaseMotivation;

            var oldphone = (await _icustomerPhoneStore.ListAsync(x => x.Where(y => y.CustomerId == newcustomerInfo.Id && !y.IsDeleted))).Select(a =>
            {
                a.Phone = new Help.EncDncHelper().Decrypt(a.Phone);
                return a;
            });

            var list1 = newcustomerInfo.CustomerPhones.Where(t => !oldphone.Select(x => new { Phone = x.Phone, IsMain = x.IsMain }).Contains(new { Phone = t.Phone, IsMain = t.IsMain }) && !string.IsNullOrEmpty(t.Phone));

            var list2 = oldphone.Where(t => !newcustomerInfo.CustomerPhones.Select(x => new { Phone = x.Phone, IsMain = x.IsMain }).Contains(new { Phone = t.Phone, IsMain = t.IsMain }));


            customerInfo.CustomerPhones = list1?.Select(q =>
            {
                q.Id = Guid.NewGuid().ToString();
                q.CustomerId = newcustomerInfo.Id;
                q.CreateUser = userId;
                q.CreateTime = DateTime.Now;
                q.Phone = new Help.EncDncHelper().Encrypt(q.Phone);
                return q;
            });

            customerInfo.CustomerPhones2 = list2?.Select(q =>
            {
                q.IsDeleted = true;
                q.DeleteTime = DateTime.Now;
                q.DeleteUser = userId;
                return q;
            });


            customerInfo.CustomerDemand = customerDemand;
            customerInfo.HousingResources = newcustomerInfo.HousingResources?.Select(x =>
            {
                x.Id = Guid.NewGuid().ToString();
                x.CustomerId = customerInfo.Id;
                x.CreateTime = DateTime.Now;
                x.CreateUser = userId;
                return x;
            });

            await _icustomerInfoStore.UpdateAsync(customerInfo, cancellationToken);



            //插入跟进信息
            var followup = new CustomerFollowUp
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = customerInfo.Id,
                Importance = customerInfo?.CustomerDemand?.Importance,
                DemandLevel = customerInfo?.CustomerDemand?.DemandLevel,
                TypeId = CustomerFollowUpType.CustomersModify,
                UserId = customerInfo.UserId,
                DepartmentId = customerInfo.DepartmentId,
                FollowUpTime = DateTime.Now,
                TrueName = customerInfo.CustomerName,
                FollowUpContents = "修改客户",
                CustomerNo = customerInfo.CustomerNo,
                IsRealFollow = false,
                CreateTime = DateTime.Now,
                CreateUser = userId

            };
            await _icustomerFollowUpStore.CreateAsync(followup, cancellationToken);
        }

        /// <summary>
        /// 删除客源信息
        /// </summary>
        /// <param name="user">删除人基本信息</param>
        /// <param name="id">删除客源Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _icustomerInfoStore.DeleteAsync(user, new CustomerInfo() { Id = id });
        }

        /// <summary>
        /// 批量删除客源信息
        /// </summary>
        /// <param name="userId">删除人Id</param>
        /// <param name="ids">删除Id数组</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerInfo = await _icustomerInfoStore.ListAsync(a => a.Where(b => ids.Contains(b.Id) && !b.IsDeleted), cancellationToken);
            if (customerInfo == null || customerInfo.Count == 0)
            {
                return;
            }
            for (int i = 0; i < customerInfo.Count; i++)
            {
                customerInfo[i].DeleteUser = userId;
                customerInfo[i].DeleteTime = DateTime.Now;
                customerInfo[i].IsDeleted = true;
            }
            await _icustomerInfoStore.UpdateListAsync(customerInfo, cancellationToken);
        }

        /// <summary>
        /// 批量修改调离客源信息
        /// </summary>
        /// <param name="userid">删除人Id</param>
        /// <param name="transferringRequest"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task TransferringListAsync(TransferringRequest transferringRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            var CustomerIds = transferringRequest.Customers.Select(a => a.Id);
            var customerInfo = await _icustomerInfoStore.ListAsync(a => a.Where(b => CustomerIds.Contains(b.Id) && !b.IsDeleted), cancellationToken);
            if (customerInfo == null || customerInfo.Count == 0)
            {
                return;
            }
            var customerfulist = new List<CustomerFollowUp>();
            var customertranlist = new List<CustomerTransactions>();
            var customertranfulist = new List<CustomerTransactionsFollowUp>();
            var customerbeltlist = new List<BeltLook>();
            var customerdemandlist = new List<CustomerDemand>();

            var newcustomerfu = new List<CustomerFollowUp>();

            if (transferringRequest.IsKeep)
            {
                for (int i = 0; i < customerInfo.Count; i++)
                {
                    customerInfo[i].UserId = transferringRequest.TerUserId;
                    customerInfo[i].DepartmentId = transferringRequest.TerDepartmentId;
                    var customerfu = await _icustomerFollowUpStore.ListAsync(x => x.Where(y => y.CustomerId == customerInfo[i].Id && !y.IsDeleted), cancellationToken);
                    var customertran = await _icustomerTransactionsStore.ListAsync(x => x.Where(y => y.CustomerId == customerInfo[i].Id && !y.IsDeleted), cancellationToken);
                    var customerbelt = await _ibeltLookStore.ListAsync(x => x.Where(y => y.CustomerId == customerInfo[i].Id && !y.IsDeleted), cancellationToken);
                    var demand = await _icustomerDemandStore.ListAsync(a => a.Where(b => b.CustomerId == customerInfo[i].Id && !b.IsDeleted), cancellationToken);

                    //还会将跟进信息 报备 带看信息等置为已删除状态
                    //如果保留以前信息 则将以前的信息统一改负责人ID 成交信息 可能不得改
                    foreach (var item in customerfu)
                    {
                        item.UserId = transferringRequest.TerUserId;
                        item.DepartmentId = transferringRequest.TerDepartmentId;
                    }
                    foreach (var item in customertran)
                    {
                        item.UserId = transferringRequest.TerUserId;
                        item.DepartmentId = transferringRequest.TerDepartmentId;
                        //获取每条的跟进信息
                        var tranfollowup = await _icustomerTransactionsFollowUpStore.ListAsync(a => a.Where(b => b.CustomerTransactionsId == item.Id && !b.IsDeleted), cancellationToken);
                        foreach (var x in tranfollowup)
                        {
                            x.UserId = transferringRequest.TerUserId;
                        }
                        customertranfulist.AddRange(tranfollowup);
                    }
                    foreach (var item in customerbelt)
                    {
                        item.UserId = transferringRequest.TerUserId;
                        item.DepartmentId = transferringRequest.TerDepartmentId;
                    }
                    foreach (var item in demand)
                    {
                        item.UserId = transferringRequest.TerUserId;
                        item.DepartmentId = transferringRequest.TerDepartmentId;
                    }
                    customerfulist.AddRange(customerfu);
                    customertranlist.AddRange(customertran);
                    customerbeltlist.AddRange(customerbelt);
                    customerdemandlist.AddRange(demand);

                    newcustomerfu.Add(new CustomerFollowUp
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = customerInfo[i].Id,
                        TypeId = CustomerFollowUpType.TransferCustomer,
                        UserId = customerInfo[i].UserId,
                        DepartmentId = customerInfo[i].DepartmentId,
                        FollowUpTime = DateTime.Now,
                        TrueName = customerInfo[i].CustomerName,
                        FollowUpContents = "从 " + transferringRequest.SourceDepartmentId + ": " + transferringRequest.SourceUserId + "转移至 " + transferringRequest.TerDepartmentName + ": " + transferringRequest.TerUserId,
                        CustomerNo = customerInfo[i].CustomerNo,
                        IsRealFollow = false,
                        CreateTime = DateTime.Now,
                        CreateUser = customerInfo[i].UserId

                    });


                }
            }
            else
            {
                foreach (var item in customerInfo)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.UserId = transferringRequest.TerUserId;
                    item.DepartmentId = transferringRequest.TerDepartmentId;
                    item.BeltNum = 0;
                    item.AboutNum = 0;
                    item.FollowUpNum = 0;
                }
            }
            await _icustomerInfoStore.TransferringListAsync(customerInfo, customerfulist, newcustomerfu, customertranlist, customertranfulist, customerbeltlist, customerdemandlist, cancellationToken);
        }

        /// <summary>
        /// 查询意向楼盘下的客户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="buildingid"></param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<RelationHouseResponse>> SearchRelationHouse(string userId, string buildingid, CustomerPageCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new PagingResponseMessage<RelationHouseResponse>();

            //暂时为仅是自己的客户
            var query = _icustomerInfoStore.CustomerInfoInRelationAll().Where(x => !x.IsDeleted && x.HousingResourcesId == buildingid && x.UserId == userId);

            pagingResponse.TotalCount = await query.CountAsync(cancellationToken);


            //需要加上排序
            var qlist = await query.OrderByDescending(x => x.CreateTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            qlist = qlist.Select(x => { x.MainPhone = new Help.EncDncHelper().Decrypt(x.MainPhone); return x; }).ToList();

            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = _mapper.Map<List<RelationHouseResponse>>(qlist);
            return pagingResponse;
        }

        /// <summary>
        /// 查询所有客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerInfoSearchResponse>> Search(UserInfo user, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new PagingResponseMessage<CustomerInfoSearchResponse>();
            var query = _icustomerInfoStore.CustomerInfoAll().Where(x => !x.IsDeleted);

            query = SearchConditionFiltration(condition, query);

            //查询部门
            if (!string.IsNullOrEmpty(condition.DepartmentId))
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(condition.DepartmentId);
                query = query.Where(x => organs.Contains(x.DepartmentId));
            }
            else
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
                query = query.Where(x => organs.Contains(x.DepartmentId));
            }

            pagingResponse.TotalCount = await query.CountAsync(cancellationToken);


            //需要加上排序
            var qlist = await query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToListAsync(cancellationToken);
            qlist = qlist.Select(x =>
            {
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                //if (!condition.IsCompletenessPhone)
                x.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                //else
                //    x.MainPhone = new Help.EncDncHelper().Decrypt(x.MainPhone);
                return x;
            }).ToList();

            pagingResponse.PageIndex = condition.pageIndex;
            pagingResponse.PageSize = condition.pageSize;
            pagingResponse.Extension = _mapper.Map<List<CustomerInfoSearchResponse>>(qlist);
            return pagingResponse;
        }

        /// <summary>
        /// 业务员客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<CustomerSearchSalemanResponse<CustomerRepeatReponse>> SearchSalemanRepeat(UserInfo user, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var Response = new CustomerSearchSalemanResponse<CustomerRepeatReponse>();

            //初始筛选SQL
            var sql = @"select MainPhone as Phone,count(*) as Count
                        from xyh_ky_customerinfo AS a
                        LEFT JOIN xyh_ky_customerdemand AS b ON a.Id = b.CustomerId
                        LEFT JOIN identityuser AS c ON a.UserId = c.Id
                        LEFT JOIN organizations AS d ON a.DepartmentId = d.Id
                        where !a.IsDeleted and (a.CustomerStatus=1 or (a.CustomerStatus=4 and a.IsSellIntention))";
            //查询主键信息
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                if (condition?.SearchType == 0)
                {
                    sql += @" and (a.MainPhone='" + new Help.EncDncHelper().Encrypt(condition.KeyWord) + "' or a.CustomerName LIKE '%" + condition.KeyWord + "%')";
                }
                else
                {
                    sql += @" and (c.TrueName like '%" + condition.KeyWord + "%' or c.PhoneNumber like '%" + condition.KeyWord + "%' or c.UserName='" + condition.KeyWord + "')";
                }
            }
            //查询部门
            if (!string.IsNullOrEmpty(condition.DepartmentId))
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(condition.DepartmentId);
                sql += @" and a.DepartmentId in ('" + string.Join("','", organs.ToArray()) + "')";
            }
            else
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
                sql += @" and a.DepartmentId in ('" + string.Join("','", organs.ToArray()) + "')";
            }

            if (condition.Importance != null)
            {
                sql += @" and b.Importance = " + (int)condition.Importance + "";
            }
            if (condition.DemandLevel != null)
            {
                sql += @" and b.DemandLevel = " + (int)condition.DemandLevel + "";
            }
            if (condition.BusinessStage != null && condition.BusinessStage.Count != 0)
            {
                if (condition.BusinessStage.Count == 2)
                {
                    sql += @" and (a.RateProgress=3 or a.RateProgress=4 or a.RateProgress=5 or a.RateProgress=6 or a.RateProgress=7)";
                }
                else if (condition.BusinessStage[0] == 0)
                {
                    sql += @" and a.RateProgress=3";
                }
                else
                {
                    sql += @" and (a.RateProgress=4 or a.RateProgress=5 or a.RateProgress=6 or a.RateProgress=7)";
                }
            }
            if (condition.Source != null && condition.Source.Count != 0)
            {
                sql += @" and a.Source in ('" + string.Join("','", condition.Source.ToArray()) + "')";
            }
            if (!string.IsNullOrEmpty(condition.CityId))
            {
                sql += @" and b.DistrictId='" + condition.CityId + "'";
            }
            if (!string.IsNullOrEmpty(condition.AreaId))
            {
                sql += @" and b.AreaId='" + condition.AreaId + "'";
            }
            if (!string.IsNullOrEmpty(condition.ProvinceId))
            {
                sql += @" and b.CityId='" + condition.ProvinceId + "'";
            }
            if (condition.PriceStart != null)
            {
                sql += @" and b.PriceStart>=" + condition.PriceStart + "";
            }
            if (condition.PriceEnd != null)
            {
                sql += @" and b.PriceEnd<=" + condition.PriceEnd + "";
            }
            if (condition.AcreageStart != null)
            {
                sql += @" and b.AcreageStart>=" + condition.AcreageStart + "";
            }
            if (condition.AcreageEnd != null)
            {
                sql += @" and b.AcreageEnd<=" + condition.AcreageEnd + "";
            }
            if (condition.CreateDateStart != null)
            {
                sql += @" and a.CreateTime>='" + condition.CreateDateStart + "'";
            }
            if (condition.CreateDateEnd != null)
            {
                sql += @" and a.CreateTime<='" + condition.CreateDateEnd.Value.AddDays(1) + "'";
            }
            if (condition.FollowUpStart != null)
            {
                sql += @" and a.FollowupTime>='" + condition.FollowUpStart + "'";
            }
            if (condition.FollowUpEnd != null)
            {
                sql += @" and a.FollowupTime<='" + condition.FollowUpEnd.Value.AddDays(1) + "'";
            }
            sql += " group by a.MainPhone HAVING count(*) > 1";
            var sdsdsfsf = sql.Replace("select MainPhone as Phone,count(*) as Count", "SELECT count(*) FROM (select MainPhone ") + ") as v";
            Response.TotalCount = _icustomerInfoStore.DapperSelect<int?>(sql.Replace("select MainPhone as Phone,count(*) as Count", "SELECT count(*) FROM (select MainPhone ")+ ") as v").FirstOrDefault().Value;
            sql += @" limit " + condition.pageIndex * condition.pageSize + "," + condition.pageSize + "";
            var query = new List<CustomerRepeatReponse>();

            //sql += @" limit " + condition.pageIndex * condition.pageSize + "," + condition.pageSize + "";
            query = _icustomerInfoStore.DapperSelect<CustomerRepeatReponse>(sql).Select(x =>
            {
                x.Phone = new Help.EncDncHelper().Decrypt(x.Phone);
                return x;
            }).ToList();
            Response.PageIndex = condition.pageIndex;
            Response.PageSize = condition.pageSize;
            Response.Extension = query;
            return Response;
        }

        /// <summary>
        /// 业务员客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<List<CustomerSearchSaleman>> GetCustomersByPhone(UserInfo user, string phone, CancellationToken cancellationToken = default(CancellationToken))
        {
            var encphone = new Help.EncDncHelper().Encrypt(phone);
            var response = await _icustomerInfoStore.CustomerInfoAll().Where(x => !x.IsDeleted && x.MainPhone == encphone && (x.CustomerStatus == CustomerStatus.ExistingCustomers || (x.CustomerStatus == CustomerStatus.EndDeal && x.IsSellIntention == true))).ToListAsync(cancellationToken);
            foreach (var item in response)
            {
                item.DepartmentName = _iorganizationExpansionStore.GetFullName(item.DepartmentId);
                item.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(item.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                item.FollowUpHouses = (await _icustomerTransactionsStore.ListAsync(x => x.Where(y => y.CustomerId == item.Id))).OrderByDescending(y => y.ReportTime).FirstOrDefault()?.BuildingName;
                item.CustomerDeal = _icustomerDealStore.CustomerDealAll().Where(y => y.Customer == item.Id/* && y.Salesman == item.UserId*/ && !y.IsDeleted && y.ExamineStatus == (int)ExamineStatusEnum.Approved).OrderByDescending(y => y.CreateTime).FirstOrDefault();
            }
            return _mapper.Map<List<CustomerSearchSaleman>>(response);

        }

        /// <summary>
        /// 业务员客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<CustomerSearchSalemanResponse<CustomerSearchSaleman>> SearchSaleman(UserInfo user, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var Response = new CustomerSearchSalemanResponse<CustomerSearchSaleman>();

            //初始筛选SQL
            var sql = @"SELECT a.*,c.TrueName as UserName,c.UserName as UserNumber from xyh_ky_customerinfo as a 
                        LEFT JOIN xyh_ky_customerdemand as b ON a.Id = b.CustomerId
                        LEFT JOIN identityuser as c ON a.UserId = c.Id
                        LEFT JOIN organizations as d ON a.DepartmentId = d.Id
                        where !a.IsDeleted and (a.CustomerStatus=1 or (a.CustomerStatus=4 and a.IsSellIntention))";


            //查询主键信息
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                if (condition?.SearchType == 0)
                {
                    sql += @" and (a.MainPhone='" + new Help.EncDncHelper().Encrypt(condition.KeyWord) + "' or a.CustomerName LIKE '%" + condition.KeyWord + "%')";
                }
                else
                {
                    sql += @" and (c.TrueName like '%" + condition.KeyWord + "%' or c.PhoneNumber like '%" + condition.KeyWord + "%' or c.UserName='" + condition.KeyWord + "')";
                }
            }
            //查询部门
            if (!string.IsNullOrEmpty(condition.DepartmentId))
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(condition.DepartmentId);
                sql += @" and a.DepartmentId in ('" + string.Join("','", organs.ToArray()) + "')";
            }
            else
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
                sql += @" and a.DepartmentId in ('" + string.Join("','", organs.ToArray()) + "')";
            }

            if (condition.Importance != null)
            {
                sql += @" and b.Importance = " + (int)condition.Importance + "";
            }
            if (condition.DemandLevel != null)
            {
                sql += @" and b.DemandLevel = " + (int)condition.DemandLevel + "";
            }
            if (condition.BusinessStage != null && condition.BusinessStage.Count != 0)
            {
                if (condition.BusinessStage.Count == 2)
                {
                    sql += @" and (a.RateProgress=3 or a.RateProgress=4 or a.RateProgress=5 or a.RateProgress=6 or a.RateProgress=7)";
                }
                else if (condition.BusinessStage[0] == 0)
                {
                    sql += @" and a.RateProgress=3";
                }
                else
                {
                    sql += @" and (a.RateProgress=4 or a.RateProgress=5 or a.RateProgress=6 or a.RateProgress=7)";
                }
            }
            if (condition.Source != null && condition.Source.Count != 0)
            {
                sql += @" and a.Source in ('" + string.Join("','", condition.Source.ToArray()) + "')";
            }
            if (!string.IsNullOrEmpty(condition.CityId))
            {
                sql += @" and b.DistrictId='" + condition.CityId + "'";
            }
            if (!string.IsNullOrEmpty(condition.AreaId))
            {
                sql += @" and b.AreaId='" + condition.AreaId + "'";
            }
            if (!string.IsNullOrEmpty(condition.ProvinceId))
            {
                sql += @" and b.CityId='" + condition.ProvinceId + "'";
            }
            if (condition.PriceStart != null)
            {
                sql += @" and b.PriceStart>=" + condition.PriceStart + "";
            }
            if (condition.PriceEnd != null)
            {
                sql += @" and b.PriceEnd<=" + condition.PriceEnd + "";
            }
            if (condition.AcreageStart != null)
            {
                sql += @" and b.AcreageStart>=" + condition.AcreageStart + "";
            }
            if (condition.AcreageEnd != null)
            {
                sql += @" and b.AcreageEnd<=" + condition.AcreageEnd + "";
            }
            if (condition.CreateDateStart != null)
            {
                sql += @" and a.CreateTime>='" + condition.CreateDateStart + "'";
            }
            if (condition.CreateDateEnd != null)
            {
                sql += @" and a.CreateTime<='" + condition.CreateDateEnd.Value.AddDays(1) + "'";
            }
            if (condition.FollowUpStart != null)
            {
                sql += @" and a.FollowupTime>='" + condition.FollowUpStart + "'";
            }
            if (condition.FollowUpEnd != null)
            {
                sql += @" and a.FollowupTime<='" + condition.FollowUpEnd.Value.AddDays(1) + "'";
            }

            //放在前面 不然的话该SQL会加一些莫名其妙的
            var validitycount = _icustomerInfoStore.DapperSelect<int?>(sql.Replace("SELECT a.*,c.TrueName as UserName,c.UserName as UserNumber ", "select Count(distinct a.MainPhone)")).FirstOrDefault().Value;

            //var validitycount = _icustomerInfoStore.DapperSelect<int?>("SELECT count(t.counts) from (select count(b.Id) counts from(" + sql + ") as b group BY b.UniqueId) t").FirstOrDefault().Value;
            Response.ValidityCustomerCount = validitycount;
            //条件已经过滤完毕 这里就直接=了
            if (condition.IsOnlyRepeat)
            {
                sql = sql + " and a.UniqueId IN (" + sql.Replace("SELECT a.*,c.TrueName as UserName,c.UserName as UserNumber ", "SELECT a.UniqueId ") + "GROUP BY a.UniqueId HAVING count(*) > 1)" + "";
            }
            Response.TotalCount = _icustomerInfoStore.DapperSelect<int?>(sql.Replace("SELECT a.*,c.TrueName as UserName,c.UserName as UserNumber ", "SELECT COUNT(*) ")).FirstOrDefault().Value;
            if (condition.OrderRule)
            {
                sql += @" ORDER BY a.FollowupTime DESC";
            }
            else
            {
                sql += @" ORDER BY a.FollowupTime";
            }

            var query = new List<CustomerInfo>();

            sql += @" limit " + condition.pageIndex * condition.pageSize + "," + condition.pageSize + "";
            query = _icustomerInfoStore.DapperSelect<CustomerInfo>(sql).ToList();

            //需要加上排序

            foreach (var item in query)
            {
                item.DepartmentName = _iorganizationExpansionStore.GetFullName(item.DepartmentId);
                item.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(item.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                item.FollowUpHouses = (await _icustomerTransactionsStore.ListAsync(x => x.Where(y => y.CustomerId == item.Id))).OrderByDescending(y => y.ReportTime).FirstOrDefault()?.BuildingName;
                item.CustomerDeal = _icustomerDealStore.CustomerDealAll().Where(y => y.Customer == item.Id/* && y.Salesman == item.UserId*/ && !y.IsDeleted && y.ExamineStatus == (int)ExamineStatusEnum.Approved).OrderByDescending(y => y.CreateTime).FirstOrDefault();
            }


            Response.PageIndex = condition.pageIndex;
            Response.PageSize = condition.pageSize;
            Response.Extension = _mapper.Map<List<CustomerSearchSaleman>>(query);
            return Response;
        }

        /// <summary>
        /// 查询业务员重复客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerSearchSaleman>> SearchRepetitionSaleman(UserInfo user, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new PagingResponseMessage<CustomerSearchSaleman>();

            var response = _icustomerInfoStore.CustomerInfoAll().Where(x => !x.IsDeleted);

            response = SearchConditionFiltration(condition, response);
            //查询部门
            if (!string.IsNullOrEmpty(condition.DepartmentId))
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(condition.DepartmentId);
                response = response.Where(x => organs.Contains(x.DepartmentId));
            }
            else
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
                response = response.Where(x => organs.Contains(x.DepartmentId));
            }



            var s1 = await response.ToListAsync(cancellationToken);
            var query = s1.GroupBy(x => x.UniqueId).Where(x => x.Count() > 1).SelectMany(x => x).ToList();

            pagingResponse.TotalCount = query.Count();
            //需要加上排序
            var qlist = query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToList();
            qlist = qlist.Select(x =>
            {
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                x.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                return x;
            }).ToList();



            pagingResponse.PageIndex = condition.pageIndex;
            pagingResponse.PageSize = condition.pageSize;
            pagingResponse.Extension = _mapper.Map<List<CustomerSearchSaleman>>(qlist);
            return pagingResponse;
        }

        /// <summary>
        /// 查询客户判重
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="MainPhone">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<CustomerHeavy> SearchCustomerHeavy(UserInfo user, string MainPhone, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(MainPhone))
            {
                throw new ArgumentNullException(nameof(MainPhone));
            }
            var Response = new CustomerHeavy();
            Response.MainPhone = MainPhone;
            //筛选
            var organids = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
            var response = await _icustomerInfoStore.CustomerInfoFowllowUp().Where(x => !x.IsDeleted && x.MainPhone == new Help.EncDncHelper().Encrypt(MainPhone)).ToListAsync();

            if (response.Where(x => organids.Contains(x.DepartmentId))?.Count() == 0 || response?.Count == 0)
                return null;

            var sourceids = response.Where(x => !string.IsNullOrEmpty(x.SourceId)).Select(x => x.SourceId);
            while (sourceids.Count() != 0)
            {
                var customers = await _icustomerInfoStore.CustomerInfoFowllowUp().Where(x => sourceids.Contains(x.Id)).ToListAsync();
                response.AddRange(customers);
                sourceids = customers.Where(x => string.IsNullOrEmpty(x.SourceId)).Select(x => x.SourceId);
            }

            Response.CustomerName = response.FirstOrDefault()?.CustomerName;


            var listful = new List<FollowUpResponse>();
            foreach (var item in response)
            {
                if (item.CustomerStatus == CustomerStatus.EndDeal)
                {
                    Response.CustomerName = item.CustomerName;
                }
                item.CustomerFollowUp = item.CustomerFollowUp.Where(x => x.TypeId == CustomerFollowUpType.BeltLook || x.TypeId == CustomerFollowUpType.CustomerReported || x.TypeId == CustomerFollowUpType.EndDeal || x.TypeId == CustomerFollowUpType.JoinPool || x.TypeId == CustomerFollowUpType.ClaimCustomer);
                var list = item.CustomerFollowUp.Select(x =>
                     {
                         x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                         return x;
                     }).ToList();
                listful.AddRange(_mapper.Map<List<FollowUpResponse>>(list));
            }
            Response.FollowUpResponses = listful.OrderByDescending(x => x.FollowUpTime).ToList();



            return Response;
        }

        /// <summary>
        /// 公客池客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerSearchPool>> SearchCustomerpool(UserInfo user, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new PagingResponseMessage<CustomerSearchPool>();



            var query = _icustomerInfoStore.CustomerInfoInPoolAll().Where(x => !x.IsDeleted);

            query = SearchConditionFiltration(condition, query);

            //查询部门
            if (!string.IsNullOrEmpty(condition.DepartmentId))
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(condition.DepartmentId);
                query = query.Where(x => organs.Contains(x.DepartmentId));
            }
            else
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
                query = query.Where(x => organs.Contains(x.DepartmentId));
            }


            pagingResponse.TotalCount = await query.CountAsync(cancellationToken);
            //需要加上排序
            var qlist = await query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToListAsync(cancellationToken);
            qlist = qlist.Select(x =>
            {
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                x.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                return x;
            }).OrderByDescending(x => x.CustomerPool.JoinDate).ToList();

            pagingResponse.PageIndex = condition.pageIndex;
            pagingResponse.PageSize = condition.pageSize;
            pagingResponse.Extension = _mapper.Map<List<CustomerSearchPool>>(qlist);
            return pagingResponse;
        }

        /// <summary>
        /// 公客池客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerSearchPool>> SearchCustomerpoolSaleman(UserInfo user, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var organs = await _permissionExpansionManager.GetParentDepartments(user.OrganizationId);

            var pagingResponse = new PagingResponseMessage<CustomerSearchPool>();

            var query = _icustomerInfoStore.CustomerInfoInPoolAll().Where(x => !x.IsDeleted && organs.Contains(x.DepartmentId));

            query = SearchConditionFiltration(condition, query);



            pagingResponse.TotalCount = await query.CountAsync(cancellationToken);
            //需要加上排序
            var qlist = await query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToListAsync(cancellationToken);
            qlist = qlist.Select(x =>
            {
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                x.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                return x;
            }).OrderByDescending(x => x.CustomerPool.JoinDate).ToList();

            pagingResponse.PageIndex = condition.pageIndex;
            pagingResponse.PageSize = condition.pageSize;
            pagingResponse.Extension = _mapper.Map<List<CustomerSearchPool>>(qlist);
            return pagingResponse;
        }

        /// <summary>
        /// 已成交客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerSearchSaleman>> SearchCustomerDeal(UserInfo user, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new PagingResponseMessage<CustomerSearchSaleman>();

            var query = _icustomerInfoStore.CustomerInfoInDealAll().Where(x => !x.IsDeleted && x.CustomerStatus == CustomerStatus.EndDeal);

            query = SearchConditionFiltration(condition, query);

            //查询部门
            if (!string.IsNullOrEmpty(condition.DepartmentId))
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(condition.DepartmentId);
                query = query.Where(x => organs.Contains(x.DepartmentId));
            }
            else
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
                query = query.Where(x => organs.Contains(x.DepartmentId));
            }



            pagingResponse.TotalCount = await query.CountAsync(cancellationToken);
            //需要加上排序
            var qlist = await query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToListAsync(cancellationToken);
            qlist = qlist.Select(x =>
            {
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                x.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                return x;
            }).OrderByDescending(x => x.CustomerDeal.CreateTime).ToList();

            pagingResponse.PageIndex = condition.pageIndex;
            pagingResponse.PageSize = condition.pageSize;
            pagingResponse.Extension = _mapper.Map<List<CustomerSearchSaleman>>(qlist);
            return pagingResponse;
        }

        /// <summary>
        /// 已失效客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerSearchLoss>> SearchCustomerLoss(UserInfo user, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new PagingResponseMessage<CustomerSearchLoss>();

            var query = _icustomerInfoStore.CustomerInfoInLossAll().Where(x => !x.IsDeleted && x.CustomerStatus == CustomerStatus.Lapse);

            query = SearchConditionFiltration(condition, query);

            //查询部门
            if (!string.IsNullOrEmpty(condition.DepartmentId))
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(condition.DepartmentId);
                query = query.Where(x => organs.Contains(x.DepartmentId));
            }
            else
            {
                var organs = await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId);
                query = query.Where(x => organs.Contains(x.DepartmentId));
            }



            pagingResponse.TotalCount = await query.CountAsync(cancellationToken);
            //需要加上排序
            var qlist = await query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToListAsync(cancellationToken);
            qlist = qlist.Select(x =>
            {
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                x.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                return x;
            }).OrderByDescending(x => x.CustomerLoss.LossTime).ToList();

            pagingResponse.PageIndex = condition.pageIndex;
            pagingResponse.PageSize = condition.pageSize;
            pagingResponse.Extension = _mapper.Map<List<CustomerSearchLoss>>(qlist);
            return pagingResponse;
        }

        /// <summary>
        /// 已失效客户客户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="condition">条件</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<CustomerSearchLoss>> SearchCustomerLossSalesMan(string userId, CustomerListSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new PagingResponseMessage<CustomerSearchLoss>();

            var query = _icustomerInfoStore.CustomerInfoInLossAll().Where(x => !x.IsDeleted && x.UserId == userId);

            query = SearchConditionFiltration(condition, query);



            pagingResponse.TotalCount = await query.CountAsync(cancellationToken);
            //需要加上排序
            var qlist = await query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToListAsync(cancellationToken);
            qlist = qlist.Select(x =>
            {
                x.DepartmentName = _iorganizationExpansionStore.GetFullName(x.DepartmentId);
                x.MainPhone = Regex.Replace(new Help.EncDncHelper().Decrypt(x.MainPhone), "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
                return x;
            }).OrderByDescending(x => x.CustomerLoss.LossTime).ToList();

            pagingResponse.PageIndex = condition.pageIndex;
            pagingResponse.PageSize = condition.pageSize;
            pagingResponse.Extension = _mapper.Map<List<CustomerSearchLoss>>(qlist);
            return pagingResponse;
        }

        /// <summary>
        /// 修改是否仍有购买意向
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="customerid"></param>
        /// <param name="mark"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage> UpdateSellIntention(string userid, string customerid, bool mark, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(customerid))
            {
                throw new ArgumentNullException(nameof(customerid));
            }
            var pagingResponse = new ResponseMessage();

            var query = await _icustomerInfoStore.GetAsync(x => x.Where(y => y.Id == customerid && y.UserId == userid));
            if (query == null)
            {
                pagingResponse.Code = ResponseCodeDefines.NotAllow;
                pagingResponse.Message = "修改失败";
                return pagingResponse;
            }
            if (!mark)
            {
                //在此处修改为成交客户
                query.CustomerStatus = CustomerStatus.EndDeal;
            }
            query.IsSellIntention = mark;
            await _icustomerInfoStore.UpdateAsync(query);
            return pagingResponse;
        }

        public IQueryable<CustomerInfo> SearchConditionFiltration(CustomerListSearchCondition condition, IQueryable<CustomerInfo> query)
        {
            //查询主键信息
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                if (condition?.SearchType == 0)
                {
                    query = query.Where(x => x.MainPhone == new Help.EncDncHelper().Encrypt(condition.KeyWord) || x.CustomerName.Contains(condition.KeyWord));
                }
                else
                {
                    query = query.Where(x => x.UserName.Contains(condition.KeyWord) || x.UserPhone.Contains(condition.KeyWord) || x.UserNumber == condition.KeyWord);
                }
            }

            if (condition.Importance != null)
            {
                query = query.Where(x => x.CustomerDemand.Importance == condition.Importance);
            }
            if (condition.DemandLevel != null)
            {
                query = query.Where(x => x.CustomerDemand.DemandLevel == condition.DemandLevel);
            }
            if (condition.BusinessStage != null && condition.BusinessStage.Count != 0)
            {
                if (condition.BusinessStage.Count == 2)
                {
                    query = query.Where(x => x.RateProgress == RateProgress.TakeALookAt || x.RateProgress == RateProgress.Look || x.RateProgress == RateProgress.SencondLook || x.RateProgress == RateProgress.SincerityGold || x.RateProgress == RateProgress.Negotiation);
                }
                else if (condition.BusinessStage[0] == 0)
                {
                    query = query.Where(x => x.RateProgress == RateProgress.TakeALookAt);
                }
                else
                {
                    query = query.Where(x => x.RateProgress == RateProgress.Look || x.RateProgress == RateProgress.SencondLook || x.RateProgress == RateProgress.SincerityGold || x.RateProgress == RateProgress.Negotiation);
                }
            }
            if (condition.Source != null && condition.Source.Count != 0)
            {
                query = query.Where(x => condition.Source.Contains(x.Source));
            }
            if (!string.IsNullOrEmpty(condition.CityId))
            {
                query = query.Where(x => x.CustomerDemand.DistrictId == condition.CityId);
            }
            if (!string.IsNullOrEmpty(condition.AreaId))
            {
                query = query.Where(x => x.CustomerDemand.AreaId == condition.AreaId);
            }
            if (!string.IsNullOrEmpty(condition.ProvinceId))
            {
                query = query.Where(x => x.CustomerDemand.CityId == condition.ProvinceId);
            }
            if (condition.PriceStart != null)
            {
                query = query.Where(x => x.CustomerDemand.PriceStart >= condition.PriceStart);
            }
            if (condition.PriceEnd != null)
            {
                query = query.Where(x => x.CustomerDemand.PriceEnd <= condition.PriceEnd);
            }
            if (condition.AcreageStart != null)
            {
                query = query.Where(x => x.CustomerDemand.AcreageStart >= condition.AcreageStart);
            }
            if (condition.AcreageEnd != null)
            {
                query = query.Where(x => x.CustomerDemand.AcreageEnd <= condition.AcreageEnd);
            }
            if (condition.CreateDateStart != null)
            {
                query = query.Where(x => x.CreateTime >= condition.CreateDateStart);
            }
            if (condition.CreateDateEnd != null)
            {
                query = query.Where(x => x.CreateTime <= condition.CreateDateEnd.Value.AddDays(1));
            }
            if (condition.FollowUpStart != null)
            {
                query = query.Where(x => x.FollowupTime >= condition.FollowUpStart);
            }
            if (condition.FollowUpEnd != null)
            {
                query = query.Where(x => x.FollowupTime <= condition.FollowUpEnd.Value.AddDays(1));
            }
            if (condition.OrderRule)
            {
                query = query.OrderByDescending(x => x.FollowupTime);
            }
            else
            {
                query = query.OrderBy(x => x.FollowupTime);
            }



            return query;
        }


        /// <summary>
        /// 根据楼盘查询推荐客户
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public async Task<PagingResponseMessage<CustomerListResponse>> RecommendFromBuilding(UserInfo user, BuildingRecommendRequest condition)
        {
            PagingResponseMessage<CustomerListResponse> r = new PagingResponseMessage<CustomerListResponse>();
            var list = await _icustomerInfoStore.RecommendFromBuilding<CustomerListResponse, CustomerDemandResponse>(user.Id, condition, (a, b) =>
            {
                a.Deamand = b;
                return a;
            });

            r.Extension = list.OrderBy(x => x.Deamand.Importance ?? Importance.NoValue).ThenBy(x => x.Deamand.DemandLevel ?? DemandLevel.Suspend).ThenByDescending(x => x.FollowupTime).ToList();

            var decoder = new Help.EncDncHelper();
            r.Extension.ForEach(item =>
            {

                item.MainPhone = decoder.Decrypt(item.MainPhone);
            });

            return r;

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
        /// 获取业务员的组织名 到大区
        /// </summary>
        /// <returns></returns>
        public List<string> GetParentIds(List<CustomerPoolDefine> PoolDefines, string oraganid)
        {
            var respons = new List<string>();

            var organization = PoolDefines.Where(x => x.DepartmentId == oraganid);
            var organ = organization.FirstOrDefault();
            if (organ == null)
            {
                var xcxcxcxc = PoolDefines.Where(x => x.ParentId == "0").FirstOrDefault();
                respons.Add(xcxcxcxc.Id);
            }
            else
            {
                if (organ.ParentId != "0")
                {
                    var result = GetParentIds(PoolDefines, organ.ParentId);
                    respons.AddRange(result);
                }
                respons.Add(organ.Id);
            }
            return respons;
        }
    }
}
