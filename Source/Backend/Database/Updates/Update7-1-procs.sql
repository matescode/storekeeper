use [StoreKeeper]

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
	select t.AId, t.BId, t.BOrigCount, t.BCurrentCount, t.BExtCount, t.BOrdCount from (
		select 
			a.Id as AId, 
			a.ExternalId as AExtId, 
			a.Code, 
			b.Id as BId, 
			b.ExternalId as BExtId, 
			b.OriginalCount as BOrigCount,
			bss.CurrentCount as BCurrentCount,
			bss.ExternStorageCount as BExtCount,
			b.OrderCount as BOrdCount
		from 
			Articles a 
			join Articles b on (a.Code = b.Code and ((a.InternalStorage is null and a.ExternalId < b.ExternalId) or (a.InternalStorage not like 'Materiál%' and b.InternalStorage like 'Materiál%')))
			join ArticleStats bss on (bss.ArticleId = b.Id)
	) t

	declare @targetId uniqueidentifier
	declare @id uniqueidentifier
	declare @origCount float
	declare @curCount float
	declare @extCount float
	declare @ordCount float

	open dupItemsCur

	fetch next from dupItemsCur into @targetId, @id, @origCount, @curCount, @extCount, @ordCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			update Articles set OriginalCount = OriginalCount + @origCount, OrderCount = OrderCount + @ordCount where Id = @targetId
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
		fetch next from dupItemsCur into @targetId, @id, @origCount, @curCount, @extCount, @ordCount
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

	--exec GroupLoadedData
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
