using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    public class CustomerLossManager
    {
        public CustomerLossManager(ICustomerLossStore customerLossStore,
            ICustomerInfoStore icustomerInfoStore,
            IMapper mapper,
            PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            Store = customerLossStore ?? throw new ArgumentNullException(nameof(customerLossStore));
            _icustomerInfoStore = icustomerInfoStore ?? throw new ArgumentNullException(nameof(icustomerInfoStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected ICustomerLossStore Store { get; }
        private readonly PermissionExpansionManager _permissionExpansionManager;
        protected ICustomerInfoStore _icustomerInfoStore { get; }

        protected IMapper _mapper { get; }


        public async Task<ResponseMessage<CustomerLossResponse>> CreateAsync(UserInfo userInfo, CustomerLossRequest customerLossRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<CustomerLossResponse> response = new ResponseMessage<CustomerLossResponse>();
            if (customerLossRequest == null || userInfo == null)
            {
                throw new ArgumentNullException(nameof(customerLossRequest));
            }
            //修改客户状态
            var customerInfo = await _icustomerInfoStore.GetAsync(a => a.Where(b => b.Id == customerLossRequest.CustomerId && !b.IsDeleted));
            if (customerInfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "拉无效的客户未找到";
                return response;
            }
            if (!await _permissionExpansionManager.HavePermission(userInfo.Id, "LossCustomer", customerInfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限拉无效该客户";
                return response;
            }
            if (await Store.GetAsync(x => x.Where(y => !y.IsDeleted && y.CustomerId == customerLossRequest.CustomerId)) != null)
            {
                response.Message = "该客户已经无效";
                return response;
            }
            CustomerLoss customerLoss = new CustomerLoss();
            customerLoss.Id = Guid.NewGuid().ToString();
            customerLoss.CustomerId = customerLossRequest.CustomerId;
            customerLoss.HouseTypeId = NeedHouseType.BusinessToBuy;
            customerLoss.IsDeleted = false;
            customerLoss.LossDepartmentId = userInfo.OrganizationId;
            customerLoss.LossTime = DateTime.Now;
            customerLoss.LossUserId = userInfo.Id;
            customerLoss.LossTypeId = customerLossRequest.LossTypeId;
            customerLoss.LossRemark = customerLossRequest.LossRemark;
            customerLoss.CreateTime = DateTime.Now;

            //if ((await _permissionExpansionManager.GetLowerDepartments(userInfo.OrganizationId)).Contains(response.DepartmentId) && (await _permissionExpansionManager.HavePermission(userInfo.Id, "LossCustomer")))
            //{
            //    response.CustomerStatus = CustomerStatus.Lapse;
            //    customerLoss.CustomerInfo = response;
            response.Extension = _mapper.Map<CustomerLossResponse>(await Store.CreateAsync(customerLoss, cancellationToken));
            return response;
            //}
        }


        public async Task<PagingResponseMessage<CustomerLossResponse>> Search(string userid, CustomerLossCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            PagingResponseMessage<CustomerLossResponse> pagingResponse = new PagingResponseMessage<CustomerLossResponse>();
            var q = Store.SimpleQuery().Where(a => !a.IsDeleted);
            if (!string.IsNullOrEmpty(condition.KeyWords))
            {
                q = q.Where(a => a.CustomerInfo.CustomerName.Contains(condition.KeyWords));
            }
            if (condition.LossTimeStart != null)
            {
                q = q.Where(a => a.LossTime >= condition.LossTimeStart);
            }
            if (condition.LossTimeEnd != null)
            {
                q = q.Where(a => a.LossTime <= condition.LossTimeEnd);
            }
            if (condition.Types?.Count > 0)
            {
                q = q.Where(a => condition.Types.Contains(a.LossTypeId));
            }
            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.OrderByDescending(a => a.LossTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            var resulte = qlist.Select(a => new CustomerLossResponse
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                CustomerName = a.CustomerInfo.CustomerName,
                LossTime = a.LossTime,
                LossDepartmentId = a.LossDepartmentId,
                LossRemark = a.LossRemark,
                LossTypeId = a.LossTypeId,
                LossUserId = a.LossUserId,
                LossUserTrueName = a.LossUser.TrueName
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }

        public async Task<bool> Activate(UserInfo user, string Id, bool isDeleteOldData, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _icustomerInfoStore.GetAsync(a => a.Where(b => b.Id == Id));
            if (response.UserId == user.Id || ((await _permissionExpansionManager.GetLowerDepartments(user.OrganizationId)).Contains(response.DepartmentId) && (await _permissionExpansionManager.HavePermission(user.Id, "ActivateCustomer"))))
                return await Store.ActivateLossUser(user, Id, isDeleteOldData, cancellationToken);
            return false;


        }



        public async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }
            var list = await Store.ListAsync(a => a.Where(b => ids.Contains(b.Id)));

            for (int i = 0; i < list.Count; i++)
            {
                list[i].DeleteUserId = userId;
                list[i].IsDeleted = true;
                list[i].DeleteTime = DateTime.Now;
            }
            await Store.UpdateListAsync(list, cancellationToken);
        }

        public async Task<CustomerLossResponse> FindByIdAsync(string userid, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var q = Store.SimpleQuery().Where(a => a.Id == id);
            if (await q?.CountAsync(cancellationToken) > 0)
            {
                var customer = await q.FirstOrDefaultAsync(cancellationToken);
                return new CustomerLossResponse
                {
                    Id = customer.Id,
                    CustomerId = customer.CustomerId,
                    CustomerName = customer.CustomerInfo.CustomerName,
                    LossTime = customer.LossTime,
                    LossDepartmentId = customer.LossDepartmentId,
                    LossRemark = customer.LossRemark,
                    LossTypeId = customer.LossTypeId,
                    LossUserId = customer.LossUserId,
                    LossUserTrueName = customer.LossUser.TrueName
                };
            }
            return null;
        }



    }
}
