using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using AutoMapper;
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
    public class CustomerFollowUpManager
    {
        #region 成员
        private readonly PermissionExpansionManager _permissionExpansionManager;

        protected ICustomerInfoStore _icustomerInfoStore { get; }

        protected ICustomerFollowUpStore _icustomerFollowUpStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public CustomerFollowUpManager(ICustomerFollowUpStore icustomerFollowUpStore, ICustomerInfoStore icustomerInfoStore, IMapper mapper, PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager;
            _icustomerInfoStore = icustomerInfoStore ?? throw new ArgumentNullException(nameof(icustomerInfoStore));
            _icustomerFollowUpStore = icustomerFollowUpStore ?? throw new ArgumentNullException(nameof(icustomerFollowUpStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 根据Id获取带看信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<FollowUpResponse> FindByIdAsync(string userid, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _icustomerFollowUpStore.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<FollowUpResponse>(response);
        }

        /// <summary>
        /// 根据UserId获取带看信息
        /// </summary>
        /// <param name="userid">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<FollowUpResponse>> FindByUserIdAsync(string userid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _icustomerFollowUpStore.ListAsync(a => a.Where(b => b.UserId == userid && !b.IsDeleted).OrderByDescending(x => x.FollowUpTime), cancellationToken);
            return _mapper.Map<List<FollowUpResponse>>(response);
        }

        /// <summary>
        /// 根据customerid获取带看信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="customerid">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<FollowUpResponse>> FindByCustomerIdAsync(string userid, string customerid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _icustomerFollowUpStore.ListAsync(a => a.Where(b => b.CustomerId == customerid && !b.IsDeleted).OrderByDescending(x => x.FollowUpTime), cancellationToken);




            return _mapper.Map<List<FollowUpResponse>>(response);
        }

        /// <summary>
        /// 新增跟进信息
        /// </summary>
        /// <param name="user">创建者</param>
        /// <param name="followUpRequest">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage<FollowUpResponse>> CreateAsync(UserInfo user, FollowUpRequest followUpRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (followUpRequest == null)
            {
                throw new ArgumentNullException(nameof(followUpRequest));
            }

            var respons = new ResponseMessage<FollowUpResponse>();

            var followUp = _mapper.Map<CustomerFollowUp>(followUpRequest);
            //新增跟进信息
            followUp.Id = Guid.NewGuid().ToString();
            followUp.TypeId = CustomerFollowUpType.WriteFollowUp;
            followUp.FollowUpTime = followUp.FollowUpTime == null ? DateTime.Now : followUp.FollowUpTime;
            followUp.CreateUser = user.Id;
            followUp.CreateTime = DateTime.Now;
            followUp.UserId = user.Id;
            followUp.DepartmentId = user.OrganizationId;
            followUp.IsRealFollow = true;
            //更新原来的用户数据跟进时间

            var customerinfo = await _icustomerInfoStore.GetAsync(a => a.Where(b => b.Id == followUpRequest.CustomerId && !b.IsDeleted));

            if (customerinfo.UserId != user.Id)
                return null;

            followUp.TrueName = customerinfo.CustomerName;
            followUp.CustomerNo = customerinfo.CustomerNo;
            followUp.IsNewHourse = customerinfo.IsNewHourse;



            try
            {

                await _icustomerFollowUpStore.CreateAsync(followUp, cancellationToken);

                respons.Code = ResponseCodeDefines.SuccessCode;
                respons.Message = "创建成功";
                respons.Extension = _mapper.Map<FollowUpResponse>(followUp);
            }
            catch (Exception e)
            {
                respons.Code = ResponseCodeDefines.ServiceError;
                respons.Message = "服务器错误：" + e.ToString();
            }

            return respons;
        }
    }
}
