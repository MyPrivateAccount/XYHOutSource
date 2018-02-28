using ApplicationCore.Dto;
using ApplicationCore.Models;
using ApplicationCore.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Managers
{
    public class FeedbackManager
    {
        public FeedbackManager(IFeedbackStore feedbackStore)
        {
            Store = feedbackStore ?? throw new ArgumentNullException(nameof(feedbackStore));
        }
        protected IFeedbackStore Store { get; }

        public virtual async Task<Feedback> CreateAsync(Feedback feedback, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }
            return await Store.CreateAsync(feedback, cancellationToken);
        }


        public virtual Task<Feedback> FindByIdAsync(string Id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Store.GetAsync(a => a.Where(b => b.Id == Id), cancellationToken);
        }

        public virtual async Task<PagingResponseMessage<Feedback>> Search(FeedBackSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<Feedback> pagingResponse = new PagingResponseMessage<Feedback>();
            var q = Store.Feedbacks;
            if (condition?.UserIds?.Count > 0)
            {
                q = q.Where(a => condition.UserIds.Contains(a.UserId));
            }
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                q = q.Where(a => a.Content.Contains(condition.KeyWord));
            }
            pagingResponse.TotalCount = await q.CountAsync();
            var resulte = await q.OrderByDescending(a => a.CreateTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte;
            return pagingResponse;
        }


    }
}
