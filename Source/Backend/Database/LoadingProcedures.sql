use [StoreKeeper]

-----------------------------------

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
	delete from ArticleCounts

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

if exists (select * from sysobjects where id = object_id('GetStorageId'))
begin
    drop function GetStorageId
end
go

create function GetStorageId
(
	@Storage nvarchar(200)
)
returns uniqueidentifier
as
begin
	declare @storageId uniqueidentifier
	set @storageId = null

	select
		@storageId = Id 
	from 
		Storages 
	where 
		Prefix is not null 
		and len(Prefix) > 0
		and Prefix = SUBSTRING(@Storage, 1, len(Prefix))
	
	return coalesce(@storageId, 'D67BFF48-E36F-4066-888B-23BA5E622FB0')
end
go

grant exec on GetStorageId to public
go
print 'SF: GetStorageId added'
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
	select Id, ExternalId, Code, InternalStorage, Quantity
	from 
		ArticleItemDatas
	where ArticleDataId = @DataId 
	order by Quantity desc

	declare @itemId uniqueidentifier
	declare @externalId int
	declare @code nvarchar(100)
	declare @storage nvarchar(200)
	declare @quantity float
	declare @type int

	declare @storageId uniqueidentifier

	declare @articleItemId uniqueidentifier
	declare @articleId uniqueidentifier

	open itemCur
	fetch next from itemCur into @itemId, @externalId, @code, @storage, @quantity

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			set @storageId = dbo.GetStorageId(@storage)

			set @articleId = null
			select @articleId = Id from Articles where Code = @code

			if @articleId is not null
			begin
				set @articleItemId = null
				select top 1 @articleItemId = pai.Id
				from
					ProductArticleItems pai
					join Articles a on (a.Id = pai.ArticleId)
				where
					pai.ProductArticleId = @ProductArticleId
					and a.Code = @code
				order by pai.Quantity desc

				if not exists (select 1 from ArticleItemDatas where ArticleDataId = @DataId and ExternalId = @externalId and Updated = 1)
				begin
					if @articleItemId is null
					begin
						set @articleItemId = NEWID()
						insert into ProductArticleItems (Id, ProductArticleId, ArticleId, Quantity, StorageId, SkipCalculation)
						values (@articleItemId, @ProductArticleId, @articleId, @quantity, @storageId, 0)
					end
					else
					begin
						update ProductArticleItems set Quantity = @quantity where Id = @articleItemId
					end

					update ArticleItemDatas set Updated = 1, ProductArticleItemId = @articleItemId where Id = @itemId
				end
			end
		end
		fetch next from itemCur into @itemId, @externalId, @code, @storage, @quantity
	end

	close itemCur
	deallocate itemCur

	delete from ProductArticleReservations where ProductArticleItemId in (
		select Id from ProductArticleItems pai
		where 
			pai.ProductArticleId = @ProductArticleId
			and not exists (
				select 1 from ArticleItemDatas aid where aid.ProductArticleItemId = pai.Id
			)
	)

	delete from ProductArticleItems 
	where 
		Id in (
			select Id from ProductArticleItems pai
			where 
				pai.ProductArticleId = @ProductArticleId
				and not exists (
					select 1 from ArticleItemDatas aid where aid.ProductArticleItemId = pai.Id
				)
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
	select Name, Type, Code, SpecialCode, OriginalCount, InternalStorage, PurchasingPrice, SellingPrice, OrderName, OrderCount from ArticleDatas

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
	declare @storageId uniqueidentifier

	declare @articleId uniqueidentifier
	declare @productArticleId uniqueidentifier

	open dataCur

	fetch next from dataCur into @name, @type, @code, @specCode, @originalCount, @storage, @pPrice, @sPrice, @order, @orderCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			set @storageId = dbo.GetStorageId(@storage)

			set @articleId = null
			select @articleId = Id from Articles where Code = @code
			if @articleId is null
			begin
				set @articleId = NEWID()

				insert into Articles (Id, Name, [Type], Code, SpecialCode, ExternStorageCount, PurchasingPrice, SellingPrice, OrderName, OrderCount, Updated)
				values (@articleId, @name, @type, @code, @specCode, 0, @pPrice, @sPrice, @order, @orderCount, 1)
			end
			else
			begin
				update Articles set 
					Name = @name,
					[Type] = @type,
					SpecialCode = @specCode,
					ExternStorageCount = 0,
					PurchasingPrice = @pPrice,
					SellingPrice = @sPrice,
					OrderName = @order,
					OrderCount = 0,
					Updated = 1
				where
					Id = @articleId

				delete from ArticleStats where ArticleId = @articleId
			end

			if not exists (select 1 from ArticleCounts where Code = @code and StorageId = @storageId)
			begin
				insert into ArticleCounts (Id, Code, StorageId, OriginalCount, OrderCount)
				values (NEWID(), @code, @storageId, @originalCount, @orderCount)
			end
			else
			begin
				update ArticleCounts 
				set OriginalCount = OriginalCount + @originalCount, OrderCount = OrderCount + @orderCount
				where Code = @code and StorageId = @storageId
			end
		end
		fetch next from dataCur into @name, @type, @code, @specCode, @originalCount, @storage, @pPrice, @sPrice, @order, @orderCount
	end

	close dataCur
	deallocate dataCur

	declare dataProductCur cursor for
	select Id, Code from ArticleDatas where [Type] = 2

	open dataProductCur
	fetch next from dataProductCur into @dataId, @code

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			select @articleId = Id from Articles where Code = @code

			set @productArticleId = null
			select @productArticleId = Id from ProductArticles where ArticleId = @articleId

			if @productArticleId is null
			begin
				set @productArticleId = NEWID()
				insert into ProductArticles (Id, ArticleId) values (@productArticleId, @articleId)
			end
				
			exec ProcessLoadedArticleItems @dataId, @productArticleId
		end
		fetch next from dataProductCur into @dataId, @code
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

if exists (select * from sysobjects where id = object_id('CalculateDataStats'))
begin
    drop procedure CalculateDataStats
end
go

create procedure CalculateDataStats
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

	update ArticleStats set CurrentCount = 0, ProductCount = 0 where ArticleId in (
		select Id from Articles where Updated = 1
	)

	update Articles set OrderCount = 0, ExternStorageCount = 0 where Updated = 1

	declare countCur cursor for
	select Code, StorageId, sum(OriginalCount), sum(OrderCount) from ArticleCounts group by Code, StorageId

	declare @code nvarchar(100)
	declare @type int
	declare @storageId uniqueidentifier
	declare @origCount float
	declare @ordCount float

	declare @articleId uniqueidentifier

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	open countCur

	fetch next from countCur into @code, @storageId, @origCount, @ordCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			select @articleId = Id from Articles where Code = @code

			insert into ArticleStats (Id, ArticleId, StorageId, CurrentCount, MissingInOrders, ProductCount) 
			values (NEWID(), @articleId, @storageId, @origCount, 0, 0)

			if @storageId = 'D67BFF48-E36F-4066-888B-23BA5E622FB0'
			begin
				update Articles set OrderCount = OrderCount + @ordCount where Id = @articleId
			end
		end
		fetch next from countCur into @code, @storageId, @origCount, @ordCount
	end

	close countCur
	deallocate countCur

	declare extCountCur cursor for
	select ArticleId, sum(CurrentCount) from ArticleStats
	where 
		StorageId != 'D67BFF48-E36F-4066-888B-23BA5E622FB0'
		and ArticleId in (
			select Id from Articles where Updated = 1
		)
	group by ArticleId

	declare @id uniqueidentifier
	declare @extCount float

	open extCountCur

	fetch next from extCountCur into @id, @extCount

	while @@FETCH_STATUS <> -1
	begin
		if @@FETCH_STATUS <> -2
		begin
			update Articles set ExternStorageCount = @extCount where Id = @id
		end
		fetch next from extCountCur into @id, @extCount
	end

	close extCountCur
	deallocate extCountCur
	
	if @useTransaction = 1
	begin
		commit
	end

return
go

grant exec on CalculateDataStats to public
go
print 'SP: CalculateDataStats added'
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

	exec ProcessLoadedData
	exec CalculateDataStats

	declare @userId char(36)
	select @userId = Value from SystemInformations where Name = 'LockedBy'

	update Articles set Updated = 0 where Updated = 1

	exec LogDataJournal @userId, 'AccountingDataUpdate'

	delete from ArticleItemDatas
	delete from ArticleDatas
	delete from ArticleCounts

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
