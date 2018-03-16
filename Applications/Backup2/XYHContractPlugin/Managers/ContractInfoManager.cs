﻿using System;
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
using ContractInfoRequest = XYHContractPlugin.Dto.Response.ContractInfoResponse;
using ContractContentInfoRequest = XYHContractPlugin.Dto.Response.ContractContentResponse;

namespace XYHContractPlugin.Managers
{
    public class ContractInfoManager
    {
        public ContractInfoManager(IContractInfoStore contractStore, IMapper mapper)
        {
            Store = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected IContractInfoStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<ContractInfoResponse> CreateAsync(UserInfo userinfo, ContractInfoRequest buildingBaseInfoRequest, string modifyid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            var baseinfo = await Store.CreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<ContractInfo>(buildingBaseInfoRequest), modifyid,cancellationToken);
            return _mapper.Map<ContractInfoResponse>(baseinfo);
        }

        public virtual async Task<ContractInfoResponse> AddContractAsync(UserInfo userinfo, ContractContentInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (buildingBaseInfoRequest == null || buildingBaseInfoRequest.Modifyinfo.Count < 1)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }

            var baseinfo = await Store.CreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<ContractInfo>(buildingBaseInfoRequest), buildingBaseInfoRequest.Modifyinfo.ElementAt(0).ID, cancellationToken);
            return _mapper.Map<ContractInfoResponse>(baseinfo);
        }

        public virtual async Task<ContractInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.ID == id), cancellationToken);
            if (baseinfo.IsDelete)
            {
                throw new ArgumentNullException("已被删除");
            }
            return _mapper.Map<ContractInfoResponse>(baseinfo);
        }

        public virtual async Task<ContractModifyResponse> CurrentModifyByContractIdAsync(string contractid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.ID == contractid), cancellationToken);
            var modifyinfo = await Store.GetModifyAsync(a => a.Where(b => b.ID == baseinfo.CurrentModify));
            if (baseinfo.IsDelete)
            {
                throw new ArgumentNullException("已被删除");
            }
            return _mapper.Map<ContractModifyResponse>(modifyinfo);
        }


        public virtual async Task<ContractContentResponse> GetAllinfoByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.ID == id), cancellationToken);

            var modifyinfo = await Store.ListModifyAsync(a => a.Where(b => b.ContractID == id));
            baseinfo.Modify = modifyinfo.Count;

            var rt = _mapper.Map<ContractContentResponse>(baseinfo);
            foreach (var item in modifyinfo)
            {
                var mf = _mapper.Map<ContractModifyResponse>(item);
                if (rt.Modifyinfo == null)
                {
                    rt.Modifyinfo = new List<ContractModifyResponse>();
                }
                rt.Modifyinfo.Add(mf);
            }
            return rt;
        }

        public virtual async Task<List<ContractContentResponse>> GetAllListinfoByUserIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.ListAsync(a => a.Where(b => b.CreateUser == id), cancellationToken);
            var returninfo = new List<ContractContentResponse>();

            foreach (var item in baseinfo)
            {
                if (item.IsDelete)
                {
                    continue;
                }

                var modifyinfo = await Store.ListModifyAsync(a => a.Where(b => b.ContractID == id));
                item.Modify = modifyinfo.Count;

                var rt = _mapper.Map<ContractContentResponse>(item);
                foreach (var it in modifyinfo)
                {
                    var mf = _mapper.Map<ContractModifyResponse>(it);
                    if (rt.Modifyinfo == null)
                    {
                        rt.Modifyinfo = new List<ContractModifyResponse>();
                    }
                    rt.Modifyinfo.Add(mf);
                }
                returninfo.Add(rt);
            }
            
            return returninfo;
        }

        public virtual async Task UpdateAsync(ContractInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            await Store.UpdateAsync(_mapper.Map<ContractInfo>(buildingBaseInfoRequest), cancellationToken);
        }


        public virtual async Task SaveAsync(UserInfo user, ContractInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }


            await Store.SaveAsync(_mapper.Map<SimpleUser>(user), _mapper.Map<ContractInfo>(buildingBaseInfoRequest), cancellationToken);
        }

        public virtual async Task SubmitAsync(string modifyid , ExamineStatusEnum ext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (modifyid == null)
            {
                throw new ArgumentNullException(nameof(modifyid));
            }
            await Store.UpdateExamineStatus(modifyid, ext, cancellationToken);
        }

        public virtual async Task SubmitAsync(ContractCheckInfoRequest checkinfo, ExamineStatusEnum ext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (checkinfo == null)
            {
                throw new ArgumentNullException(nameof(checkinfo));
            }
            await Store.UpdateExamineStatus(checkinfo.ModifyID, ext, cancellationToken);
        }

        public virtual async Task DiscardAsync(UserInfo userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contractid == null)
            {
                throw new ArgumentNullException(nameof(contractid));
            }

            await Store.DeleteAsync(_mapper.Map<SimpleUser>(userinfo), contractid, cancellationToken);
        }
    }

}