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
        public virtual async Task CreateListAsync(string userId, List<FileInfoRequest> fileInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfoRequest == null)
            {
                return;
               // throw new ArgumentNullException(nameof(fileInfoCallbackRequestList));
            }

            
            var fileInfos = new List<FileInfo>();
            foreach (var item in fileInfoRequest)
            {
                var fileInfo = new FileInfo();
                fileInfo.IsDeleted = false;
                fileInfo.CreateTime = DateTime.Now;
                fileInfo.CreateUser = userId;
                fileInfo.Driver = item.WXPath.Substring(0, 1);
                fileInfo.Uri = CovertPath(item.WXPath);
                fileInfo.Type = "ICON";
                fileInfo.Name = item.Name;
                fileInfo.FileGuid = item.FileGuid;
                fileInfo.Group = item.Group;
                
                
                fileInfos.Add(fileInfo);
            }
            await Store.CreateListAsync(fileInfos, cancellationToken);
        }
        public string CovertPath(string path)
        {
            path = path.Replace('\\', '/');
            int index1 = path.IndexOf("Images/");
            int nlength = ("Images/").Length;
            string newPath = path.Substring(index1 + nlength);

            return newPath;
        }
    }
}
