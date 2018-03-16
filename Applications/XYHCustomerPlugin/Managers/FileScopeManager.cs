using ApplicationCore.Dto;
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
    public class FileScopeManager
    {
        public FileScopeManager(
               IDealFileScopeStore dealFileScopeStore,
               ICustomerDealStore icustomerDealStore,
               IFileInfoStore fileInfoStore,
               ICustomerFilescopeStore customerFilescopeStore,
               IMapper mapper)
        {
            _dealFileScopeStore = dealFileScopeStore ?? throw new ArgumentNullException(nameof(dealFileScopeStore));
            _icustomerDealStore = icustomerDealStore ?? throw new ArgumentNullException(nameof(icustomerDealStore));
            _fileInfoStore = fileInfoStore ?? throw new ArgumentNullException(nameof(fileInfoStore));
            _icustomerFilescopeStore = customerFilescopeStore ?? throw new ArgumentNullException(nameof(customerFilescopeStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IDealFileScopeStore _dealFileScopeStore { get; }
        protected ICustomerDealStore _icustomerDealStore { get; }
        protected IFileInfoStore _fileInfoStore { get; }
        protected ICustomerFilescopeStore _icustomerFilescopeStore { get; }
        protected IMapper _mapper { get; }

        public virtual async Task CreateAsync(UserInfo user, string source, string Id, DealFileInfoRequest fileInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(fileInfoRequest));
            }
            if (source == "customer")
            {
                var customerfile = _mapper.Map<CustomerFileScope>(fileInfoRequest);
                customerfile.FileType = DealFileType.Image;
                customerfile.CreateTime = DateTime.Now;
                customerfile.CreateUser = user.Id;
                customerfile.CustomerId = fileInfoRequest.SourceId;

                await _icustomerFilescopeStore.SaveAsync(_mapper.Map<SimpleUser>(user), Id, new List<CustomerFileScope>() { customerfile }, cancellationToken);
            }
            else 
            {
                var dealfile = _mapper.Map<DealFileScope>(fileInfoRequest);
                dealfile.IsDeleted = false;
                dealfile.DealId = fileInfoRequest.SourceId;
                dealfile.FileType = DealFileType.Image;
                dealfile.CreateTime = DateTime.Now;
                dealfile.CreateUser = user.Id;

                await _dealFileScopeStore.SaveAsync(_mapper.Map<SimpleUser>(user), Id, new List<DealFileScope>() { dealfile }, cancellationToken);
            }


            

        }
        public virtual async Task<List<DealFileInfoResponse>> FindByDealIdAsync(string userId, string shopsId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dealFiles = await _dealFileScopeStore.ListAsync(a => a.Where(b => b.DealId == shopsId && !b.IsDeleted));
            if (dealFiles?.Count == 0)
            {
                return new List<DealFileInfoResponse>();
            }
            List<DealFileInfoResponse> list = new List<DealFileInfoResponse>();
            foreach (var item in dealFiles)
            {
                list.AddRange(_mapper.Map<List<DealFileInfoResponse>>(await _fileInfoStore.ListAsync(a => a.Where(b => b.FileGuid == item.FileGuid))));
            }
            return list;
        }


        public virtual async Task<DealFileScope> FindByDealFileGuidAsync(string dealFileGuid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _dealFileScopeStore.GetAsync(a => a.Where(b => b.FileGuid == dealFileGuid && !b.IsDeleted));
        }
        public virtual async Task<CustomerFileScope> FindByCustomerFileGuidAsync(string customerFileGuid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _icustomerFilescopeStore.GetAsync(a => a.Where(b => b.FileGuid == customerFileGuid && !b.IsDeleted));
        }

        public virtual async Task DeleteDealScopeListAsync(string userId, string dealId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dealFiles = await _dealFileScopeStore.ListAsync(a => a.Where(b => b.DealId == dealId && !b.IsDeleted));
            if (dealFiles?.Count == 0)
            {
                return;
            }
            for (int i = 0; i < dealFiles.Count; i++)
            {
                dealFiles[i].DeleteTime = DateTime.Now;
                dealFiles[i].DeleteUser = userId;
                dealFiles[i].IsDeleted = true;
            }
            await _dealFileScopeStore.UpdateListAsync(dealFiles, cancellationToken);


        }


        public virtual async Task DeleteDealFileListAsync(string userId, string dealId, List<string> fileGuids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dealFiles = await _dealFileScopeStore.ListAsync(a => a.Where(b => b.DealId == dealId && fileGuids.Contains(b.FileGuid) && !b.IsDeleted));
            if (dealFiles?.Count == 0)
            {
                return;
            }
            for (int i = 0; i < dealFiles.Count; i++)
            {
                dealFiles[i].DeleteTime = DateTime.Now;
                dealFiles[i].DeleteUser = userId;
                dealFiles[i].IsDeleted = true;
            }
            await _dealFileScopeStore.UpdateListAsync(dealFiles, cancellationToken);


        }

        public virtual async Task DeleteCustomerFileListAsync(string userId, string customerId, List<string> fileGuids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildingFiles = await _icustomerFilescopeStore.ListAsync(a => a.Where(b => b.CustomerId == customerId && fileGuids.Contains(b.FileGuid) && !b.IsDeleted));
            if (buildingFiles?.Count == 0)
            {
                return;
            }
            for (int i = 0; i < buildingFiles.Count; i++)
            {
                buildingFiles[i].DeleteTime = DateTime.Now;
                buildingFiles[i].DeleteUser = userId;
                buildingFiles[i].IsDeleted = true;
            }
            await _icustomerFilescopeStore.UpdateListAsync(buildingFiles, cancellationToken);


        }
    }
}
