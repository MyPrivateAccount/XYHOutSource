using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class FileScopeManager
    {
        public FileScopeManager(IShopsFileScopeStore shopsFileScopeStore,
            IBuildingFileScopeStore buildingFileScopeStore,
            IUpdateRecordFileScopeStore updateRecordFileScopeStore,
            IBuildingNoticeFileScopeStore buildingNoticeFileScopeStore,
            IBuildingsStore buildingsStore,
            IShopsStore shopsStore,
            IFileInfoStore fileInfoStore,
            IMapper mapper)
        {
            _shopsFileScopeStore = shopsFileScopeStore ?? throw new ArgumentNullException(nameof(shopsFileScopeStore));
            _buildingFileScopeStore = buildingFileScopeStore ?? throw new ArgumentNullException(nameof(buildingFileScopeStore));
            _updateRecordFileScopeStore = updateRecordFileScopeStore ?? throw new ArgumentNullException(nameof(updateRecordFileScopeStore));
            _buildingsStore = buildingsStore ?? throw new ArgumentNullException(nameof(buildingsStore));
            _shopsStore = shopsStore ?? throw new ArgumentNullException(nameof(shopsStore));
            _fileInfoStore = fileInfoStore ?? throw new ArgumentNullException(nameof(fileInfoStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _buildingNoticeFileScopeStore = buildingNoticeFileScopeStore ?? throw new ArgumentNullException(nameof(buildingNoticeFileScopeStore));
        }

        protected IShopsFileScopeStore _shopsFileScopeStore { get; }
        protected IBuildingFileScopeStore _buildingFileScopeStore { get; }
        protected IBuildingsStore _buildingsStore { get; }
        protected IShopsStore _shopsStore { get; }
        protected IFileInfoStore _fileInfoStore { get; }
        protected IMapper _mapper { get; }
        protected IUpdateRecordFileScopeStore _updateRecordFileScopeStore { get; }
        protected IBuildingNoticeFileScopeStore _buildingNoticeFileScopeStore { get; }

        public virtual async Task CreateAsync(UserInfo user, string source, string buildingId, FileInfoRequest fileInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(fileInfoRequest));
            }

            if (source == "shops")
            {
                var shopsfile = _mapper.Map<ShopsFileScope>(fileInfoRequest);
                shopsfile.IsDeleted = false;
                shopsfile.ShopsId = fileInfoRequest.SourceId;

                await _shopsFileScopeStore.SaveAsync(_mapper.Map<SimpleUser>(user), buildingId, new List<ShopsFileScope>() { shopsfile }, cancellationToken);
            }
            else if (source == "building")
            {
                var buildingfile = _mapper.Map<BuildingFileScope>(fileInfoRequest);
                buildingfile.IsDeleted = false;
                buildingfile.BuildingId = fileInfoRequest.SourceId;

                await _buildingFileScopeStore.SaveAsync(_mapper.Map<SimpleUser>(user), buildingId, new List<BuildingFileScope>() { buildingfile }, cancellationToken);
            }
            else if (source == "updaterecord")
            {
                var updateRecordfile = _mapper.Map<UpdateRecordFileScope>(fileInfoRequest);
                updateRecordfile.IsDeleted = false;
                updateRecordfile.UpdateRecordId = fileInfoRequest.SourceId;
                await _updateRecordFileScopeStore.SaveAsync(_mapper.Map<SimpleUser>(user), buildingId, new List<UpdateRecordFileScope>() { updateRecordfile }, cancellationToken);
            }
            else if (source == "buildingnotice")
            {
                var buildingNoticeFile = _mapper.Map<BuildingNoticeFileScope>(fileInfoRequest);
                buildingNoticeFile.IsDeleted = false;
                buildingNoticeFile.BuildingNoticeId = fileInfoRequest.SourceId;
                await _buildingNoticeFileScopeStore.SaveAsync(_mapper.Map<SimpleUser>(user), buildingId, new List<BuildingNoticeFileScope>() { buildingNoticeFile }, cancellationToken);
            }
        }


        public virtual async Task<List<FileInfoResponse>> FindByShopsIdAsync(string userId, string shopsId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var shopsFiles = await _shopsFileScopeStore.ListAsync(a => a.Where(b => b.ShopsId == shopsId && !b.IsDeleted));
            if (shopsFiles?.Count == 0)
            {
                return new List<FileInfoResponse>();
            }
            List<FileInfoResponse> list = new List<FileInfoResponse>();
            foreach (var item in shopsFiles)
            {
                list.AddRange(_mapper.Map<List<FileInfoResponse>>(await _fileInfoStore.ListAsync(a => a.Where(b => b.FileGuid == item.FileGuid))));
            }
            return list;
        }

        public virtual async Task<List<FileInfoResponse>> FindByBuildingIdAsync(string userId, string shopsId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildingFiles = await _buildingFileScopeStore.ListAsync(a => a.Where(b => b.BuildingId == shopsId && !b.IsDeleted));
            if (buildingFiles?.Count == 0)
            {
                return new List<FileInfoResponse>();
            }
            List<FileInfoResponse> list = new List<FileInfoResponse>();
            foreach (var item in buildingFiles)
            {
                list.AddRange(_mapper.Map<List<FileInfoResponse>>(await _fileInfoStore.ListAsync(a => a.Where(b => b.FileGuid == item.FileGuid))));
            }
            return list;
        }


        public virtual async Task<ShopsFileScope> FindByShopsFileGuidAsync(string userId, string shopsFileGuid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _shopsFileScopeStore.GetAsync(a => a.Where(b => b.FileGuid == shopsFileGuid && !b.IsDeleted));
        }

        public virtual async Task<BuildingFileScope> FindByBuildingFileGuidAsync(string userId, string buildingFileGuid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _buildingFileScopeStore.GetAsync(a => a.Where(b => b.FileGuid == buildingFileGuid && !b.IsDeleted));
        }



        public virtual async Task DeleteShopsFileListAsync(string userId, string shopsId, List<string> fileGuids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var shopsFiles = await _shopsFileScopeStore.ListAsync(a => a.Where(b => b.ShopsId == shopsId && fileGuids.Contains(b.FileGuid) && !b.IsDeleted));
            if (shopsFiles?.Count == 0)
            {
                return;
            }
            for (int i = 0; i < shopsFiles.Count; i++)
            {
                shopsFiles[i].DeleteTime = DateTime.Now;
                shopsFiles[i].DeleteUser = userId;
                shopsFiles[i].IsDeleted = true;
            }
            await _shopsFileScopeStore.UpdateListAsync(shopsFiles, cancellationToken);
        }


        public virtual async Task DeleteBuildingFileListAsync(string userId, string buildingId, List<string> fileGuids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buildingFiles = await _buildingFileScopeStore.ListAsync(a => a.Where(b => b.BuildingId == buildingId && fileGuids.Contains(b.FileGuid) && !b.IsDeleted));
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
            await _buildingFileScopeStore.UpdateListAsync(buildingFiles, cancellationToken);


        }

    }
}
