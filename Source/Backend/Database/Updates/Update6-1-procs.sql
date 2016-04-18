use [StoreKeeper]

--------------------
---- Procedures ----
--------------------

if exists (select * from sysobjects where id = object_id('MaxOf2'))
begin
    drop function MaxOf2
end
go

create function MaxOf2
(
	@val1 float,
	@val2 float
)
returns float
as
begin
	if @val1 > @val2
	begin
		return @val1
	end
	return @val2
end
go

grant exec on MaxOf2 to public
go
print 'SF: MaxOf2 added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('LogDataJournal'))
begin
    drop procedure LogDataJournal
end
go

create procedure LogDataJournal
(
	@UserId char(36),
	@Code nvarchar(50)
)
as
	set nocount on

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @username nvarchar(50)
	select @username = Name from Users where Id = @UserId

	insert into DataJournal (Id, [User], Code, StampTime) values (NEWID(), @username, @Code, GETDATE())

	delete from DataJournal where DATEDIFF(day, StampTime, GETDATE()) >= 30

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on LogDataJournal to public
go
print 'SP: LogDataJournal added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('LockDatabase'))
begin
    drop procedure LockDatabase
end
go

create procedure LockDatabase
(
	@UserId char(36),
	@Unlock bit
)
as
	set nocount on

	declare @usId nvarchar(200)
	select @usId = Value from SystemInformations where Name = 'LockedBy'

	declare @lockedBy nvarchar(50)
	set @lockedBy = ''

	declare @username nvarchar(50)
	select @username = Name from Users where Id = @UserId

	if @usId is not null and @usId != ''
	begin
		select @lockedBy = Name from Users where Id = @usId
	end

	declare @err nvarchar(500)
	set @err = ''

	if @Unlock = 1
	begin
		if @usId is null or @usId = ''
		begin
			raiserror ('Database is not locked!', 16, 1)
			return
		end

		if @usId is not null and @usId != '' and @usId != @UserId
		begin
			set @err = 'Database is not locked by ' + @username + '!'
			raiserror (@err, 16, 1)
			return
		end
	end
	else
	begin
		if @usId is not null and @usId != ''
		begin
			if @usId != @UserId
			begin
				set @err = 'Database is already locked by ' + @lockedBy + '!'
				raiserror (@err, 16, 1)
				return
			end
			else
			begin
				return
			end
		end
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @operation nvarchar(20)
	if @Unlock = 1
	begin
		set @operation = 'ReleaseLock'
		update SystemInformations set Value = '' where Name = 'LockedBy'
	end
	else
	begin
		set @operation = 'GetLock'
		update SystemInformations set Value = @UserId where Name = 'LockedBy'
	end

	exec LogDataJournal @UserId, @operation

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on LockDatabase to public
go
print 'SP: LockDatabase added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('AssureLock'))
begin
    drop function AssureLock
end
go

create function AssureLock()
returns bit
as
begin

	declare @usId nvarchar(200)
	select @usId = Value from SystemInformations where Name = 'LockedBy'

	if @usId is not null and @usId != ''
	begin
		return 1
	end

	return 0
end
go

grant exec on AssureLock to public
go
print 'SF: AssureLock added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('ResetArticleStats'))
begin
    drop procedure ResetArticleStats
end
go

create procedure ResetArticleStats
(
	@ArticleId uniqueidentifier,
	@Reload bit
)
as
	set nocount on

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @originalCount float
	declare @externStorageCount float
	declare @lossCount float
	
	select @originalCount = OriginalCount from Articles where Id = @ArticleId

	if @Reload = 1
	begin
		select @externStorageCount = sum(ProductCount), @lossCount = sum(LossCount) from ExternStorageStats where ArticleId = @ArticleId and UserId is null
		set @externStorageCount = coalesce(@externStorageCount, 0)
		set @lossCount = coalesce(@lossCount, 0)

		update ArticleStats set ExternStorageCount = @externStorageCount, CurrentCount = @originalCount - @externStorageCount - @lossCount where ArticleId = @ArticleId and UserId is null
	end

	
	declare @extUserId char(36)
	select distinct top 1 @extUserId = UserId from ExternStorageStats where ArticleId = @ArticleId and UserId is not null

	declare @statUserId char(36)
	select distinct top 1 @statUserId = UserId from ArticleStats where ArticleId = @ArticleId and UserId is not null
	
	declare @otherUserId char(36)
	set @otherUserId = coalesce(@extUserId, @statUserId, null)

	if @otherUserId is not null
	begin
		select 
			@externStorageCount = sum(ProductCount),
			@lossCount = sum(LossCount)
		from ExternStorageStats
		where
			ArticleId = @ArticleId
			and (
				(UserId is null and not exists (select 1 from ExternStorageStats es where es.ArticleId = ExternStorageStats.ArticleId and UserId = @otherUserId))
				or
				UserId = @otherUserId
			)

		set @externStorageCount = coalesce(@externStorageCount, 0)
		set @lossCount = coalesce(@lossCount, 0)

		if not exists (select 1 from ArticleStats where ArticleId = @ArticleId and UserId = @otherUserId)
		begin
			insert into ArticleStats (Id, ArticleId, CurrentCount, MissingInOrders, ProductCount, ExternStorageCount, UserId)
			select NEWID(), @ArticleId, @originalCount - @externStorageCount - @lossCount, MissingInOrders, ProductCount, @externStorageCount, @otherUserId from ArticleStats where ArticleId = @ArticleId and UserId is null
		end
		else
		begin
			update ArticleStats set ExternStorageCount = @externStorageCount, CurrentCount = @originalCount - @externStorageCount - @lossCount where ArticleId = @ArticleId and UserId = @otherUserId
		end
	end

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on ResetArticleStats to public
go
print 'SP: ResetArticleStats added'
go

-----------------------------------


if exists (select * from sysobjects where id = object_id('PublishData'))
begin
    drop procedure PublishData
end
go

create procedure PublishData
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @modAS int
	declare @modES int

	delete from ArticleStats where UserId is null and exists (select 1 from ArticleStats n where n.ArticleId = ArticleStats.ArticleId and n.UserId = @userId)
	update ArticleStats set UserId = null where UserId = @userId
	set @modAS = @@ROWCOUNT

	delete from ExternStorageStats where UserId is null and exists (select 1 from ExternStorageStats n where n.ArticleId = ExternStorageStats.ArticleId and n.StorageId = ExternStorageStats.StorageId and n.UserId = @userId)
	update ExternStorageStats set UserId = null where UserId = @userId
	set @modES = @@ROWCOUNT

	update ProductArticleOrders set UserId = null where UserId = @userId
	
	if @modAS > 0 or @modES > 0
	begin
		delete from ArticleOrders where UserId is null
		delete from ProductArticleReservations where UserId is null
	end
	
	update ArticleOrders set UserId = null where UserId = @userId

	update ProductArticleReservations set UserId = null where UserId = @userId

	update ArticleTransfers set UserId = null where UserId = @userId

	update ArticleTransferIdentifiers set UserId = null where UserId = @userId

	update ProductArticleOrderHistories set UserId = null where UserId = @userId

	delete from ProductArticleReservations where ProductArticleOrderId in (select ProductArticleOrderId from ProductOrderCompletions where UserId = @userId)
	delete from ProductArticleOrders where Id in (select ProductArticleOrderId from ProductOrderCompletions where UserId = @userId)
	delete from ProductOrderCompletions where UserId = @userId

	exec LogDataJournal @userId, 'DataPublishing'
	update SystemInformations set Value = (select Name from Users where Id = @userId) where Name = 'ResponsibleUser'

	exec LockDatabase @userId, 1

	if @useTransaction = 1
	begin
		commit
	end
return
go

grant exec on PublishData to public
go
print 'SP: PublishData added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('RefreshAllPriorities'))
begin
    drop procedure RefreshAllPriorities
end
go

create procedure RefreshAllPriorities
(
	@UserId char(36)
)
as
	set nocount on

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare poCur cursor for
	select Id from ProductArticleOrders
	where
		not exists (select 1 from ProductOrderCompletions where ProductArticleOrderId = ProductArticleOrders.Id)
		and (
			UserId is null
			or
			UserId = @UserId
		)
	order by [Priority]

	declare @poId char(36)

	open poCur
	fetch next from poCur into @poId

	declare @current integer
	set @current = 1

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			update ProductArticleOrders set [Priority] = @current where Id = @poId
			set @current = @current + 1
		end
		fetch next from poCur into @poId
	end

	close poCur
	deallocate poCur

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on RefreshAllPriorities to public
go
print 'SP: RefreshAllPriorities added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('PrepareCalculation'))
begin
    drop procedure PrepareCalculation
end
go

create procedure PrepareCalculation
(
	@UserId char(36)
)
as
	set nocount on

	if @UserId is null or not exists (select 1 from Users where Id = @UserId)
	begin
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	delete from ProductArticleReservations where UserId = @UserId
	delete from ArticleOrders where UserId = @UserId

	exec RefreshAllPriorities @UserId

	insert into ArticleStats (Id, ArticleId, CurrentCount, MissingInOrders, ProductCount, ExternStorageCount, UserId)
	select NEWID(), t.ArticleId, t.CurrentCount, 0, 0, t.ExternStorageCount, @UserId from 
	(
		select distinct ArticleStats.* 
		from 
			ProductArticleItems
			join ArticleStats on (ArticleStats.ArticleId = ProductArticleItems.ArticleId)
		where 
			ProductArticleId in (select distinct ProductArticleId from ProductArticleOrders)
			and UserId is null
			and not exists (select 1 from ArticleStats o where o.ArticleId = ArticleStats.ArticleId and o.UserId = @UserId)
	) t

	update ArticleStats set MissingInOrders = 0, ProductCount = 0 where exists (select 1 from ArticleStats o where o.ArticleId = ArticleStats.ArticleId and o.UserId = @UserId)

	insert into ExternStorageStats (Id, ArticleTransferIdentifierId, ArticleId, StorageId, ProductCount, MissingCount, LossCount, UserId)
	select NEWID(), t.ArticleTransferIdentifierId, t.ArticleId, t.StorageId, t.ProductCount, 0, t.LossCount, @UserId
	from
	(
		select distinct ExternStorageStats.* 
		from 
			ProductArticleItems
			join ExternStorageStats on (ExternStorageStats.ArticleId = ProductArticleItems.ArticleId and ExternStorageStats.StorageId = ProductArticleItems.StorageId)
		where 
			ProductArticleId in (select distinct ProductArticleId from ProductArticleOrders)
			and exists (select 1 from ExternStorageStats o where o.ArticleId = ExternStorageStats.ArticleId and o.StorageId = ExternStorageStats.StorageId and UserId is null)
			and ExternStorageStats.UserId is null
			and not exists (select 1 from ExternStorageStats o where o.ArticleId = ExternStorageStats.ArticleId and o.StorageId = ExternStorageStats.StorageId and UserId = @UserId)
	) t

	update ExternStorageStats set MissingCount = 0 where exists (select 1 from ExternStorageStats o where o.ArticleId = ExternStorageStats.ArticleId and o.StorageId = ExternStorageStats.StorageId and UserId = @UserId)

	delete from MaterialCache where UserId = @UserId

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on PrepareCalculation to public
go
print 'SP: PrepareCalculation added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CalculateMaterialCache'))
begin
    drop procedure CalculateMaterialCache
end
go

create procedure CalculateMaterialCache
(
	@ProductArticleId uniqueidentifier,
	@UserId char(36)
)
as
	set nocount on

	if @UserId is null or not exists (select 1 from Users where Id = @UserId)
	begin
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare itemCur cursor for
	select  
		pa.ArticleId,
		pa.StorageId,
		ars.CurrentCount
	from 
		ProductArticleItems pa
		join ArticleStats ars on (ars.ArticleId = pa.ArticleId and UserId = @UserId)
	where
		pa.ProductArticleId = @ProductArticleId
		and pa.SkipCalculation = 0

	declare @articleId uniqueidentifier
	declare @storageId uniqueidentifier
	declare @currentCount float

	open itemCur

	fetch next from itemCur into @articleId, @storageId, @currentCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			if @storageId != 'D67BFF48-E36F-4066-888B-23BA5E622FB0'
			begin
				declare @newCount float
				set @newCount = null

				select 
					top 1 @newCount = ProductCount 
				from ExternStorageStats 
				where ArticleId = @articleId and StorageId = @storageId and (UserId is null or UserId = @UserId)
				order by
					case when UserId = @UserId then 1 else 2 end

				if @newCount is not null
				begin
					set @currentCount = @newCount
				end
			end

			if exists (select 1 from MaterialCache where UserId = @UserId and ArticleId = @articleId)
			begin
				if not exists (select 1 from MaterialCache where UserId = @UserId and ArticleId = @articleId and StorageId = @storageId)
				begin
					insert into MaterialCache (UserId, ArticleId, StorageId, CurrentCount) values (@UserId, @articleId, @storageId, @currentCount)
				end
			end
			else
			begin
				insert into MaterialCache (UserId, ArticleId, StorageId, CurrentCount) values (@UserId, @articleId, @storageId, @currentCount)
			end
		end
		fetch next from itemCur into @articleId, @storageId, @currentCount
	end

	close itemCur
	deallocate itemCur

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on CalculateMaterialCache to public
go
print 'SP: CalculateMaterialCache added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('GetMaxProductionCount'))
begin
    drop function GetMaxProductionCount
end
go

create function GetMaxProductionCount
(
	@ProductArticleId uniqueidentifier,
	@UserId char(36)
)
returns float
as
begin
	declare itemCur cursor for
	select  
		ArticleId,
		StorageId,
		Quantity
	from 
		ProductArticleItems
	where
		ProductArticleId = @ProductArticleId
		and SkipCalculation = 0

	declare @articleId uniqueidentifier
	declare @storageId uniqueidentifier
	declare @quantity float

	declare @maxProductionCount float
	set @maxProductionCount = 1000000000

	open itemCur

	fetch next from itemCur into @articleId, @storageId, @quantity

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			if ABS(@quantity - 0) < 0.0001
			begin
				fetch next from itemCur into @articleId, @storageId, @quantity
				continue
			end

			declare @currentCount float
			declare @canProduct float
			
			select @currentCount = CurrentCount from MaterialCache where UserId = @UserId and ArticleId = @articleId and StorageId = @storageId
			set @canProduct = FLOOR(@currentCount / @quantity)
			if @canProduct < @maxProductionCount
			begin
				set @maxProductionCount = @canProduct
			end
		end
		fetch next from itemCur into @articleId, @storageId, @quantity
	end

	close itemCur
	deallocate itemCur

	return dbo.MaxOf2(@maxProductionCount, 0)
end
go

grant exec on GetMaxProductionCount to public
go
print 'SF: GetMaxProductionCount added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CreateReservations'))
begin
    drop procedure CreateReservations
end
go

create procedure CreateReservations
(
	@ProductOrderId uniqueidentifier,
	@UserId char(36)
)
as
	set nocount on

	if @UserId is null or not exists (select 1 from Users where Id = @UserId)
	begin
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @count float
	select @count = [Count] from ProductArticleOrders where Id = @ProductOrderId

	declare @itemId uniqueidentifier
	declare @articleId uniqueidentifier
	declare @storageId uniqueidentifier
	declare @quantity float
	declare @type int

	declare @currentCount float
	declare @reservationCount float
	declare @orderCount float
	declare @rest float

	declare itemCur cursor for
	select 
		pai.Id,
		pai.ArticleId,
		pai.StorageId,
		pai.Quantity,
		a.[Type]
	from
		ProductArticleItems pai
		join ProductArticleOrders po on (po.ProductArticleId = pai.ProductArticleId)
		join Articles a on (a.Id = pai.ArticleId)
	where
		po.Id = @ProductOrderId
		and pai.SkipCalculation = 0

	open itemCur

	fetch next from itemCur into @itemId, @articleId, @storageId, @quantity, @type

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			select @currentCount = CurrentCount from MaterialCache where UserId = @UserId and ArticleId = @articleId and StorageId = @storageId

			set @reservationCount = @quantity * @count
			set @orderCount = dbo.MaxOf2(@reservationCount - @currentCount, 0)
			set @rest = dbo.MaxOf2(@currentCount - @reservationCount, 0)

			insert into ProductArticleReservations (Id, ProductArticleOrderId, ProductArticleItemId, CurrentCount, ReservationCount, OrderCount, UserId)
			values (NEWID(), @ProductOrderId, @itemId, @currentCount, @reservationCount, @orderCount, @UserId)

			update MaterialCache set CurrentCount = @rest where UserId = @UserId and ArticleId = @articleId and StorageId = @storageId

			update ArticleStats set MissingInOrders = MissingInOrders + @orderCount where ArticleId = @articleId and UserId = @UserId

			if @orderCount > 0
			begin
				if (@type = 1 and not exists (select 1 from ArticleOrders where ArticleId = @articleId and UserId = @UserId))
				begin
					insert into ArticleOrders (Id, ArticleId, [Count], UserId) values (NEWID(), @articleId, 0, @UserId)
				end

				if (@storageId != 'D67BFF48-E36F-4066-888B-23BA5E622FB0')
				begin
					update ExternStorageStats set MissingCount = MissingCount + @orderCount where ArticleId = @articleId and StorageId = @storageId and UserId = @UserId
				end
			end
		end
		fetch next from itemCur into @itemId, @articleId, @storageId, @quantity, @type
	end

	close itemCur
	deallocate itemCur

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on CreateReservations to public
go
print 'SP: CreateReservations added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CalculateMaterialOrderCounts'))
begin
    drop procedure CalculateMaterialOrderCounts
end
go

create procedure CalculateMaterialOrderCounts
(
	@UserId char(36)
)
as
	set nocount on

	if @UserId is null or not exists (select 1 from Users where Id = @UserId)
	begin
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end
	
	declare @orderId uniqueidentifier
	declare @articleId uniqueidentifier
	declare @currentCount float

	declare orderCur cursor for
	select 
		ao.Id, ao.ArticleId, ass.CurrentCount
	from 
		ArticleOrders ao
		join ArticleStats ass on (ass.ArticleId = ao.ArticleId)
	where
		ao.UserId = @UserId
		and ass.UserId = @UserId

	open orderCur

	fetch next from orderCur into @orderId, @articleId, @currentCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			declare @sum1 float
			declare @sum2 float
			declare @totalSum float
			
			select 
				@sum1 = SUM(par.OrderCount) 
			from 
				ProductArticleReservations par
				join ProductArticleItems pai on (pai.Id = par.ProductArticleItemId)
			where 
				pai.ArticleId = @articleId
				and pai.StorageId = 'D67BFF48-E36F-4066-888B-23BA5E622FB0'
				and pai.SkipCalculation = 0
				and par.UserId = @UserId

			select @sum2 = SUM(MissingCount) from ExternStorageStats where ArticleId = @articleId and UserId = @UserId

			set @sum1 = coalesce(@sum1, 0)
			set @sum2 = coalesce(@sum2, 0)

			set @totalSum = @sum1
			if (@sum2 > @currentCount)
			begin
				set @totalSum = @totalSum + (@sum2 - @currentCount)
			end

			update ArticleOrders set [Count] = @totalSum where Id = @orderId
		end
		fetch next from orderCur into @orderId, @articleId, @currentCount
	end

	delete from ArticleOrders where UserId = @UserId and ABS([Count] - 0) < 0.0001

	close orderCur
	deallocate orderCur

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on CalculateMaterialOrderCounts to public
go
print 'SP: CalculateMaterialOrderCounts added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CalculateData'))
begin
    drop procedure CalculateData
end
go

create procedure CalculateData
(
	@Publish bit
)
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	exec PrepareCalculation @userId

	declare orderCur cursor for
	select 
		Id, 
		ProductArticleId, 
		[Count] 
	from ProductArticleOrders 
	where 
		not exists (select 1 from ProductOrderCompletions where ProductArticleOrderId = ProductArticleOrders.Id) 
	order by 
		[Priority],
		case when (OrderPeriod is null) then 1 else 2 end

	declare @orderId uniqueidentifier
	declare @productArticleId uniqueidentifier
	declare @orderCount float

	open orderCur

	fetch next from orderCur into @orderId, @productArticleId, @orderCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			exec CalculateMaterialCache @productArticleId, @userId
			update ProductArticleOrders set ProductionCount = dbo.GetMaxProductionCount(@productArticleId, @userId) where Id = @orderId

			exec CreateReservations @orderId, @userId

			update ArticleStats set ProductCount = ProductCount + 1 
			where 
				UserId = @userId
				and ArticleId in (
					select ArticleId from ProductArticleItems where ProductArticleId = @productArticleId
				)
		end
		fetch next from orderCur into @orderId, @productArticleId, @orderCount
	end

	close orderCur
	deallocate orderCur

	exec CalculateMaterialOrderCounts @userId

	delete from MaterialCache where UserId = @userId

	if @Publish = 1
	begin
		exec PublishData
	end

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on CalculateData to public
go
print 'SP: CalculateData added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('ResolveProductOrder'))
begin
    drop procedure ResolveProductOrder
end
go

create procedure ResolveProductOrder
(
	@OrderId uniqueidentifier,
	@Resolve bit
)
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	declare reservCur cursor for
	select 
		pai.ArticleId,
		pai.StorageId,
		r.ReservationCount
	from 
		ProductArticleReservations r
		join ProductArticleItems pai on (pai.Id = r.ProductArticleItemId)
	where
		r.ProductArticleOrderId = @OrderId

	declare @articleId uniqueidentifier
	declare @storageId uniqueidentifier
	declare @reservationCount float

	open reservCur

	fetch next from reservCur into @articleId, @storageId, @reservationCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			if @Resolve = 1 and @storageId != 'D67BFF48-E36F-4066-888B-23BA5E622FB0'
			begin
				if not exists (select 1 from ExternStorageStats where ArticleId = @articleId and StorageId = @storageId and UserId = @userId)
				begin
					insert into ExternStorageStats (Id, ArticleTransferIdentifierId, ArticleId, StorageId, ProductCount, MissingCount, LossCount, UserId)
					select NEWID(), t.ArticleTransferIdentifierId, t.ArticleId, t.StorageId, t.ProductCount, t.MissingCount, t.LossCount, @userId from ExternStorageStats t where t.ArticleId = @articleId and t.StorageId = @storageId and t.UserId is null
				end

				update ExternStorageStats set ProductCount = ProductCount - @reservationCount where ArticleId = @articleId and StorageId = @storageId and UserId = @userId

				exec ResetArticleStats @articleId, 0
			end
		end
		fetch next from reservCur into @articleId, @storageId, @reservationCount
	end

	close reservCur
	deallocate reservCur

	insert into ProductOrderCompletions (Id, ProductArticleOrderId, [Status], UserId) values (NEWID(), @OrderId, case when @Resolve = 1 then 1 else 2 end, @userId)

	if @Resolve = 1
	begin
		delete from ProductArticleOrderHistories where Id = @OrderId
		insert into ProductArticleOrderHistories (Id, ProductArticleId, [Count], [Priority], ProductionCount, OrderPeriod, PlannedPeriod, EndPeriod, StampTime, UserId)
		select Id, ProductArticleId, [Count], [Priority], ProductionCount, OrderPeriod, PlannedPeriod, EndPeriod, GETDATE(), UserId from ProductArticleOrders where Id = @OrderId
	end

	exec RefreshAllPriorities @userId

	select 
		@articleId = pa.ArticleId 
	from 
		ProductArticles pa
		join ProductArticleOrders po on (po.ProductArticleId = pa.Id)
	where
		po.Id = @OrderId

	exec ResetArticleStats @articleId, 0

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on ResolveProductOrder to public
go
print 'SP: ResolveProductOrder added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('TransferMaterial'))
begin
    drop procedure TransferMaterial
end
go

create procedure TransferMaterial
(
	@ExtStatId uniqueidentifier,
	@Count float,
	@RealTransfer bit
)
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	declare @transferId uniqueidentifier
	declare @articleId uniqueidentifier
	declare @storageId uniqueidentifier
	declare @productCount float

	select @transferId = ArticleTransferIdentifierId, @articleId = ArticleId, @storageId = StorageId, @productCount = ProductCount from ExternStorageStats where Id = @ExtStatId

	if not exists (select 1 from ArticleStats where ArticleId = @articleId and UserId = @userId)
	begin
		insert into ArticleStats (Id, ArticleId, CurrentCount, MissingInOrders, ProductCount, ExternStorageCount, UserId)
		select NEWID(), @articleId, t.CurrentCount, t.MissingInOrders, t.ProductCount, t.ExternStorageCount, @userId from ArticleStats t where t.ArticleId = @articleId and UserId is null
	end

	if not exists (select 1 from ExternStorageStats where ArticleId = @articleId and StorageId = @storageId and UserId = @userId)
	begin
		insert into ExternStorageStats (Id, ArticleTransferIdentifierId, ArticleId, StorageId, ProductCount, MissingCount, LossCount, UserId)
		select NEWID(), t.ArticleTransferIdentifierId, t.ArticleId, t.StorageId, t.ProductCount, t.MissingCount, t.LossCount, @userId from ExternStorageStats t where t.ArticleId = @articleId and t.StorageId = @storageId and t.UserId is null
	end

	if @RealTransfer = 1
	begin
		insert into ArticleTransfers (Id, ArticleTransferIdentifierId, SendingDate, ProductCount, LatestCount, TotalCount, [Status], UserId)
		values (NEWID(), @transferId, GETDATE(), @Count, @productCount, @productCount + @Count, 0, @userId)

		update ArticleStats set CurrentCount = CurrentCount - @Count where ArticleId = @articleId and UserId = @userId
		update ExternStorageStats set ProductCount = ProductCount + @Count where ArticleId = @articleId and StorageId = @storageId and UserId = @userId
	end
	else
	begin
		update ExternStorageStats set LossCount = LossCount + (ProductCount - @Count) where ArticleId = @articleId and StorageId = @storageId and UserId = @userId
		update ExternStorageStats set ProductCount = @Count where ArticleId = @articleId and StorageId = @storageId and UserId = @userId
	end

	exec ResetArticleStats @articleId, 0

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on TransferMaterial to public
go
print 'SP: TransferMaterial added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('RemoveAllTransfers'))
begin
    drop procedure RemoveAllTransfers
end
go

create procedure RemoveAllTransfers
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	update ArticleTransfers set [Status] = 1 where [Status] = 0

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on RemoveAllTransfers to public
go
print 'SP: RemoveAllTransfers added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('CreateSubOrder'))
begin
    drop procedure CreateSubOrder
end
go

create procedure CreateSubOrder
(
	@OrderId uniqueidentifier,
	@Count float
)
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	declare @newOrderId char(36)
	set @newOrderId = NEWID()

	insert into ProductArticleOrders (Id, ProductArticleId, [Count], [Priority], ProductionCount, UserId)
	select @newOrderId, ProductArticleId, @Count, ([Priority]-1), 0, @userId from ProductArticleOrders where Id = @OrderId

	update ProductArticleOrders set [Count] = [Count] - @Count where Id = @OrderId

	exec CalculateData 0

	exec ResolveProductOrder @newOrderId, 1

	exec CalculateData 0

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on CreateSubOrder to public
go
print 'SP: CreateSubOrder added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('DeleteLoss'))
begin
    drop procedure DeleteLoss
end
go

create procedure DeleteLoss
(
	@StatId uniqueidentifier
)
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	declare @extStat uniqueidentifier
	
	if exists (select 1 from ExternStorageStats where Id = @StatId and UserId = @userId)
	begin
		set @extStat = @StatId
	end
	else
	begin
		set @extStat = NEWID()
		insert into ExternStorageStats (Id, ArticleTransferIdentifierId, ArticleId, StorageId, ProductCount, MissingCount, LossCount, UserId)
		select @extStat, t.ArticleTransferIdentifierId, t.ArticleId, t.StorageId, t.ProductCount, t.MissingCount, t.LossCount, @userId from ExternStorageStats t where t.Id = @StatId and t.UserId is null
	end

	update ExternStorageStats set LossCount = 0 where Id = @extStat

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on DeleteLoss to public
go
print 'SP: DeleteLoss added'
go

-----------------
---- Loading ----
-----------------

if exists (select * from sysobjects where id = object_id('PrepareLoading'))
begin
    drop procedure PrepareLoading
end
go

create procedure PrepareLoading
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	delete from ArticleItemDatas
	delete from ArticleDatas

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on PrepareLoading to public
go
print 'SP: PrepareLoading added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('ProcessLoadedArticleItems'))
begin
    drop procedure ProcessLoadedArticleItems
end
go

create procedure ProcessLoadedArticleItems
(
	@DataId uniqueidentifier,
	@ProductArticleId uniqueidentifier
)
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare itemCur cursor for
	select Id, ExternalId, Quantity from ArticleItemDatas where ArticleDataId = @DataId order by Quantity desc

	declare @itemId uniqueidentifier
	declare @externalId int
	declare @quantity float

	declare @articleItemId uniqueidentifier
	declare @articleId uniqueidentifier

	open itemCur
	fetch next from itemCur into @itemId, @externalId, @quantity

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			set @articleId = null
			select @articleId = Id from Articles where ExternalId = @externalId

			if @articleId is not null
			begin
				set @articleItemId = null
				select top 1 @articleItemId = pai.Id
				from
					ProductArticleItems pai
					join Articles a on (a.Id = pai.ArticleId)
				where
					pai.ProductArticleId = @ProductArticleId
					and a.ExternalId = @externalId
				order by pai.Quantity desc

				if not exists (select 1 from ArticleItemDatas where ArticleDataId = @DataId and ExternalId = @externalId and Updated = 1)
				begin
					if @articleItemId is null
					begin
						set @articleItemId = NEWID()
						insert into ProductArticleItems (Id, ProductArticleId, ArticleId, Quantity, StorageId, SkipCalculation)
						values (@articleItemId, @ProductArticleId, @articleId, @quantity, 'D67BFF48-E36F-4066-888B-23BA5E622FB0', 0)
					end
					else
					begin
						update ProductArticleItems set Quantity = @quantity where Id = @articleItemId
					end

					update ArticleItemDatas set Updated = 1, ProductArticleItemId = @articleItemId where Id = @itemId
				end
			end
		end
		fetch next from itemCur into @itemId, @externalId, @quantity
	end

	close itemCur
	deallocate itemCur

	delete from ProductArticleReservations where ProductArticleItemId in (
		select Id from ProductArticleItems 
		where 
			ProductArticleId = @ProductArticleId 
			and not exists (
				select 1 from ArticleItemDatas where ProductArticleItemId = ProductArticleItems.Id
			)
	)

	delete from ProductArticleItems 
	where 
		ProductArticleId = @ProductArticleId 
		and not exists (
			select 1 from ArticleItemDatas where ProductArticleItemId = ProductArticleItems.Id
		)

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on ProcessLoadedArticleItems to public
go
print 'SP: ProcessLoadedArticleItems added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('ProcessLoadedData'))
begin
    drop procedure ProcessLoadedData
end
go

create procedure ProcessLoadedData
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	update ArticleItemDatas set Updated = 0

	declare dataCur cursor for
	select ExternalId, Name, Type, Code, SpecialCode, OriginalCount, InternalStorage, PurchasingPrice, SellingPrice, OrderName, OrderCount from ArticleDatas

	declare @dataId uniqueidentifier
	declare @externalId int
	declare @name nvarchar(200)
	declare @type int
	declare @code nvarchar(100)
	declare @specCode nvarchar(100)
	declare @originalCount float
	declare @storage nvarchar(200)
	declare @pPrice float
	declare @sPrice float
	declare @order nvarchar(200)
	declare @orderCount float

	declare @articleId uniqueidentifier
	declare @productArticleId uniqueidentifier

	open dataCur

	fetch next from dataCur into @externalId, @name, @type, @code, @specCode, @originalCount, @storage, @pPrice, @sPrice, @order, @orderCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			set @articleId = null
			select @articleId = Id from Articles where ExternalId = @externalId
			if @articleId is null
			begin
				set @articleId = NEWID()

				insert into Articles (Id, ExternalId, Name, Type, Code, SpecialCode, OriginalCount, InternalStorage, PurchasingPrice, SellingPrice, OrderName, OrderCount, Updated)
				values (@articleId, @externalId, @name, @type, @code, @specCode, @originalCount, @storage, @pPrice, @sPrice, @order, @orderCount, 1)

				insert into ArticleStats (Id, ArticleId, CurrentCount, MissingInOrders, ProductCount, ExternStorageCount) 
				values (NEWID(), @articleId, 0, 0, 0, 0)
			end
			else
			begin
				update Articles set 
					Name = @name,
					Type = @type,
					Code = @code,
					SpecialCode = @specCode,
					OriginalCount = @originalCount,
					InternalStorage = @storage,
					PurchasingPrice = @pPrice,
					SellingPrice = @sPrice,
					OrderName = @order,
					OrderCount = @orderCount,
					Updated = 1
				where
					Id = @articleId
			end
		end
		fetch next from dataCur into @externalId, @name, @type, @code, @specCode, @originalCount, @storage, @pPrice, @sPrice, @order, @orderCount
	end

	close dataCur
	deallocate dataCur

	declare dataProductCur cursor for
	select Id, ExternalId from ArticleDatas where Type = 2

	open dataProductCur
	fetch next from dataProductCur into @dataId, @externalId

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			select @articleId = Id from Articles where ExternalId = @externalId

			set @productArticleId = null
			select @productArticleId = Id from ProductArticles where ArticleId = @articleId

			if @productArticleId is null
			begin
				set @productArticleId = NEWID()
				insert into ProductArticles (Id, ArticleId) values (@productArticleId, @articleId)
			end
				
			exec ProcessLoadedArticleItems @dataId, @productArticleId
		end
		fetch next from dataProductCur into @dataId, @externalId
	end

	close dataProductCur
	deallocate dataProductCur
	
	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on ProcessLoadedData to public
go
print 'SP: ProcessLoadedData added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('GroupLoadedData'))
begin
    drop procedure GroupLoadedData
end
go

create procedure GroupLoadedData
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare dupCur cursor for
	select 
		Id, Code, OriginalCount, OrderCount
	from 
		ArticleDatas a
	where 
		exists (
			select 1 from ArticleDatas b where b.Id != a.Id and a.Code = b.Code
		)
		and not exists (
			select 1 from Articles where ExternalId = a.ExternalId
		)

	declare @id uniqueidentifier
	declare @code nvarchar(100)
	declare @originalCount float
	declare @orderCount float

	open dupCur

	fetch next from dupCur into @id, @code, @originalCount, @orderCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			update ArticleDatas set OriginalCount = OriginalCount + @originalCount, OrderCount = OrderCount + @orderCount where Code = @code and Id != @id
		end
		fetch next from dupCur into @id, @code, @originalCount, @orderCount
	end

	close dupCur
	deallocate dupCur

	delete from ArticleItemDatas where ArticleDataId in (
		select 
			Id
		from 
			ArticleDatas a
		where 
			exists (
				select 1 from ArticleDatas b where b.Id != a.Id and a.Code = b.Code
			)
			and not exists (
				select 1 from Articles where ExternalId = a.ExternalId
			)
	)

	delete from ArticleDatas where Id in (
		select 
			Id
		from 
			ArticleDatas a
		where 
			exists (
				select 1 from ArticleDatas b where b.Id != a.Id and a.Code = b.Code
			)
			and not exists (
				select 1 from Articles where ExternalId = a.ExternalId
			)
	)

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on GroupLoadedData to public
go
print 'SP: GroupLoadedData added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('RemoveDuplicities'))
begin
    drop procedure RemoveDuplicities
end
go

create procedure RemoveDuplicities
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	declare dupItemsCur cursor for
	select t.AId, t.BId, t.BOrigCount, t.BCurrentCount, t.BExtCount from (
		select 
			a.Id as AId, 
			a.ExternalId as AExtId, 
			a.Code, 
			b.Id as BId, 
			b.ExternalId as BExtId, 
			b.OriginalCount as BOrigCount,
			bss.CurrentCount as BCurrentCount,
			bss.ExternStorageCount as BExtCount
		from 
			Articles a 
			join Articles b on (a.Code = b.Code and ((a.InternalStorage is null and a.ExternalId > b.ExternalId) or (a.InternalStorage not like 'Materiál%' and b.InternalStorage like 'Materiál%')))
			join ArticleStats bss on (bss.ArticleId = b.Id)
	) t

	declare @targetId uniqueidentifier
	declare @id uniqueidentifier
	declare @origCount float
	declare @curCount float
	declare @extCount float

	open dupItemsCur

	fetch next from dupItemsCur into @targetId, @id, @origCount, @curCount, @extCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			update Articles set OriginalCount = OriginalCount + @origCount where Id = @targetId
			update ArticleStats set CurrentCount = CurrentCount + @curCount, ExternStorageCount = ExternStorageCount + @extCount where ArticleId = @targetId

			delete from ProductArticleReservations where ProductArticleItemId in (select Id from ProductArticleItems where ProductArticleId = (select Id from ProductArticles where ArticleId = @id))
			delete from ProductArticleItems where ProductArticleId = (select Id from ProductArticles where ArticleId = @id)
			delete from ArticleOrders where ArticleId = @id
			delete from ProductArticleReservations where ProductArticleItemId in (select Id from ProductArticleItems where ArticleId = @id)
			delete from ProductArticleItems where ArticleId = @id
			delete from ProductArticles where ArticleId = @id
			delete from ArticleStats where ArticleId = @id
			delete from Articles where Id = @id
		end
		fetch next from dupItemsCur into @targetId, @id, @origCount, @curCount, @extCount
	end

	close dupItemsCur
	deallocate dupItemsCur

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on RemoveDuplicities to public
go
print 'SP: RemoveDuplicities added'
go

-----------------------------------

if exists (select * from sysobjects where id = object_id('PostLoadUpdate'))
begin
    drop procedure PostLoadUpdate
end
go

create procedure PostLoadUpdate
as
	set nocount on

	if (dbo.AssureLock() = 0)
	begin
		raiserror ('Database is not locked!', 16, 1)
		return
	end

	declare @useTransaction bit
	set @useTransaction = 0
	if @@TRANCOUNT = 0
	begin
		set @useTransaction = 1
		begin tran
	end

	exec GroupLoadedData
	exec ProcessLoadedData
	exec RemoveDuplicities

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	declare articleCur cursor for
	select Id from Articles where Updated = 1

	declare @id uniqueidentifier

	open articleCur

	fetch next from articleCur into @id

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			exec ResetArticleStats @id, 1
		end
		fetch next from articleCur into @id
	end

	close articleCur
	deallocate articleCur

	update Articles set Updated = 0 where Updated = 1

	exec LogDataJournal @userId, 'AccountingDataUpdate'

	delete from ArticleItemDatas
	delete from ArticleDatas

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on PostLoadUpdate to public
go
print 'SP: PostLoadUpdate added'
go

