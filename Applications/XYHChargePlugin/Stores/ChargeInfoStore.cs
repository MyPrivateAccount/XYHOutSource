using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Models;
using XYHChargePlugin.Models;
using Microsoft.EntityFrameworkCore;

namespace XYHChargePlugin.Stores
{
    public class ChargeInfoStore : IChargeInfoStore
    {
        class OrgPar
        {
            public string OrganizationId { get; set; }

            public string ParentId { get; set; }

            public string ParValue { get; set; }

            public OrgPar Parent { get; set; }
        }

        public ChargeInfoStore(ChargeDbContext hudb)
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
                        where c.Type ==1
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
                            ChargeId = c.ChargeId,
                            ReimbursedAmount = c.ReimbursedAmount,
                            RecordingStatus = c.RecordingStatus,
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

        public IQueryable<CostInfo> CostQuery
        {
            get
            {
                var query = from c in Context.CostInfos.AsNoTracking()
                            join ci in Context.ChargeInfos.AsNoTracking() on c.ChargeId equals ci.ID
                            join oe1 in Context.OrganizationExpansions.AsNoTracking() on new { ci.ReimburseDepartment, Type = "Region" } equals new { ReimburseDepartment = oe1.SonId, oe1.Type } into oe2
                            from oe in oe2.DefaultIfEmpty()
                            join o1 in Context.Organizations.AsNoTracking() on ci.ReimburseDepartment equals o1.Id into o2
                            from o in o2.DefaultIfEmpty()
                            join ru1 in Context.HumanInfo.AsNoTracking() on ci.ReimburseUser equals ru1.ID into ru2
                            from ru in ru2.DefaultIfEmpty()
                            orderby ci.CreateTime descending
                            select new CostInfo()
                            {
                                Type = c.Type,
                                Memo = c.Memo,
                                Amount = c.Amount,
                                ChargeId = c.ChargeId,
                                Id = c.Id,
                                ChargeInfo = new ChargeInfo()
                                {
                                    ID = ci.ID,
                                    ChargeNo = ci.ChargeNo,
                                    CreateTime = ci.CreateTime,
                                    ReimburseDepartment = ci.ReimburseDepartment,
                                    ReimburseUser = ci.ReimburseUser,
                                    IsPayment = ci.IsPayment,
                                    PaymentTime = ci.PaymentTime,
                                    IsDeleted = ci.IsDeleted,
                                    IsBackup = ci.IsBackup,
                                    Memo = ci.Memo,
                                    Status = ci.Status,
                                    BillStatus = ci.BillStatus,
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
                                }
                            };

                return query;
            }
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


        public async Task<int> GetPaymentNo(string branchId, string prefix, DateTime time)
        {

            DateTime dt = time.Date;
            DateTime nextDt = dt.AddDays(1);



            var q = from c in Context.PaymentInfo.AsNoTracking()
                    where c.BranchPrefix == prefix && c.CreateTime >= dt && c.CreateTime < nextDt
                    orderby c.Seq descending
                    select c.Seq;

            int maxSeq = await q.FirstOrDefaultAsync();


            return maxSeq + 1;

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
                if(chargeInfo.Status == 4)
                {
                    chargeInfo.SubmitTime = DateTime.Now;
                    chargeInfo.SubmitUser = user.Id;
                }

               // chargeInfo.ChargeNo = GetChargeNo()
                //新增
                Context.ChargeInfos.Add(chargeInfo);
                if (chargeInfo.FeeList != null && chargeInfo.FeeList.Count > 0)
                {
                    Context.CostInfos.AddRange(chargeInfo.FeeList);
                }
                if(chargeInfo.BillList!=null && chargeInfo.BillList.Count > 0)
                {
                    chargeInfo.BillList.ForEach(item =>
                    {
                        item.CreateUser = user.Id;
                        item.CreateTime = DateTime.Now;
                    });
                    Context.ReceiptInfos.AddRange(chargeInfo.BillList);

                    chargeInfo.BillList.ForEach(item =>
                    {
                        if (item.FileScopes != null && item.FileScopes.Count>0)
                        {
                            Context.FileScopeInfo.AddRange(item.FileScopes);

                            item.FileScopes.ForEach(f =>
                            {
                                if(f.FileList!=null && f.FileList.Count > 0)
                                {
                                    f.FileList.ForEach(fi =>
                                    {
                                        fi.CreateTime = DateTime.Now;
                                        fi.CreateUser = user.Id;
                                    });
                                    Context.FileInfos.AddRange(f.FileList);
                                }
                            });
                        }
                    });
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
                    Type = chargeInfo.Status == 4 ? ModifyTypeEnum.Submit : ModifyTypeEnum.Add,
                    TypeMemo = chargeInfo.Status == 4 ? ModifyTypeConstans.Submit :  ModifyTypeConstans.Add
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

                if (chargeInfo.Status == 4 && old.Status != 4)
                {
                    old.SubmitTime = DateTime.Now;
                    old.SubmitUser = user.Id;
                }

                var fl = await Context.CostInfos.Where(x => x.ChargeId == chargeInfo.ID).ToListAsync();
                var dl = fl.Where(x => !chargeInfo.FeeList.Any(y => y.Id == x.Id)).ToList();
                if (dl.Count > 0)
                {
                    Context.CostInfos.RemoveRange(dl);
                }
                var al = chargeInfo.FeeList.Where(x => !fl.Any(y => y.Id == x.Id)).ToList();
                if (al.Count > 0)
                {
                    Context.CostInfos.AddRange(al);
                }
                var ul = fl.Where(x => chargeInfo.FeeList.Any(y => y.Id == x.Id)).ToList();
                ul.ForEach(x =>
                {
                    var ni = chargeInfo.FeeList.FirstOrDefault(y => y.Id == x.Id);
                    x.Amount = ni.Amount;
                    x.Memo = ni.Memo;
                    x.Type = ni.Type;
                });


                var bl = await Context.ReceiptInfos.Where(x => x.ChargeId == chargeInfo.ID).ToListAsync();
                var bdl = bl.Where(x => !chargeInfo.BillList.Any(y => y.Id == x.Id)).ToList();
                if (bdl.Count > 0)
                {
                    Context.ReceiptInfos.RemoveRange(bdl);

                    List<string> ids = bdl.Select(x => x.Id).Distinct().ToList();
                    //删除对应文件
                    var scopes = await Context.FileScopeInfo.Where(x => ids.Contains(x.ReceiptId)).ToListAsync();
                    if (scopes.Count > 0)
                    {
                        Context.FileScopeInfo.RemoveRange(scopes);

                        ids = scopes.Select(x => x.FileGuid).Distinct().ToList();
                        var files = await Context.FileInfos.Where(x => ids.Contains(x.FileGuid)).ToListAsync();
                        if (files.Count > 0)
                        {
                            Context.FileInfos.RemoveRange(files);
                        }
                    }


                }
                var bal = chargeInfo.BillList.Where(x => !bl.Any(y => y.Id == x.Id)).ToList();
                if (bal.Count > 0)
                {
                    bal.ForEach(item =>
                    {
                        item.CreateTime = DateTime.Now;
                        item.CreateUser = user.Id;
                    });
                    Context.ReceiptInfos.AddRange(bal);

                    //新增
                    bal.ForEach(item =>
                    {
                        if (item.FileScopes != null && item.FileScopes.Count > 0)
                        {
                            Context.FileScopeInfo.AddRange(item.FileScopes);

                            item.FileScopes.ForEach(f =>
                            {
                                if (f.FileList != null && f.FileList.Count > 0)
                                {
                                    Context.FileInfos.AddRange(f.FileList);
                                }
                            });
                        }
                    });
                }
                var bul = bl.Where(x => chargeInfo.BillList.Any(y => y.Id == x.Id)).ToList();
                bul.ForEach(x =>
                {
                    var ni = chargeInfo.BillList.FirstOrDefault(y => y.Id == x.Id);
                    x.ReceiptMoney = ni.ReceiptMoney;
                    x.Memo = ni.Memo;
                    x.ReceiptNumber = ni.ReceiptNumber;
                   
                });

                List<String> ids2 = bul.Select(x => x.Id).Distinct().ToList();
                if (ids2.Count > 0)
                {


                    var newScopes = chargeInfo.BillList.Where(x => ids2.Contains(x.Id)).SelectMany(x => x.FileScopes).ToList(); // bul.SelectMany(x => x.FileScopes).ToList();
                    //old scopes
                    var scopes = await Context.FileScopeInfo.Where(x => ids2.Contains(x.ReceiptId)).ToListAsync();
                    //需增加的scope
                    var addList = newScopes.Where(x => !scopes.Any(y => y.ReceiptId == x.ReceiptId && y.FileGuid == x.FileGuid)).ToList();
                    if (addList.Count > 0)
                    {
                        Context.FileScopeInfo.AddRange(addList);
                    }
                    //需删除的scope
                    var delList = scopes.Where(x => !newScopes.Any(y => y.ReceiptId == x.ReceiptId && y.FileGuid == x.FileGuid)).ToList();
                    if (delList.Count > 0)
                    {
                        Context.FileScopeInfo.AddRange(delList);
                    }

                    //文件列表更新
                    var newFiles = newScopes.SelectMany(x => x.FileList).ToList();
                    var fids = newFiles.Select(x => x.FileGuid).ToList();
                    var files = await Context.FileInfos.Where(x => fids.Contains(x.FileGuid)).ToListAsync();
                    //需新增的文件
                    var addFl = newFiles.Where(x => !files.Any(y => y.FileGuid == x.FileGuid && y.Type == x.Type && y.FileExt == x.FileExt)).ToList();
                    if (addFl.Count > 0)
                    {
                        addFl.ForEach(f =>
                        {
                            f.CreateTime = DateTime.Now;
                            f.CreateUser = user.Id;
                        });
                        Context.FileInfos.AddRange(addFl);
                    }

                    //需删除的
                    var delFl = files.Where(x => !newFiles.Any(y => y.FileGuid == x.FileGuid && y.Type == x.Type && y.FileExt == x.FileExt)).ToList();
                    if (delFl.Count > 0)
                    {
                        delFl.ForEach(f =>
                        {
                            f.IsDeleted = true;
                            f.DeleteTime = DateTime.Now;
                            f.DeleteUser = user.Id;
                        });
                    }
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
                    Type = chargeInfo.Status == 4 ? ModifyTypeEnum.Submit: ModifyTypeEnum.Modify,
                    TypeMemo = chargeInfo.Status == 4 ? ModifyTypeConstans.Submit :  ModifyTypeConstans.Modify
                };
                Context.ModifyInfos.Add(mi);
            }

            await Context.SaveChangesAsync();
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

            //获取费用列表
            var cq = from f in Context.CostInfos.AsNoTracking()
                     where f.ChargeId == id
                     select f;
            ci.FeeList = await cq.ToListAsync();

            //发票列表
            var bq = from b in Context.ReceiptInfos.AsNoTracking()
                     where b.ChargeId == id
                     select b;
            ci.BillList = await bq.ToListAsync();

            //文件列表
            var fids = ci.BillList.Select(x => x.Id).Distinct().ToList();
            if (fids.Count == 0)
            {
                return ci;
            }

            var fq = from fs in Context.FileScopeInfo.AsNoTracking()
                     where fids.Contains(fs.ReceiptId)
                     select fs;
            var fsList = await fq.ToListAsync();

            var ids = fsList.Select(x => x.FileGuid).Distinct().ToList();
            if (ids.Count > 0)
            {
                var fiq = from f in Context.FileInfos.AsNoTracking()
                          where ids.Contains(f.FileGuid) && f.IsDeleted ==false
                          select f;
                var fl = await fiq.ToListAsync();

                fsList.ForEach(item =>
                {
                    item.FileList = fl.Where(x => x.FileGuid == item.FileGuid).ToList();
                });


            }

            ci.BillList.ForEach(item =>
            {
                item.FileScopes = fsList.Where(x => x.ReceiptId == item.Id).ToList();
            });

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

        public async Task DeleteCharge(SimpleUser user, string id)
        {
            string cn = await Context.ChargeInfos.Where(x => x.ID == id).Select(x=>x.ChargeNo).FirstOrDefaultAsync();

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
            if (message!=null)
            {
                entry.Property(c => c.ConfirmMessage).IsModified = true;
            }
            if(status == 4)
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

        public async Task UpdateBillStatus(SimpleUser user, string id,  int status, string message, ModifyTypeEnum mtype, string mtypememo)
        {
            string cn = await Context.ChargeInfos.Where(x => x.ID == id).Select(x => x.ChargeNo).FirstOrDefaultAsync();

            ChargeInfo ci = new ChargeInfo()
            {
                ID = id,
                BillStatus = status,
                UpdateTime = DateTime.Now,
                UpdateUser = user.Id,
                ChargeNo = cn,
                ConfirmBillMessage = message
            };

            var entry = Context.ChargeInfos.Attach(ci);
            entry.Property(c => c.BillStatus).IsModified = true;
            entry.Property(c => c.UpdateTime).IsModified = true;
            entry.Property(c => c.UpdateUser).IsModified = true;
            if (message != null)
            {
                entry.Property(c => c.ConfirmBillMessage).IsModified = true;
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

        public async Task<ChargeInfo> BackupBill(SimpleUser user, ChargeInfo chargeInfo)
        {
            //后补发票
            var ci = new ChargeInfo()
            {
                ID = chargeInfo.ID,
                BillAmount = chargeInfo.BillAmount,
                BillCount = chargeInfo.BillCount,
                BillStatus = chargeInfo.BillStatus,
                Backuped = chargeInfo.Backuped,
                ChargeNo = chargeInfo.ChargeNo
            };

            var ciEntry = Context.ChargeInfos.Attach(ci);
            ciEntry.Property(c => c.BillAmount).IsModified = true;
            ciEntry.Property(c => c.BillCount).IsModified = true;
            ciEntry.Property(c => c.BillStatus).IsModified = true;
            ciEntry.Property(c => c.Backuped).IsModified = true;

            //单据更新
            var oldBills = await Context.ReceiptInfos.Where(x => x.ChargeId == chargeInfo.ID).ToListAsync();
            List<String> bids = oldBills.Select(x => x.Id).Distinct().ToList();

            List<FileScopeInfo> oldFileScopes = new List<FileScopeInfo>();
            if (bids.Count > 0)
            {
                oldFileScopes = await Context.FileScopeInfo.Where(x => bids.Contains(x.ReceiptId)).ToListAsync();
            }
            List<FileInfo> oldFiles = new List<FileInfo>();
            List<string> fsIds = oldFileScopes.Select(x => x.FileGuid).Distinct().ToList();
            if (fsIds.Count > 0)
            {
                oldFiles = await Context.FileInfos.Where(x => fsIds.Contains(x.FileGuid)).ToListAsync();
            }

            //新增的
            var addBills = chargeInfo.BillList.AddedItems(oldBills, (a, b) => a.Id == b.Id);
            addBills.ForEach(item =>
            {
                item.CreateTime = DateTime.Now;
                item.CreateUser = user.Id;
                Context.ReceiptInfos.Add(item);
            });

            //删除的
            var removedBills = chargeInfo.BillList.RemovedItems(oldBills, (a, b) => a.Id == b.Id);
            if (removedBills.Count > 0)
            {
                Context.ReceiptInfos.RemoveRange(removedBills);
            }

            //更新的
            var updatedBills = oldBills.UpdatedItems(chargeInfo.BillList, (a, b) => a.Id == b.Id);
            if (updatedBills.Count > 0)
            {
                updatedBills.ForEach(item =>
                {
                    var newItem = chargeInfo.BillList.FirstOrDefault(x => x.Id == item.Id);
                    item.ReceiptMoney = newItem.ReceiptMoney;
                    item.ReceiptNumber = newItem.ReceiptNumber;
                    item.Memo = newItem.Memo;
                });
            }

            //更新文件列表
            var newFileScopes = chargeInfo.BillList.SelectMany(b => b.FileScopes?? new List<FileScopeInfo>()).ToList();
            var addedFileScopes = newFileScopes.AddedItems(oldFileScopes, (a, b) => a.FileGuid == b.FileGuid);
            if (addedFileScopes.Count > 0)
            {
                Context.FileScopeInfo.AddRange(addedFileScopes);
            }
            var removedFileScopes = newFileScopes.RemovedItems(oldFileScopes, (a, b) => a.FileGuid == b.FileGuid);
            if (removedFileScopes.Count > 0)
            {
                Context.FileScopeInfo.RemoveRange(removedFileScopes);
            }

            //文件
            Func<FileInfo, FileInfo, bool> checkFile = (a, b) => a.FileGuid == b.FileGuid && a.Type == b.Type && a.FileExt == b.FileExt;

            var newFiles = newFileScopes.SelectMany(x => x.FileList ?? new List<FileInfo>()).ToList();
            var addedFiles = newFiles.AddedItems(oldFiles,  checkFile);
            if (addedFiles.Count > 0)
            {
                Context.FileInfos.AddRange(addedFiles);
            }
            var removedFiles = newFiles.RemovedItems(oldFiles,  checkFile);
            if (removedFiles.Count > 0)
            {
                removedFiles.ForEach(fi =>
                {
                    fi.IsDeleted = true;
                    fi.DeleteTime = DateTime.Now;
                    fi.DeleteUser = user.Id;
                });
            }

            ModifyInfo mi = new ModifyInfo()
            {
                Id = Guid.NewGuid().ToString("N").ToLower(),
                ChargeId = chargeInfo.ID,
                CreateTime = DateTime.Now,
                CreateUser = user.Id,
                Department = user.OrganizationId,
                Status = chargeInfo.Status,
                Type = ci.BillStatus ==4? ModifyTypeEnum.SubmitBill: ModifyTypeEnum.Backup,
                TypeMemo = ci.BillStatus == 4 ? ModifyTypeConstans.SubmitBill :ModifyTypeConstans.Backup
            };
            Context.ModifyInfos.Add(mi);

            await Context.SaveChangesAsync();

            return chargeInfo;
        }

        public async Task<PaymentInfo> Payment(SimpleUser user, PaymentInfo payment)
        {
            decimal total =await Context.PaymentInfo.AsNoTracking().Where(x => x.ChargeId == payment.ChargeId).SumAsync(x => x.PaymentAmount);

            var old = await Context.PaymentInfo.Where(x => x.Id == payment.Id).FirstOrDefaultAsync();
            if (old != null)
            {
                total = total - old.PaymentAmount;
                //更新
                old.Memo = payment.Memo;
                old.Payee = payment.Payee;
                old.PaymentAmount = payment.PaymentAmount;
                old.PaymentDate = payment.PaymentDate;
                old.Status = payment.Status;
                
            }
            else
            {
                //新增
                payment.CreateTime = DateTime.Now;
                payment.CreateUser = user.Id;
                payment.Department = user.OrganizationId;

                Context.PaymentInfo.Add(payment);
            }
            total = total + payment.PaymentAmount;

            var ci = await Context.ChargeInfos.AsNoTracking().Where(x => x.ID == payment.ChargeId).FirstOrDefaultAsync();
            var entry = Context.ChargeInfos.Attach(ci);
            if (ci.ChargeAmount == total)
            {
                ci.IsPayment = true;
                ci.PaymentTime = payment.PaymentDate;
                
            }
            ci.PaymentAmount = total;

            ModifyInfo mi = new ModifyInfo()
            {
                Id = Guid.NewGuid().ToString("N").ToLower(),
                ChargeId = ci.ID,
                CreateTime = DateTime.Now,
                CreateUser = user.Id,
                Department = user.OrganizationId,
                Status = ci.Status,
                RelativeId = payment.Id,
                Type =  ModifyTypeEnum.Payment,
                TypeMemo = ModifyTypeConstans.Payment
            };
            Context.ModifyInfos.Add(mi);


            await Context.SaveChangesAsync();

            payment.ChargeInfo = ci;

            return payment;


        }

        public async Task<ChargeInfo> Get(string id)
        {
            return await Context.ChargeInfos.AsNoTracking().Where(c => c.ID == id).FirstOrDefaultAsync();
        }

        public async Task<LimitTipInfo> GetLimitTip(string userId, DateTime startTime, DateTime endTime, string ignoreId)
        {
            LimitTipInfo li = new LimitTipInfo();
            //限额
            var s = await Context.LimitInfos.AsNoTracking().Where(x => x.UserId == userId && x.IsDeleted == false).FirstOrDefaultAsync();
            if (s != null)
            {
                li.LimitAmount = s.Amount;
            }

            //已报销
            var q = from c in Context.ChargeInfos.AsNoTracking()
                    join f in Context.CostInfos.AsNoTracking() on c.ID equals f.ChargeId
                    join d in Context.DictionaryDefine.AsNoTracking() on new { Type = f.Type.ToString(), GroupId = "CHARGE_COST_TYPE" } equals new { Type = d.Value, d.GroupId }
                    where c.IsDeleted == false && c.CreateTime >= startTime && c.CreateTime < endTime && d.Ext1=="1" && c.ReimburseUser == userId
                    select new
                    {
                        c = c,
                        f = f
                    };
            if (!String.IsNullOrEmpty(ignoreId))
            {
                q = q.Where(c => c.c.ID != ignoreId);
            }
            li.TotalAmount = await q.SumAsync(f => f.f.Amount);

            List<int> status = new List<int>() { 0, 16 };
            var q2 = from c in Context.ChargeInfos.AsNoTracking()
                     join f in Context.CostInfos.AsNoTracking() on c.ID equals f.ChargeId
                     join d in Context.DictionaryDefine.AsNoTracking() on new { Type = f.Type.ToString(), GroupId = "CHARGE_COST_TYPE" } equals new { Type = d.Value, d.GroupId }
                     where c.IsDeleted == false && c.CreateTime >= startTime && c.CreateTime < endTime && d.Ext1 == "1" && status.Contains(c.Status) && c.ReimburseUser == userId
                     select new
                     {
                         c = c,
                         f = f
                     };
            if (!String.IsNullOrEmpty(ignoreId))
            {
                q2 = q2.Where(c => c.c.ID != ignoreId);
            }
            li.UnSubmitAmount = await q2.SumAsync(f=>f.f.Amount );

            return li;
        }

        public async Task<List<CostInfo>> GetBillList(string chargeId)
        {
            var query = from c in Context.CostInfos.AsNoTracking()
                        join d1 in Context.DictionaryDefine.AsNoTracking() on new { Type = c.Type.ToString(), GroupId = "CHARGE_COST_TYPE" } equals new { Type = d1.Value, d1.GroupId } into d2
                        from d in d2.DefaultIfEmpty()
                        where c.ChargeId == chargeId
                        select new CostInfo() {
                            Amount = c.Amount,
                            ChargeId = c.ChargeId,
                            Id = c.Id,
                            Memo = c.Memo,
                            Type = c.Type,
                            TypeInfo = new DictionaryDefine()
                            {
                                Ext1 = d.Ext1,
                                Ext2 = d.Ext2,
                                Key = d.Key,
                                Value = d.Value,
                                GroupId = d.GroupId,
                                Order = d.Order
                            },
                            TypeName = d.Key
                        };

            return await query.ToListAsync();
        }

        public async Task<List<DictionaryDefine>> GetDictionaryDefine(string groupId)
        {
            return await Context.DictionaryDefine.AsNoTracking().Where(d => d.GroupId == groupId).ToListAsync();
        }
    }
}
