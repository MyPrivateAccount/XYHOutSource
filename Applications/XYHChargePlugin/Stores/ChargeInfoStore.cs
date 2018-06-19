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

            }

            await Context.SaveChangesAsync();
        }



        public async Task<ChargeInfo> GetDetail(SimpleUser user, string id)
        {
            var q = SimpleQuery;
            q = q.Where(c => c.ID == id);
            ChargeInfo ci = await q.FirstOrDefaultAsync();
            if(ci == null)
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
                          where ids.Contains(f.FileGuid)
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

            return ci;

        }
    }
}
