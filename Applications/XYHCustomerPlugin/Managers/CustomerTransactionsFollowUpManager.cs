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
    /// <summary>
    /// 成交信息模块处理
    /// </summary>
    public class CustomerTransactionsFollowUpManager
    {
        #region 成员

        /// <summary>
        /// 实现接口
        /// </summary>
        protected ICustomerInfoStore _icustomerInfoStore { get; }

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
        /// <param name="icustomerTransactionsFollowUpStore"></param>
        /// <param name="icustomerInfoStore"></param>
        /// <param name="mapper"></param>
        public CustomerTransactionsFollowUpManager(ICustomerTransactionsFollowUpStore icustomerTransactionsFollowUpStore, ICustomerInfoStore icustomerInfoStore, IMapper mapper)
        { 
            _icustomerInfoStore = icustomerInfoStore ?? throw new ArgumentNullException(nameof(icustomerInfoStore));
            _icustomerTransactionsFollowUpStore = icustomerTransactionsFollowUpStore ?? throw new ArgumentNullException(nameof(icustomerTransactionsFollowUpStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 根据Id获取成交信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<TransactionsFollowUpResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _icustomerTransactionsFollowUpStore.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<TransactionsFollowUpResponse>(response);
        }

        /// <summary>
        /// 新增成交信息
        /// </summary>
        /// <param name="user">创建者</param>
        /// <param name="transactionsFollowUpRequest">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage<TransactionsFollowUpResponse>> CreateAsync(UserInfo user, TransactionsFollowUpRequest transactionsFollowUpRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (transactionsFollowUpRequest == null)
            {
                throw new ArgumentNullException(nameof(transactionsFollowUpRequest));
            }

            var respons = new ResponseMessage<TransactionsFollowUpResponse>();

            var tsfollowUp = _mapper.Map<CustomerTransactionsFollowUp>(transactionsFollowUpRequest);
            tsfollowUp.Id = Guid.NewGuid().ToString();
            tsfollowUp.UserId = user.Id;
            tsfollowUp.CreateTime = DateTime.Now;
            tsfollowUp.CreateUser = user.Id;
            try
            {

                await _icustomerTransactionsFollowUpStore.CreateAsync(tsfollowUp, cancellationToken);

                respons.Code = ResponseCodeDefines.SuccessCode;
                respons.Message = "创建成功";
                respons.Extension = _mapper.Map<TransactionsFollowUpResponse>(tsfollowUp);
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
