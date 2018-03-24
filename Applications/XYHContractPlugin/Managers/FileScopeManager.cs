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
using XYHContractPlugin.Dto.Request;

namespace XYHContractPlugin.Managers
{
    /*   public class FileScopeManager
       {
           public FileScopeManager(IContractInfoStore contractStore, IMapper mapper)
           {
               Store = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
               _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
           }

           protected IContractInfoStore Store { get; }
           protected IMapper _mapper { get; }


       }*/
    public class FileScopeManager
    {


        public FileScopeManager(
        IContractFileScopeStore contractFileScopeStore,
        IFileInfoStore fileInfoStore,
        IMapper mapper)
        {
            _contractFileScopeStore = contractFileScopeStore ?? throw new ArgumentNullException(nameof(contractFileScopeStore));      
            _fileInfoStore = fileInfoStore ?? throw new ArgumentNullException(nameof(fileInfoStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
           
        }


        protected IContractFileScopeStore _contractFileScopeStore { get; }
  
        protected IFileInfoStore _fileInfoStore { get; }
        protected IMapper _mapper { get; }
    

        public virtual async Task CreateAsync(UserInfo user, string source, string contractId, FileInfoRequest fileInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(fileInfoRequest));
            }

                var contractfile = _mapper.Map<AnnexInfo>(fileInfoRequest);
                contractfile.IsDeleted = false;
                contractfile.ContractID = contractId;

                await _contractFileScopeStore.SaveAsync(_mapper.Map<SimpleUser>(user), contractId, new List<AnnexInfo>() { contractfile }, cancellationToken);

        }
        public virtual async Task DeleteContractFileListAsync(string userId, string contractId, List<string> fileGuids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contractFiles = await _contractFileScopeStore.ListAsync(a => a.Where(b => b.ContractID == contractId && fileGuids.Contains(b.FileGuid) && !b.IsDeleted));
            if (contractFiles?.Count == 0)
            {
                return;
            }
            for (int i = 0; i < contractFiles.Count; i++)
            {
                contractFiles[i].DeleteTime = DateTime.Now;
                contractFiles[i].DeleteUser = userId;
                contractFiles[i].IsDeleted = true;
            }
            await _contractFileScopeStore.UpdateListAsync(contractFiles, cancellationToken);


        }

        
        public virtual async Task<List<FileInfo>> FindByContractIdAsync(string userId, string contractId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contractFiles = await _contractFileScopeStore.ListAsync(a => a.Where(b => b.ContractID == contractId && !b.IsDeleted));
            if (contractFiles?.Count == 0)
            {
                return new List<FileInfo>();
            }
            List<FileInfo> list = new List<FileInfo>();
            foreach (var item in contractFiles)
            {
                list.AddRange(_mapper.Map<List<FileInfo>>(await _fileInfoStore.ListAsync(a => a.Where(b => b.FileGuid == item.FileGuid))));
            }
            return list;
        }



/*
                public virtual async Task<BuildingFileScope> FindByBuildingFileGuidAsync(string userId, string buildingFileGuid, CancellationToken cancellationToken = default(CancellationToken))
                {
                    return await _buildingFileScopeStore.GetAsync(a => a.Where(b => b.FileGuid == buildingFileGuid && !b.IsDeleted));
                }




        */


    }

}