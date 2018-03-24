using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHContractPlugin.Dto.Request;
using XYHContractPlugin.Models;
using XYHContractPlugin.Stores;

namespace XYHContractPlugin.Managers
{
    public class FileInfoManager
    {
        public FileInfoManager(IFileInfoStore fileInfoStore, IMapper mapper)
        {
            Store = fileInfoStore ?? throw new ArgumentNullException(nameof(fileInfoStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IFileInfoStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task CreateListAsync(string userId, List<FileInfoCallbackRequest> fileInfoCallbackRequestList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoCallbackRequestList == null)
            {
                throw new ArgumentNullException(nameof(fileInfoCallbackRequestList));
            }
            var fileInfos = _mapper.Map<List<FileInfo>>(fileInfoCallbackRequestList);
            for (int i = 0; i < fileInfos.Count; i++)
            {
                fileInfos[i].IsDeleted = false;
                fileInfos[i].CreateTime = DateTime.Now;
                fileInfos[i].CreateUser = userId;
                fileInfos[i].Uri = fileInfoCallbackRequestList[i].FilePath;
            }
            await Store.CreateListAsync(fileInfos, cancellationToken);
        }

    }
}
