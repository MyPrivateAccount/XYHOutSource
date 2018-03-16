using ExamineCenterPlugin.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExamineCenterPlugin.Stores
{
    public class ExamineNoticeStore : IExamineNoticeStore
    {
        public ExamineNoticeStore(ExamineDbContext examineDbContext)
        {
            Context = examineDbContext;
            ExamineNotices = Context.ExamineNotices;
        }
        protected ExamineDbContext Context { get; }
        public IQueryable<ExamineNotice> ExamineNotices { get; set; }


        public async Task NoticeCallbackAsync(List<string> userIds, ExamineFlow examineFlow, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineFlow == null)
            {
                throw new ArgumentNullException(nameof(examineFlow));
            }
            var examineRecord = new ExamineRecord();
            examineRecord.Id = Guid.NewGuid().ToString();
            examineRecord.RecordTime = DateTime.Now;
            examineRecord.RecordStstus = RecordStatus.Examined;
            examineRecord.ExamineUserId = "";
            examineRecord.FlowId = examineFlow.Id;
            examineRecord.IsDeleted = false;
            examineRecord.ExamineContents = "";

            List<ExamineNotice> list = new List<ExamineNotice>();

            foreach (var item in userIds)
            {
                list.Add(new ExamineNotice
                {
                    FlowId = examineFlow.Id,
                    Id = Guid.NewGuid().ToString(),
                    NoticeStatus = NoticeStatus.Noticed,
                    NoticeTime = DateTime.Now,
                    IsDeleted = false,
                    NoticeUserId = item,
                    RecordId = examineRecord.Id
                });
            }
            try
            {
                Context.Attach(examineFlow);
                Context.Update(examineFlow);
                Context.Add(examineRecord);
                Context.AddRange(list);
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }



        }

        public async Task CreateExamineNotice(List<ExamineNotice> examineNotice, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineNotice == null)
            {
                throw new ArgumentNullException(nameof(examineNotice));
            }
            Context.AddRange(examineNotice);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateExamineNotice(List<ExamineNotice> examineNotice, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineNotice == null)
            {
                throw new ArgumentNullException(nameof(examineNotice));
            }
            Context.AttachRange(examineNotice);
            Context.AddRange(examineNotice);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public IQueryable<ExamineNotice> GetQuery()
        {
            var query = from n in Context.ExamineNotices.AsNoTracking()
                        join r in Context.ExamineRecords.AsNoTracking() on n.RecordId equals r.Id into rtemp
                        from record in rtemp.DefaultIfEmpty()
                        join f in Context.ExamineFlows.AsNoTracking() on n.FlowId equals f.Id into ftemp
                        from flow in ftemp.DefaultIfEmpty()
                            //user
                        join nu in Context.Users.AsNoTracking() on n.NoticeUserId equals nu.Id into nutemp
                        from nuser in nutemp.DefaultIfEmpty()
                        join eu in Context.Users.AsNoTracking() on record.ExamineUserId equals eu.Id into eutemp
                        from euser in eutemp.DefaultIfEmpty()
                        join su in Context.Users.AsNoTracking() on flow.SubmitUserId equals su.Id into sutemp
                        from suser in sutemp.DefaultIfEmpty()
                        join deu in Context.Users.AsNoTracking() on record.DeleteUserId equals deu.Id into deutemp
                        from deuser in deutemp.DefaultIfEmpty()
                        join dnu in Context.Users.AsNoTracking() on n.DeleteUserId equals dnu.Id into dnutemp
                        from dnuser in dnutemp.DefaultIfEmpty()
                        join dfu in Context.Users.AsNoTracking() on flow.DeleteUserId equals dfu.Id into dfutemp
                        from dfuser in dfutemp.DefaultIfEmpty()
                            //组织
                        join o1 in Context.Organizations.AsNoTracking() on flow.SubmitOrganizationId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()

                        select new ExamineNotice()
                        {
                            NoticeStatus = n.NoticeStatus,
                            DeleteTime = n.DeleteTime,
                            NoticeTime = n.NoticeTime,
                            NoticeUserId = n.NoticeUserId,
                            NoticeUserName = nuser.TrueName,
                            RecordId = n.RecordId,
                            IsDeleted = n.IsDeleted,
                            ExamineRecord = new ExamineRecord
                            {
                                ExamineTime = record.ExamineTime,
                                ExamineUserId = record.ExamineUserId,
                                FlowId = record.FlowId,
                                Id = record.Id,
                                RecordStstus = record.RecordStstus,
                                RecordTime = record.RecordTime,
                                IsDeleted = record.IsDeleted,
                                DeleteUserId = record.DeleteUserId,
                                DeleteTime = record.DeleteTime,
                                Sort = record.Sort,
                                ExamineContents = record.ExamineContents,
                                DeleteUserName = deuser.TrueName,
                                ExamineUserName = euser.TrueName,
                            },
                            ExamineFlow = new ExamineFlow
                            {
                                Action = flow.Action,
                                ContentId = flow.ContentId,
                                ContentName = flow.ContentName,
                                ContentType = flow.ContentType,
                                CurrentStepId = flow.CurrentStepId,
                                DeleteTime = flow.DeleteTime,
                                Desc = flow.Desc,
                                ExamineStatus = flow.ExamineStatus,
                                Id = flow.Id,
                                IsDeleted = flow.IsDeleted,
                                SubmitOrganizationId = flow.SubmitOrganizationId,
                                SubmitTime = flow.SubmitTime,
                                SubmitUserId = flow.SubmitUserId,
                                TaskGuid = flow.TaskGuid,
                                TaskName = flow.TaskName,
                                DeleteUserId = flow.DeleteUserId,
                                DeleteUserName = dfuser.TrueName,
                                SubmitOrganizationName = o.OrganizationName,
                                SubmitUserName = suser.TrueName,
                            }
                        };
            return query;
        }

        public async Task SetReadingStatus(string noticeId)
        {
            var notice = await Context.ExamineNotices.FirstOrDefaultAsync(a => a.Id == noticeId);
            if (notice == null)
            {
                throw new Exception("通过noticeId未找到notice信息");
            }
            notice.NoticeStatus = NoticeStatus.Read;
            Context.Update(notice);
            try
            {
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }



        public Task<TResult> GetAsync<TResult>(Func<IQueryable<ExamineNotice>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ExamineNotices.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<ExamineNotice>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ExamineNotices.AsNoTracking()).ToListAsync(cancellationToken);
        }


    }
}
