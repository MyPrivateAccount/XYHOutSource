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

namespace XYHContractPlugin.Managers
{
    public class FileScopeManager
    {
        public FileScopeManager(IContractInfoStore contractStore, IMapper mapper)
        {
            Store = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IContractInfoStore Store { get; }
        protected IMapper _mapper { get; }

        
    }
}