using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using XYHContractPlugin.Models;
using XYHContractPlugin.Stores;
using XYHContractPlugin.Dto.Response;
using System.Threading;
using System.Linq;
using ApplicationCore.Dto;
using ApplicationCore.Models;
using CompanyAInfoRequest = XYHContractPlugin.Dto.Response.CompanyAInfoResponse;
using ApplicationCore;
using XYHContractPlugin.Dto.Request;
using Microsoft.EntityFrameworkCore;

namespace XYHContractPlugin.Managers
{
     public  class ExtraInfoDataManager
    {
        public ExtraInfoDataManager(IExtraDataInfoStrore extraData,IContractInfoStore contractStore, IMapper mapper)
        {
            Store = extraData ?? throw new ArgumentNullException(nameof(extraData));
            _contractStore = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected IExtraDataInfoStrore Store { get; set; }
        protected IContractInfoStore _contractStore { get; set; }
        protected IMapper _mapper { get; }

        public virtual async Task<CompanyAInfoResponse> CreateAsync(UserInfo userInfo, CompanyAInfoRequest companyAInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(companyAInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(companyAInfoRequest));
            }

            var companyAInfo = _mapper.Map<CompanyAInfo>(companyAInfoRequest);
            if(string.IsNullOrEmpty(companyAInfo.ID))
            {
                companyAInfo.ID = Guid.NewGuid().ToString();
            }
            companyAInfo.CreateUser = userInfo.Id;
            companyAInfo.CreateTime = DateTime.Now;
            var info = await Store.CreateAsync(companyAInfo, cancellationToken);
            return _mapper.Map<CompanyAInfoResponse>(info);
        }

        public virtual async  Task<ResponseMessage> DeleteListAsync(UserInfo userInfo, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<string> tempIds = new List<string>();
            List<CompanyAInfo> infos = new List<CompanyAInfo>();
            ResponseMessage response = new ResponseMessage();
            foreach(var item in ids)
            {
                var contract = await _contractStore.GetAsync(x => x.Where(a => a.CompanyAId == item));
                if(contract == null)
                {
                    var companyAInfo = await Store.GetAsync(x => x.Where(a => !a.IsDelete && a.ID == item));
                    companyAInfo.DeleteTime = DateTime.Now;
                    companyAInfo.DeleteUser = userInfo.Id;
                    companyAInfo.IsDelete = true;
                    infos.Add(companyAInfo);
              
                }
    
            }
            
            if(ids.Count > infos.Count)
            {
                response.Message = "部分删除完成";
                response.Code = "部分删除";
            }
            else 
            {
                response.Message = "全部删除完成";
            }
            if (infos.Count == 0)
            {
                response.Message = "删除失败";
                response.Code = "删除失败";
                return response;
            }
            await Store.DeleteListAsync(userInfo,infos, cancellationToken);
            return response;
        }
        public virtual async Task DeleteAsync(UserInfo userInfo, CompanyAInfo companyAInfo, CancellationToken cancellationToken = default(CancellationToken))
        {

            //var companyAInfo = Store.GetAsync<CompanyAInfo>(a => a.Where(b => b.ID == id), cancellationToken);
            if (companyAInfo == null)
            {
                throw new ArgumentNullException(nameof(companyAInfo));
            }

            await Store.DeleteAsync(userInfo, companyAInfo, cancellationToken);
        }
        public virtual async Task ModifyAsync(CompanyAInfoRequest companyAInfoRequest, CancellationToken cancellation = default(CancellationToken))
        {
            if (companyAInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(companyAInfoRequest));
            }

            var companyAInfo = _mapper.Map<CompanyAInfo>(companyAInfoRequest);
            await Store.UpdateAsync(companyAInfo);
           
        }

        public virtual async Task<CompanyAInfo> GetCompanyAInfoAsync(string id, CancellationToken cancellation = default(CancellationToken))
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var companyAInfo = await Store.GetAsync<CompanyAInfo>(a => a.Where(b => b.ID == id), cancellation);
            return companyAInfo;
        }
        public virtual async Task<ResponseMessage<List<CompanyAInfoResponse>>> GetAllAsync(UserInfo user,  CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<List<CompanyAInfoResponse>> response = new ResponseMessage<List<CompanyAInfoResponse>>();
            
            var info = await Store.ListAsync<CompanyAInfo>(a => a.Where(b => !b.IsDelete), cancellationToken);
            response.Extension = _mapper.Map<List<CompanyAInfoResponse>>(info);
            return response;
        }
        public virtual async Task<CompanyASearchResponse<CompanyAInfoResponse>> Search(UserInfo user, CompanyASearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var pagingResponse = new CompanyASearchResponse<CompanyAInfoResponse>();
            var query = Store.CompanyAInfoAll().Where(a => !a.IsDelete);
            query = SearchConditionFiltration(condition, query);

            pagingResponse.TotalCount = await query.CountAsync(cancellationToken);
            //需要加上排序
            var qlist = await query.Skip(condition.pageIndex * condition.pageSize).Take(condition.pageSize).ToListAsync(cancellationToken);


            pagingResponse.PageIndex = condition.pageIndex;
            pagingResponse.PageSize = condition.pageSize;
            pagingResponse.Extension = _mapper.Map<List<CompanyAInfoResponse>>(qlist);
            return pagingResponse;
        }

        public IQueryable<CompanyAInfo> SearchConditionFiltration(CompanyASearchCondition condition, IQueryable<CompanyAInfo> query)
        {
            //查询主键信息
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {

               query = query.Where(x => x.Name.Contains(condition.KeyWord) || x.Address.Contains(condition.KeyWord) || x.PhoneNum == condition.KeyWord);
                
            }

            if (!string.IsNullOrEmpty(condition.Address))
            {
                query = query.Where(x => x.Address.Contains(condition.Address));
            }
            if (!string.IsNullOrEmpty(condition.SearchType) )
            {
                query = query.Where(x => x.Type == condition.SearchType);
            }

            if (condition.OrderRule)
            {
                query = query.OrderByDescending(x => x.CreateTime);
            }
            else
            {
                query = query.OrderBy(x => x.Type);
            }



            return query;
        }
    }
}
