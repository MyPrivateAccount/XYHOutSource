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
using ApplicationCore.Managers;

namespace XYHContractPlugin.Managers
{
    public class ContractInfoManager
    {
        public static int CreateContract = 1;
        public static int ModifyContract = 2;
        public static int AddAnnexContract = 3;
        public static int UpdateComplementContract = 4;


        public ContractInfoManager(
            IContractInfoStore contractStore,
            IMapper mapper,
            IOrganizationExpansionStore organizationExpansionStore,
            PermissionExpansionManager permissionExpansionManager,
            UserManager userManager
            )
        {
            Store = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _iorganizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _curUserManager = userManager;
        }
        protected IContractInfoStore Store { get; }
        protected IMapper _mapper { get; }
        protected IOrganizationExpansionStore _iorganizationExpansionStore { get; }

        protected PermissionExpansionManager _permissionExpansionManager { get; }
        private UserManager _curUserManager { get; }
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
                    itm.CurrentModify = strModify;
                }

                ret = await Store.CreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<List<ComplementInfo>>(buildingBaseInfoRequest), cancellationToken);
            }

            await Store.CreateModifyAsync(_mapper.Map<SimpleUser>(userinfo), strcontractid, strModify, UpdateComplementContract, strCheck, ExamineStatusEnum.Auditing, false);

            return ret;
        }

        public virtual async Task<bool> AutoUpdateComplementAsync(UserInfo userinfo, string strcontractid, string strCheck, string strModify, List<ContractComplementRequest> buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null || buildingBaseInfoRequest.Count < 1)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }

            foreach (var itm in buildingBaseInfoRequest)
            {
                if (string.IsNullOrEmpty(itm.ID))
                {
                    itm.ID = Guid.NewGuid().ToString();
                }

                itm.ContractID = strcontractid;
                itm.CurrentModify = strModify;
            }

            await Store.CreateModifyAsync(_mapper.Map<SimpleUser>(userinfo), strcontractid, strModify, UpdateComplementContract, strCheck, ExamineStatusEnum.Auditing, false);

            return await Store.AutoCreateAsync(_mapper.Map<SimpleUser>(userinfo), _mapper.Map<List<ComplementInfo>>(buildingBaseInfoRequest), cancellationToken);
        }

        public virtual async Task CreateComplementModifyAsync(UserInfo userinfo, string strcontractid, string strCheck, string strModify, string ext1, string ext2, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Store.CreateModifyAsync(_mapper.Map<SimpleUser>(userinfo), strcontractid, strModify, UpdateComplementContract, strCheck, ExamineStatusEnum.Auditing, false, ext1, ext2);
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

                if (string.IsNullOrEmpty(itm.ContractID))
                {
                    itm.ContractID = strcontractid;
                }
                itm.CurrentModify = strModify;
            }
            if (buildingBaseInfoRequest != null && buildingBaseInfoRequest.Count > 0)
            {
                await Store.UpdateListAsync(_mapper.Map<List<ComplementInfo>>(buildingBaseInfoRequest), cancellationToken);
                ret = true;
            }

            await Store.CreateModifyAsync(_mapper.Map<SimpleUser>(userinfo), strcontractid, strModify, UpdateComplementContract, strCheck, ExamineStatusEnum.Auditing, false);

            return ret;
        }

        public virtual async Task<string> ModifyContractBeforCheckAsync(UserInfo userinfo, ContractContentInfoRequest buildingBaseInfoRequest, string strmodify, string checkaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }

            await Store.CreateModifyAsync(_mapper.Map<SimpleUser>(userinfo),
                buildingBaseInfoRequest.BaseInfo.ID,
                strmodify, ModifyContract, checkaction, ExamineStatusEnum.Auditing, true,
                JsonHelper.ToJson(buildingBaseInfoRequest),null);//2是修改

            return strmodify;
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

        public virtual async Task ModifyContractAfterCheckAsync(string modifyid, string contractid, string ext1, ExamineStatusEnum ext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (modifyid == null)
            {
                throw new ArgumentNullException(nameof(modifyid));
            }
            
            var tc = await Store.GetAsync(a => a.Where(b => b.ID==contractid));

            if (ext == ExamineStatusEnum.Approved && tc.CurrentModify == modifyid)//当前修改的审核，批准才能修改数据
            {
                var modifyedinfo = JsonHelper.ToObject<ContractContentInfoRequest>(ext1);
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
        /// <summary>
        /// 给合同分配编号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<string> GetContractNum(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.ID == id), cancellationToken);
            List<ContractInfo> baseList =  await Store.ListAsync(a => a.Where(b => true), cancellationToken);
            string temp = baseList.Max(a => a.Num);
            string prevTemp = "XYH" + DateTime.Now.ToString("yyyyMMdd");
            var contractNum = "";
            if (baseinfo == null || string.IsNullOrEmpty(baseinfo.Num))
            {
                if(temp != "" && temp.Contains(prevTemp))
                {
                    string nowTemp = string.Format("{0:d3}", int.Parse(temp.Substring(temp.Length - 3, 3)) + 1);
                    contractNum = string.Format("XYH{0}{1}", DateTime.Now.ToString("yyyyMMdd"), nowTemp);
                }
                else
                {
                    contractNum = string.Format("XYH{0}{1}", DateTime.Now.ToString("yyyyMMdd"), "001");
                }
                
            }
            else
            {
                contractNum = baseinfo.Num; 
            }
            return contractNum;
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
            if (annexinfo.Count > 0)
            {
                var modify = await Store.GetModifyAsync(a => a.Where(b => b.ID == annexinfo.ElementAt(0).CurrentModify), cancellationToken);
                foreach (var item in returninfo.AnnexInfo)
                {
                    item.ExamineStatus = (int)modify.ExamineStatus;
                }
            }

            var complementinfo = await Store.GetListComplementAsync(a => a.Where(b => b.ContractID == id));
            returninfo.ComplementInfo = _mapper.Map<List<ContractComplementResponse>>(complementinfo);
            if (complementinfo.Count > 0)
            {
                var modify = await Store.GetModifyAsync(a => a.Where(b => b.ID == complementinfo.ElementAt(0).CurrentModify), cancellationToken);
                if (modify != null)
                {
                    foreach (var item in returninfo.ComplementInfo)
                    {
                        item.ExamineStatus = (int)modify.ExamineStatus;
                    }
                }
                
            }

            var modifyinfo = await Store.GetListModifyAsync(a => a.Where(b => b.ContractID == id));
            returninfo.Modifyinfo = _mapper.Map<List<ContractModifyResponse>>(modifyinfo);

            return returninfo;
        }
        public virtual async Task<ContractContentResponse> GetAllinfoByIdAsync2(UserInfo user,string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = Store.ContractInfoAll();//.Where(x => x.ID == id);
            var qlist = query.ToList();
            var contractinfo = qlist.Where(x => x.ID == id).FirstOrDefault();
            if (contractinfo != null)
            {
                List<string> fullId = await _permissionExpansionManager.GetParentDepartments(contractinfo.OrganizateID);
                fullId.Remove("0");
                contractinfo.OrganizateFullId = string.Join("*", fullId.ToArray());

                contractinfo.Organizate = _iorganizationExpansionStore.GetFullName(contractinfo.OrganizateID).Replace("默认顶级-", "");
                contractinfo.CreateDepartment = _iorganizationExpansionStore.GetFullName(contractinfo.CreateDepartmentID).Replace("默认顶级-", "");
            }


            var followHistory = await GetFollowHistory(user, id);
            var returninfo = _mapper.Map<ContractContentResponse>(contractinfo);
            returninfo.FollowHistory = followHistory;
            var annexinfo = await Store.GetListAnnexAsync(a => a.Where(b => b.ContractID == id), cancellationToken);
            returninfo.AnnexInfo = _mapper.Map<List<ContractAnnexResponse>>(annexinfo);
            if (annexinfo.Count > 0)
            {
                var modify = await Store.GetModifyAsync(a => a.Where(b => b.ID == annexinfo.ElementAt(0).CurrentModify), cancellationToken);
                foreach (var item in returninfo.AnnexInfo)
                {
                    item.ExamineStatus = (int)modify.ExamineStatus;
                }
            }

            var complementinfo = await Store.GetListComplementAsync(a => a.Where(b => b.ContractID == id));
            returninfo.ComplementInfo = _mapper.Map<List<ContractComplementResponse>>(complementinfo);
            if (complementinfo.Count > 0)
            {
                var modify = await Store.GetModifyAsync(a => a.Where(b => b.ID == complementinfo.ElementAt(0).CurrentModify), cancellationToken);
                if (modify != null)
                {
                    foreach (var item in returninfo.ComplementInfo)
                    {
                        item.ExamineStatus = (int)modify.ExamineStatus;
                    }
                }

            }

            var modifyinfo = await Store.GetListModifyAsync(a => a.Where(b => b.ContractID == id));
            foreach(var item in modifyinfo)
            { 

                var userInfo = await _curUserManager.GetUserAsync(item.ModifyPepole);
                item.ModifyPepole = userInfo.UserName;
             }
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

        public virtual async Task<ModifyInfo> OperModifyInfoAsync(string modifyid, string contractid, ExamineStatusEnum ext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (modifyid == null)
            {
                throw new ArgumentNullException(nameof(modifyid));
            }

            var modify = await Store.GetModifyAsync(a => a.Where(b => b.ID == modifyid));
            
            await Store.UpdateExamineStatus(modifyid, ext, cancellationToken);

            return (modify);
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
        public virtual async Task<List<ContractInfoResponse>> GetFollowHistory(UserInfo userinfo, string contractid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contractinfo = await Store.GetAsync(a => a.Where(b => b.ID == contractid), cancellationToken);
            if(contractinfo.IsFollow == false)
            {
                return null;
            }
            List<ContractInfoResponse> infos = new List<ContractInfoResponse>();
            infos.Insert(0, _mapper.Map<ContractInfoResponse>(contractinfo));
            while (contractinfo.IsFollow == true && !string.IsNullOrEmpty(contractinfo.FollowId))
            {
                string id = contractinfo.FollowId;
                contractinfo = await Store.GetAsync(a => a.Where(b => b.ID == id), cancellationToken);
                infos.Insert(0, _mapper.Map<ContractInfoResponse>(contractinfo));
            }
            return infos;
        }
    }

}
