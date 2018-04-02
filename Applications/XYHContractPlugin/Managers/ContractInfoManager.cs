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
using ContractInfoRequest = XYHContractPlugin.Dto.Response.ContractInfoResponse;
using ContractContentInfoRequest = XYHContractPlugin.Dto.Response.ContractContentResponse;
using ContractComplementRequest = XYHContractPlugin.Dto.Response.ContractComplementResponse;
using ApplicationCore;

namespace XYHContractPlugin.Managers
{
    public class ContractInfoManager
    {
        public static int CreateContract = 1;
        public static int ModifyContract = 2;
        public static int AddAnnexContract = 3;
        public static int AddComplementContract = 4;
        public static int ModifyComplementContract = 5;

        public ContractInfoManager(IContractInfoStore contractStore, IMapper mapper)
        {
            Store = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected IContractInfoStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<ContractInfoResponse> CreateAsync(UserInfo userinfo, ContractInfoRequest buildingBaseInfoRequest, string modifyid, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            var baseinfo = await Store.CreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<ContractInfo>(buildingBaseInfoRequest), modifyid, checkaction, cancellationToken);
            return _mapper.Map<ContractInfoResponse>(baseinfo);
        }

        public virtual async Task<ContractInfoResponse> AddContractAsync(UserInfo userinfo, ContractContentInfoRequest buildingBaseInfoRequest, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }

            if (buildingBaseInfoRequest.AnnexInfo != null && buildingBaseInfoRequest.AnnexInfo.Count > 0)
            {
                await Store.CreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<List<AnnexInfo>>(buildingBaseInfoRequest.AnnexInfo), cancellationToken);
            }

            if (buildingBaseInfoRequest.ComplementInfo != null && buildingBaseInfoRequest.ComplementInfo.Count >0)
            {
                await Store.CreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<List<ComplementInfo>>(buildingBaseInfoRequest.ComplementInfo), cancellationToken);
            }

            var baseinfo = await Store.CreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<ContractInfo>(buildingBaseInfoRequest),
                (buildingBaseInfoRequest.Modifyinfo!=null&&buildingBaseInfoRequest.Modifyinfo.Count>0)?buildingBaseInfoRequest.Modifyinfo.ElementAt(0).ID:null, checkaction, cancellationToken);
            return _mapper.Map<ContractInfoResponse>(baseinfo);
        }

        public virtual async Task<bool> AddComplementAsync(UserInfo userinfo, string strcontractid, string strCheck, string strModify, List<ContractComplementRequest> buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }

            bool ret= false;
            if (buildingBaseInfoRequest != null && buildingBaseInfoRequest.Count > 0)
            {
                foreach (var itm in buildingBaseInfoRequest)
                {
                    if (string.IsNullOrEmpty(itm.ID))
                    {
                        itm.ID = Guid.NewGuid().ToString();
                    }

                    itm.ContractID = strcontractid;
                }

                ret = await Store.CreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<List<ComplementInfo>>(buildingBaseInfoRequest), cancellationToken);
            }

            await Store.CreateModifyAsync(_mapper.Map<SimpleUser>(userinfo), strcontractid, strModify, AddComplementContract, strCheck, ExamineStatusEnum.Auditing);

            return ret;
        }

        public virtual async Task<bool> ModifyComplementAsync(UserInfo userinfo, string strcontractid, string strCheck, string strModify, List<ContractComplementRequest> buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }

            bool ret = false;

            foreach (var itm in buildingBaseInfoRequest)
            {
                if (string.IsNullOrEmpty(itm.ID))
                {
                    buildingBaseInfoRequest.Remove(itm);
                }
            }
            if (buildingBaseInfoRequest != null && buildingBaseInfoRequest.Count > 0)
            {
                await Store.UpdateListAsync(_mapper.Map<List<ComplementInfo>>(buildingBaseInfoRequest), cancellationToken);
                ret = true;
            }

            await Store.CreateModifyAsync(_mapper.Map<SimpleUser>(userinfo), strcontractid, strModify, ModifyComplementContract, strCheck, ExamineStatusEnum.Auditing);

            return ret;
        }

        public virtual async Task<string> ModifyContractBeforCheckAsync(UserInfo userinfo, ContractContentInfoRequest buildingBaseInfoRequest, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }

            string guid = Guid.NewGuid().ToString();
            await Store.CreateModifyAsync(_mapper.Map<SimpleUser>(userinfo),
                buildingBaseInfoRequest.BaseInfo.ID,
                guid, ModifyContract, checkaction, ExamineStatusEnum.UnSubmit, true,
                JsonHelper.ToJson(buildingBaseInfoRequest),null, cancellationToken);//2是修改

            return guid;
        }

        public virtual async Task ModifyContractAsync(ContractContentInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }

            if (buildingBaseInfoRequest.AnnexInfo != null && buildingBaseInfoRequest.AnnexInfo.Count > 0)
                await Store.UpdateListAsync(_mapper.Map<List<AnnexInfo>>(buildingBaseInfoRequest.AnnexInfo), cancellationToken);

            if (buildingBaseInfoRequest.ComplementInfo != null && buildingBaseInfoRequest.ComplementInfo.Count > 0)
                await Store.UpdateListAsync(_mapper.Map<List<ComplementInfo>>(buildingBaseInfoRequest.ComplementInfo), cancellationToken);

            await Store.UpdateAsync(_mapper.Map<ContractInfo>(buildingBaseInfoRequest), cancellationToken);
        }

        public virtual async Task ModifyContractAfterCheckAsync(string modifyid, string contractid, ExamineStatusEnum ext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (modifyid == null)
            {
                throw new ArgumentNullException(nameof(modifyid));
            }

            var tv = await Store.GetModifyAsync(a => a.Where(b=> b.ID == modifyid), cancellationToken);
            var tc = await Store.GetAsync(a => a.Where(b => b.ID==contractid));

            if (ext == ExamineStatusEnum.Approved && tv.Type == ModifyContract && tc.CurrentModify == tv.ID)//当前修改的审核，批准才能修改数据
            {
                var modifyedinfo = JsonHelper.ToObject<ContractContentInfoRequest>(tv.Ext1);
                await ModifyContractAsync(modifyedinfo, cancellationToken);
            }
        }

        public virtual async Task<List<ContractModifyResponse>> GetAllModifyInfo(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var data = await Store.GetListModifyAsync(a => a.Where(b => b.ContractID == id), cancellationToken);
            return _mapper.Map<List<ContractModifyResponse>>(data);
        }

        public virtual async Task<ContractModifyResponse> GetCurrentModifyInfo(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var contract = await Store.GetAsync(a => a.Where(b => b.ID == id));
            var data = await Store.GetModifyAsync(a => a.Where(b => b.ID == contract.CurrentModify), cancellationToken);
            return _mapper.Map<ContractModifyResponse>(data);
        }

        public virtual async Task<ContractInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.ID == id), cancellationToken);
            if (baseinfo == null)
            {
                throw new ArgumentNullException("无合同");
            }
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


        //public virtual async Task<ContractContentResponse> GetAllinfoByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    var baseinfo = await Store.GetAsync(a => a.Where(b => b.ID == id), cancellationToken);

        //    var modifyinfo = await Store.GetListModifyAsync(a => a.Where(b => b.ContractID == id));
        //    baseinfo.Modify = modifyinfo.Count;

        //    var rt = _mapper.Map<ContractContentResponse>(baseinfo);
        //    foreach (var item in modifyinfo)
        //    {
        //        var mf = _mapper.Map<ContractModifyResponse>(item);
        //        if (rt.Modifyinfo == null)
        //        {
        //            rt.Modifyinfo = new List<ContractModifyResponse>();
        //        }
        //        rt.Modifyinfo.Add(mf);
        //    }
        //    return rt;
        //}

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

                var modifyinfo = await Store.GetListModifyAsync(a => a.Where(b => b.ContractID == id));
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

        public virtual async Task<ContractContentResponse> GetAllinfoByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contractinfo = await Store.GetAsync(a => a.Where(b => b.ID == id), cancellationToken);
            var returninfo = _mapper.Map<ContractContentResponse>(contractinfo);

            var annexinfo = await Store.GetListAnnexAsync(a => a.Where(b => b.ContractID == id), cancellationToken);
            returninfo.AnnexInfo = _mapper.Map<List<ContractAnnexResponse>>(annexinfo);

            var complementinfo = await Store.GetListComplementAsync(a => a.Where(b => b.ContractID == id));
            returninfo.ComplementInfo = _mapper.Map<List<ContractComplementResponse>>(complementinfo);

            var modifyinfo = await Store.GetListModifyAsync(a => a.Where(b => b.ContractID == id));
            returninfo.Modifyinfo = _mapper.Map<List<ContractModifyResponse>>(modifyinfo);

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
