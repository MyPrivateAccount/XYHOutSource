using ApplicationCore.Dto;
using ApplicationCore.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using XYHCustomerPlugin.Dto;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public class CustomerInfoStore : ICustomerInfoStore
    {
        private ILogger logger = LoggerManager.GetLogger("CustomerInfo");
        //Db
        protected CustomerDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public CustomerInfoStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
            CustomerInfos = Context.CustomerInfos;
        }

        public IQueryable<CustomerInfo> CustomerInfos { get; set; }

        public IEnumerable<T> DapperSelect<T>(string sql)
        {
            return Context.Database.GetDbConnection().Query<T>(sql);
        }

        public IQueryable<CustomerInfo> CustomerInfoSimple()
        {
            var query = from b in Context.CustomerInfos.AsNoTracking()
                        join basic1 in Context.CustomerDemands.AsNoTracking() on b.Id equals basic1.CustomerId into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //所属人及部门
                        join cu in Context.Users.AsNoTracking() on b.UserId equals cu.Id into cu1
                        from cu2 in cu1.DefaultIfEmpty()
                        select new CustomerInfo()
                        {
                            Id = b.Id,
                            UserName = cu2.TrueName,
                            DepartmentId = b.DepartmentId,
                            Mark = b.Mark,
                            CustomerName = b.CustomerName,
                            UserId = b.UserId,
                            MainPhone = b.MainPhone,
                            IsDeleted = b.IsDeleted,
                            CreateTime = b.CreateTime,
                            FollowupTime = b.FollowupTime,
                            CustomerStatus = b.CustomerStatus,
                            UserPhone = cu2.PhoneNumber,
                            UserNumber = cu2.UserName,
                            WeChat = b.WeChat,
                            QQ = b.QQ,
                            Email = b.Email,
                            Source = b.Source,
                            Birthday = b.Birthday,
                            Sex = b.Sex,
                            RateProgress = b.RateProgress,
                            IsSellIntention = b.IsSellIntention,
                            UniqueId = b.UniqueId,
                            HeadImg = b.HeadImg,
                            CustomerDemand = basic

                        };
            return query;
        }

        /// <summary>
        /// 获取所有客源信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<CustomerInfo> CustomerInfoAll()
        {
            var query = from b in Context.CustomerInfos.AsNoTracking()
                        join basic1 in Context.CustomerDemands.AsNoTracking() on b.Id equals basic1.CustomerId into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //所属人及部门
                        join cu in Context.Users.AsNoTracking() on b.UserId equals cu.Id into cu1
                        from cu2 in cu1.DefaultIfEmpty()
                        join ru1a in Context.Organizations.AsNoTracking() on b.DepartmentId equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()

                        select new CustomerInfo()
                        {
                            Id = b.Id,
                            UserName = cu2.TrueName,
                            DepartmentId = b.DepartmentId,
                            DepartmentName = ru1.OrganizationName,
                            CustomerDemand = basic,
                            Mark = b.Mark,
                            CustomerName = b.CustomerName,
                            UserId = b.UserId,
                            MainPhone = b.MainPhone,
                            IsDeleted = b.IsDeleted,
                            CreateTime = b.CreateTime,
                            FollowupTime = b.FollowupTime,
                            CustomerStatus = b.CustomerStatus,
                            UserPhone = cu2.PhoneNumber,
                            UserNumber = cu2.UserName,
                            WeChat = b.WeChat,
                            QQ = b.QQ,
                            Email = b.Email,
                            Source = b.Source,
                            Birthday = b.Birthday,
                            Sex = b.Sex,
                            RateProgress = b.RateProgress,
                            IsSellIntention = b.IsSellIntention,
                            UniqueId = b.UniqueId,
                            HeadImg = b.HeadImg,
                            CustomerFollowUp = from CustomerFollowUp in Context.CustomerFollowUps.AsNoTracking()
                                               where !CustomerFollowUp.IsDeleted && CustomerFollowUp.CustomerId == b.Id
                                               orderby CustomerFollowUp.FollowUpTime descending
                                               select new CustomerFollowUp
                                               {
                                                   Id = CustomerFollowUp.Id,
                                                   TypeId = CustomerFollowUp.TypeId,
                                                   TrueName = CustomerFollowUp.TrueName,
                                                   FollowUpTime = CustomerFollowUp.FollowUpTime,
                                                   FollowUpContents = CustomerFollowUp.FollowUpContents,
                                                   CustomerId = CustomerFollowUp.CustomerId,
                                                   IsDeleted = CustomerFollowUp.IsDeleted,
                                                   DemandLevel = CustomerFollowUp.DemandLevel,
                                                   Importance = CustomerFollowUp.Importance,
                                                   FollowMode = CustomerFollowUp.FollowMode,
                                                   UserTrueName = cu2.TrueName,
                                                   DepartmentId = CustomerFollowUp.DepartmentId
                                               },
                            HousingResources = from hr in Context.RelationHouses.AsNoTracking()
                                               where !hr.IsDeleted && hr.CustomerId == b.Id
                                               orderby hr.FeedbackTime descending
                                               select new RelationHouse
                                               {
                                                   Id = hr.Id,
                                                   CustomerId = hr.CustomerId,
                                                   IsDeleted = hr.IsDeleted,
                                                   HousingResourcesId = hr.HousingResourcesId,
                                                   PropertyName = hr.PropertyName,
                                                   RoomNo = hr.RoomNo,
                                                   ShopNumber = hr.ShopNumber,
                                                   AreaFullName = hr.AreaFullName,
                                                   Remark = hr.Remark,
                                                   ImageUrl = hr.ImageUrl
                                               },
                            CustomerTransactions = from t in Context.CustomerTransactions.AsNoTracking()
                                                   where t.CustomerId == b.Id && !t.IsDeleted

                                                   select new CustomerTransactions()
                                                   {
                                                       Id = t.Id,
                                                       IsDeleted = t.IsDeleted,
                                                       CustomerId = t.CustomerId,
                                                       SignTime = t.SignTime,
                                                       TransactionsStatus = t.TransactionsStatus,
                                                       BuildingId = t.BuildingId,
                                                       ShopsId = t.ShopsId,
                                                       BuildingName = t.BuildingName,
                                                       ShopsName = t.ShopsName,
                                                       UserTrueName = cu2.TrueName,
                                                       DepartmentId = t.DepartmentId,
                                                       UserPhone = cu2.PhoneNumber,
                                                       CustomerName = t.CustomerName,
                                                       MainPhone = t.MainPhone,
                                                       Source = t.Source,
                                                       ReportTime = t.ReportTime,
                                                       BeltLookId = t.BeltLookId,
                                                       BeltLookTime = t.BeltLookTime,
                                                       ExpectedBeltTime = t.ExpectedBeltTime,
                                                       CustomerTransactionsFollowUps = from fu in Context.CustomerTransactionsFollowUps.AsNoTracking()
                                                                                       where fu.CustomerTransactionsId == t.Id && !fu.IsDeleted
                                                                                       orderby fu.CreateTime descending
                                                                                       select new CustomerTransactionsFollowUp
                                                                                       {
                                                                                           Id = fu.Id,
                                                                                           Contents = fu.Contents,
                                                                                           IsDeleted = fu.IsDeleted,
                                                                                           CustomerTransactionsId = fu.CustomerTransactionsId,
                                                                                           MarkTime = fu.MarkTime,
                                                                                           CreateTime = fu.CreateTime,
                                                                                           TransactionsStatus = fu.TransactionsStatus
                                                                                       }
                                                   },
                            CustomerPhones = from phone in Context.CustomerPhones.AsNoTracking()
                                             where phone.CustomerId == b.Id && !phone.IsDeleted
                                             orderby phone.IsMain
                                             select new CustomerPhone
                                             {
                                                 Id = phone.Id,
                                                 IsMain = phone.IsMain,
                                                 IsDeleted = phone.IsDeleted,
                                                 CustomerId = phone.CustomerId,
                                                 Phone = phone.Phone
                                             },
                            //CustomerReport = from report in Context.CustomerReports.AsNoTracking()
                            //                 where !report.IsDeleted && report.CustomerId == b.Id
                            //                 orderby report.ReportTime descending
                            //                 select new CustomerReport
                            //                 {
                            //                     Id = report.Id,
                            //                     CustomerId = report.CustomerId,
                            //                     IsDeleted = report.IsDeleted,
                            //                     UserId = report.UserId,
                            //                     BuildingId = report.BuildingId,
                            //                     ReportTime = report.ReportTime,
                            //                     ReportStatus = report.ReportStatus,
                            //                     ShopsId = report.ShopsId,
                            //                     BuildingName = report.BuildingName,
                            //                     ShopsName = report.ShopsName
                            //                 },
                            BeltLook = from beltlook in Context.BeltLooks.AsNoTracking()
                                       where !beltlook.IsDeleted && beltlook.CustomerId == b.Id
                                       orderby beltlook.BeltTime descending
                                       select new BeltLook
                                       {
                                           Id = beltlook.Id,
                                           CustomerId = beltlook.CustomerId,
                                           IsDeleted = beltlook.IsDeleted,
                                           UserId = beltlook.UserId
                                       },
                            CustomerFileInfos = from f1 in Context.FileInfos.AsNoTracking()
                                                join file in Context.CustomerFileScopes.AsNoTracking() on f1.FileGuid equals file.FileGuid
                                                where !file.IsDeleted && file.CustomerId == b.Id
                                                orderby file.CreateTime descending
                                                select new CustomerDealFileInfo
                                                {
                                                    FileGuid = f1.FileGuid,
                                                    IsDeleted = f1.IsDeleted,
                                                    FileExt = f1.FileExt,
                                                    Ext1 = f1.Ext1,
                                                    Ext2 = f1.Ext2,
                                                    Height = f1.Height,
                                                    Name = f1.Name,
                                                    Size = f1.Size,
                                                    Summary = f1.Summary,
                                                    Type = f1.Type,
                                                    Uri = f1.Uri,
                                                    Width = f1.Width
                                                },

                        };

            return query;
        }

        public async Task<List<T1>> RecommendFromBuilding<T1, T2>(string saleman, BuildingRecommendRequest condition, Func<T1, T2, T1> map)
        {
            string tempTableSql = @"CREATE TEMPORARY TABLE `temp_shops` (
  `Id` varchar(65) DEFAULT NULL,
  `BuildingArea` double DEFAULT NULL,
  `Depth` double DEFAULT NULL,
  `FloorNo` varchar(255) DEFAULT NULL,
  `Floors` int(11) DEFAULT NULL,
  `HasStreet` bit(1) DEFAULT NULL,
  `Height` double DEFAULT NULL,
  `HouseArea` double DEFAULT NULL,
  `IsCorner` bit(1) DEFAULT NULL,
  `IsFaceStreet` bit(1) DEFAULT NULL,
  `OutsideArea` double DEFAULT NULL,
  `Price` decimal(65,4) DEFAULT NULL,
  `GuidingPrice` decimal(65,4) DEFAULT NULL,
  `ShopCategory` varchar(255) DEFAULT NULL,
  `StreetDistance` double DEFAULT NULL,
  `TotalPrice` decimal(65,4) DEFAULT NULL,
  `Toward` varchar(255) DEFAULT NULL,
  `Width` double DEFAULT NULL,
	`DeliveryDate` datetime DEFAULT NULL
);";
            //插入一条空条件，可以查出没有填写核心意向的客户

            var dbConn = Context.Database.GetDbConnection();
            await dbConn.OpenAsync();
            var count = await dbConn.ExecuteAsync(tempTableSql);
            string insertSql = @"INSERT INTO TEMP_SHOPS(Id,BuildingArea, Depth, FloorNo, Floors, HasStreet, Height, HouseArea, IsCorner, IsFaceStreet, OutsideArea, Price, GuidingPrice, ShopCategory, StreetDistance, TotalPrice, Toward, Width, DeliveryDate)
            Values(NULL,NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL);";
            count = await dbConn.ExecuteAsync(insertSql, null, null);

            //插入商铺条件列表
            if (condition.Shops != null && condition.Shops.Count > 0)
            {

                await dbConn.ExecuteAsync(@"INSERT TEMP_SHOPS(Id,BuildingArea, Depth, FloorNo, Floors, HasStreet, Height, HouseArea, IsCorner, IsFaceStreet, OutsideArea, Price, GuidingPrice, ShopCategory, StreetDistance, TotalPrice, Toward, Width, DeliveryDate)
                    Values(@Id, @BuildingArea, @Depth, @FloorNo, @Floors, @HasStreet, @Height, @HouseArea, @IsCorner, @IsFaceStreet, @OutsideArea, @Price, @GuidingPrice, @ShopCategory, @StreetDistance, @TotalPrice, @Toward, @Width, @DeliveryDate)", condition.Shops, null);
            }

            count = await dbConn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM TEMP_SHOPS", null, null);

            string notCareShopsSql = "";
            string notCareProjectSql = "";
            if (condition.NotCareItems != null && condition.NotCareItems.Count(x => (x.Type ?? 0) == 0) > 0)
            {
                //创建不关心商铺临时表
                string notCareSql = @"CREATE TEMPORARY TABLE `temp_notcare_shops` (
	`ShopsId` VARCHAR (127) NOT NULL,
	`CustomerId` VARCHAR (127) NOT NULL,
	`UserId` VARCHAR (127) DEFAULT NULL,
	`CreateDate` datetime DEFAULT NULL
);";
                await dbConn.ExecuteAsync(notCareSql, null, null);

                //插入数据
                await dbConn.ExecuteAsync(@"INSERT INTO `temp_notcare_shops`(ShopsId,CustomerId, UserId, CreateDate) 
	VALUES (@ShopsId,@CustomerId, @UserId, @CreateDate)", condition.NotCareItems.Where(x => (x.Type ?? 0) == 0).ToList(), null);

                notCareShopsSql = @" AND NOT EXISTS (
					SELECT
						'X'
					FROM
						temp_notcare_shops f
					WHERE
						f.ShopsId = c.id
					AND f.customerid = a.id
				)";
            }

            //不管新的楼盘
            if (condition.NotCareItems != null && condition.NotCareItems.Count(x => (x.Type ?? 0) == 1) > 0)
            {
                //创建不关心商铺临时表
                string notCareSql = @"CREATE TEMPORARY TABLE `temp_notcare_projects` (
	`ShopsId` VARCHAR (127) NOT NULL,
	`CustomerId` VARCHAR (127) NOT NULL,
	`UserId` VARCHAR (127) DEFAULT NULL,
	`CreateDate` datetime DEFAULT NULL
);";
                await dbConn.ExecuteAsync(notCareSql, null, null);

                //插入数据
                await dbConn.ExecuteAsync(@"INSERT INTO `temp_notcare_projects`(ShopsId,CustomerId, UserId, CreateDate) 
	VALUES (@ShopsId,@CustomerId, @UserId, @CreateDate)", condition.NotCareItems.Where(x => (x.Type ?? 0) == 1).ToList(), null);

                notCareProjectSql = $@" AND NOT EXISTS (
	SELECT
		'X'
	FROM
		temp_notcare_projects e
	WHERE
		e.ShopsId = '{condition.BuildingId ?? ""}'
	AND ifnull(e.type, 0) = 1
	AND e.customerid = a.id
)";
            }

            try
            {


                //   using (var t = dbConn.BeginTransaction())
                //    {


                string sql = $@"SELECT
	a.*, b.*
FROM
	xyh_ky_customerinfo a
LEFT JOIN xyh_ky_customerdemand b ON a.id = b.CustomerId
WHERE
	a.IsDeleted = 0
AND a.CustomerStatus = 1
AND a.UserId = '{saleman}'
AND (
	(
		ifnull(b.CityId, '') = '{condition.City ?? ""}' || ifnull(b.CityId, '') = ''
	)
	AND (
		ifnull(b.DistrictId, '') = '{condition.District ?? ""}' || ifnull(b.DistrictId, '') = ''
	)
	AND (
		ifnull(b.AreaId, '') ='{condition.Area ?? ""}' || ifnull(b.AreaId, '') = ''
	)
)
AND (
	EXISTS (
		SELECT
			'x'
		FROM
			xyh_ky_relationhouse d
		WHERE
			a.Id = d.customerid
		AND d.housingresourcesid = '{condition.BuildingId ?? ""}'
	)
	OR EXISTS (
		SELECT
			'x'
		FROM
			temp_shops c
		WHERE
			(
				(
					(ifnull(b.PriceEnd, 0) * 10000) = 0 || (ifnull(b.PriceEnd, 0) * 10000) >=ifnull( c.GuidingPrice, ifnull(c.TotalPrice,0))
				) && (
					(
						ifnull(b.PriceStart, 0) * 10000
					) = 0 || (
						ifnull(b.PriceStart, 0) * 10000
					) <= ifnull( c.GuidingPrice, ifnull(c.TotalPrice,0))
				) && (
					ifnull(b.AcreageEnd, 0) = 0 || ifnull(b.AcreageEnd, 0) >= c.BuildingArea
				) && (
					ifnull(b.AcreageStart, 0) = 0 || ifnull(b.AcreageStart, 0) <= c.BuildingArea
				) && (
					ifnull(b.RequirementType, 1) = 1 || (
						b.RequirementType = 2 && c.DeliveryDate <= NOW()
					) || (
						b.RequirementType = 3 && c.DeliveryDate >= NOW()
					)
				)
                {notCareShopsSql}
			)
	)
) {notCareProjectSql}";

                logger.Info("推荐客户查询SQL: \r\n{0}", sql);

                var q = await dbConn.QueryAsync<T1, T2, T1>(sql, map, null, null);


                var list = q.ToList();


                return list;
                //   }
            }
            catch
            {
                if (dbConn != null && dbConn.State == System.Data.ConnectionState.Open)
                {
                    dbConn.Close();
                }
                throw;
            }



        }

        /// <summary>
        /// 获取所有客源跟进信息（重客）
        /// </summary>
        /// <returns></returns>
        public IQueryable<CustomerInfo> CustomerInfoFowllowUp()
        {
            var query = from b in Context.CustomerInfos.AsNoTracking()
                        join basic1 in Context.CustomerDemands.AsNoTracking() on b.Id equals basic1.CustomerId into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //所属人及部门
                        join cu in Context.Users.AsNoTracking() on b.UserId equals cu.Id into cu1
                        from cu2 in cu1.DefaultIfEmpty()
                        join ru1a in Context.Organizations.AsNoTracking() on b.DepartmentId equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()


                        select new CustomerInfo()
                        {
                            Id = b.Id,
                            UserName = cu2.TrueName,
                            DepartmentId = b.DepartmentId,
                            DepartmentName = ru1.OrganizationName,
                            CustomerDemand = basic,
                            Mark = b.Mark,
                            CustomerName = b.CustomerName,
                            UserId = b.UserId,
                            MainPhone = b.MainPhone,
                            IsDeleted = b.IsDeleted,
                            CreateTime = b.CreateTime,
                            FollowupTime = b.FollowupTime,
                            CustomerStatus = b.CustomerStatus,
                            UserPhone = cu2.PhoneNumber,
                            UserNumber = cu2.UserName,
                            WeChat = b.WeChat,
                            QQ = b.QQ,
                            Email = b.Email,
                            Source = b.Source,
                            Birthday = b.Birthday,
                            Sex = b.Sex,
                            RateProgress = b.RateProgress,
                            IsSellIntention = b.IsSellIntention,
                            UniqueId = b.UniqueId,
                            HeadImg = b.HeadImg,
                            SourceId = b.SourceId,
                            CustomerFollowUp = from CustomerFollowUp in Context.CustomerFollowUps.AsNoTracking()
                                               where !CustomerFollowUp.IsDeleted && CustomerFollowUp.CustomerId == b.Id
                                               orderby CustomerFollowUp.FollowUpTime descending
                                               select new CustomerFollowUp
                                               {
                                                   Id = CustomerFollowUp.Id,
                                                   TypeId = CustomerFollowUp.TypeId,
                                                   TrueName = CustomerFollowUp.TrueName,
                                                   FollowUpTime = CustomerFollowUp.FollowUpTime,
                                                   FollowUpContents = CustomerFollowUp.FollowUpContents,
                                                   CustomerId = CustomerFollowUp.CustomerId,
                                                   IsDeleted = CustomerFollowUp.IsDeleted,
                                                   DemandLevel = CustomerFollowUp.DemandLevel,
                                                   Importance = CustomerFollowUp.Importance,
                                                   FollowMode = CustomerFollowUp.FollowMode,
                                                   UserTrueName = cu2.TrueName,
                                                   DepartmentId = CustomerFollowUp.DepartmentId
                                               }
                        };

            return query;
        }

        /// <summary>
        /// 获取意向楼盘中的客源信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<RelationHouse> CustomerInfoInRelationAll()
        {
            var query = from cp in Context.RelationHouses.AsNoTracking()

                        join b in Context.CustomerInfos.AsNoTracking() on cp.CustomerId equals b.Id
                        join basic1 in Context.CustomerDemands.AsNoTracking() on b.Id equals basic1.CustomerId into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //所属人及部门
                        join cu1 in Context.Users.AsNoTracking() on b.UserId equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()
                        join ru1a in Context.Organizations.AsNoTracking() on b.DepartmentId equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()

                        where !b.IsDeleted
                        select new RelationHouse()
                        {
                            Id = cp.Id,
                            PropertyName = cp.PropertyName,
                            RoomNo = cp.RoomNo,
                            ShopNumber = cp.ShopNumber,
                            AreaFullName = cp.AreaFullName,
                            Remark = cp.Remark,
                            HousingResourcesId = cp.HousingResourcesId,
                            HousingResourcesNo = cp.HousingResourcesNo,
                            CustomerId = cp.CustomerId,
                            CustomerName = b.CustomerName,
                            MainPhone = b.MainPhone,
                            IsDeleted = cp.IsDeleted,
                            UserId = b.UserId,
                            CreateTime = cp.CreateTime
                        };
            return query;
        }

        /// <summary>
        /// 获取所有在公客池中的客源信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<CustomerInfo> CustomerInfoInPoolAll()
        {
            var query = from cp in Context.CustomerPools.AsNoTracking()

                        join b in Context.CustomerInfos.AsNoTracking() on cp.CustomerId equals b.Id
                        join basic1 in Context.CustomerDemands.AsNoTracking() on b.Id equals basic1.CustomerId into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //所属人及部门
                        join cu1 in Context.Users.AsNoTracking() on b.UserId equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()
                        join ru1a in Context.Organizations.AsNoTracking() on b.DepartmentId equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()

                        where !cp.IsDeleted && !b.IsDeleted

                        select new CustomerInfo()
                        {
                            Id = b.Id,
                            UserName = cu.TrueName,
                            DepartmentId = cp.DepartmentId,
                            DepartmentName = ru1.OrganizationName,
                            CustomerDemand = basic,
                            Mark = b.Mark,
                            CustomerName = b.CustomerName,
                            UserId = b.UserId,
                            MainPhone = b.MainPhone,
                            IsDeleted = b.IsDeleted,
                            CreateTime = b.CreateTime,
                            FollowupTime = b.FollowupTime,
                            CustomerStatus = b.CustomerStatus,
                            UserPhone = cu.PhoneNumber,
                            UserNumber = cu.UserName,
                            WeChat = b.WeChat,
                            QQ = b.QQ,
                            Email = b.Email,
                            Source = b.Source,
                            Birthday = b.Birthday,
                            Sex = b.Sex,
                            CustomerPool = cp,
                            RateProgress = b.RateProgress
                        };
            return query;
        }

        /// <summary>
        /// 获取所有在成交信息中的客源信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<CustomerInfo> CustomerInfoInDealAll()
        {
            var query = from cp in Context.CustomerDeals.AsNoTracking()

                        join b in Context.CustomerInfos.AsNoTracking() on cp.Customer equals b.Id
                        join basic1 in Context.CustomerDemands.AsNoTracking() on b.Id equals basic1.CustomerId into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //所属人及部门
                        join cu1 in Context.Users.AsNoTracking() on b.UserId equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()
                        join ru1a in Context.Organizations.AsNoTracking() on b.DepartmentId equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()

                        where !cp.IsDeleted && !b.IsDeleted && cp.ExamineStatus == (int)ExamineStatusEnum.Approved

                        select new CustomerInfo()
                        {
                            Id = b.Id,
                            UserName = cu.TrueName,
                            DepartmentId = b.DepartmentId,
                            DepartmentName = ru1.OrganizationName,
                            CustomerDemand = basic,
                            Mark = b.Mark,
                            CustomerName = b.CustomerName,
                            UserId = b.UserId,
                            MainPhone = b.MainPhone,
                            IsDeleted = b.IsDeleted,
                            CreateTime = b.CreateTime,
                            FollowupTime = b.FollowupTime,
                            CustomerStatus = b.CustomerStatus,
                            UserPhone = cu.PhoneNumber,
                            UserNumber = cu.UserName,
                            WeChat = b.WeChat,
                            QQ = b.QQ,
                            Email = b.Email,
                            Source = b.Source,
                            Birthday = b.Birthday,
                            Sex = b.Sex,
                            CustomerDeal = cp,
                            RateProgress = b.RateProgress
                        };
            return query;
        }

        /// <summary>
        /// 获取所有在无效客源信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<CustomerInfo> CustomerInfoInLossAll()
        {
            var query = from cp in Context.CustomerLosss.AsNoTracking()
                        join b in Context.CustomerInfos.AsNoTracking() on cp.CustomerId equals b.Id
                        join basic1 in Context.CustomerDemands.AsNoTracking() on b.Id equals basic1.CustomerId into basic2
                        from basic in basic2.DefaultIfEmpty()

                            //所属人及部门
                        join cu1 in Context.Users.AsNoTracking() on b.UserId equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()
                        join ru1a in Context.Organizations.AsNoTracking() on b.DepartmentId equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()

                        where !cp.IsDeleted && !b.IsDeleted

                        orderby cp.LossTime descending

                        select new CustomerInfo()
                        {
                            Id = b.Id,
                            UserName = cu.TrueName,
                            DepartmentId = b.DepartmentId,
                            DepartmentName = ru1.OrganizationName,
                            CustomerDemand = basic,
                            Mark = b.Mark,
                            CustomerName = b.CustomerName,
                            UserId = b.UserId,
                            MainPhone = b.MainPhone,
                            IsDeleted = b.IsDeleted,
                            CreateTime = b.CreateTime,
                            FollowupTime = b.FollowupTime,
                            CustomerStatus = b.CustomerStatus,
                            UserPhone = cu.PhoneNumber,
                            UserNumber = cu.UserName,
                            WeChat = b.WeChat,
                            QQ = b.QQ,
                            Email = b.Email,
                            Source = b.Source,
                            Birthday = b.Birthday,
                            Sex = b.Sex,
                            CustomerLoss = cp,
                            RateProgress = b.RateProgress
                        };
            return query;
        }

        /// <summary>
        /// 获取所有部门信息
        /// </summary>
        /// <returns></returns>
        public IQueryable<Organizations> OrganizationsAll()
        {
            return Context.Organizations.AsNoTracking();
        }

        /// <summary>
        /// 根据某一成员获取一条客源信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerInfos.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表客源信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerInfos.AsNoTracking().OrderByDescending(x => x.FollowupTime).ThenByDescending(x => x.CreateTime)).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增客源信息
        /// </summary>
        /// <param name="customerInfo">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerInfo> CreateAsync(CustomerInfo customerInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerInfo == null)
            {
                throw new ArgumentNullException(nameof(customerInfo));
            }
            if (customerInfo != null)
            {
                Context.Add(customerInfo);
            }
            if (customerInfo.CustomerDemand != null)
            {
                Context.Add(customerInfo.CustomerDemand);
            }
            if (customerInfo.CustomerPhones != null)
            {
                Context.AddRange(customerInfo.CustomerPhones);
            }
            if (customerInfo.HousingResources != null)
            {
                Context.AddRange(customerInfo.HousingResources);
            }
            await Context.SaveChangesAsync(cancellationToken);
            return customerInfo;
        }

        /// <summary>
        /// 删除客源
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customerInfo">客源实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(UserInfo user, CustomerInfo customerInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (customerInfo == null)
            {
                throw new ArgumentNullException(nameof(customerInfo));
            }
            //删除基本信息
            customerInfo.DeleteTime = DateTime.Now;
            customerInfo.DeleteUser = user.Id;
            customerInfo.IsDeleted = true;
            Context.Attach(customerInfo);
            var entry = Context.Entry(customerInfo);
            entry.Property(x => x.IsDeleted).IsModified = true;
            entry.Property(x => x.DeleteUser).IsModified = true;
            entry.Property(x => x.DeleteTime).IsModified = true;

            //删除需求信息
            var customerdemand = Context.CustomerDemands.AsNoTracking().FirstOrDefault(x => x.CustomerId == customerInfo.Id);
            customerdemand.DeleteTime = DateTime.Now;
            customerdemand.DeleteUser = user.Id;
            customerdemand.IsDeleted = true;
            Context.Attach(customerdemand);
            var entryDemand = Context.Entry(customerdemand);
            entryDemand.Property(x => x.IsDeleted).IsModified = true;
            entryDemand.Property(x => x.DeleteUser).IsModified = true;
            entryDemand.Property(x => x.DeleteTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 移交
        /// </summary>
        /// <param name="customerInfoList">客源实体</param>
        /// <param name="customerdemandList"></param>
        /// <param name="customerTransactions"></param>
        /// <param name="customerTransactionsFollowUp"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HandOverAsync(List<CustomerInfo> customerInfoList,
            List<CustomerDemand> customerdemandList,
            List<CustomerTransactions> customerTransactions,
            List<CustomerTransactionsFollowUp> customerTransactionsFollowUp,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerInfoList == null)
            {
                throw new ArgumentNullException(nameof(customerInfoList));
            }
            if (customerdemandList == null)
            {
                throw new ArgumentNullException(nameof(customerdemandList));
            }
            //if (customerreportList == null)
            //{
            //    throw new ArgumentNullException(nameof(customerreportList));
            //}
            //if (aboutlookList == null)
            //{
            //    throw new ArgumentNullException(nameof(aboutlookList));
            //}
            //if (beltlookList == null)
            //{
            //    throw new ArgumentNullException(nameof(beltlookList));
            //}
            //if (customerfollowupList == null)
            //{
            //    throw new ArgumentNullException(nameof(customerfollowupList));
            //}

            //修改用户信息
            Context.AttachRange(customerInfoList);
            Context.UpdateRange(customerInfoList);

            //修改需求信息
            Context.AttachRange(customerdemandList);
            Context.UpdateRange(customerdemandList);

            //修改成交信息
            Context.AttachRange(customerTransactions);
            Context.UpdateRange(customerTransactions);

            //修改成交跟进信息
            Context.AttachRange(customerTransactionsFollowUp);
            Context.UpdateRange(customerTransactionsFollowUp);

            ////修改报备信息
            //Context.AttachRange(customerreportList);
            //Context.UpdateRange(customerreportList);

            ////修改约看信息
            //Context.AttachRange(aboutlookList);
            //Context.UpdateRange(aboutlookList);

            ////修改带看信息
            //Context.AttachRange(beltlookList);
            //Context.UpdateRange(beltlookList);

            ////修改跟进信息
            //Context.AttachRange(customerfollowupList);
            //Context.UpdateRange(customerfollowupList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除客源List
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customerInfoList">客源实体</param>
        /// <param name="newcustomerInfoList"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task HandOverAsync(UserInfo user,
            List<CustomerInfo> customerInfoList,
            List<CustomerInfo> newcustomerInfoList,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (customerInfoList == null)
            {
                throw new ArgumentNullException(nameof(customerInfoList));
            }
            customerInfoList = customerInfoList.Select(p =>
            {
                p.DeleteTime = DateTime.Now;
                p.DeleteUser = user.Id;
                p.IsDeleted = true;
                return p;
            }).ToList();
            //删除信息
            Context.AttachRange(customerInfoList);
            Context.UpdateRange(customerInfoList);

            //新增信息
            Context.AttachRange(newcustomerInfoList);
            Context.AddRange(newcustomerInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 从数据库中删除List
        /// </summary>
        /// <param name="customerInfoList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<CustomerInfo> customerInfoList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerInfoList == null)
            {
                throw new ArgumentNullException(nameof(customerInfoList));
            }
            Context.RemoveRange(customerInfoList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 修改客源信息
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(CustomerInfo customerInfo, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerInfo == null)
            {
                throw new ArgumentNullException(nameof(customerInfo));
            }
            Context.Attach(customerInfo);
            Context.Update(customerInfo);
            if (customerInfo.CustomerDemand != null)
            {
                Context.Attach(customerInfo.CustomerDemand);
                Context.Update(customerInfo.CustomerDemand);
            }
            if (customerInfo.CustomerPhones != null)
            {
                Context.AddRange(customerInfo.CustomerPhones);
            }
            if (customerInfo.CustomerPhones2 != null)
            {
                Context.AttachRange(customerInfo.CustomerPhones2);
                Context.UpdateRange(customerInfo.CustomerPhones2);
            }
            if (customerInfo.HousingResources != null)
            {
                Context.RemoveRange(Context.RelationHouses.Where(x => x.CustomerId == customerInfo.Id));

                Context.AddRange(customerInfo.HousingResources);
            }

            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改客源信息(一般修改删除状态)
        /// </summary>
        /// <param name="customerInfos"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<CustomerInfo> customerInfos, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerInfos == null)
            {
                throw new ArgumentNullException(nameof(customerInfos));
            }
            Context.AttachRange(customerInfos);
            Context.UpdateRange(customerInfos);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 调离
        /// </summary>
        /// <param name="customerInfos"></param>
        /// <param name="customerFollowUps"></param>
        /// <param name="customerTransactions"></param>
        /// <param name="beltLook"></param>
        /// <param name="customerDemands"></param>
        /// <param name="customertranfus"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task TransferringListAsync(List<CustomerInfo> customerInfos, List<CustomerFollowUp> customerFollowUps, List<CustomerFollowUp> newcustomerFollowUps, List<CustomerTransactions> customerTransactions, List<CustomerTransactionsFollowUp> customertranfus, List<BeltLook> beltLook, List<CustomerDemand> customerDemands, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerInfos == null)
            {
                throw new ArgumentNullException(nameof(customerInfos));
            }
            try
            {
                using (var trans = await Context.Database.BeginTransactionAsync())
                {
                    Context.AttachRange(customerInfos);
                    Context.UpdateRange(customerInfos);
                    await Context.SaveChangesAsync(cancellationToken);

                    if (customerFollowUps?.Count > 0)
                    {
                        Context.AttachRange(customerFollowUps);
                        Context.UpdateRange(customerFollowUps);
                    }
                    await Context.SaveChangesAsync(cancellationToken);
                    if (newcustomerFollowUps?.Count > 0)
                    {
                        Context.AddRange(newcustomerFollowUps);
                    }
                    await Context.SaveChangesAsync(cancellationToken);
                    if (customerTransactions?.Count > 0)
                    {
                        Context.AttachRange(customerTransactions);
                        Context.UpdateRange(customerTransactions);
                    }
                    await Context.SaveChangesAsync(cancellationToken);
                    if (customertranfus?.Count > 0)
                    {
                        Context.AttachRange(customertranfus);
                        Context.UpdateRange(customertranfus);
                    }
                    await Context.SaveChangesAsync(cancellationToken);
                    if (beltLook?.Count > 0)
                    {
                        Context.AttachRange(beltLook);
                        Context.UpdateRange(beltLook);
                    }
                    await Context.SaveChangesAsync(cancellationToken);
                    if (customerDemands?.Count > 0)
                    {
                        Context.AttachRange(customerDemands);
                        Context.UpdateRange(customerDemands);
                    }
                    await Context.SaveChangesAsync(cancellationToken);

                    trans.Commit();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 新增查看电话历史
        /// </summary>
        /// <param name="chekPhoneHistory">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<ChekPhoneHistory> CreateCheckPhone(ChekPhoneHistory chekPhoneHistory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (chekPhoneHistory == null)
            {
                throw new ArgumentNullException(nameof(chekPhoneHistory));
            }
            Context.Add(chekPhoneHistory);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
            return chekPhoneHistory;
        }
    }
}
