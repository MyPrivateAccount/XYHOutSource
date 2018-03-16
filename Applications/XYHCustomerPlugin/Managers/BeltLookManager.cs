using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class BeltLookManager
    {
        #region 成员

        protected IBeltLookStore _ibeltLookStore { get; }

        protected ICustomerReportStore _icustomerreportstore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public BeltLookManager(IBeltLookStore ibeltLookStore, ICustomerReportStore icustomerreportstore, IMapper mapper)
        {
            _ibeltLookStore = ibeltLookStore ?? throw new ArgumentNullException(nameof(ibeltLookStore));
            _icustomerreportstore = icustomerreportstore ?? throw new ArgumentNullException(nameof(icustomerreportstore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 根据Id获取带看信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<BeltLookResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _ibeltLookStore.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<BeltLookResponse>(response);
        }

        /// <summary>
        /// 根据Userid查询我的带看
        /// </summary>
        /// <param name="userid">userid</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<BeltLookResponse>> FindBeltLookByMyAsync(string userid, CustomerPageCondition pageCondition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (pageCondition == null)
            {
                throw new ArgumentNullException(nameof(pageCondition));
            }
            var pagingResponse = new PagingResponseMessage<BeltLookResponse>();

            var response = await _ibeltLookStore.ListAsync(a => a.Where(b => b.UserId == userid && !b.IsDeleted), cancellationToken);
            var result = response.OrderByDescending(a => a.CreateTime).Skip(pageCondition.PageIndex * pageCondition.PageSize).Take(pageCondition.PageSize).ToList();
            pagingResponse.TotalCount = response.Count;
            pagingResponse.PageIndex = pageCondition.PageIndex;
            pagingResponse.PageSize = pageCondition.PageSize;

            pagingResponse.Extension = _mapper.Map<List<BeltLookResponse>>(result);

            return pagingResponse;
        }

        /// <summary>
        /// 新增带看信息
        /// </summary>
        /// <param name="userId">创建者</param>
        /// <param name="customerInfoCreateResponse">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<BeltLookResponse> CreateAsync(UserInfo user, BeltLookRequest beltLookRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (beltLookRequest == null)
            {
                throw new ArgumentNullException(nameof(beltLookRequest));
            }


            var beltLook = _mapper.Map<BeltLook>(beltLookRequest);
            //带看基本信息
            beltLook.Id = Guid.NewGuid().ToString();
            beltLook.CreateUser = user.Id;
            beltLook.CreateTime = DateTime.Now;
            beltLook.UserId = user.Id;
            beltLook.DepartmentId = user.OrganizationId;
            //beltLook.BeltState = BeltLookState.WaitLook;

            var report = await _icustomerreportstore.GetAsync(a => a.Where(b => b.BuildingId == beltLook.BeltHouse && !b.IsDeleted && b.CustomerId == beltLook.CustomerId ), cancellationToken);

            if (report != null)
            {
                await _ibeltLookStore.CreateAsync(beltLook, cancellationToken);



                //report.ReportStatus = ReportStatus.;
                await _icustomerreportstore.UpdateAsync(report, cancellationToken);
            }
            else
            {
                return null;
            }

            return _mapper.Map<BeltLookResponse>(beltLook);
        }

        /// <summary>
        /// 删除带看信息
        /// </summary>
        /// <param name="user">删除人基本信息</param>
        /// <param name="id">删除带看Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _ibeltLookStore.DeleteAsync(_mapper.Map<Models.SimpleUser>(user), new BeltLook() { Id = id });
        }

        /// <summary>
        /// 修改单个带看信息
        /// </summary>
        /// <param name="id">请求者Id</param>
        /// <param name="beltLookRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(string userId, BeltLookRequest beltLookRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (beltLookRequest == null)
            {
                throw new ArgumentNullException(nameof(beltLookRequest));
            }
            var beltLook = await _ibeltLookStore.GetAsync(a => a.Where(b => b.Id == beltLookRequest.Id && !b.IsDeleted));
            if (beltLook == null)
            {
                return;
            }
            var newbeltLook = _mapper.Map<BeltLook>(beltLookRequest);
            //带看基本信息
            newbeltLook.IsDeleted = beltLook.IsDeleted;
            newbeltLook.CreateTime = beltLook.CreateTime;
            newbeltLook.CreateUser = beltLook.CreateUser;
            newbeltLook.UpdateTime = DateTime.Now;
            newbeltLook.UpdateUser = userId;
            await _ibeltLookStore.UpdateAsync(newbeltLook, cancellationToken);
        }



        /// <summary>
        /// 查询我的带看
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<BeltLookResponse>> SelectMyCustomer(string id, MyBeltLookCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var reponse = new PagingResponseMessage<BeltLookResponse>();

            var list = new List<BeltLookResponse>();
            var q = _ibeltLookStore.BeltLookAll().Where(x => !x.IsDeleted).Where(x => x.UserId == id);
            reponse.TotalCount = await q.CountAsync();
            if (condition != null)
            {
                if (condition.mark == 1)
                {
                    q = q.Where(x => x.BeltState == BeltLookState.WaitLook);
                }
                else if (condition.mark == 2)
                {
                    q = q.Where(x => x.BeltState == BeltLookState.WaitDeal);
                }
                else if (condition.mark == 3)
                {
                    q = q.Where(x => x.BeltState == BeltLookState.EndDeal);
                }
                else if (condition.mark == 4)
                {
                    q = q.Where(x => x.BeltState == BeltLookState.Cancel);
                }
            }
            q = q.OrderByDescending(x => x.BeltTime).Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize);
            var result = await q.ToListAsync();
            list.AddRange(_mapper.Map<List<BeltLookResponse>>(result));

            reponse.PageIndex = condition.pageIndex;
            reponse.PageSize = condition.pageSize;
            reponse.Extension = list;
            return reponse;
        }



        /// <summary>
        /// 查询报备信息接口
        /// </summary>
        /// <param name="id"></param>
        /// <param name="searchcondition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<BeltLookResponse>> Search(string id, BeltLookSearchRequest searchcondition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var pagingResponse = new PagingResponseMessage<BeltLookResponse>();

            var response = _ibeltLookStore.BeltLookAll().Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(searchcondition.BeltHouse))
            {
                response = response.Where(x => x.BeltHouse.Contains(searchcondition.BeltHouse));
            }


            var result = await response.OrderByDescending(a => a.CreateTime).Skip(searchcondition.PageIndex * searchcondition.PageSize).Take(searchcondition.PageSize).ToListAsync(cancellationToken);

            pagingResponse.TotalCount = await response.CountAsync(cancellationToken);
            pagingResponse.PageIndex = searchcondition.PageIndex;
            pagingResponse.PageSize = searchcondition.PageSize;

            pagingResponse.Extension = _mapper.Map<List<BeltLookResponse>>(result);

            return pagingResponse;
        }
    }
}
