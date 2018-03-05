using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class ExtendUserManager<TUser> : AspNetUserManager<TUser>, IDisposable where TUser : class
    {
        public ExtendUserManager(IUserStore<TUser> store,
            RoleManager<Roles> roleManager,
            IOrganizationExpansionStore organizationExpansionStore,
            IMapper mapper,
            IUserRoleStore userRoleStore,
            ExtendUserStore<ApplicationDbContext> extendUserStore,
            IOrganizationStore organizationStore,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TUser> passwordHasher,
            IEnumerable<IUserValidator<TUser>> userValidators,
            IEnumerable<IPasswordValidator<TUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<TUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            _extendUserStore = extendUserStore ?? throw new ArgumentNullException(nameof(extendUserStore));
            _organizationStore = organizationStore ?? throw new ArgumentNullException(nameof(organizationStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userRoleStore = userRoleStore ?? throw new ArgumentNullException(nameof(userRoleStore));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _organizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
        }

        protected new IUserStore<TUser> Store { get; }
        protected ExtendUserStore<ApplicationDbContext> _extendUserStore { get; }
        protected IOrganizationStore _organizationStore { get; }
        protected IMapper _mapper { get; }
        protected RoleManager<Roles> _roleManager { get; }
        protected IUserRoleStore _userRoleStore { get; }
        protected IOrganizationExpansionStore _organizationExpansionStore { get; }

        public async Task<PagingResponseMessage<UserInfoResponse>> Search(UserSearchCondition condition, CancellationToken cancellationToken)
        {
            PagingResponseMessage<UserInfoResponse> pagingResponse = new PagingResponseMessage<UserInfoResponse>();
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            if (!string.IsNullOrEmpty(condition.RoleId))
            {
                var q = from ur in _userRoleStore.UserRoles.AsNoTracking()
                        join r1 in _roleManager.Roles.AsNoTracking() on ur.RoleId equals r1.Id into r2
                        from r in r2.DefaultIfEmpty()
                        join u1 in _extendUserStore.Users.AsNoTracking() on ur.UserId equals u1.Id into u2
                        from u in u2.DefaultIfEmpty()
                        join b in _organizationStore.Organizations.AsNoTracking() on u.OrganizationId equals b.Id into b1
                        from b2 in b1.DefaultIfEmpty()
                        join c in _organizationStore.Organizations.AsNoTracking() on u.FilialeId equals c.Id into c1
                        from c2 in c1.DefaultIfEmpty()
                        where !u.IsDeleted
                        select new UserRole
                        {
                            UserId = ur.UserId,
                            RoleId = ur.RoleId,
                            Roles = new Roles
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Type = r.Type,
                                NormalizedName = r.NormalizedName,
                                OrganizationId = r.OrganizationId
                            },
                            Users = new Users
                            {
                                Avatar = u.Avatar,
                                Email = u.Email,
                                Filiale = c2.OrganizationName,
                                Id = u.Id,
                                OrganizationId = u.OrganizationId,
                                FilialeId = u.FilialeId,
                                Organization = b2.OrganizationName,
                                PhoneNumber = u.PhoneNumber,
                                Position = u.Position,
                                TrueName = u.TrueName,
                                UserName = u.UserName,
                                CityCode = c2.City,
                            }
                        };
                if (condition?.OrganizationIds?.Count > 0)
                {
                    q = q.Where(x => condition.OrganizationIds.Contains(x.Users.OrganizationId));
                }
                if (!string.IsNullOrEmpty(condition.KeyWords))
                {
                    q = q.Where(x => x.Users.TrueName.Contains(condition.KeyWords) || x.Users.UserName.Contains(condition.KeyWords) || x.Users.PhoneNumber.Contains(condition.KeyWords));
                }
                if (!string.IsNullOrEmpty(condition.RoleId))
                {
                    q = q.Where(x => x.RoleId == condition.RoleId);
                }
                pagingResponse.TotalCount = await q.CountAsync();
                var qlist = await q.OrderBy(a => a.Users.TrueName).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
                var resulte = qlist.Select(a => new UserInfoResponse
                {
                    Avatar = a.Users.Avatar,
                    CityCode = a.Users.CityCode,
                    Email = a.Users.Email,
                    Filiale = a.Users.Filiale,
                    FilialeId = a.Users.FilialeId,
                    Id = a.Users.Id,
                    Organization = a.Users.Organization,
                    OrganizationId = a.Users.OrganizationId,
                    PhoneNumber = a.Users.PhoneNumber,
                    Position = a.Users.Position,
                    TrueName = a.Users.TrueName,
                    UserName = a.Users.UserName,
                    RoleId = a.RoleId
                }).ToList();
                pagingResponse.PageIndex = condition.PageIndex;
                pagingResponse.PageSize = condition.PageSize;
                pagingResponse.Extension = resulte;
            }
            else
            {
                var q = from a in _extendUserStore.Users.AsNoTracking()
                        join b in _organizationStore.Organizations.AsNoTracking() on a.OrganizationId equals b.Id into b1
                        from b2 in b1.DefaultIfEmpty()
                        join c in _organizationStore.Organizations.AsNoTracking() on a.FilialeId equals c.Id into c1
                        from c2 in c1.DefaultIfEmpty()
                        where !a.IsDeleted
                        select new UserInfoResponse
                        {
                            Avatar = a.Avatar,
                            Email = a.Email,
                            Filiale = c2.OrganizationName,
                            Id = a.Id,
                            OrganizationId = a.OrganizationId,
                            FilialeId = a.FilialeId,
                            Organization = b2.OrganizationName,
                            PhoneNumber = a.PhoneNumber,
                            Position = a.Position,
                            TrueName = a.TrueName,
                            UserName = a.UserName,
                            CityCode = c2.City
                        };
                if (condition?.OrganizationIds?.Count > 0)
                {
                    q = q.Where(x => condition.OrganizationIds.Contains(x.OrganizationId));
                }
                if (!string.IsNullOrEmpty(condition.KeyWords))
                {
                    q = q.Where(x => x.TrueName.Contains(condition.KeyWords) || x.UserName.Contains(condition.KeyWords) || x.PhoneNumber.Contains(condition.KeyWords));
                }
                pagingResponse.TotalCount = await q.CountAsync();
                var resulte = await q.OrderBy(a => a.TrueName).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
                for (int i = 0; i < resulte.Count; i++)
                {
                    resulte[i].Roles = (await _extendUserStore.GetRolesAsync(_mapper.Map<Users>(resulte[i]))).ToList();
                }
                pagingResponse.PageIndex = condition.PageIndex;
                pagingResponse.PageSize = condition.PageSize;
                pagingResponse.Extension = resulte;
            }
            return pagingResponse;
        }

        public async Task<UserInfoResponse> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await (from a in _extendUserStore.Users
                              join b in _organizationStore.Organizations on a.OrganizationId equals b.Id
                              join c in _organizationStore.Organizations on a.FilialeId equals c.Id
                              where a.Id == userId
                              select new UserInfoResponse
                              {
                                  Avatar = a.Avatar,
                                  Email = a.Email,
                                  Filiale = c.OrganizationName,
                                  Id = a.Id,
                                  OrganizationId = a.OrganizationId,
                                  FilialeId = a.FilialeId,
                                  Organization = b.OrganizationName,
                                  PhoneNumber = a.PhoneNumber,
                                  Position = a.Position,
                                  TrueName = a.TrueName,
                                  UserName = a.UserName,
                                  CityCode = c.City
                              }).SingleOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                return new UserInfoResponse();
            }
            user.Roles = (await _extendUserStore.GetRolesAsync(_mapper.Map<Users>(user))).ToList();
            return user;
        }
        //public async Task<Users> FindByUserIdAsync(string userId)
        //{
        //    return await _extendUserStore.GetAsync(a => a.Where(b => b.Id == userId));
        //}

        //public async Task<List<string>> FindAllSonRolesByUserIdAsync(string userId)
        //{
        //    var user = await _extendUserStore.GetAsync(a => a.Where(b => b.Id == userId));
        //    var rolenameList = await _extendUserStore.GetRolesAsync(user);
        //    var roleList = await _roleManager.Roles.Where(a => rolenameList.Contains(a.Name)).ToListAsync();
        //    var roleIds = (await _organizationExpansionStore.ListAsync(a => a.Where(b => roleList.Select(x => x.OrganizationId).Contains(b.OrganizationId)))).Select(o => o.SonId).Distinct();
        //    return roleIds.ToList();
        //}
        //public async Task<IdentityResult> AddToRolesExtentionAsync(TUser user, AddUserToRolesExtRequest roles)
        //{
        //    if (user == null)
        //    {
        //        throw new ArgumentNullException(nameof(user));
        //    }
        //    if (roles == null)
        //    {
        //        throw new ArgumentNullException(nameof(roles));
        //    }

        //    foreach (var role in roles.Roles.Distinct())
        //    {
        //        var normalizedRole = NormalizeKey(role);
        //        if (await _userRoleStore.IsInRoleAsync(user, normalizedRole, CancellationToken))
        //        {
        //            return await UserAlreadyInRoleError(user, role);
        //        }
        //        await _userRoleStore.AddToRoleAsync(user, normalizedRole, CancellationToken);
        //    }
        //    return await UpdateUserAsync(user);
        //}



        public async Task DeleteListAsync(List<string> userIds, CancellationToken cancellationToken = default(CancellationToken))
        {
            var users = await _extendUserStore.ListAsync(a => a.Where(b => userIds.Contains(b.Id)));
            if (users?.Count == 0)
            {
                return;
            }
            await _extendUserStore.DeleteListAsync(users, cancellationToken);
        }
    }
}
