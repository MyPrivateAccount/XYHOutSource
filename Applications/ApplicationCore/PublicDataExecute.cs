using ApplicationCore.Dto;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationCore
{
    public class PublicDataExecute
    {
        public static event Func<IServiceProvider, Func<UserInfo, DateTime, Task<List<GetMyWorkResponse>>>> MyWorkEvent;

        public static event Func<IServiceProvider, Func<UserInfo, List<DateTime>, Task<List<GetMyWorkTimesResponse>>>> MyWorkTimesEvents;

        public virtual async Task<ResponseMessage<List<GetMyWorkResponse>>> GetMyWork(UserInfo user, DateTime date, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<List<GetMyWorkResponse>> responseMessage = new ResponseMessage<List<GetMyWorkResponse>>();
            Delegate[] delegList = MyWorkEvent.GetInvocationList();
            List<GetMyWorkResponse> resultList = new List<GetMyWorkResponse>();
            using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //int hashCode = scope.ServiceProvider.GetHashCode();
                foreach (Func<IServiceProvider, Func<UserInfo, DateTime, Task<List<GetMyWorkResponse>>>> del in delegList)
                {
                    try
                    {

                        //    using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                        //    {
                        var response = await del(scope.ServiceProvider)(user, date);
                        if (response != null && response.Count != 0)
                            resultList.AddRange(response);
                        //   }


                    }
                    catch { }
                }
            }

            responseMessage.Extension = resultList;
            responseMessage.Extension.Sort((x, y) => -x.Sort.CompareTo(y.Sort));
            return responseMessage;
        }

        public virtual async Task<ResponseMessage<List<GetMyWorkTimesResponse>>> GetMyWorksTimes(UserInfo user, List<DateTime> dates, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<List<GetMyWorkTimesResponse>> responseMessage = new ResponseMessage<List<GetMyWorkTimesResponse>>();
            Delegate[] delegList = MyWorkTimesEvents.GetInvocationList();
            List<GetMyWorkTimesResponse> resultList = new List<GetMyWorkTimesResponse>();
            using (var scope = ApplicationContext.Current.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //还不晓得怎么合并
                foreach (Func<IServiceProvider, Func<UserInfo, List<DateTime>, Task<List<GetMyWorkTimesResponse>>>> del in delegList)
                {
                    try
                    {
                        var response = await del(scope.ServiceProvider)(user, dates);
                        if (response != null && response.Count != 0)
                            resultList.AddRange(response);
                    }
                    catch { }
                }
            }
            responseMessage.Extension = resultList.GroupBy(x => x.DateTime).Select(x => new GetMyWorkTimesResponse
            {
                DateTime = x.Key,
                IsMeassage = x.OrderByDescending(a => a.IsMeassage).FirstOrDefault().IsMeassage
            }).ToList();
            return responseMessage;
        }
    }
}
