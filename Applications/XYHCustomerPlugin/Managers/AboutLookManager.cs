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
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    public class AboutLookManager
    {
        #region 成员

        protected IAboutLookStore _iaboutLookStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public AboutLookManager(IAboutLookStore iaboutLookStore, IMapper mapper)
        {
            _iaboutLookStore = iaboutLookStore ?? throw new ArgumentNullException(nameof(iaboutLookStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 根据Id获取带看信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<AboutLookResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _iaboutLookStore.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<AboutLookResponse>(response);
        }

        /// <summary>
        /// 新增带看信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="aboutLookRequest"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<AboutLookResponse> CreateAsync(UserInfo user, AboutLookRequest aboutLookRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (aboutLookRequest == null)
            {
                throw new ArgumentNullException(nameof(aboutLookRequest));
            }

            var aboutLook = _mapper.Map<AboutLook>(aboutLookRequest);
            //带看基本信息
            aboutLook.Id = Guid.NewGuid().ToString();
            aboutLook.CreateUser = user.Id;
            aboutLook.CreateTime = DateTime.Now;
            aboutLook.UserId = user.Id;
            aboutLook.DepartmentId = user.OrganizationId;
            aboutLook.AboutState = AboutLookState.WaitLook;

            await _iaboutLookStore.CreateAsync(aboutLook, cancellationToken);

            return _mapper.Map<AboutLookResponse>(aboutLook);
        }

        /// <summary>
        /// 删除带看信息
        /// </summary>
        /// <param name="user">删除人基本信息</param>
        /// <param name="id">删除客源Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _iaboutLookStore.DeleteAsync(_mapper.Map<SimpleUser>(user), new AboutLook() { Id = id });
        }

        /// <summary>
        /// 修改单个客源信息
        /// </summary>
        /// <param name="id">请求者Id</param>
        /// <param name="aboutLookRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(string userId, AboutLookRequest aboutLookRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (aboutLookRequest == null)
            {
                throw new ArgumentNullException(nameof(aboutLookRequest));
            }
            var aboutLook = await _iaboutLookStore.GetAsync(a => a.Where(b => b.Id == aboutLookRequest.Id && !b.IsDeleted));
            if (aboutLook == null)
            {
                return;
            }
            var newaboutLook = _mapper.Map<AboutLook>(aboutLookRequest);
            //客户基本信息
            newaboutLook.IsDeleted = aboutLook.IsDeleted;
            newaboutLook.CreateTime = aboutLook.CreateTime;
            newaboutLook.CreateUser = aboutLook.CreateUser;
            newaboutLook.UpdateTime = DateTime.Now;
            newaboutLook.UpdateUser = userId;
            await _iaboutLookStore.UpdateAsync(newaboutLook, cancellationToken);
        }



        /// <summary>
        /// 查询我的带看
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<PagingResponseMessage<AboutLookResponse>> SelectMyCustomer(string id, MyAboutLookCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            var reponse = new PagingResponseMessage<AboutLookResponse>();

            var list = new List<AboutLookResponse>();
            var q = _iaboutLookStore.AboutLookAll().Where(x => !x.IsDeleted).Where(x => x.UserId == id);
            reponse.TotalCount = await q.CountAsync();
            if (condition != null)
            {
                if (condition.mark == 1)
                {
                    q = q.Where(x => x.AboutState == AboutLookState.WaitLook);
                }
                else if (condition.mark == 2)
                {
                    q = q.Where(x => x.AboutState == AboutLookState.WaitDeal);
                }
                else if (condition.mark == 3)
                {
                    q = q.Where(x => x.AboutState == AboutLookState.EndDeal);
                }
                else if (condition.mark == 4)
                {
                    q = q.Where(x => x.AboutState == AboutLookState.Cancel);
                }
            }
            q = q.OrderByDescending(x => x.AboutTime).Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize);
            var result = await q.ToListAsync();
            list.AddRange(_mapper.Map<List<AboutLookResponse>>(result));

            reponse.PageIndex = condition.pageIndex;
            reponse.PageSize = condition.pageSize;
            reponse.Extension = list;
            return reponse;
        }
    }
}
