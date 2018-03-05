using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class BuildingNoManager
    {
        #region 成员

        protected IBuildingNoStore _ibuildingnosStore { get; }

        protected IShopsStore _shopsStore { get; }

        protected IMapper _mapper { get; }

        #endregion

        public BuildingNoManager(
            IShopsStore shopsStore,
            IBuildingNoStore ibuildingnosStore,
            IMapper mapper)
        {
            _ibuildingnosStore = ibuildingnosStore ?? throw new ArgumentNullException(nameof(ibuildingnosStore));
            _shopsStore = shopsStore ?? throw new ArgumentNullException(nameof(shopsStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 新增楼栋批次信息
        /// </summary>
        /// <param name="user">创建者</param>
        /// <param name="buildingid">楼栋ID</param>
        /// <param name="buildingnoRequests">请求实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<ResponseMessage<List<BuildingNoCreateResponse>>> UpdateAsync(UserInfo user, string buildingid, List<BuildingNoCreateRequest> buildingnoRequests, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingnoRequests == null)
            {
                throw new ArgumentNullException(nameof(buildingnoRequests));
            }

            var request = buildingnoRequests;

            var respons = new ResponseMessage<List<BuildingNoCreateResponse>>();

            var oldbuildingnos = _ibuildingnosStore.BuildingNoAll().Where(b => b.BuildingId == buildingid && !b.IsDeleted);

            var deletebuildingnos = oldbuildingnos.ToList();

            if (deletebuildingnos.Count != 0)
            {

                var q = _shopsStore.GetSimpleQuery().Where(x => !x.IsDeleted && x.BuildingId == buildingid);

                var sor1 = from xczxc in oldbuildingnos select xczxc.Storied;

                var sor2 = from xczxc in q select xczxc.ShopBaseInfo.BuildingNo;
                
                var sss = buildingnoRequests.Select(a => a.Storied);

                var qqq = sor1.ToList().Except(sss).Intersect(sor2.ToList()).Distinct().ToList();


                if (qqq.Count != 0)
                {
                    respons.Message = "以下楼栋拥有商铺：";
                    foreach (var item in qqq)
                    {
                        respons.Message += item + ",";
                        deletebuildingnos.Remove(deletebuildingnos.FirstOrDefault(x => x.Storied == item));
                        request.Remove(request.FirstOrDefault(x => x.Storied == item));
                    }
                    respons.Message.Substring(0, respons.Message.Length - 1);
                }
                //删除以前的
                await _ibuildingnosStore.DeleteListAsync(deletebuildingnos, cancellationToken);
            }


            var buildingnos = new List<BuildingNo>();
            foreach (var item in request)
            {
                buildingnos.Add(new BuildingNo
                {
                    Id = Guid.NewGuid().ToString(),
                    BuildingId = buildingid,
                    Storied = item.Storied,
                    OpenDate = item.OpenDate,
                    DeliveryDate = item.DeliveryDate,
                    UserId = user.Id,
                    OrganizationId = user.OrganizationId,
                    CreateUser = user.Id,
                    CreateTime = DateTime.Now
                });
            }


            try
            {
                await _ibuildingnosStore.CreateListAsync(buildingnos, cancellationToken);
                respons.Code = ResponseCodeDefines.SuccessCode;
            }
            catch (Exception e)
            {
                respons.Code = ResponseCodeDefines.ServiceError;
                respons.Message = "服务器错误:" + e.ToString();
            }
            respons.Extension = _mapper.Map<List<BuildingNoCreateResponse>>(_ibuildingnosStore.BuildingNoAll().Where(b => b.BuildingId == buildingid && !b.IsDeleted).ToList());
            return respons;
        }

        /// <summary>
        /// 根据Id获取楼栋批次信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<BuildingNoCreateResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _ibuildingnosStore.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<BuildingNoCreateResponse>(response);
        }

        /// <summary>
        /// 根据楼盘id获取楼栋批次信息
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task<List<BuildingNoCreateResponse>> FindBuildingIdAsync(string buildingid, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildingnos = await _ibuildingnosStore.ListAsync(a => a.Where(b => b.BuildingId == buildingid && !b.IsDeleted), cancellationToken);

            return _mapper.Map<List<BuildingNoCreateResponse>>(buildingnos);
        }

        /// <summary>
        /// 修改单个楼栋批次信息
        /// </summary>
        /// <param name="user">请求者</param>
        /// <param name="buildingnoRequest">请求数据</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        //public virtual async Task UpdateAsync(UserInfo user, List<BuildingNoCreateRequest> buildingnoRequest, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    if (buildingnoRequest == null)
        //    {
        //        throw new ArgumentNullException(nameof(buildingnoRequest));
        //    }

        //    var buildingnos = _ibuildingnosStore.BuildingNoAll().Where(b => b.BuildingId == buildingnoRequest.BuildingId && !b.IsDeleted);

        //    if (buildingnos != null)
        //    {
        //        //删除以前的
        //        await _ibuildingnosStore.DeleteListAsync(buildingnos.ToList(), cancellationToken);
        //    }
        //    //新增
        //    var buildingno = _mapper.Map<BuildingNo>(buildingnoRequest);
        //    buildingno.CreateUser = user.Id;
        //    buildingno.CreateTime = DateTime.Now;
        //    buildingno.UserId = user.Id;
        //    buildingno.OrganizationId = user.OrganizationId;
        //    try
        //    {
        //        await _ibuildingnosStore.CreateAsync(buildingno, cancellationToken);
        //    }
        //    catch
        //    { }
        //}

        /// <summary>
        /// 删除楼栋批次信息
        /// </summary>
        /// <param name="user">删除人基本信息</param>
        /// <param name="id">删除楼栋批次Id</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await _ibuildingnosStore.DeleteAsync(_mapper.Map<SimpleUser>(user), new BuildingNo() { Id = id });
            }
            catch { }
        }

        /// <summary>
        /// 批量删除楼栋批次信息
        /// </summary>
        /// <param name="userId">删除人Id</param>
        /// <param name="ids">删除Id数组</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public virtual async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildingno = await _ibuildingnosStore.ListAsync(a => a.Where(b => ids.Contains(b.Id) && !b.IsDeleted), cancellationToken);
            if (buildingno == null || buildingno.Count == 0)
            {
                return;
            }
            for (int i = 0; i < buildingno.Count; i++)
            {
                buildingno[i].DeleteUser = userId;
                buildingno[i].DeleteTime = DateTime.Now;
                buildingno[i].IsDeleted = true;
            }
            try
            {
                await _ibuildingnosStore.UpdateListAsync(buildingno, cancellationToken);
            }
            catch { }
        }
    }
}
