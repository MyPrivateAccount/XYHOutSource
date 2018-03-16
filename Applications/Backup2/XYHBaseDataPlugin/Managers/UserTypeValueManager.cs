using ApplicationCore.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Dto.Request;
using XYHBaseDataPlugin.Dto.Response;
using XYHBaseDataPlugin.Models;
using XYHBaseDataPlugin.Stores;

namespace XYHBaseDataPlugin.Managers
{
    public class UserTypeValueManager
    {
        #region 成员

        protected IUserTypeValueStore _iuserTypeValueStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public UserTypeValueManager(
            IUserTypeValueStore ibuildingnosStore,
            IMapper mapper)
        {
            _iuserTypeValueStore = ibuildingnosStore ?? throw new ArgumentNullException(nameof(ibuildingnosStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 根据UserId获取用户定义类型信息
        /// </summary>
        /// <param name="Userid"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<UserTypeValueResponse>> FindByUserIdAsync(string Userid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _iuserTypeValueStore.ListAsync(a => a.Where(b => b.UserId == Userid), cancellationToken);
            return _mapper.Map<List<UserTypeValueResponse>>(response);
        }

        /// <summary>
        /// 根据UserId、Type获取用户定义类型信息
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="type"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<UserTypeValueResponse> FindByUserIdAndTypeAsync(string UserId, string type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _iuserTypeValueStore.GetAsync(a => a.Where(b => b.UserId == UserId && b.Type == type), cancellationToken);
            return _mapper.Map<UserTypeValueResponse>(response);
        }

        /// <summary>
        /// 根据类型获取用户定义类型信息
        /// </summary>
        /// <param name="Userid"></param>
        /// <param name="type">type</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<UserTypeValueResponse>> FindByTypeAsync(string Userid, string type, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _iuserTypeValueStore.ListAsync(a => a.Where(b => b.Type == type && b.UserId == Userid), cancellationToken);
            return _mapper.Map<List<UserTypeValueResponse>>(response);
        }

        /// <summary>
        /// 新增用户定义类型信息
        /// </summary>
        /// <param name="user">创建者</param>
        /// <param name="userTypeValueRequest">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<UserTypeValueResponse> CreateAsync(UserInfo user, UserTypeValueRequest userTypeValueRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userTypeValueRequest == null)
            {
                throw new ArgumentNullException(nameof(userTypeValueRequest));
            }
            var buildingno = _mapper.Map<UserTypeValue>(userTypeValueRequest);

            buildingno.Id = Guid.NewGuid().ToString();
            buildingno.UserId = user.Id;
            try
            {
                await _iuserTypeValueStore.CreateAsync(buildingno, cancellationToken);
            }
            catch
            {
            }
            return _mapper.Map<UserTypeValueResponse>(buildingno);
        }

        /// <summary>
        /// 修改单个用户定义类型信息
        /// </summary>
        /// <param name="user">请求者</param>
        /// <param name="userTypeValueRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task UpdateAsync(UserInfo user, UserTypeValueRequest userTypeValueRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userTypeValueRequest == null)
            {
                throw new ArgumentNullException(nameof(userTypeValueRequest));
            }

            var userTypeValue = await _iuserTypeValueStore.GetAsync(a => a.Where(b => b.UserId == user.Id && b.Type == userTypeValueRequest.Type));
            if (userTypeValue == null)
            {
                return;
            }
            userTypeValue.Value = userTypeValueRequest.Value;
            try
            {
                await _iuserTypeValueStore.UpdateAsync(userTypeValue, cancellationToken);
            }
            catch { }
        }

        /// <summary>
        /// 批量删除用户定义类型信息
        /// </summary>
        /// <param name="userId">删除人Id</param>
        /// <param name="ids">删除Id数组</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userTypeValue = await _iuserTypeValueStore.ListAsync(a => a.Where(b => ids.Contains(b.Id)), cancellationToken);
            if (userTypeValue == null || userTypeValue.Count == 0)
            {
                return;
            }
            try
            {
                await _iuserTypeValueStore.DeleteListAsync(userTypeValue, cancellationToken);
            }
            catch { }
        }
    }
}
