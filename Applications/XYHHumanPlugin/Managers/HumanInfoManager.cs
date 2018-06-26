using ApplicationCore;
using ApplicationCore.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Stores;

namespace XYHHumanPlugin.Managers
{
    public class HumanInfoManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="humanInfoStore"></param>
        /// <param name="mapper"></param>
        /// <param name="restClient"></param>
        public HumanInfoManager(IHumanInfoStore humanInfoStore, IMapper mapper, RestClient restClient)
        {
            Store = humanInfoStore;
            _mapper = mapper;
            _restClient = restClient;
        }

        protected IHumanInfoStore Store { get; }
        protected IMapper _mapper { get; }
        protected RestClient _restClient { get; }

        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoManager");


        public async Task<ResponseMessage<HumanInfoResponse>> SaveHumanInfo(UserInfo user, HumanInfoRequest humanInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoResponse> response = new ResponseMessage<HumanInfoResponse>();
            if (user == null || humanInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(UserInfo) + nameof(HumanInfoRequest));
            }

            response.Extension = _mapper.Map<HumanInfoResponse>(await Store.SaveAsync(user, _mapper.Map<HumanInfo>(humanInfoRequest), cancellationToken));
            return response;
        }



    }
}
