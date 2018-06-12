using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore
{
    public interface ITransaction<TContext> where TContext : DbContext
    {
        Task<IDbContextTransaction> BeginTransaction();
    }

}
