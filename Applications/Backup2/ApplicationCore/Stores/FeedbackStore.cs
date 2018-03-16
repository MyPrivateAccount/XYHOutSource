using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public class FeedbackStore : IFeedbackStore
    {
        public FeedbackStore(CoreDbContext coreDbContext)
        {
            Context = coreDbContext;
            Feedbacks = Context.Feedbacks;
        }

        protected CoreDbContext Context { get; }

        public IQueryable<Feedback> Feedbacks { get; set; }


        public async Task<Feedback> CreateAsync(Feedback feedback, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            Context.Add(feedback);
            await Context.SaveChangesAsync(cancellationToken);
            return feedback;
        }


        public async Task DeleteAsync(Feedback feedback, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            Context.Remove(feedback);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<Feedback> feedbackList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (feedbackList == null)
            {
                throw new ArgumentNullException(nameof(feedbackList));
            }
            Context.RemoveRange(feedbackList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<Feedback>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Feedbacks).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Feedback>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.Feedbacks).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(Feedback feedback, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            Context.Attach(feedback);
            Context.Update(feedback);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        public async Task UpdateListAsync(List<Feedback> feedbackList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (feedbackList == null)
            {
                throw new ArgumentNullException(nameof(feedbackList));
            }
            Context.AttachRange(feedbackList);
            Context.UpdateRange(feedbackList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

    }
}
