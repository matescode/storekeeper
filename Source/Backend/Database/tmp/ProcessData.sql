begin tran

declare dataCur cursor for
select Id, ExternalId, Name, Type, Code, SpecialCode, OriginalCount, InternalStorage, PurchasingPrice, SellingPrice, OrderName, OrderCount from ArticleDatas

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
declare @isNew bit
declare @productArticleId uniqueidentifier

open dataCur

fetch next from dataCur into @dataId, @externalId, @name, @type, @code, @specCode, @originalCount, @storage, @pPrice, @sPrice, @order, @orderCount

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

			set @isNew = 1
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

			set @isNew = 0
		end

		if @type = 2
		begin
			if @isNew = 1
			begin
				set @productArticleId = NEWID()
				insert into ProductArticles (Id, ArticleId) values (@productArticleId, @articleId)


			end
			--else
			--begin
			--end
		end
	end
	fetch next from dataCur into @dataId, @externalId, @name, @type, @code, @specCode, @originalCount, @storage, @pPrice, @sPrice, @order, @orderCount
end

close dataCur
deallocate dataCur

rollback