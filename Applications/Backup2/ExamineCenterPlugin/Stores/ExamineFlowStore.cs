using ApplicationCore.Models;
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
    public class ExamineFlowStore : IExamineFlowStore
    {
        public ExamineFlowStore(ExamineDbContext examineDbContext)
        {
            Context = examineDbContext;
            ExamineFlows = Context.ExamineFlows;
            ExamineRecords = Context.ExamineRecords;
        }
        protected ExamineDbContext Context { get; }
        public IQueryable<ExamineFlow> ExamineFlows { get; set; }
        public IQueryable<ExamineRecord> ExamineRecords { get; set; }

        public async Task<ExamineFlow> CreateExamineFlowAsync(ExamineFlow examineFlow, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineFlow == null)
            {
                throw new ArgumentNullException(nameof(examineFlow));
            }
            try
            {
                var examineRecord = new ExamineRecord();
                examineRecord.Id = Guid.NewGuid().ToString();
                examineRecord.RecordTime = DateTime.Now;
                examineRecord.RecordStstus = RecordStatus.Examined;
                examineRecord.ExamineUserId = examineFlow.SubmitUserId;
                examineRecord.FlowId = examineFlow.Id;
                examineRecord.RecordType = RecordTypes.Submit;
                examineRecord.IsDeleted = false;
                examineRecord.IsCurrent = true;
                examineRecord.ExamineContents = "提交审核";
                var flowlist = await Context.ExamineFlows.AsNoTracking().Where(a => a.ContentId == examineFlow.ContentId && a.Action == examineFlow.Action && a.ExamineStatus != ExamineStatus.Examined).ToListAsync(cancellationToken);
                var examiningflow = flowlist.SingleOrDefault(a => a.ExamineStatus == ExamineStatus.Examining);
                if (examiningflow != null)
                {
                    return examineFlow;
                }
                var rejectflow = flowlist.SingleOrDefault(a => a.ExamineStatus == ExamineStatus.Reject);
                if (rejectflow != null)
                {
                    var records = Context.ExamineRecords.AsNoTracking().Where(a => a.FlowId == rejectflow.Id && a.IsCurrent).ToList();
                    for (int i = 0; i < records.Count; i++)
                    {
                        records[0].IsCurrent = false;
                    }
                    rejectflow.CallbackUrl = examineFlow.CallbackUrl;
                    rejectflow.ExamineStatus = examineFlow.ExamineStatus;
                    rejectflow.TaskGuid = examineFlow.TaskGuid;
                    rejectflow.SubmitOrganizationId = examineFlow.SubmitOrganizationId;
                    rejectflow.SubmitTime = examineFlow.SubmitTime;
                    rejectflow.SubmitDefineId = examineFlow.SubmitDefineId;
                    rejectflow.TaskName = examineFlow.TaskName;
                    rejectflow.CurrentStepId = "";
                    rejectflow.Ext1 = examineFlow.Ext1;
                    rejectflow.Ext2 = examineFlow.Ext2;
                    rejectflow.Ext3 = examineFlow.Ext3;
                    rejectflow.Ext4 = examineFlow.Ext4;
                    rejectflow.Ext5 = examineFlow.Ext5;
                    rejectflow.Ext6 = examineFlow.Ext6;
                    rejectflow.Ext7 = examineFlow.Ext7;
                    rejectflow.Ext8 = examineFlow.Ext8;
                    examineRecord.FlowId = rejectflow.Id;
                    Context.UpdateRange(records);
                    Context.Update(rejectflow);
                    Context.Add(examineRecord);
                    await Context.SaveChangesAsync(cancellationToken);
                    return rejectflow;
                }
                Context.Add(examineFlow);
                Context.Add(examineRecord);
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return examineFlow;
        }

        public async Task<ExamineRecord> CreateExamineRecordAsync(ExamineRecord examineRecord, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineRecord == null)
            {
                throw new ArgumentNullException(nameof(examineRecord));
            }
            try
            {
                Context.Add(examineRecord);
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return examineRecord;
        }

        /// <summary>
        /// 审核步骤回调
        /// </summary>
        /// <param name="examineUserId"></param>
        /// <param name="examineFlow"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StepCallBackUpdateExamineFlowAsync(string examineUserId, ExamineFlow examineFlow, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineFlow == null)
            {
                throw new ArgumentNullException(nameof(examineFlow));
            }
            var record = await Context.ExamineRecords.AsNoTracking().Where(a => a.FlowId == examineFlow.Id).OrderByDescending(a => a.Sort)?.FirstOrDefaultAsync();
            //如果最新的记录为待审核状态，则不再添加审核记录
            if (record?.RecordStstus != RecordStatus.Waiting)
            {
                var examineRecord = new ExamineRecord();
                examineRecord.Id = Guid.NewGuid().ToString();
                examineRecord.RecordTime = DateTime.Now;
                examineRecord.RecordType = RecordTypes.Examine;
                examineRecord.RecordStstus = RecordStatus.Waiting;
                examineRecord.ExamineUserId = examineUserId;
                examineRecord.FlowId = examineFlow.Id;
                examineRecord.IsCurrent = true;
                examineRecord.IsDeleted = false;
                examineRecord.ExamineContents = "";
                Context.Attach(examineFlow);
                Context.Update(examineFlow);
                Context.Add(examineRecord);
                try
                {
                    await Context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
        }



        public async Task FlowCallBackUpdateExamineFlowAsync(ExamineFlow examineFlow, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineFlow == null)
            {
                throw new ArgumentNullException(nameof(examineFlow));
            }
            var examineRecord = new ExamineRecord();
            examineRecord.Id = Guid.NewGuid().ToString();
            examineRecord.RecordTime = DateTime.Now;
            examineRecord.RecordStstus = RecordStatus.Complete;
            examineRecord.RecordType = RecordTypes.End;
            examineRecord.ExamineUserId = "";
            examineRecord.IsCurrent = true;
            examineRecord.IsDeleted = false;
            examineRecord.FlowId = examineFlow.Id;
            examineRecord.ExamineContents = "审核完成";
            try
            {
                Context.Attach(examineFlow);
                Context.Update(examineFlow);
                Context.Add(examineRecord);
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task UpdateExamineRecordAsync(ExamineRecord examineRecord, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineRecord == null)
            {
                throw new ArgumentNullException(nameof(examineRecord));
            }
            try
            {
                Context.Attach(examineRecord);
                Context.Update(examineRecord);
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateExamineRejectAsync(ExamineRecord examineRecord, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (examineRecord == null)
            {
                throw new ArgumentNullException(nameof(examineRecord));
            }
            try
            {
                var examineFlow = Context.ExamineFlows.AsNoTracking().FirstOrDefault(a => a.Id == examineRecord.FlowId);
                if (examineFlow == null)
                {
                    throw new Exception("未找到审核流程");
                }
                examineFlow.ExamineStatus = ExamineStatus.Reject;

                Context.Update(examineFlow);
                Context.Attach(examineRecord);
                Context.Update(examineRecord);
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public IQueryable<ExamineRecord> GetRecordQuery()
        {
            var query = from r in Context.ExamineRecords.AsNoTracking()
                        join f in Context.ExamineFlows.AsNoTracking() on r.FlowId equals f.Id into flowtemp
                        from flow in flowtemp.DefaultIfEmpty()

                            //user
                        join eu in Context.Users.AsNoTracking() on r.ExamineUserId equals eu.Id into eutemp
                        from examineuser in eutemp.DefaultIfEmpty()
                        join su in Context.Users.AsNoTracking() on flow.SubmitUserId equals su.Id into sutemp
                        from submituser in sutemp.DefaultIfEmpty()
                        join du in Context.Users.AsNoTracking() on flow.DeleteUserId equals du.Id into dutemp
                        from duser in dutemp.DefaultIfEmpty()

                            //组织
                        join o1 in Context.Organizations.AsNoTracking() on flow.SubmitOrganizationId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()

                        orderby r.Sort descending
                        select new ExamineRecord()
                        {
                            ExamineContents = r.ExamineContents,
                            DeleteTime = r.DeleteTime,
                            ExamineTime = r.ExamineTime,
                            ExamineUserId = r.ExamineUserId,
                            ExamineUserName = examineuser.TrueName,
                            FlowId = r.FlowId,
                            Sort = r.Sort,
                            Id = r.Id,
                            IsCurrent = r.IsCurrent,
                            RecordStstus = r.RecordStstus,
                            RecordTime = r.RecordTime,
                            IsDeleted = r.IsDeleted,
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
                                CallbackUrl = flow.CallbackUrl,
                                DeleteUserId = flow.DeleteUserId,
                                SubmitDefineId = flow.SubmitDefineId,
                                DeleteUserName = duser.TrueName,
                                SubmitOrganizationName = o.OrganizationName,
                                SubmitUserName = submituser.TrueName,
                                Content = flow.Content,
                                Ext1 = flow.Ext1,
                                Ext2 = flow.Ext2,
                                Ext3 = flow.Ext3,
                                Ext4 = flow.Ext4,
                                Ext5 = flow.Ext5,
                                Ext6 = flow.Ext6,
                                Ext7 = flow.Ext7,
                                Ext8 = flow.Ext8
                            }

                        };
            return query;
        }

        public IQueryable<ExamineFlow> GetFlowQuery()
        {
            var query = from f in Context.ExamineFlows.AsNoTracking()
                            //user
                        join su in Context.Users.AsNoTracking() on f.SubmitUserId equals su.Id into sutemp
                        from submituser in sutemp.DefaultIfEmpty()
                        join du in Context.Users.AsNoTracking() on f.DeleteUserId equals du.Id into dutemp
                        from duser in dutemp.DefaultIfEmpty()
                            //组织
                        join o1 in Context.Organizations.AsNoTracking() on f.SubmitOrganizationId equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()

                        select new ExamineFlow()
                        {
                            Action = f.Action,
                            ContentId = f.ContentId,
                            ContentName = f.ContentName,
                            ContentType = f.ContentType,
                            CurrentStepId = f.CurrentStepId,
                            DeleteTime = f.DeleteTime,
                            Desc = f.Desc,
                            ExamineStatus = f.ExamineStatus,
                            Id = f.Id,
                            IsDeleted = f.IsDeleted,
                            SubmitOrganizationId = f.SubmitOrganizationId,
                            SubmitTime = f.SubmitTime,
                            SubmitUserId = f.SubmitUserId,
                            TaskGuid = f.TaskGuid,
                            TaskName = f.TaskName,
                            CallbackUrl = f.CallbackUrl,
                            SubmitUserName = submituser.TrueName,
                            SubmitOrganizationName = o.OrganizationName,
                            DeleteUserId = f.DeleteUserId,
                            SubmitDefineId = f.SubmitDefineId,
                            DeleteUserName = duser.TrueName,
                            Content = f.Content,
                            Ext1 = f.Ext1,
                            Ext2 = f.Ext2,
                            Ext3 = f.Ext3,
                            Ext4 = f.Ext4,
                            Ext5 = f.Ext5,
                            Ext6 = f.Ext6,
                            Ext7 = f.Ext7,
                            Ext8 = f.Ext8,
                            ExamineRecords = from record in Context.ExamineRecords.AsNoTracking()
                                             join u1 in Context.Users.AsNoTracking() on record.ExamineUserId equals u1.Id into u2
                                             from u in u2.DefaultIfEmpty()
                                             where record.FlowId == f.Id
                                             orderby record.Sort descending
                                             select new ExamineRecord
                                             {
                                                 ExamineContents = record.ExamineContents,
                                                 DeleteTime = record.DeleteTime,
                                                 ExamineTime = record.ExamineTime,
                                                 ExamineUserId = record.ExamineUserId,
                                                 ExamineUserName = u.TrueName,
                                                 IsCurrent = record.IsCurrent,
                                                 FlowId = record.FlowId,
                                                 Id = record.Id,
                                                 Sort = record.Sort,
                                                 RecordStstus = record.RecordStstus,
                                                 RecordTime = record.RecordTime,
                                                 IsDeleted = record.IsDeleted,
                                             }
                        };
            return query;
        }



        public Task<TResult> ExamineFlowGetAsync<TResult>(Func<IQueryable<ExamineFlow>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            //return query.Invoke(Context.ExamineFlows.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
            return query.Invoke(Context.ExamineFlows.AsNoTracking().OrderByDescending(x => x.SubmitTime)).FirstOrDefaultAsync(cancellationToken);
        }
        public Task<List<TResult>> ExamineFlowListAsync<TResult>(Func<IQueryable<ExamineFlow>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ExamineFlows.AsNoTracking()).ToListAsync(cancellationToken);
        }


        public Task<TResult> ExamineRecordGetAsync<TResult>(Func<IQueryable<ExamineRecord>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ExamineRecords.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }
        public Task<List<TResult>> ExamineRecordListAsync<TResult>(Func<IQueryable<ExamineRecord>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.ExamineRecords.AsNoTracking()).ToListAsync(cancellationToken);
        }


    }
}
