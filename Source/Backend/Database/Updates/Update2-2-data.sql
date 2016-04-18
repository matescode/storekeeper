use [StoreKeeper]

begin tran

-------------------------
----- unknown items -----
-------------------------

delete from ProductArticleReservations where ProductArticleItemId in (
	select Id from ProductArticleItems where ArticleId in (
		select Id from Articles where InternalStorage is null
	)
)

delete from ProductArticleItems where ProductArticleId in (
	select Id from ProductArticles where ArticleId in (
		select Id from Articles where InternalStorage is null
	)
)

delete from ProductArticleItems where ArticleId in (
	select Id from Articles where InternalStorage is null
)

delete from ProductArticles where ArticleId in (
	select Id from Articles where InternalStorage is null
)

delete from ArticleStats where ArticleId in (
	select Id from Articles where InternalStorage is null
)

delete from ExternStorageStats where ArticleId in (
	select Id from Articles where InternalStorage is null
)

delete from Articles where InternalStorage is null

---------------------
----- duplicity -----
---------------------

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
		join Articles b on (a.Code = b.Code and ((a.InternalStorage is null and a.ExternalId < b.ExternalId) or (a.InternalStorage like 'Výroba%' and b.InternalStorage like 'Materiál%')))
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

commit