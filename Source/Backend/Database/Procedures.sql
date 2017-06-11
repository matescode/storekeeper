use [StoreKeeper]

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

	update ProductArticleOrders set UserId = null where UserId = @userId
	
	delete from ArticleOrders where UserId is null
	delete from ProductArticleReservations where UserId is null
	
	update ArticleOrders set UserId = null where UserId = @userId

	update ProductArticleReservations set UserId = null where UserId = @userId

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

	update ArticleStats set MissingInOrders = 0, ProductCount = 0 
	where ArticleId in (
		select ArticleId from ProductArticleItems
		where ProductArticleId in (
			select distinct ProductArticleId from ProductArticleOrders
		)
	)

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
		join ArticleStats ars on (ars.ArticleId = pa.ArticleId and ars.StorageId = pa.StorageId)
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

			update ArticleStats set MissingInOrders = MissingInOrders + @orderCount where ArticleId = @articleId and StorageId = @storageId

			if @orderCount > 0
			begin
				if (@type = 1 and not exists (select 1 from ArticleOrders where ArticleId = @articleId and UserId = @UserId))
				begin
					insert into ArticleOrders (Id, ArticleId, [Count], UserId) values (NEWID(), @articleId, 0, @UserId)
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
		and ass.StorageId = 'D67BFF48-E36F-4066-888B-23BA5E622FB0'

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

			select @sum2 = SUM(MissingInOrders) from ArticleStats where ArticleId = @articleId and StorageId != 'D67BFF48-E36F-4066-888B-23BA5E622FB0'

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

if exists (select * from sysobjects where id = object_id('IncrementProductCount'))
begin
    drop procedure IncrementProductCount
end
go

create procedure IncrementProductCount
(
	@ProductArticleId uniqueidentifier
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

	declare @articleId uniqueidentifier
	declare @storageId uniqueidentifier
	
	declare itemCur cursor for
	select 
		ArticleId, StorageId
	from
		ProductArticleItems
	where
		ProductArticleId = @ProductArticleId
		and SkipCalculation = 0

	open itemCur

	fetch next from itemCur into @articleId, @storageId

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			update ArticleStats set ProductCount = ProductCount + 1 where ArticleId = @articleId and StorageId = @storageId
		end
		fetch next from itemCur into @articleId, @storageId
	end

	close itemCur
	deallocate itemCur

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on IncrementProductCount to public
go
print 'SP: IncrementProductCount added'
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

			exec IncrementProductCount @productArticleId
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

	insert into ProductOrderCompletions (Id, ProductArticleOrderId, [Status], UserId) values (NEWID(), @OrderId, case when @Resolve = 1 then 1 else 2 end, @userId)

	exec RefreshAllPriorities @userId

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

if exists (select * from sysobjects where id = object_id('RemoveMaterial'))
begin
    drop procedure RemoveMaterial
end
go

create procedure RemoveMaterial
(
	@ArticleId uniqueidentifier
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

	declare @type int
	set @type = 0
	select @type = [Type] from Articles where Id = @ArticleId
	if @type = 1
	begin
		delete from ProductArticleReservations where ProductArticleItemId in (select Id from ProductArticleItems where ArticleId = @ArticleId)
		delete from ProductArticleItems where ArticleId = @ArticleId
	end
	else if @type = 2
	begin
		declare @pid uniqueidentifier
		select @pid = Id from ProductArticles where ArticleId = @ArticleId
		
		delete from ProductArticleReservations where ProductArticleItemId in (select Id from ProductArticleItems where ProductArticleId = @pid)
		delete from ProductArticleOrders where ProductArticleId = @pid
		delete from ProductArticleItems where ProductArticleId = @pid
		delete from ProductArticles where ArticleId = @ArticleId

	end

	delete from ArticleOrders where ArticleId = @ArticleId
	delete from ArticleStats where ArticleId = @ArticleId
	delete from Articles where Id = @ArticleId

	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on RemoveMaterial to public
go
print 'SP: RemoveMaterial added'
go

-----------------------------------
