using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using XYHChargePlugin.Models;

namespace XYHChargePlugin.Stores
{
    public class BorrowingStore : IBorrowingStore
    {
        public BorrowingStore(ChargeDbContext hudb)
        {
            Context = hudb;
        }

        protected ChargeDbContext Context { get; }

        public IQueryable<ChargeInfo> SimpleQuery
        {
            get
            {
                var q = from c in Context.ChargeInfos.AsNoTracking()
                        join oe1 in Context.OrganizationExpansions.AsNoTracking() on new { c.ReimburseDepartment, Type = "Region" } equals new { ReimburseDepartment = oe1.SonId, oe1.Type } into oe2
                        from oe in oe2.DefaultIfEmpty()
                        join o1 in Context.Organizations.AsNoTracking() on c.ReimburseDepartment equals o1.Id into o2
                        from o in o2.DefaultIfEmpty()
                        join ru1 in Context.HumanInfo.AsNoTracking() on c.ReimburseUser equals ru1.ID into ru2
                        from ru in ru2.DefaultIfEmpty()
                        join cu1 in Context.Users.AsNoTracking() on c.CreateUser equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()
                        join bo1 in Context.Organizations.AsNoTracking() on c.BranchId equals bo1.Id into bo2
                        from bo in bo2.DefaultIfEmpty()
                        orderby c.CreateTime descending
                        select new ChargeInfo()
                        {
                            BillAmount = c.BillAmount,
                            BillCount = c.BillCount,
                            ChargeAmount = c.ChargeAmount,
                            ChargeNo = c.ChargeNo,
                            BillStatus = c.BillStatus,
                            BranchId = c.BranchId,
                            Department = c.Department,
                            ID = c.ID,
                            Payee = c.Payee,
                            CreateTime = c.CreateTime,
                            CreateUser = c.CreateUser,
                            PaymentAmount = c.PaymentAmount,
                            IsBackup = c.IsBackup,
                            IsPayment = c.IsPayment,
                            Memo = c.Memo,
                            ReimburseDepartment = c.ReimburseDepartment,
                            ReimburseUser = c.ReimburseUser,
                            Seq = c.Seq,
                            Status = c.Status,
                            Type = c.Type,
                            UpdateTime = c.UpdateTime,
                            UpdateUser = c.UpdateUser,
                            IsDeleted = c.IsDeleted,
                            DeleteTime = c.DeleteTime,
                            DeleteUser = c.DeleteUser,
                            Backuped = c.Backuped,
                            PaymentTime = c.PaymentTime,
                            ConfirmBillMessage = c.ConfirmBillMessage,
                            ConfirmMessage = c.ConfirmMessage,
                            SubmitTime = c.SubmitTime,
                            SubmitUser = c.SubmitUser,
                            ReimbursedAmount = c.ReimbursedAmount,
                            IsReimbursed = c.IsReimbursed,
                            LastReimbursedTime = c.LastReimbursedTime,
                            ExpectedPaymentDate = c.ExpectedPaymentDate,
                            ChargeId = c.ChargeId,
                            CreateUserInfo = new SimpleUser()
                            {
                                Id = cu.Id,
                                UserName = cu.TrueName,
                                OrganizationId = cu.OrganizationId
                            },
                            BranchInfo = new Organizations()
                            {
                                Id = bo.Id,
                                OrganizationName = bo.OrganizationName
                            },
                            OrganizationExpansion = new OrganizationExpansion()
                            {
                                FullName = oe.FullName,
                                SonId = oe.SonId
                            },
                            Organizations = new Organizations()
                            {
                                Id = o.Id,
                                OrganizationName = o.OrganizationName
                            },
                            ReimburseUserInfo = new HumanInfo()
                            {
                                ID = ru.ID,
                                UserID = ru.UserID,
                                Name = ru.Name,

                            }
                        };

                return q;
            }
        }

        public async Task DeleteBorrowing(SimpleUser user, string id)
        {
            string cn = await Context.ChargeInfos.Where(x => x.ID == id).Select(x => x.ChargeNo).FirstOrDefaultAsync();

            ChargeInfo ci = new ChargeInfo()
            {
                ID = id,
                IsDeleted = true,
                DeleteTime = DateTime.Now,
                DeleteUser = user.Id,
                ChargeNo = cn
            };

            var entry = Context.ChargeInfos.Attach(ci);
            entry.Property(c => c.IsDeleted).IsModified = true;
            entry.Property(c => c.DeleteTime).IsModified = true;
            entry.Property(c => c.DeleteUser).IsModified = true;

            ModifyInfo mi = new ModifyInfo()
            {
                Id = Guid.NewGuid().ToString("N").ToLower(),
                ChargeId = id,
                CreateTime = DateTime.Now,
                CreateUser = user.Id,
                Department = user.OrganizationId,
                Type = ModifyTypeEnum.Deleted,
                TypeMemo = ModifyTypeConstans.Deleted
            };
            Context.ModifyInfos.Add(mi);

            await Context.SaveChangesAsync();
        }

        public async Task<ChargeInfo> Get(string id)
        {
            return await Context.ChargeInfos.AsNoTracking().Where(c => c.ID == id).FirstOrDefaultAsync();
        }

        public async Task<int> GetChargeNo(string branchId, string prefix, DateTime time, int type)
        {
            DateTime dt = time.Date;
            DateTime nextDt = dt.AddDays(1);



            var q = from c in Context.ChargeInfos.AsNoTracking()
                    where c.BranchPrefix == prefix && c.CreateTime >= dt && c.CreateTime < nextDt && c.Type == type
                    orderby c.Seq descending
                    select c.Seq;

            int maxSeq = await q.FirstOrDefaultAsync();


            return maxSeq + 1;
        }

        public async Task<ChargeInfo> GetDetail(SimpleUser user, string id)
        {
            var q = SimpleQuery;
            q = q.Where(c => c.ID == id);
            ChargeInfo ci = await q.FirstOrDefaultAsync();
            if (ci == null)
            {
                return ci;
            }


            //历史
            var hq = from h in Context.ModifyInfos.AsNoTracking()
                     join oe1 in Context.OrganizationExpansions.AsNoTracking() on new { h.Department, Type = "Region" } equals new { Department = oe1.SonId, oe1.Type } into oe2
                     from oe in oe2.DefaultIfEmpty()
                     join o1 in Context.Organizations.AsNoTracking() on h.Department equals o1.Id into o2
                     from o in o2.DefaultIfEmpty()
                     join u1 in Context.Users.AsNoTracking() on h.CreateUser equals u1.Id into u2
                     from u in u2.DefaultIfEmpty()
                     where h.ChargeId == id
                     orderby h.Seq descending
                     select new ModifyInfo()
                     {
                         Id = h.Id,
                         Type = h.Type,
                         TypeMemo = h.TypeMemo,
                         CreateTime = h.CreateTime,
                         CreateUser = h.CreateUser,
                         Department = h.Department,
                         RelativeId = h.RelativeId,
                         Status = h.Status,
                         CreateUserInfo = new SimpleUser()
                         {
                             Id = h.CreateUser,
                             UserName = u.TrueName
                         },
                         OrganizationExpansion = new OrganizationExpansion()
                         {
                             FullName = oe.FullName,
                             OrganizationId = oe.OrganizationId
                         },
                         Organizations = new Organizations()
                         {
                             OrganizationName = o.OrganizationName,
                             Id = o.Id
                         }
                     };
            ci.History = await hq.ToListAsync();

            return ci;
        }

        public async Task Save(SimpleUser user, ChargeInfo chargeInfo)
        {
            var q = from c in Context.ChargeInfos
                    where c.ID == chargeInfo.ID
                    select c;
            var old = await q.FirstOrDefaultAsync();
            if (old == null)
            {
                chargeInfo.CreateTime = DateTime.Now;
                chargeInfo.CreateUser = user.Id;
                if (chargeInfo.Status == 4)
                {
                    chargeInfo.SubmitTime = DateTime.Now;
                    chargeInfo.SubmitUser = user.Id;
                }

                Context.ChargeInfos.Add(chargeInfo);
                
                ModifyInfo mi = new ModifyInfo()
                {
                    Id = Guid.NewGuid().ToString("N").ToLower(),
                    ChargeId = chargeInfo.ID,
                    CreateTime = DateTime.Now,
                    CreateUser = user.Id,
                    Department = user.OrganizationId,
                    RelativeId = chargeInfo.ChargeNo,
                    Status = chargeInfo.Status,
                    Type = chargeInfo.Status == 4 ? ModifyTypeEnum.Submit : ModifyTypeEnum.Add,
                    TypeMemo = chargeInfo.Status == 4 ? ModifyTypeConstans.Submit : ModifyTypeConstans.Add
                };
                Context.ModifyInfos.Add(mi);

            }
            else
            {
                //修改
                old.BillCount = chargeInfo.BillCount;
                old.BillAmount = chargeInfo.BillAmount;
                old.ChargeAmount = chargeInfo.ChargeAmount;
                old.IsPayment = chargeInfo.IsPayment;
                old.PaymentAmount = chargeInfo.PaymentAmount;
                old.Status = chargeInfo.Status;
                old.Memo = chargeInfo.Memo;
                old.IsBackup = chargeInfo.IsBackup;
                old.BillStatus = chargeInfo.BillStatus;
                old.UpdateTime = DateTime.Now;
                old.UpdateUser = user.Id;
                old.ExpectedPaymentDate = chargeInfo.ExpectedPaymentDate;
                

                if (chargeInfo.Status == 4 && old.Status != 4)
                {
                    old.SubmitTime = DateTime.Now;
                    old.SubmitUser = user.Id;
                }

                
                ModifyInfo mi = new ModifyInfo()
                {
                    Id = Guid.NewGuid().ToString("N").ToLower(),
                    ChargeId = chargeInfo.ID,
                    CreateTime = DateTime.Now,
                    CreateUser = user.Id,
                    Department = user.OrganizationId,
                    RelativeId = chargeInfo.ChargeNo,
                    Status = chargeInfo.Status,
                    Type = chargeInfo.Status == 4 ? ModifyTypeEnum.Submit : ModifyTypeEnum.Modify,
                    TypeMemo = chargeInfo.Status == 4 ? ModifyTypeConstans.Submit : ModifyTypeConstans.Modify
                };
                Context.ModifyInfos.Add(mi);
            }

            await Context.SaveChangesAsync();
        }

        public async Task UpdateStatus(SimpleUser user, string id, int status, string message, ModifyTypeEnum mtype, string mtypememo)
        {
            string cn = await Context.ChargeInfos.Where(x => x.ID == id).Select(x => x.ChargeNo).FirstOrDefaultAsync();

            ChargeInfo ci = new ChargeInfo()
            {
                ID = id,
                Status = status,
                UpdateTime = DateTime.Now,
                UpdateUser = user.Id,
                ChargeNo = cn,
                ConfirmMessage = message,
                SubmitUser = user.Id,
                SubmitTime = DateTime.Now
            };

            var entry = Context.ChargeInfos.Attach(ci);
            entry.Property(c => c.Status).IsModified = true;
            entry.Property(c => c.UpdateUser).IsModified = true;
            entry.Property(c => c.UpdateTime).IsModified = true;
            if (message != null)
            {
                entry.Property(c => c.ConfirmMessage).IsModified = true;
            }
            if (status == 4)
            {
                entry.Property(c => c.SubmitTime).IsModified = true;
                entry.Property(c => c.SubmitUser).IsModified = true;
            }

            ModifyInfo mi = new ModifyInfo()
            {
                Id = Guid.NewGuid().ToString("N").ToLower(),
                ChargeId = id,
                CreateTime = DateTime.Now,
                CreateUser = user.Id,
                Department = user.OrganizationId,
                Status = status,
                Type = mtype,
                TypeMemo = mtypememo
            };
            Context.ModifyInfos.Add(mi);

            await Context.SaveChangesAsync();
        }

        public async Task<LimitTipInfo> UpdateReimbursedAmount(SimpleUser user, ChargeInfo bill)
        {
            if (String.IsNullOrEmpty(bill.ChargeId))
            {
                return null;
            }

            var ci = await Context.ChargeInfos.AsNoTracking().Where(c => c.ID == bill.ChargeId).FirstOrDefaultAsync();
            if (ci == null)
            {
                return null;
            }
            LimitTipInfo li = new LimitTipInfo();
            li.LimitAmount = ci.ChargeAmount;

            var query = from c in Context.ChargeInfos.AsNoTracking()
                        where c.ChargeId == bill.ChargeId && c.ID != bill.ID && c.IsDeleted == false
                        select c;
            decimal total = await query.SumAsync(c => c.ReimbursedAmount ?? 0);

            List<int> status = new List<int>() { 0, 16 };
            var query2 = from c in Context.ChargeInfos.AsNoTracking()
                         where c.ChargeId == bill.ChargeId && c.ID != bill.ID && c.IsDeleted == false && status.Contains(c.Status)
                         select c;
            decimal unSubmitAmount = await query2.SumAsync(c => c.ReimbursedAmount ?? 0);

            li.TotalAmount = total;
            li.UnSubmitAmount = unSubmitAmount;

            if( (li.TotalAmount - li.UnSubmitAmount + bill.ReimbursedAmount) > li.LimitAmount)
            {
                return li;
            }
            else
            {
                ci.LastReimbursedTime = DateTime.Now;
                ci.ReimbursedAmount = li.TotalAmount - li.UnSubmitAmount + bill.ReimbursedAmount;
                if( ci.ReimbursedAmount == li.LimitAmount)
                {
                    ci.IsReimbursed = true;
                }
                var entry = Context.ChargeInfos.Attach(ci);
                entry.Property(c => c.LastReimbursedTime).IsModified = true;
                entry.Property(c => c.ReimbursedAmount).IsModified = true;
                entry.Property(c => c.IsReimbursed).IsModified = true;

                await Context.SaveChangesAsync();
            }

            return li;
        }
    }
}
