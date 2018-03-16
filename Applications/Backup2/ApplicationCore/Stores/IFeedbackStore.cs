using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore.Stores
{
    public interface IFeedbackStore
    {

        IQueryable<Feedback> Feedbacks { get; set; }


        Task<Feedback> CreateAsync(Feedback feedback, CancellationToken cancellationToken = default(CancellationToken));



        Task DeleteAsync(Feedback feedback, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<Feedback> feedbackList, CancellationToken cancellationToken = default(CancellationToken));


        Task<TResult> GetAsync<TResult>(Func<IQueryable<Feedback>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));



        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<Feedback>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));


        Task UpdateAsync(Feedback feedback, CancellationToken cancellationToken = default(CancellationToken));


        Task UpdateListAsync(List<Feedback> feedbackList, CancellationToken cancellationToken = default(CancellationToken));





    }
}
