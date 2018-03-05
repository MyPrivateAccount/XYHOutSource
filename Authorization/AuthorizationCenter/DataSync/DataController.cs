using AuthorizationCenter.Managers;
using AuthorizationCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationCenter.DataSync
{
    [Obsolete]
    [Produces("application/json")]
    [Route("api/dataimport")]
    public class DataController : Controller
    {

        public DataController(ApplicationDbContext xyhDbContext,
            InitManager initManager,
            ExtendUserManager<Users> extendUserManager)
        {
            _xyhDbContext = xyhDbContext;
            _initManager = initManager;
            _extendUserManager = extendUserManager;
        }
        private readonly ApplicationDbContext _xyhDbContext;
        private readonly InitManager _initManager;
        private readonly ExtendUserManager<Users> _extendUserManager;

        //[HttpGet("position")]
        //public string ImportPosition()
        //{
        //    try
        //    {
        //        var oalist = _oaDbContext.OaUserPrivs.AsNoTracking().ToList();
        //        List<DictionaryDefine> dlist = new List<DictionaryDefine>();
        //        foreach (var item in oalist)
        //        {
        //            var q = _xyhDbContext.DictionaryDefines.AsNoTracking().Where(a => a.GroupId == "position" && a.Value == item.USER_PRIV.ToString()).ToList();
        //            if (q?.Count == 0)
        //            {
        //                dlist.Add(new DictionaryDefine
        //                {
        //                    CreateTime = DateTime.Now,
        //                    CreateUser = "admin",
        //                    GroupId = "position",
        //                    Key = item.PRIV_NAME,
        //                    Value = item.USER_PRIV.ToString()
        //                });
        //            }
        //        }
        //        _xyhDbContext.AddRange(dlist);
        //        _xyhDbContext.SaveChanges();
        //        var queryable = _xyhDbContext.DictionaryGroups.AsNoTracking().Where(a => a.Id == "position").ToList();
        //        if (queryable?.Count == 0)
        //        {
        //            DictionaryGroup dictionaryGroup = new DictionaryGroup()
        //            {
        //                CreateTime = DateTime.Now,
        //                CreateUser = "admin",
        //                Name = "职位",
        //                Id = "position"
        //            };
        //            _xyhDbContext.Add(dictionaryGroup);
        //            _xyhDbContext.SaveChanges();
        //        }
        //        return "成功";
        //    }
        //    catch (Exception e)
        //    {
        //        return e.ToString();
        //    }
        //}

        //[HttpGet("user")]
        //public async Task<string> ImportUser()
        //{
        //    try
        //    {
        //        var oalist = _oaDbContext.OaUsers.AsNoTracking().ToList();
        //        List<Users> ulist = new List<Users>();
        //        foreach (var item in oalist)
        //        {
        //            //  var user = await _extendUserManager.FindByIdAsync(item.USER_ID.ToString());
        //            var user = await _xyhDbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == item.USER_ID);

        //            if (user == null)
        //            {
        //                await _extendUserManager.CreateAsync(new Users
        //                {
        //                    Avatar = item.AVATAR,
        //                    TrueName = item.USER_NAME,
        //                    Id = item.USER_ID,
        //                    Position = item.USER_PRIV_NO.ToString(),
        //                    OrganizationId = item.DEPT_ID.ToString(),
        //                    UserName = item.BYNAME
        //                }, "123456");
        //            }
        //        }


        //        //var oq = from o in _xyhDbContext.Organizations.AsNoTracking()
        //        //         where o.ParentId == "0"
        //        //         select o;
        //        //var pl = await oq.ToListAsync();
        //        //foreach(var o in pl)
        //        //{
        //        //    var q1 = from o2 in _xyhDbContext.OrganizationExpansions.AsNoTracking()
        //        //             join u in _xyhDbContext.Users.AsNoTracking() on o2.SonId equals u.OrganizationId
        //        //             where o2.OrganizationId == o.Id
        //        //             select u;
        //        //    var u1List = await q1.ToListAsync();

        //        //    var q2 = from u in _xyhDbContext.Users.AsNoTracking()
        //        //             where u.OrganizationId == o.Id
        //        //             select u;
        //        //    u1List.AddRange(await q2.ToListAsync());

        //        //    u1List.ForEach(u =>
        //        //    {
        //        //        Users user = new Users()
        //        //        {
        //        //            Id = u.Id,
        //        //            FilialeId = o.Id
        //        //        };

        //        //        _xyhDbContext.Attach(user);
        //        //        var entry = _xyhDbContext.Entry(user);
        //        //        entry.Property(x => x.FilialeId).IsModified = true;

        //        //    });



        //        //}
        //        //await _xyhDbContext.SaveChangesAsync();

        //        return "成功";
        //    }
        //    catch (Exception e)
        //    {
        //        return e.ToString();
        //    }
        //}

        //[HttpGet("organizations")]
        //public string ImportOrganization()
        //{
        //    try
        //    {
        //        var oalist = _oaDbContext.OaDepartments.AsNoTracking().ToList();

        //        List<Organization> olist = new List<Organization>();
        //        foreach (var item in oalist)
        //        {
        //            var o = _xyhDbContext.Organizations.AsNoTracking().Where(a => a.Id == item.DEPT_ID.ToString()).ToList();
        //            if (o?.Count == 0)
        //            {
        //                olist.Add(new Organization
        //                {
        //                    Id = item.DEPT_ID.ToString(),
        //                    OrganizationName = item.DEPT_NAME,
        //                    ParentId = item.DEPT_PARENT.ToString(),
        //                    Assistant = item.ASSISTANT_ID,
        //                    LeaderManager = string.IsNullOrEmpty(item.LEADER1.Split(',')[0]) ? "" : item.LEADER1.Split(',')[0],
        //                    Manager = string.IsNullOrEmpty(item.MANAGER.Split(',')[0]) ? "" : item.MANAGER.Split(',')[0],
        //                    Sort = 0
        //                });
        //            }
        //        }
        //        _xyhDbContext.AddRange(olist);
        //        _xyhDbContext.SaveChanges();
        //        return "成功";
        //    }
        //    catch (Exception e)
        //    {
        //        return e.ToString();
        //    }
        //}


        [HttpGet("init")]
        public async Task<string> Init()
        {
            try
            {
                await _initManager.InitDate();
                return "成功";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}
