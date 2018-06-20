using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public class Transaction<TContext> : ITransaction<TContext> where TContext : DbContext
    {
        private readonly TContext dbContext;
        public Transaction(TContext _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }
    }
}
